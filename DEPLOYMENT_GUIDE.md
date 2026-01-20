# Deployment Guide - Mitt Telenor Demo

## Overview

This guide provides step-by-step instructions for deploying the Mitt Telenor Demo application to production AWS infrastructure.

## Prerequisites

- AWS Account with appropriate permissions
- AWS CLI configured
- Docker installed
- Node.js 20+ and pnpm 8+
- .NET 8 SDK
- GitHub repository set up

## Local Development Deployment

### 1. Start Infrastructure Services

```bash
# Start all services with Docker Compose
docker-compose up -d

# Verify services are running
docker-compose ps

# Check logs
docker-compose logs -f
```

### 2. Run Backend Services Locally

Each microservice can be run independently:

```bash
# Auth Service (Port 5001)
cd backend/src/AuthService
dotnet restore
dotnet ef database update
dotnet run

# User Service (Port 5002)
cd backend/src/UserService
dotnet restore
dotnet ef database update
dotnet run

# Subscription Service (Port 5003)
cd backend/src/SubscriptionService
dotnet restore
dotnet ef database update
dotnet run

# Usage Service (Port 5004)
cd backend/src/UsageService
dotnet restore
dotnet run

# Billing Service (Port 5005)
cd backend/src/BillingService
dotnet restore
dotnet ef database update
dotnet run
```

### 3. Run Frontend Locally

```bash
cd frontend
pnpm install
pnpm dev
```

Access the application at `http://localhost:5173`

## AWS Production Deployment

### Architecture Overview

- **Frontend**: S3 + CloudFront
- **Backend**: ECS Fargate
- **Databases**: RDS PostgreSQL, DynamoDB
- **Caching**: ElastiCache Redis
- **Messaging**: SNS/SQS
- **Storage**: S3
- **Secrets**: Secrets Manager

### Step 1: Setup AWS Infrastructure

#### Create VPC and Networking

```bash
# Create VPC
aws ec2 create-vpc --cidr-block 10.0.0.0/16 --tag-specifications 'ResourceType=vpc,Tags=[{Key=Name,Value=telenor-vpc}]'

# Create subnets (public and private)
aws ec2 create-subnet --vpc-id <vpc-id> --cidr-block 10.0.1.0/24 --availability-zone us-east-1a
aws ec2 create-subnet --vpc-id <vpc-id> --cidr-block 10.0.2.0/24 --availability-zone us-east-1b
```

#### Create RDS PostgreSQL Database

```bash
aws rds create-db-instance \
  --db-instance-identifier telenor-postgres \
  --db-instance-class db.t3.micro \
  --engine postgres \
  --engine-version 16.1 \
  --master-username postgres \
  --master-user-password <secure-password> \
  --allocated-storage 20 \
  --vpc-security-group-ids <security-group-id> \
  --db-subnet-group-name <subnet-group-name> \
  --backup-retention-period 7 \
  --multi-az
```

#### Create ElastiCache Redis

```bash
aws elasticache create-cache-cluster \
  --cache-cluster-id telenor-redis \
  --cache-node-type cache.t3.micro \
  --engine redis \
  --num-cache-nodes 1 \
  --cache-subnet-group-name <subnet-group-name> \
  --security-group-ids <security-group-id>
```

#### Create DynamoDB Table

```bash
aws dynamodb create-table \
  --table-name telenor-usage-data \
  --attribute-definitions \
    AttributeName=UserId,AttributeType=S \
    AttributeName=Period,AttributeType=S \
  --key-schema \
    AttributeName=UserId,KeyType=HASH \
    AttributeName=Period,KeyType=RANGE \
  --billing-mode PAY_PER_REQUEST
```

#### Create SNS Topics and SQS Queues

```bash
# Create SNS topics
aws sns create-topic --name user-events
aws sns create-topic --name subscription-changes
aws sns create-topic --name usage-alerts
aws sns create-topic --name invoice-generated

# Create SQS queues
aws sqs create-queue --queue-name user-events-queue
aws sqs create-queue --queue-name subscription-changes-queue
aws sqs create-queue --queue-name usage-alerts-queue
aws sqs create-queue --queue-name invoice-queue

# Subscribe queues to topics
aws sns subscribe \
  --topic-arn <topic-arn> \
  --protocol sqs \
  --notification-endpoint <queue-arn>
```

### Step 2: Setup Secrets Manager

```bash
# Store database credentials
aws secretsmanager create-secret \
  --name telenor/database/credentials \
  --secret-string '{"username":"postgres","password":"<secure-password>"}'

# Store JWT secret
aws secretsmanager create-secret \
  --name telenor/jwt/secret \
  --secret-string '{"secretKey":"<your-jwt-secret>"}'

# Store Contentful credentials
aws secretsmanager create-secret \
  --name telenor/contentful/credentials \
  --secret-string '{"spaceId":"<space-id>","accessToken":"<access-token>"}'
```

### Step 3: Build and Push Docker Images

```bash
# Login to ECR
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin <account-id>.dkr.ecr.us-east-1.amazonaws.com

# Create ECR repositories
for service in auth user subscription usage billing; do
  aws ecr create-repository --repository-name telenor-${service}-service
done

# Build and push images
for service in AuthService UserService SubscriptionService UsageService BillingService; do
  docker build -t telenor-${service,,} -f backend/src/${service}/Dockerfile backend
  docker tag telenor-${service,,}:latest <account-id>.dkr.ecr.us-east-1.amazonaws.com/telenor-${service,,}:latest
  docker push <account-id>.dkr.ecr.us-east-1.amazonaws.com/telenor-${service,,}:latest
done
```

### Step 4: Create ECS Cluster and Services

```bash
# Create ECS cluster
aws ecs create-cluster --cluster-name telenor-cluster

# Create task definitions (example for Auth Service)
aws ecs register-task-definition --cli-input-json file://ecs-task-definition-auth.json

# Create services
aws ecs create-service \
  --cluster telenor-cluster \
  --service-name auth-service \
  --task-definition telenor-auth-service \
  --desired-count 2 \
  --launch-type FARGATE \
  --network-configuration "awsvpcConfiguration={subnets=[<subnet-ids>],securityGroups=[<security-group-id>],assignPublicIp=ENABLED}" \
  --load-balancers "targetGroupArn=<target-group-arn>,containerName=auth-service,containerPort=80"
```

### Step 5: Setup Application Load Balancer

```bash
# Create ALB
aws elbv2 create-load-balancer \
  --name telenor-alb \
  --subnets <subnet-id-1> <subnet-id-2> \
  --security-groups <security-group-id> \
  --scheme internet-facing

# Create target groups for each service
for service in auth user subscription usage billing; do
  aws elbv2 create-target-group \
    --name telenor-${service}-tg \
    --protocol HTTP \
    --port 80 \
    --vpc-id <vpc-id> \
    --target-type ip \
    --health-check-path /health
done

# Create listener rules
aws elbv2 create-listener \
  --load-balancer-arn <alb-arn> \
  --protocol HTTP \
  --port 80 \
  --default-actions Type=forward,TargetGroupArn=<target-group-arn>
```

### Step 6: Deploy Frontend to S3 + CloudFront

```bash
# Create S3 bucket
aws s3 mb s3://telenor-frontend-prod

# Configure bucket for static website hosting
aws s3 website s3://telenor-frontend-prod --index-document index.html --error-document index.html

# Build frontend
cd frontend
pnpm build

# Upload to S3
aws s3 sync dist/ s3://telenor-frontend-prod --delete

# Create CloudFront distribution
aws cloudfront create-distribution --cli-input-json file://cloudfront-config.json

# Get distribution domain name
aws cloudfront list-distributions --query "DistributionList.Items[?Comment=='Telenor Frontend'].DomainName"
```

### Step 7: Setup Route 53 DNS

```bash
# Create hosted zone
aws route53 create-hosted-zone --name telenor-demo.com --caller-reference $(date +%s)

# Create A record for CloudFront
aws route53 change-resource-record-sets \
  --hosted-zone-id <zone-id> \
  --change-batch file://route53-changes.json
```

### Step 8: Setup SSL Certificates

```bash
# Request certificate from ACM
aws acm request-certificate \
  --domain-name telenor-demo.com \
  --subject-alternative-names www.telenor-demo.com api.telenor-demo.com \
  --validation-method DNS

# Validate domain ownership (follow DNS validation instructions)
```

## CI/CD Setup with GitHub Actions

### 1. Configure GitHub Secrets

Add the following secrets to your GitHub repository:

- `AWS_ACCESS_KEY_ID`
- `AWS_SECRET_ACCESS_KEY`
- `AWS_S3_BUCKET`
- `CLOUDFRONT_DISTRIBUTION_ID`
- `VITE_API_BASE_URL`
- `VITE_CONTENTFUL_SPACE_ID`
- `VITE_CONTENTFUL_ACCESS_TOKEN`

### 2. Workflows

The project includes two GitHub Actions workflows:

- `.github/workflows/frontend.yml` - Frontend CI/CD
- `.github/workflows/backend.yml` - Backend CI/CD

These workflows automatically:
- Run tests on pull requests
- Build and deploy on merge to main
- Run Lighthouse performance checks
- Upload code coverage reports

## Monitoring and Logging

### CloudWatch Setup

```bash
# Create log groups for each service
for service in auth user subscription usage billing; do
  aws logs create-log-group --log-group-name /ecs/telenor-${service}-service
done

# Create CloudWatch dashboard
aws cloudwatch put-dashboard --dashboard-name telenor-dashboard --dashboard-body file://dashboard-config.json
```

### Setup Alarms

```bash
# CPU utilization alarm
aws cloudwatch put-metric-alarm \
  --alarm-name telenor-high-cpu \
  --alarm-description "Alert when CPU exceeds 80%" \
  --metric-name CPUUtilization \
  --namespace AWS/ECS \
  --statistic Average \
  --period 300 \
  --threshold 80 \
  --comparison-operator GreaterThanThreshold \
  --evaluation-periods 2

# Error rate alarm
aws cloudwatch put-metric-alarm \
  --alarm-name telenor-high-errors \
  --alarm-description "Alert when error rate exceeds 5%" \
  --metric-name 5XXError \
  --namespace AWS/ApplicationELB \
  --statistic Sum \
  --period 300 \
  --threshold 100 \
  --comparison-operator GreaterThanThreshold \
  --evaluation-periods 1
```

## Backup and Disaster Recovery

### Database Backups

```bash
# Enable automated backups (already configured in RDS creation)
# Manual snapshot
aws rds create-db-snapshot \
  --db-instance-identifier telenor-postgres \
  --db-snapshot-identifier telenor-manual-snapshot-$(date +%Y%m%d)
```

### DynamoDB Backups

```bash
# Enable point-in-time recovery
aws dynamodb update-continuous-backups \
  --table-name telenor-usage-data \
  --point-in-time-recovery-specification PointInTimeRecoveryEnabled=true

# Create on-demand backup
aws dynamodb create-backup \
  --table-name telenor-usage-data \
  --backup-name telenor-usage-backup-$(date +%Y%m%d)
```

## Scaling Configuration

### Auto Scaling for ECS

```bash
# Register scalable target
aws application-autoscaling register-scalable-target \
  --service-namespace ecs \
  --resource-id service/telenor-cluster/auth-service \
  --scalable-dimension ecs:service:DesiredCount \
  --min-capacity 2 \
  --max-capacity 10

# Create scaling policy
aws application-autoscaling put-scaling-policy \
  --service-namespace ecs \
  --resource-id service/telenor-cluster/auth-service \
  --scalable-dimension ecs:service:DesiredCount \
  --policy-name telenor-cpu-scaling \
  --policy-type TargetTrackingScaling \
  --target-tracking-scaling-policy-configuration file://scaling-policy.json
```

## Security Checklist

- [ ] All secrets stored in AWS Secrets Manager
- [ ] Security groups configured with least privilege
- [ ] SSL/TLS certificates installed
- [ ] WAF rules configured for CloudFront
- [ ] Database encryption at rest enabled
- [ ] VPC flow logs enabled
- [ ] CloudTrail logging enabled
- [ ] IAM roles follow least privilege principle
- [ ] MFA enabled for AWS root account
- [ ] Regular security patches applied

## Cost Optimization

### Estimated Monthly Costs (Production)

- **ECS Fargate**: ~$150 (5 services, 2 tasks each)
- **RDS PostgreSQL**: ~$30 (db.t3.micro)
- **ElastiCache Redis**: ~$15 (cache.t3.micro)
- **DynamoDB**: ~$5 (on-demand pricing)
- **S3 + CloudFront**: ~$10
- **Data Transfer**: ~$20
- **Total**: ~$230/month

### Cost Saving Tips

1. Use Reserved Instances for predictable workloads
2. Enable S3 Intelligent-Tiering
3. Use CloudFront caching effectively
4. Implement DynamoDB auto-scaling
5. Schedule non-production environments to stop during off-hours

## Troubleshooting

### Common Issues

**Services not starting**
```bash
# Check ECS task logs
aws ecs describe-tasks --cluster telenor-cluster --tasks <task-id>
aws logs tail /ecs/telenor-auth-service --follow
```

**Database connection issues**
```bash
# Verify security group rules
aws ec2 describe-security-groups --group-ids <security-group-id>

# Test connection from ECS task
aws ecs execute-command --cluster telenor-cluster --task <task-id> --command "/bin/sh" --interactive
```

**High latency**
```bash
# Check CloudWatch metrics
aws cloudwatch get-metric-statistics \
  --namespace AWS/ApplicationELB \
  --metric-name TargetResponseTime \
  --dimensions Name=LoadBalancer,Value=<alb-name> \
  --start-time $(date -u -d '1 hour ago' +%Y-%m-%dT%H:%M:%S) \
  --end-time $(date -u +%Y-%m-%dT%H:%M:%S) \
  --period 300 \
  --statistics Average
```

## Rollback Procedures

### Frontend Rollback

```bash
# List previous CloudFront invalidations
aws cloudfront list-invalidations --distribution-id <distribution-id>

# Revert S3 to previous version
aws s3 sync s3://telenor-frontend-prod-backup/ s3://telenor-frontend-prod/ --delete

# Create new invalidation
aws cloudfront create-invalidation --distribution-id <distribution-id> --paths "/*"
```

### Backend Rollback

```bash
# Update ECS service to previous task definition
aws ecs update-service \
  --cluster telenor-cluster \
  --service auth-service \
  --task-definition telenor-auth-service:previous-revision

# Monitor rollback
aws ecs describe-services --cluster telenor-cluster --services auth-service
```

## Support and Maintenance

### Regular Maintenance Tasks

- **Daily**: Monitor CloudWatch dashboards and alarms
- **Weekly**: Review application logs for errors
- **Monthly**: Review and optimize costs
- **Quarterly**: Security audit and dependency updates
- **Annually**: Disaster recovery drill

### Health Check Endpoints

- Auth Service: `http://<alb-dns>/health`
- User Service: `http://<alb-dns>/api/user/health`
- Subscription Service: `http://<alb-dns>/api/subscription/health`
- Usage Service: `http://<alb-dns>/api/usage/health`
- Billing Service: `http://<alb-dns>/api/billing/health`

## Additional Resources

- [AWS ECS Documentation](https://docs.aws.amazon.com/ecs/)
- [.NET on AWS](https://aws.amazon.com/developer/language/net/)
- [Vue.js Deployment](https://vuejs.org/guide/best-practices/production-deployment.html)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)

---

**Last Updated**: January 2024
**Version**: 1.0.0

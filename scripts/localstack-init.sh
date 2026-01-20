#!/bin/bash

echo "Initializing LocalStack resources..."

# Set AWS region
export AWS_DEFAULT_REGION=eu-west-1

# Create S3 bucket for file uploads
awslocal s3 mb s3://telenor-uploads
awslocal s3api put-bucket-cors --bucket telenor-uploads --cors-configuration '{
  "CORSRules": [{
    "AllowedOrigins": ["*"],
    "AllowedMethods": ["GET", "PUT", "POST", "DELETE"],
    "AllowedHeaders": ["*"]
  }]
}'

# Create DynamoDB table for usage records
awslocal dynamodb create-table \
  --table-name usage_records \
  --attribute-definitions \
    AttributeName=userId,AttributeType=S \
    AttributeName=timestamp,AttributeType=N \
    AttributeName=date,AttributeType=S \
  --key-schema \
    AttributeName=userId,KeyType=HASH \
    AttributeName=timestamp,KeyType=RANGE \
  --global-secondary-indexes \
    "IndexName=ByDate,KeySchema=[{AttributeName=userId,KeyType=HASH},{AttributeName=date,KeyType=RANGE}],Projection={ProjectionType=ALL},ProvisionedThroughput={ReadCapacityUnits=5,WriteCapacityUnits=5}" \
  --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5

# Create SNS topics for events
awslocal sns create-topic --name user-registered
awslocal sns create-topic --name subscription-changed
awslocal sns create-topic --name usage-alert
awslocal sns create-topic --name invoice-generated

# Create SQS queues
awslocal sqs create-queue --queue-name email-notifications
awslocal sqs create-queue --queue-name billing-updates
awslocal sqs create-queue --name usage-alerts

# Subscribe SQS queues to SNS topics
USER_REGISTERED_ARN=$(awslocal sns list-topics --query "Topics[?contains(TopicArn, 'user-registered')].TopicArn" --output text)
SUBSCRIPTION_CHANGED_ARN=$(awslocal sns list-topics --query "Topics[?contains(TopicArn, 'subscription-changed')].TopicArn" --output text)
USAGE_ALERT_ARN=$(awslocal sns list-topics --query "Topics[?contains(TopicArn, 'usage-alert')].TopicArn" --output text)
INVOICE_GENERATED_ARN=$(awslocal sns list-topics --query "Topics[?contains(TopicArn, 'invoice-generated')].TopicArn" --output text)

EMAIL_QUEUE_ARN=$(awslocal sqs get-queue-attributes --queue-url http://localhost:4566/000000000000/email-notifications --attribute-names QueueArn --query "Attributes.QueueArn" --output text)
BILLING_QUEUE_ARN=$(awslocal sqs get-queue-attributes --queue-url http://localhost:4566/000000000000/billing-updates --attribute-names QueueArn --query "Attributes.QueueArn" --output text)
USAGE_QUEUE_ARN=$(awslocal sqs get-queue-attributes --queue-url http://localhost:4566/000000000000/usage-alerts --attribute-names QueueArn --query "Attributes.QueueArn" --output text)

awslocal sns subscribe --topic-arn $USER_REGISTERED_ARN --protocol sqs --notification-endpoint $EMAIL_QUEUE_ARN
awslocal sns subscribe --topic-arn $SUBSCRIPTION_CHANGED_ARN --protocol sqs --notification-endpoint $BILLING_QUEUE_ARN
awslocal sns subscribe --topic-arn $USAGE_ALERT_ARN --protocol sqs --notification-endpoint $USAGE_QUEUE_ARN

# Create Secrets Manager secrets
awslocal secretsmanager create-secret \
  --name telenor/jwt-secret \
  --secret-string '{"secret":"your-256-bit-secret-key-here-change-in-production-environment"}'

awslocal secretsmanager create-secret \
  --name telenor/database \
  --secret-string '{"host":"postgres","database":"telenor_db","username":"telenor","password":"dev_password"}'

echo "LocalStack initialization complete!"

#!/bin/bash

# AWS Deployment Script for Mitt Telenor Demo
# This script deploys the Vue.js frontend to AWS S3 with CloudFront

set -e

echo "🚀 Starting AWS Deployment for Mitt Telenor Demo..."

# Configuration
BUCKET_NAME="mitt-telenor-demo-${RANDOM}"
REGION="eu-north-1"  # Stockholm region (closest to Norway/Sweden)
DISTRIBUTION_COMMENT="Mitt Telenor Demo CDN"

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${BLUE}📦 Step 1: Building the application...${NC}"
cd frontend
npm run build
cd ..

echo -e "${GREEN}✅ Build completed!${NC}"

echo -e "${BLUE}📦 Step 2: Creating S3 bucket...${NC}"
aws s3 mb s3://${BUCKET_NAME} --region ${REGION}

echo -e "${BLUE}📦 Step 3: Configuring S3 bucket for static website hosting...${NC}"
aws s3 website s3://${BUCKET_NAME} \
  --index-document index.html \
  --error-document index.html

echo -e "${BLUE}📦 Step 4: Setting bucket policy for public access...${NC}"
cat > bucket-policy.json <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "PublicReadGetObject",
      "Effect": "Allow",
      "Principal": "*",
      "Action": "s3:GetObject",
      "Resource": "arn:aws:s3:::${BUCKET_NAME}/*"
    }
  ]
}
EOF

aws s3api put-bucket-policy \
  --bucket ${BUCKET_NAME} \
  --policy file://bucket-policy.json

echo -e "${BLUE}📦 Step 5: Uploading files to S3...${NC}"
aws s3 sync frontend/dist/ s3://${BUCKET_NAME}/ \
  --delete \
  --cache-control "public, max-age=31536000" \
  --exclude "index.html" \
  --exclude "*.map"

# Upload index.html with no-cache
aws s3 cp frontend/dist/index.html s3://${BUCKET_NAME}/index.html \
  --cache-control "no-cache, no-store, must-revalidate"

echo -e "${GREEN}✅ Files uploaded to S3!${NC}"

echo -e "${BLUE}📦 Step 6: Creating CloudFront distribution...${NC}"
cat > cloudfront-config.json <<EOF
{
  "CallerReference": "mitt-telenor-$(date +%s)",
  "Comment": "${DISTRIBUTION_COMMENT}",
  "Enabled": true,
  "DefaultRootObject": "index.html",
  "Origins": {
    "Quantity": 1,
    "Items": [
      {
        "Id": "S3-${BUCKET_NAME}",
        "DomainName": "${BUCKET_NAME}.s3-website.${REGION}.amazonaws.com",
        "CustomOriginConfig": {
          "HTTPPort": 80,
          "HTTPSPort": 443,
          "OriginProtocolPolicy": "http-only"
        }
      }
    ]
  },
  "DefaultCacheBehavior": {
    "TargetOriginId": "S3-${BUCKET_NAME}",
    "ViewerProtocolPolicy": "redirect-to-https",
    "AllowedMethods": {
      "Quantity": 2,
      "Items": ["GET", "HEAD"],
      "CachedMethods": {
        "Quantity": 2,
        "Items": ["GET", "HEAD"]
      }
    },
    "ForwardedValues": {
      "QueryString": false,
      "Cookies": {
        "Forward": "none"
      }
    },
    "MinTTL": 0,
    "DefaultTTL": 86400,
    "MaxTTL": 31536000,
    "Compress": true
  },
  "CustomErrorResponses": {
    "Quantity": 1,
    "Items": [
      {
        "ErrorCode": 404,
        "ResponsePagePath": "/index.html",
        "ResponseCode": "200",
        "ErrorCachingMinTTL": 300
      }
    ]
  }
}
EOF

DISTRIBUTION_ID=$(aws cloudfront create-distribution \
  --distribution-config file://cloudfront-config.json \
  --query 'Distribution.Id' \
  --output text)

DISTRIBUTION_DOMAIN=$(aws cloudfront get-distribution \
  --id ${DISTRIBUTION_ID} \
  --query 'Distribution.DomainName' \
  --output text)

echo -e "${GREEN}✅ CloudFront distribution created!${NC}"

# Clean up temporary files
rm -f bucket-policy.json cloudfront-config.json

echo ""
echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}🎉 Deployment Successful!${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""
echo -e "${YELLOW}S3 Bucket:${NC} ${BUCKET_NAME}"
echo -e "${YELLOW}S3 Website URL:${NC} http://${BUCKET_NAME}.s3-website.${REGION}.amazonaws.com"
echo -e "${YELLOW}CloudFront Distribution ID:${NC} ${DISTRIBUTION_ID}"
echo -e "${YELLOW}CloudFront URL:${NC} https://${DISTRIBUTION_DOMAIN}"
echo ""
echo -e "${BLUE}Note: CloudFront distribution may take 15-20 minutes to fully deploy.${NC}"
echo ""
echo -e "${YELLOW}Next Steps:${NC}"
echo "1. Wait for CloudFront distribution to deploy"
echo "2. Access your app at: https://${DISTRIBUTION_DOMAIN}"
echo "3. (Optional) Configure custom domain in Route 53"
echo "4. (Optional) Add SSL certificate via ACM"
echo ""

# Save deployment info
cat > deployment-info.txt <<EOF
Mitt Telenor Demo - AWS Deployment Information
================================================

Deployment Date: $(date)
Region: ${REGION}

S3 Bucket: ${BUCKET_NAME}
S3 Website URL: http://${BUCKET_NAME}.s3-website.${REGION}.amazonaws.com

CloudFront Distribution ID: ${DISTRIBUTION_ID}
CloudFront URL: https://${DISTRIBUTION_DOMAIN}

Environment Variables:
- VITE_SUPABASE_URL: (from .env file)
- VITE_SUPABASE_ANON_KEY: (from .env file)

To update the deployment:
1. Make changes to your code
2. Run: npm run build (in frontend directory)
3. Run: aws s3 sync frontend/dist/ s3://${BUCKET_NAME}/ --delete
4. Run: aws cloudfront create-invalidation --distribution-id ${DISTRIBUTION_ID} --paths "/*"

To delete the deployment:
1. Run: aws s3 rm s3://${BUCKET_NAME} --recursive
2. Run: aws s3 rb s3://${BUCKET_NAME}
3. Run: aws cloudfront delete-distribution --id ${DISTRIBUTION_ID} --if-match <ETag>
EOF

echo -e "${GREEN}Deployment info saved to deployment-info.txt${NC}"

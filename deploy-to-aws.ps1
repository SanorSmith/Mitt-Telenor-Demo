# AWS Deployment Script for Mitt Telenor Demo (PowerShell)
# This script deploys the Vue.js frontend to AWS S3 with CloudFront

$ErrorActionPreference = "Stop"

Write-Host "🚀 Starting AWS Deployment for Mitt Telenor Demo..." -ForegroundColor Cyan

# Configuration
$BUCKET_NAME = "mitt-telenor-demo-$(Get-Random -Maximum 99999)"
$REGION = "eu-north-1"  # Stockholm region (closest to Norway/Sweden)
$DISTRIBUTION_COMMENT = "Mitt Telenor Demo CDN"

Write-Host "`n📦 Step 1: Building the application..." -ForegroundColor Blue
Set-Location frontend
npm run build
Set-Location ..

Write-Host "✅ Build completed!" -ForegroundColor Green

Write-Host "`n📦 Step 2: Creating S3 bucket..." -ForegroundColor Blue
aws s3 mb "s3://$BUCKET_NAME" --region $REGION

Write-Host "`n📦 Step 3: Configuring S3 bucket for static website hosting..." -ForegroundColor Blue
aws s3 website "s3://$BUCKET_NAME" --index-document index.html --error-document index.html

Write-Host "`n📦 Step 4: Setting bucket policy for public access..." -ForegroundColor Blue
$bucketPolicy = @"
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "PublicReadGetObject",
      "Effect": "Allow",
      "Principal": "*",
      "Action": "s3:GetObject",
      "Resource": "arn:aws:s3:::$BUCKET_NAME/*"
    }
  ]
}
"@

$bucketPolicy | Out-File -FilePath "bucket-policy.json" -Encoding utf8
aws s3api put-bucket-policy --bucket $BUCKET_NAME --policy file://bucket-policy.json

Write-Host "`n📦 Step 5: Uploading files to S3..." -ForegroundColor Blue
aws s3 sync frontend/dist/ "s3://$BUCKET_NAME/" --delete --cache-control "public, max-age=31536000" --exclude "index.html" --exclude "*.map"

# Upload index.html with no-cache
aws s3 cp frontend/dist/index.html "s3://$BUCKET_NAME/index.html" --cache-control "no-cache, no-store, must-revalidate"

Write-Host "✅ Files uploaded to S3!" -ForegroundColor Green

Write-Host "`n📦 Step 6: Creating CloudFront distribution..." -ForegroundColor Blue
$cloudfrontConfig = @"
{
  "CallerReference": "mitt-telenor-$(Get-Date -Format 'yyyyMMddHHmmss')",
  "Comment": "$DISTRIBUTION_COMMENT",
  "Enabled": true,
  "DefaultRootObject": "index.html",
  "Origins": {
    "Quantity": 1,
    "Items": [
      {
        "Id": "S3-$BUCKET_NAME",
        "DomainName": "$BUCKET_NAME.s3-website.$REGION.amazonaws.com",
        "CustomOriginConfig": {
          "HTTPPort": 80,
          "HTTPSPort": 443,
          "OriginProtocolPolicy": "http-only"
        }
      }
    ]
  },
  "DefaultCacheBehavior": {
    "TargetOriginId": "S3-$BUCKET_NAME",
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
"@

$cloudfrontConfig | Out-File -FilePath "cloudfront-config.json" -Encoding utf8

$DISTRIBUTION_ID = aws cloudfront create-distribution --distribution-config file://cloudfront-config.json --query 'Distribution.Id' --output text

$DISTRIBUTION_DOMAIN = aws cloudfront get-distribution --id $DISTRIBUTION_ID --query 'Distribution.DomainName' --output text

Write-Host "✅ CloudFront distribution created!" -ForegroundColor Green

# Clean up temporary files
Remove-Item -Path "bucket-policy.json" -ErrorAction SilentlyContinue
Remove-Item -Path "cloudfront-config.json" -ErrorAction SilentlyContinue

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "🎉 Deployment Successful!" -ForegroundColor Green
Write-Host "========================================`n" -ForegroundColor Green

Write-Host "S3 Bucket: " -ForegroundColor Yellow -NoNewline
Write-Host $BUCKET_NAME

Write-Host "S3 Website URL: " -ForegroundColor Yellow -NoNewline
Write-Host "http://$BUCKET_NAME.s3-website.$REGION.amazonaws.com"

Write-Host "CloudFront Distribution ID: " -ForegroundColor Yellow -NoNewline
Write-Host $DISTRIBUTION_ID

Write-Host "CloudFront URL: " -ForegroundColor Yellow -NoNewline
Write-Host "https://$DISTRIBUTION_DOMAIN"

Write-Host "`nNote: CloudFront distribution may take 15-20 minutes to fully deploy." -ForegroundColor Blue

Write-Host "`nNext Steps:" -ForegroundColor Yellow
Write-Host "1. Wait for CloudFront distribution to deploy"
Write-Host "2. Access your app at: https://$DISTRIBUTION_DOMAIN"
Write-Host "3. (Optional) Configure custom domain in Route 53"
Write-Host "4. (Optional) Add SSL certificate via ACM`n"

# Save deployment info
$deploymentInfo = @"
Mitt Telenor Demo - AWS Deployment Information
================================================

Deployment Date: $(Get-Date)
Region: $REGION

S3 Bucket: $BUCKET_NAME
S3 Website URL: http://$BUCKET_NAME.s3-website.$REGION.amazonaws.com

CloudFront Distribution ID: $DISTRIBUTION_ID
CloudFront URL: https://$DISTRIBUTION_DOMAIN

Environment Variables:
- VITE_SUPABASE_URL: (from .env file)
- VITE_SUPABASE_ANON_KEY: (from .env file)

To update the deployment:
1. Make changes to your code
2. Run: npm run build (in frontend directory)
3. Run: aws s3 sync frontend/dist/ s3://$BUCKET_NAME/ --delete
4. Run: aws cloudfront create-invalidation --distribution-id $DISTRIBUTION_ID --paths "/*"

To delete the deployment:
1. Run: aws s3 rm s3://$BUCKET_NAME --recursive
2. Run: aws s3 rb s3://$BUCKET_NAME
3. Run: aws cloudfront delete-distribution --id $DISTRIBUTION_ID --if-match <ETag>
"@

$deploymentInfo | Out-File -FilePath "deployment-info.txt" -Encoding utf8

Write-Host "Deployment info saved to deployment-info.txt" -ForegroundColor Green

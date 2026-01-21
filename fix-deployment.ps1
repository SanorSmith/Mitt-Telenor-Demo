# Fix AWS Deployment Issues
$ErrorActionPreference = "Stop"

$BUCKET_NAME = "mitt-telenor-demo-48625"
$REGION = "eu-north-1"

Write-Host "🔧 Fixing AWS Deployment Issues..." -ForegroundColor Cyan

# Step 1: Verify bucket exists
Write-Host "`n1. Checking S3 bucket..." -ForegroundColor Blue
try {
    aws s3 ls s3://$BUCKET_NAME | Out-Null
    Write-Host "   ✅ Bucket exists" -ForegroundColor Green
} catch {
    Write-Host "   ❌ Bucket not found" -ForegroundColor Red
    exit 1
}

# Step 2: Enable public access
Write-Host "`n2. Enabling public access..." -ForegroundColor Blue
aws s3api put-public-access-block `
    --bucket $BUCKET_NAME `
    --public-access-block-configuration "BlockPublicAcls=false,IgnorePublicAcls=false,BlockPublicPolicy=false,RestrictPublicBuckets=false"
Write-Host "   ✅ Public access enabled" -ForegroundColor Green

# Step 3: Configure website hosting
Write-Host "`n3. Configuring website hosting..." -ForegroundColor Blue
aws s3 website s3://$BUCKET_NAME `
    --index-document index.html `
    --error-document index.html
Write-Host "   ✅ Website configuration applied" -ForegroundColor Green

# Step 4: Apply bucket policy
Write-Host "`n4. Applying bucket policy..." -ForegroundColor Blue
$policy = @"
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
$policy | Out-File -FilePath "temp-policy.json" -Encoding ASCII
aws s3api put-bucket-policy --bucket $BUCKET_NAME --policy file://temp-policy.json
Remove-Item "temp-policy.json"
Write-Host "   ✅ Bucket policy applied" -ForegroundColor Green

# Step 5: Verify files are uploaded
Write-Host "`n5. Checking uploaded files..." -ForegroundColor Blue
$fileCount = (aws s3 ls s3://$BUCKET_NAME/ --recursive | Measure-Object).Count
if ($fileCount -gt 0) {
    Write-Host "   ✅ $fileCount files found in bucket" -ForegroundColor Green
} else {
    Write-Host "   ⚠️  No files found - uploading..." -ForegroundColor Yellow
    if (Test-Path "frontend/dist") {
        aws s3 sync frontend/dist/ s3://$BUCKET_NAME/ --delete
        Write-Host "   ✅ Files uploaded" -ForegroundColor Green
    } else {
        Write-Host "   ❌ frontend/dist not found - run 'npm run build' first" -ForegroundColor Red
    }
}

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "✅ Deployment Fixed!" -ForegroundColor Green
Write-Host "========================================`n" -ForegroundColor Green

$S3_URL = "http://$BUCKET_NAME.s3-website.$REGION.amazonaws.com"

Write-Host "Your app is now accessible at:" -ForegroundColor Yellow
Write-Host $S3_URL -ForegroundColor Cyan

Write-Host "`nTesting S3 website..." -ForegroundColor Blue
try {
    $response = Invoke-WebRequest -Uri $S3_URL -Method Head -TimeoutSec 10
    if ($response.StatusCode -eq 200) {
        Write-Host "✅ S3 website is working!" -ForegroundColor Green
    }
} catch {
    Write-Host "⚠️  S3 website check failed: $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host "   Try accessing manually: $S3_URL" -ForegroundColor Cyan
}

Write-Host "`nCloudFront URL (may take 15-20 min to activate):" -ForegroundColor Yellow
Write-Host "https://dhhho0vm7geyy.cloudfront.net" -ForegroundColor Cyan

Write-Host "`nNote: If CloudFront still doesn't work, it's still deploying globally." -ForegroundColor Blue
Write-Host "Use the S3 URL above for immediate access.`n" -ForegroundColor Blue

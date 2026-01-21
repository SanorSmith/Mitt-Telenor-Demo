# Complete AWS Deployment - Fix CloudFront
$ErrorActionPreference = "Stop"

$BUCKET_NAME = "mitt-telenor-demo-48625"
$REGION = "eu-north-1"

Write-Host "🚀 Completing AWS Deployment - Creating CloudFront Distribution..." -ForegroundColor Cyan

# Create CloudFront distribution config without BOM
$cloudfrontConfig = @{
    CallerReference = "mitt-telenor-$(Get-Date -Format 'yyyyMMddHHmmss')"
    Comment = "Mitt Telenor Demo CDN"
    Enabled = $true
    DefaultRootObject = "index.html"
    Origins = @{
        Quantity = 1
        Items = @(
            @{
                Id = "S3-$BUCKET_NAME"
                DomainName = "$BUCKET_NAME.s3-website.$REGION.amazonaws.com"
                CustomOriginConfig = @{
                    HTTPPort = 80
                    HTTPSPort = 443
                    OriginProtocolPolicy = "http-only"
                }
            }
        )
    }
    DefaultCacheBehavior = @{
        TargetOriginId = "S3-$BUCKET_NAME"
        ViewerProtocolPolicy = "redirect-to-https"
        AllowedMethods = @{
            Quantity = 2
            Items = @("GET", "HEAD")
            CachedMethods = @{
                Quantity = 2
                Items = @("GET", "HEAD")
            }
        }
        ForwardedValues = @{
            QueryString = $false
            Cookies = @{
                Forward = "none"
            }
        }
        MinTTL = 0
        DefaultTTL = 86400
        MaxTTL = 31536000
        Compress = $true
    }
    CustomErrorResponses = @{
        Quantity = 1
        Items = @(
            @{
                ErrorCode = 404
                ResponsePagePath = "/index.html"
                ResponseCode = "200"
                ErrorCachingMinTTL = 300
            }
        )
    }
}

# Convert to JSON and save without BOM
$jsonConfig = $cloudfrontConfig | ConvertTo-Json -Depth 10
[System.IO.File]::WriteAllLines("$PWD\cloudfront-config.json", $jsonConfig)

Write-Host "Creating CloudFront distribution..." -ForegroundColor Blue

try {
    $result = aws cloudfront create-distribution --distribution-config file://cloudfront-config.json --output json | ConvertFrom-Json
    
    $DISTRIBUTION_ID = $result.Distribution.Id
    $DISTRIBUTION_DOMAIN = $result.Distribution.DomainName
    
    Write-Host "✅ CloudFront distribution created successfully!" -ForegroundColor Green
    
    Write-Host "`n========================================" -ForegroundColor Green
    Write-Host "🎉 Deployment Complete!" -ForegroundColor Green
    Write-Host "========================================`n" -ForegroundColor Green
    
    Write-Host "S3 Bucket: " -ForegroundColor Yellow -NoNewline
    Write-Host $BUCKET_NAME
    
    Write-Host "S3 Website URL: " -ForegroundColor Yellow -NoNewline
    Write-Host "http://$BUCKET_NAME.s3-website.$REGION.amazonaws.com"
    
    Write-Host "CloudFront Distribution ID: " -ForegroundColor Yellow -NoNewline
    Write-Host $DISTRIBUTION_ID
    
    Write-Host "CloudFront URL: " -ForegroundColor Yellow -NoNewline
    Write-Host "https://$DISTRIBUTION_DOMAIN"
    
    Write-Host "`n✨ Your app is now live at: https://$DISTRIBUTION_DOMAIN" -ForegroundColor Cyan
    Write-Host "⏳ CloudFront may take 15-20 minutes to fully deploy globally.`n" -ForegroundColor Blue
    
    # Update deployment info
    $deploymentInfo = @"
Mitt Telenor Demo - AWS Deployment Information
================================================

Deployment Date: $(Get-Date)
Region: $REGION

S3 Bucket: $BUCKET_NAME
S3 Website URL: http://$BUCKET_NAME.s3-website.$REGION.amazonaws.com

CloudFront Distribution ID: $DISTRIBUTION_ID
CloudFront URL: https://$DISTRIBUTION_DOMAIN

Access your application:
- S3 Direct: http://$BUCKET_NAME.s3-website.$REGION.amazonaws.com
- CloudFront (HTTPS): https://$DISTRIBUTION_DOMAIN

Environment Variables (configured in .env):
- VITE_SUPABASE_URL: https://vdxcfvvwomshydjuydoa.supabase.co
- VITE_SUPABASE_ANON_KEY: (from .env file)

To update the deployment:
1. Make changes to your code
2. Run: cd frontend && npm run build && cd ..
3. Run: aws s3 sync frontend/dist/ s3://$BUCKET_NAME/ --delete
4. Run: aws cloudfront create-invalidation --distribution-id $DISTRIBUTION_ID --paths "/*"

To delete the deployment:
1. Disable distribution: aws cloudfront get-distribution-config --id $DISTRIBUTION_ID
2. Delete files: aws s3 rm s3://$BUCKET_NAME --recursive
3. Delete bucket: aws s3 rb s3://$BUCKET_NAME
4. Delete distribution after disabled
"@
    
    $deploymentInfo | Out-File -FilePath "deployment-info.txt" -Encoding utf8
    Write-Host "📄 Deployment info saved to deployment-info.txt" -ForegroundColor Green
    
} catch {
    Write-Host "❌ Error creating CloudFront distribution: $_" -ForegroundColor Red
    Write-Host "`nYour app is still accessible via S3:" -ForegroundColor Yellow
    Write-Host "http://$BUCKET_NAME.s3-website.$REGION.amazonaws.com" -ForegroundColor Cyan
}

# Clean up
Remove-Item -Path "cloudfront-config.json" -ErrorAction SilentlyContinue

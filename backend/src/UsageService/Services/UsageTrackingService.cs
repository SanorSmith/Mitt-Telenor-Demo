using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using UsageService.Models;
using Shared.Messaging;
using Shared.Events;

namespace UsageService.Services;

public class UsageTrackingService : IUsageTrackingService
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly IEventPublisher _eventPublisher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UsageTrackingService> _logger;
    private readonly string _tableName;

    public UsageTrackingService(
        IAmazonDynamoDB dynamoDb,
        IEventPublisher eventPublisher,
        IConfiguration _configuration,
        ILogger<UsageTrackingService> logger)
    {
        _dynamoDb = dynamoDb;
        _eventPublisher = eventPublisher;
        this._configuration = _configuration;
        _logger = logger;
        _tableName = _configuration["AWS:DynamoDBTableName"]!;
    }

    public async Task RecordUsageAsync(Guid userId, RecordUsageRequest request)
    {
        var period = DateTime.UtcNow.ToString("yyyy-MM");
        var timestamp = DateTime.UtcNow;

        var recordItem = new Dictionary<string, AttributeValue>
        {
            ["Id"] = new AttributeValue { S = Guid.NewGuid().ToString() },
            ["UserId"] = new AttributeValue { S = userId.ToString() },
            ["Type"] = new AttributeValue { S = request.Type },
            ["Amount"] = new AttributeValue { N = request.Amount.ToString() },
            ["Timestamp"] = new AttributeValue { S = timestamp.ToString("O") },
            ["Period"] = new AttributeValue { S = period }
        };

        await _dynamoDb.PutItemAsync(new PutItemRequest
        {
            TableName = _tableName,
            Item = recordItem
        });

        await UpdateUsageSummaryAsync(userId, period, request.Type, request.Amount);

        _logger.LogInformation("Recorded {Type} usage of {Amount} for user {UserId}",
            request.Type, request.Amount, userId);

        var summary = await GetUsageSummaryForAlertsAsync(userId, period);
        await CheckAndSendAlertsAsync(userId, summary);
    }

    private async Task UpdateUsageSummaryAsync(Guid userId, string period, string type, double amount)
    {
        var key = new Dictionary<string, AttributeValue>
        {
            ["UserId"] = new AttributeValue { S = userId.ToString() },
            ["Period"] = new AttributeValue { S = period }
        };

        var attributeName = type switch
        {
            "Data" => "DataUsedMB",
            "Voice" => "VoiceMinutesUsed",
            "Sms" => "SmsUsed",
            _ => throw new ArgumentException($"Invalid usage type: {type}")
        };

        var updateRequest = new UpdateItemRequest
        {
            TableName = _tableName,
            Key = key,
            UpdateExpression = $"ADD {attributeName} :amount SET LastUpdated = :timestamp",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                [":amount"] = new AttributeValue { N = amount.ToString() },
                [":timestamp"] = new AttributeValue { S = DateTime.UtcNow.ToString("O") }
            }
        };

        await _dynamoDb.UpdateItemAsync(updateRequest);
    }

    private async Task<UsageSummary> GetUsageSummaryForAlertsAsync(Guid userId, string period)
    {
        var key = new Dictionary<string, AttributeValue>
        {
            ["UserId"] = new AttributeValue { S = userId.ToString() },
            ["Period"] = new AttributeValue { S = period }
        };

        var response = await _dynamoDb.GetItemAsync(new GetItemRequest
        {
            TableName = _tableName,
            Key = key
        });

        if (!response.IsItemSet)
        {
            return new UsageSummary
            {
                Period = period,
                Data = new DataUsage { AllowanceMB = 10240 },
                Voice = new VoiceUsage { AllowanceMinutes = 500 },
                Sms = new SmsUsage { AllowanceCount = 200 }
            };
        }

        var item = response.Item;
        var dataUsed = item.ContainsKey("DataUsedMB") ? double.Parse(item["DataUsedMB"].N) : 0;
        var voiceUsed = item.ContainsKey("VoiceMinutesUsed") ? int.Parse(item["VoiceMinutesUsed"].N) : 0;
        var smsUsed = item.ContainsKey("SmsUsed") ? int.Parse(item["SmsUsed"].N) : 0;

        const double dataAllowance = 10240;
        const int voiceAllowance = 500;
        const int smsAllowance = 200;

        return new UsageSummary
        {
            Period = period,
            Data = new DataUsage
            {
                UsedMB = dataUsed,
                AllowanceMB = dataAllowance,
                PercentageUsed = (dataUsed / dataAllowance) * 100
            },
            Voice = new VoiceUsage
            {
                UsedMinutes = voiceUsed,
                AllowanceMinutes = voiceAllowance,
                PercentageUsed = ((double)voiceUsed / voiceAllowance) * 100
            },
            Sms = new SmsUsage
            {
                UsedCount = smsUsed,
                AllowanceCount = smsAllowance,
                PercentageUsed = ((double)smsUsed / smsAllowance) * 100
            }
        };
    }

    private async Task CheckAndSendAlertsAsync(Guid userId, UsageSummary summary)
    {
        var thresholdPercent = double.Parse(_configuration["UsageAlerts:DataThresholdPercent"]!);

        if (summary.Data.PercentageUsed >= thresholdPercent)
        {
            await _eventPublisher.PublishAsync(
                _configuration["AWS:SnsTopicArn"]!,
                new UsageAlertEvent
                {
                    UserId = userId,
                    AlertType = "Data",
                    CurrentUsage = (decimal)summary.Data.UsedMB,
                    Allowance = (decimal)summary.Data.AllowanceMB,
                    PercentageUsed = (decimal)summary.Data.PercentageUsed,
                    Timestamp = DateTime.UtcNow
                });

            _logger.LogWarning("Data usage alert sent for user {UserId}: {Percentage}% used",
                userId, summary.Data.PercentageUsed);
        }

        if (summary.Voice.PercentageUsed >= thresholdPercent)
        {
            await _eventPublisher.PublishAsync(
                _configuration["AWS:SnsTopicArn"]!,
                new UsageAlertEvent
                {
                    UserId = userId,
                    AlertType = "Voice",
                    CurrentUsage = summary.Voice.UsedMinutes,
                    Allowance = summary.Voice.AllowanceMinutes,
                    PercentageUsed = (decimal)summary.Voice.PercentageUsed,
                    Timestamp = DateTime.UtcNow
                });
        }

        if (summary.Sms.PercentageUsed >= thresholdPercent)
        {
            await _eventPublisher.PublishAsync(
                _configuration["AWS:SnsTopicArn"]!,
                new UsageAlertEvent
                {
                    UserId = userId,
                    AlertType = "SMS",
                    CurrentUsage = summary.Sms.UsedCount,
                    Allowance = summary.Sms.AllowanceCount,
                    PercentageUsed = (decimal)summary.Sms.PercentageUsed,
                    Timestamp = DateTime.UtcNow
                });
        }
    }
}

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using UsageService.Models;

namespace UsageService.Services;

public class UsageService : IUsageService
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UsageService> _logger;
    private readonly string _tableName;

    public UsageService(
        IAmazonDynamoDB dynamoDb,
        IConfiguration configuration,
        ILogger<UsageService> logger)
    {
        _dynamoDb = dynamoDb;
        _configuration = configuration;
        _logger = logger;
        _tableName = _configuration["AWS:DynamoDBTableName"]!;
    }

    public async Task<UsageSummary> GetCurrentUsageAsync(Guid userId)
    {
        var period = DateTime.UtcNow.ToString("yyyy-MM");
        var key = new Dictionary<string, AttributeValue>
        {
            ["UserId"] = new AttributeValue { S = userId.ToString() },
            ["Period"] = new AttributeValue { S = period }
        };

        var request = new GetItemRequest
        {
            TableName = _tableName,
            Key = key
        };

        var response = await _dynamoDb.GetItemAsync(request);

        if (!response.IsItemSet)
        {
            return new UsageSummary
            {
                Period = period,
                Data = new DataUsage { AllowanceMB = 10240 },
                Voice = new VoiceUsage { AllowanceMinutes = 500 },
                Sms = new SmsUsage { AllowanceCount = 200 },
                LastUpdated = DateTime.UtcNow
            };
        }

        var item = response.Item;
        var dataUsed = double.Parse(item["DataUsedMB"].N);
        var voiceUsed = int.Parse(item["VoiceMinutesUsed"].N);
        var smsUsed = int.Parse(item["SmsUsed"].N);

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
                PercentageUsed = (dataUsed / dataAllowance) * 100,
                RemainingMB = dataAllowance - dataUsed
            },
            Voice = new VoiceUsage
            {
                UsedMinutes = voiceUsed,
                AllowanceMinutes = voiceAllowance,
                PercentageUsed = ((double)voiceUsed / voiceAllowance) * 100,
                RemainingMinutes = voiceAllowance - voiceUsed
            },
            Sms = new SmsUsage
            {
                UsedCount = smsUsed,
                AllowanceCount = smsAllowance,
                PercentageUsed = ((double)smsUsed / smsAllowance) * 100,
                RemainingCount = smsAllowance - smsUsed
            },
            LastUpdated = DateTime.Parse(item["LastUpdated"].S)
        };
    }

    public async Task<List<DailyUsage>> GetDailyUsageAsync(Guid userId, int days = 30)
    {
        var dailyUsage = new List<DailyUsage>();
        var startDate = DateTime.UtcNow.AddDays(-days);

        for (int i = 0; i < days; i++)
        {
            var date = startDate.AddDays(i);
            var random = new Random(userId.GetHashCode() + i);

            dailyUsage.Add(new DailyUsage
            {
                Date = date.ToString("yyyy-MM-dd"),
                DataMB = random.Next(100, 500),
                VoiceMinutes = random.Next(5, 30),
                SmsCount = random.Next(1, 10)
            });
        }

        return dailyUsage;
    }

    public async Task<UsageHistoryResponse> GetUsageHistoryAsync(Guid userId, UsageHistoryRequest request)
    {
        var queryRequest = new QueryRequest
        {
            TableName = _tableName,
            IndexName = "UserId-Timestamp-Index",
            KeyConditionExpression = "UserId = :userId",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                [":userId"] = new AttributeValue { S = userId.ToString() }
            },
            Limit = request.PageSize,
            ScanIndexForward = false
        };

        if (!string.IsNullOrEmpty(request.LastEvaluatedKey))
        {
            queryRequest.ExclusiveStartKey = new Dictionary<string, AttributeValue>
            {
                ["UserId"] = new AttributeValue { S = userId.ToString() },
                ["Timestamp"] = new AttributeValue { S = request.LastEvaluatedKey }
            };
        }

        var response = await _dynamoDb.QueryAsync(queryRequest);

        var records = response.Items.Select(item => new UsageRecord
        {
            Id = item.ContainsKey("Id") ? item["Id"].S : Guid.NewGuid().ToString(),
            UserId = item["UserId"].S,
            Type = item.ContainsKey("Type") ? item["Type"].S : "Data",
            Amount = item.ContainsKey("Amount") ? double.Parse(item["Amount"].N) : 0,
            Timestamp = item.ContainsKey("Timestamp") ? DateTime.Parse(item["Timestamp"].S) : DateTime.UtcNow
        }).ToList();

        return new UsageHistoryResponse
        {
            Records = records,
            NextPageToken = response.LastEvaluatedKey?.ContainsKey("Timestamp") == true
                ? response.LastEvaluatedKey["Timestamp"].S
                : null,
            TotalCount = records.Count
        };
    }
}

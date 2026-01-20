using System.ComponentModel.DataAnnotations;

namespace UsageService.Models;

public class RecordUsageRequest
{
    [Required]
    [RegularExpression("^(Data|Voice|Sms)$")]
    public string Type { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public double Amount { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = new();
}

public class UsageHistoryRequest
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Type { get; set; }
    public int PageSize { get; set; } = 50;
    public string? LastEvaluatedKey { get; set; }
}

public class UsageHistoryResponse
{
    public List<UsageRecord> Records { get; set; } = new();
    public string? NextPageToken { get; set; }
    public int TotalCount { get; set; }
}

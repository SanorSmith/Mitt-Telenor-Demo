namespace UsageService.Models;

public class UsageData
{
    public string UserId { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public double DataUsedMB { get; set; }
    public int VoiceMinutesUsed { get; set; }
    public int SmsUsed { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class UsageRecord
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public double Amount { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}

public class UsageSummary
{
    public string Period { get; set; } = string.Empty;
    public DataUsage Data { get; set; } = new();
    public VoiceUsage Voice { get; set; } = new();
    public SmsUsage Sms { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

public class DataUsage
{
    public double UsedMB { get; set; }
    public double AllowanceMB { get; set; }
    public double PercentageUsed { get; set; }
    public double RemainingMB { get; set; }
}

public class VoiceUsage
{
    public int UsedMinutes { get; set; }
    public int AllowanceMinutes { get; set; }
    public double PercentageUsed { get; set; }
    public int RemainingMinutes { get; set; }
}

public class SmsUsage
{
    public int UsedCount { get; set; }
    public int AllowanceCount { get; set; }
    public double PercentageUsed { get; set; }
    public int RemainingCount { get; set; }
}

public class DailyUsage
{
    public string Date { get; set; } = string.Empty;
    public double DataMB { get; set; }
    public int VoiceMinutes { get; set; }
    public int SmsCount { get; set; }
}

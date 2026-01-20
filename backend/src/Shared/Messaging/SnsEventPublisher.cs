using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Shared.Messaging;

public class SnsEventPublisher : IEventPublisher
{
    private readonly IAmazonSimpleNotificationService _snsClient;
    private readonly ILogger<SnsEventPublisher> _logger;

    public SnsEventPublisher(IAmazonSimpleNotificationService snsClient, ILogger<SnsEventPublisher> logger)
    {
        _snsClient = snsClient;
        _logger = logger;
    }

    public async Task PublishAsync<T>(string topicArn, T eventData) where T : class
    {
        try
        {
            var message = JsonConvert.SerializeObject(eventData);
            var request = new PublishRequest
            {
                TopicArn = topicArn,
                Message = message,
                Subject = typeof(T).Name
            };

            var response = await _snsClient.PublishAsync(request);
            _logger.LogInformation("Event published to SNS: {EventType}, MessageId: {MessageId}", 
                typeof(T).Name, response.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event to SNS: {EventType}", typeof(T).Name);
            throw;
        }
    }
}

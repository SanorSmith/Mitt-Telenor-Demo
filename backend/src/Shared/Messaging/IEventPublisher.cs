namespace Shared.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<T>(string topicArn, T eventData) where T : class;
}

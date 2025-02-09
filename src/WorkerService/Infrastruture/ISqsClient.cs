// ISqsClient.cs
namespace WorkerService.Infrastructure;

using Amazon.SQS.Model;

public interface ISqsClient
{
    Task SendMessageAsync(string queueUrl, string messageBody);
    Task<ReceiveMessageResponse> ReceiveMessageAsync(string queueUrl);
    Task DeleteMessageAsync(string queueUrl, Message messageId);
}
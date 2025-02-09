// SqsClient.cs
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Threading.Tasks;

namespace WorkerService.Infrastructure;

public class SqsClient : ISqsClient
{
    private readonly IAmazonSQS _sqsClient;

    public SqsClient(IAmazonSQS sqsClient)
    {
        _sqsClient = sqsClient;
    }

    public virtual async Task SendMessageAsync(string queueUrl, string messageBody)
    {
        var request = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = messageBody
        };

        await _sqsClient.SendMessageAsync(request);
    }

    public async Task<ReceiveMessageResponse> ReceiveMessageAsync(string queueUrl)
    {
        var request = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl,
            MaxNumberOfMessages = 10
        };

        return await _sqsClient.ReceiveMessageAsync(request);
    }

    public virtual async Task DeleteMessageAsync(string queueUrl, Message message)
    {
        await _sqsClient.DeleteMessageAsync(new DeleteMessageRequest
        {
            QueueUrl = queueUrl,
            ReceiptHandle = message.ReceiptHandle
        });
    }
}
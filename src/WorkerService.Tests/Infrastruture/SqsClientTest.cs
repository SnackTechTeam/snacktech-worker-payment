using Amazon.SQS;
using Amazon.SQS.Model;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace WorkerService.Infrastructure.Tests
{
    public class SqsClientTests
    {
        private readonly Mock<IAmazonSQS> _mockSqsClient;
        private readonly SqsClient _sqsClient;

        public SqsClientTests()
        {
            _mockSqsClient = new Mock<IAmazonSQS>();
            _sqsClient = new SqsClient(_mockSqsClient.Object);
        }

        #region SendMessageAsync Tests

        [Fact]
        public async Task SendMessageAsync_Should_Invoke_SendMessage_With_Correct_Parameters()
        {
            // Arrange
            const string queueUrl = "https://queue.example.com";
            const string messageBody = "Test message";

            _mockSqsClient.Setup(x => x.SendMessageAsync(
                It.Is<SendMessageRequest>(r =>
                    r.QueueUrl == queueUrl &&
                    r.MessageBody == messageBody), CancellationToken.None)
                ).ReturnsAsync(new SendMessageResponse()).Verifiable();

            // Act
            await _sqsClient.SendMessageAsync(queueUrl, messageBody);

            // Assert
            _mockSqsClient.Verify(x => x.SendMessageAsync(
                It.Is<SendMessageRequest>(r =>
                    r.QueueUrl == queueUrl &&
                    r.MessageBody == messageBody), CancellationToken.None), Times.Once);
        }

        #endregion

        #region ReceiveMessageAsync Tests

        [Fact]
        public async Task ReceiveMessageAsync_Should_Return_Messages_When_Available()
        {
            // Arrange
            const string queueUrl = "https://queue.example.com";
            var mockMessages = new List<Message>
            {
                new Message { Body = "message1", ReceiptHandle = "handle1" },
                new Message { Body = "message2", ReceiptHandle = "handle2" }
            };

            _mockSqsClient.Setup(x => x.ReceiveMessageAsync(
                It.Is<ReceiveMessageRequest>(r => r.QueueUrl == queueUrl), CancellationToken.None))
                .ReturnsAsync(new ReceiveMessageResponse { Messages = mockMessages })
                .Verifiable();

            // Act
            var response = await _sqsClient.ReceiveMessageAsync(queueUrl);

            // Assert
            Assert.Equal(mockMessages.Count, response.Messages.Count);
            Assert.Contains(mockMessages[0], response.Messages);
        }

        [Fact]
        public async Task ReceiveMessageAsync_Should_Return_Empty_List_When_No_Messages_Are_Present()
        {
            // Arrange
            const string queueUrl = "https://queue.example.com";

            _mockSqsClient.Setup(x => x.ReceiveMessageAsync(
                It.Is<ReceiveMessageRequest>(r => r.QueueUrl == queueUrl), CancellationToken.None))
                .ReturnsAsync(new ReceiveMessageResponse { Messages = new List<Message>() })
                .Verifiable();

            // Act
            var response = await _sqsClient.ReceiveMessageAsync(queueUrl);

            // Assert
            Assert.Empty(response.Messages);
        }

        #endregion

        #region DeleteMessageAsync Tests

        [Fact]
        public async Task DeleteMessageAsync_Should_Invoke_DeleteMessage_With_Correct_Parameters()
        {
            // Arrange
            const string queueUrl = "https://queue.example.com";
            var message = new Message { ReceiptHandle = "test-handle" };

            _mockSqsClient.Setup(x => x.DeleteMessageAsync(
                It.Is<DeleteMessageRequest>(r =>
                    r.QueueUrl == queueUrl &&
                    r.ReceiptHandle == message.ReceiptHandle), CancellationToken.None))
                .ReturnsAsync(new DeleteMessageResponse())
                .Verifiable();

            // Act
            await _sqsClient.DeleteMessageAsync(queueUrl, message);

            // Assert
            _mockSqsClient.Verify(x => x.DeleteMessageAsync(
                It.Is<DeleteMessageRequest>(r =>
                    r.QueueUrl == queueUrl &&
                    r.ReceiptHandle == message.ReceiptHandle), CancellationToken.None), Times.Once);
        }

        #endregion
    }
}
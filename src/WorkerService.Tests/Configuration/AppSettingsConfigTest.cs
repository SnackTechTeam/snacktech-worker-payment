using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Xunit;

namespace WorkerService.Configuration.Tests
{
    public class AppSettingsConfigTests
    {
        [Fact]
        public void LoadConfiguration_ShouldLoadAwsConfig_WhenValidConfigurationExists()
        {
            // Arrange
            var configBuilderMock = MakeConfigBuilderMock();

            // Act
            var appSettingsConfig = AppSettingsConfig.LoadConfiguration(configBuilderMock);

            // Assert
            Assert.NotNull(appSettingsConfig);
            Assert.NotNull(appSettingsConfig.Aws);
            Assert.Equal("queue.com", appSettingsConfig.Aws.QueueURL);
            Assert.Equal("dlq.com", appSettingsConfig.Aws.DlqQueueURL);
            Assert.Equal("us-west-2", appSettingsConfig.Aws.RegionEndpoint);
            Assert.Equal("https://sqs.us-west-2.amazonaws.com", appSettingsConfig.Aws.ServiceURL);
            Assert.Equal("test-access-key", appSettingsConfig.Aws.AccessKey);
            Assert.Equal("test-secret-key", appSettingsConfig.Aws.SecretKey);
        }

        internal static MockConfigurationBuilder MakeConfigBuilderMock()
        {
            var inMemorySettings = new Dictionary<string, string?> {
                {"Aws:QueueURL", "queue.com"},
                {"Aws:DlqQueueURL", "dlq.com"},
                {"Aws:RegionEndpoint", "us-west-2"},
                {"Aws:ServiceURL", "https://sqs.us-west-2.amazonaws.com"},
                {"Aws:AccessKey", "test-access-key"},
                {"Aws:SecretKey", "test-secret-key"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var configBuilderMock = new MockConfigurationBuilder(configuration);
            
            return configBuilderMock;
        }

        // Mock ConfigurationBuilder to simulate the behavior of loading from JSON files
        internal class MockConfigurationBuilder : IConfigurationBuilder
        {
            private readonly IConfigurationRoot _configuration;

            public MockConfigurationBuilder(IConfigurationRoot configuration)
            {
                _configuration = configuration;
            }

            public IConfigurationBuilder Add(IConfigurationSource source) => this;

            public IConfigurationRoot Build() => _configuration;

            public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();

            public IList<IConfigurationSource> Sources { get; } = new List<IConfigurationSource>();
        }
    }
}
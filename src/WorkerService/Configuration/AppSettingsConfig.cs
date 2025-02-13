namespace WorkerService.Configuration;

public class AppSettingsConfig
{
    public AwsConfig Aws { get; private set; } = new AwsConfig();

    private AppSettingsConfig(){}

    public static AppSettingsConfig LoadConfiguration(IConfigurationBuilder builder)
    {
        var configuration = builder.Build();

#pragma warning disable CS8601 // Possible null reference assignment.
        var AwsConfig = new AwsConfig(){
            QueueURL = configuration["Aws:QueueURL"],
            DlqQueueURL = configuration["Aws:DlqQueueURL"],
            ServiceURL = configuration["Aws:ServiceURL"],
            AccessKey = configuration["Aws:AccessKey"],
            SecretKey = configuration["Aws:SecretKey"]
        };
#pragma warning restore CS8601 // Possible null reference assignment.

        return new AppSettingsConfig
        {
            Aws = AwsConfig
        };
    }
}

public class AwsConfig 
{
    public string AccessKey { get; set; }  = String.Empty;
    public string SecretKey { get; set; }  = String.Empty;
    public string ServiceURL { get; set; }  = String.Empty;
    public string QueueURL { get; set; } = String.Empty;
    public string DlqQueueURL { get; set; }  = String.Empty;
}
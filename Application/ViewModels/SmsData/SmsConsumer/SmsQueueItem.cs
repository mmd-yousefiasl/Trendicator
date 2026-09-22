namespace Application.ViewModels.SmsData.SmsConsumer
{
    public record SmsQueueItem(
    string Mobile,
    string ApiKey,
    int PatternId,
    List<SmsParameterItem> Parameters);
}

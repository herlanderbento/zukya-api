namespace Zukya.Application.Common.Interfaces;

public abstract class SmsModel
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public int ExpireMinutes { get; set; }
}

public interface ISmsProvider
{
    Task SendSms(SmsModel model, CancellationToken cancellationToken = default);
}

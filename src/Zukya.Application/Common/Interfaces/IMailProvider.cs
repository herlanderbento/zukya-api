namespace Zukya.Application.Common.Interfaces;

public interface IMailProvider
{
    public Task Send(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default
    );
    public Task SendFromTemplate<TModel>(
        string to,
        string subject,
        string templateKey,
        TModel model,
        CancellationToken cancellationToken = default
    );
}

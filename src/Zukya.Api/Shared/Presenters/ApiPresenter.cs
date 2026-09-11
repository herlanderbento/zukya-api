namespace Zukya.Api.Shared.Presenters;

public class ApiPresenter<TData>(TData data)
{
    public TData Data { get; private set; } = data;
}

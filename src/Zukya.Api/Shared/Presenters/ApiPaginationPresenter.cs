namespace Zukya.Api.Shared.Presenters;

public class ApiPaginationPresenter
{
    public int CurrentPage { get; set; }
    private int PerPage { get; }
    private int Total { get; }
    public int LastPage { get; private set; }

    public ApiPaginationPresenter(int currentPage, int perPage, int total)
    {
        CurrentPage = currentPage;
        PerPage = perPage;
        Total = total;
        LastPage = (int)Math.Ceiling((double)Total / PerPage);
    }
}

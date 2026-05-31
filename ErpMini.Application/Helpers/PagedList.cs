// ErpMini.Application/Helpers/PagedList.cs
namespace ErpMini.Application.Helpers;

public class PagedList<T>
{
    public List<T> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrev => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;

    public static PagedList<T> Create(IEnumerable<T> source,
        int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedList<T>
        {
            Items       = items,
            PageNumber  = pageNumber,
            PageSize    = pageSize,
            TotalCount  = count
        };
    }
}
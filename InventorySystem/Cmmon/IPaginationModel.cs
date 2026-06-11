namespace InventorySystem.Cmmon;

public interface IPaginationModel
{
    int PageNumber { get; }
    int PageSize { get; }
    int TotalItems { get; }
    int TotalPages { get; }
    bool HasPreviousPage { get; }
    bool HasNextPage { get; }
}
public class PaginationModel<T> : IPaginationModel
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

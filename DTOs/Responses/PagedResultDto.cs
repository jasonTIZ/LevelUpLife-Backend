public class PagedResultDto<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalRecords { get; set; }     
    public int TotalPages { get; set; }       
    public int CurrentPage { get; set; }     
    public int PageSize { get; set; }        
}

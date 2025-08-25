namespace WebApi.Application.Queries
{
    public class QueryObject
    {
        public string? Symbol { get; set; } = null;
        public string? CompanyName { get; set; } = null;
        public string? StortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;
    }
}

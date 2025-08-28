namespace WebApi.Application.Queries
{
    public class QueryObject
    {
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string StortBy { get; set; } = string.Empty;
        public bool IsDescending { get; set; } = false;
    }
}

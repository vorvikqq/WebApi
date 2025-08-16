namespace WebApi.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int? StockId { get; set; }

        // Navigation
        public Stock? Stock { get; set; }
    }
}

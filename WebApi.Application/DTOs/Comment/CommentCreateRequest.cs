namespace WebApi.Application.DTOs.Comment
{
    public class CommentCreateRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}

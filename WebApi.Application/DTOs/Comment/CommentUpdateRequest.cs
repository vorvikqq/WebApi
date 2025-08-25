using System.ComponentModel.DataAnnotations;

namespace WebApi.Application.DTOs.Comment
{
    public class CommentUpdateRequest
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Title can not be over 50 characters")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(150, ErrorMessage = "Content can not be over 150 characters")]
        public string Content { get; set; } = string.Empty;
    }
}

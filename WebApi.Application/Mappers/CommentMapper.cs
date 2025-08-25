using WebApi.Application.DTOs.Comment;
using WebApi.Domain.Models;

namespace WebApi.Application.Mappers
{
    public static class CommentMapper
    {
        public static CommentResponseDto ToCommentResponse(this Comment model)
        {
            return new CommentResponseDto
            {
                Id = model.Id,
                Title = model.Title,
                Content = model.Content,
                CreatedOn = model.CreatedOn,
                StockId = model.StockId,
            };
        }

        public static Comment ToCommentFromCreateRequest(this CommentCreateRequest dto, int stockId)
        {
            return new Comment
            {
                Title = dto.Title,
                Content = dto.Content,
                StockId = stockId,
            };
        }

        public static Comment ToCommentFromUpdateRequest(this CommentUpdateRequest dto)
        {
            return new Comment
            {
                Title = dto.Title,
                Content = dto.Content,
            };
        }
    }
}

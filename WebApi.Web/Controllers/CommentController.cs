using Microsoft.AspNetCore.Mvc;
using WebApi.Application.DTOs.Comment;
using WebApi.Application.Mappers;
using WebApi.Application.Repositories.Interfaces;

namespace WebApi.Web.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IStockRepository _stockRepository;

        public CommentController(ICommentRepository commentRepository, IStockRepository stockRepository)
        {
            _commentRepository = commentRepository;
            _stockRepository = stockRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _commentRepository.GetAllAsync();

            var commentsResponse = comments.Select(c => c.ToCommentResponse());

            return Ok(commentsResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);

            if (comment == null)
                return NotFound();

            return Ok(comment.ToCommentResponse());
        }

        [HttpPost("{stockId}")]
        public async Task<IActionResult> Create([FromRoute] int stockId, [FromBody] CommentCreateRequest commentDto)
        {
            if (!await _stockRepository.IsExistAsnyc(stockId))
                return BadRequest("stock does not exist");

            var commentModel = commentDto.ToCommentFromCreateRequest(stockId);
            await _commentRepository.CreateAsync(commentModel);

            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentResponse());
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using WebApi.Application.DTOs.Comment;
using WebApi.Application.Mappers;
using WebApi.Application.Services.Interfaces;

namespace WebApi.Web.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var commentsResponse = await _commentService.GetAllCommentsAsync();

            return Ok(commentsResponse);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);

            if (comment == null)
                return NotFound();

            return Ok(comment);
        }

        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> Create([FromRoute] int stockId, [FromBody] CommentCreateRequest commentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var commentModel = await _commentService.CreateCommentAsync(stockId, commentDto);

            if (commentModel == null)
                return NotFound("stock not found");

            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentResponse());
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CommentUpdateRequest commentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var isUpdated = await _commentService.UpdateCommentAsync(id, commentDto);

            if (!isUpdated)
                return NotFound("Comment not found");

            return Ok(commentDto);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var isDeleted = await _commentService.DeleteCommentAsync(id);

            if (!isDeleted)
                return NotFound("Comment not found");

            return NoContent();
        }
    }
}

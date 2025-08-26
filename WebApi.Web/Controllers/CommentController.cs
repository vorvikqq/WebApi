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
            CommentResponseDto comment;
            try
            {
                comment = await _commentService.GetCommentByIdAsync(id);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }

            return Ok(comment);
        }

        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> Create([FromRoute] int stockId, [FromBody] CommentCreateRequest commentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var commentModel = await _commentService.CreateCommentAsync(stockId, commentDto);

            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentResponse());
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CommentUpdateRequest commentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _commentService.UpdateCommentAsync(id, commentDto);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

            return Ok(commentDto);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                await _commentService.DeleteCommentAsync(id);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

            return NoContent();
        }
    }
}

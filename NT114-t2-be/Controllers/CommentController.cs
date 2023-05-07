using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NT114_t2_be.Models;

namespace NT114_t2_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly Nt114T2DbContext _context;
        public CommentController(Nt114T2DbContext dbContext)
        {
            _context = dbContext;

        }

        //create an api for user to show all comments
        [HttpGet("showComment")]
        public IActionResult GetComment()
        {
            return Ok(_context.CommentTables);
        }

        //create an api for user to post new comment
        [HttpPost("postComment")]
        public IActionResult PostComment(string commentContent, int article_id, int user_id)
        {
            var article = _context.ArticleTables.Find(article_id);
            var user = _context.UserTables.Find(user_id);
            if (article == null || user ==null)
            {
                return NotFound();
            }

            //save the comment to the database
            _context.CommentTables.Add(new CommentTable
            {
                Text = commentContent,
                ArticleId = article_id, 
                UserId = user_id
            });
            _context.SaveChanges();
            return Ok("Successfully commented");
        }

        //create an api for user to update the comment
        [HttpPut("updateComment")]
        public IActionResult UpdateComment(int id, string commentContent)
        {
            var comment = _context.CommentTables.Find(id);
            if (comment == null)
            {
                return NotFound();
            }
            comment.Text = commentContent;
            _context.SaveChanges();
            return Ok(_context.CommentTables);
        }

        //create an api for user to delete the comment
        [HttpDelete("deleteComment")]
        public IActionResult DeleteComment(int id)
        {
            var comment = _context.CommentTables.Find(id);
            if (comment == null)
            {
                return NotFound();
            }
            _context.CommentTables.Remove(comment);
            _context.SaveChanges();
            return Ok(_context.CommentTables);
        }

        //create an api for user to delete all comments
        [HttpDelete("deleteAllComment")]
        public IActionResult DeleteAllComment()
        {
            _context.CommentTables.RemoveRange(_context.CommentTables);
            _context.SaveChanges();
            return Ok(_context.CommentTables);
        }
    }
}

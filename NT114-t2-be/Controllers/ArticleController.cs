using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NT114_t2_be.Models;

namespace NT114_t2_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly Nt114T2DbContext _context;
        public ArticleController(Nt114T2DbContext dbContext)
        {
            _context = dbContext;
        }

        [HttpGet("showArticle")]
        public async Task<ActionResult<IEnumerable<ArticleTable>>> GetArticle()
        {
            if(_context.ArticleTables == null)
            {
                return NotFound();
            }
            return await _context.ArticleTables.ToListAsync();
        }

        //create an api for user to post new article, the author of the article is the user who is existing in the database
        [HttpPost("postArticle")]
        public IActionResult PostArticle(int id, string title, string context)
        {            
            //save the article link to user who create the article
            var user = _context.UserTables.Find(id);
            if (user == null)
            {
                return NotFound();
            }   

            user.ArticleTables.Add(new ArticleTable
            {
                ArticleTitle = title,
                Body = context
            });
            //save the article to the database
            _context.SaveChanges();
            return Ok("Article has been posted");
        }

        //create an api for user to update the article
        [HttpPut("updateArticle")]
        public IActionResult UpdateArticle(int id, string title, string context)
        {
            var article = _context.ArticleTables.Find(id);
            if (article == null)
            {
                return NotFound();
            }
            article.ArticleTitle = title;
            article.Body = context;
            article.LastUpdated = DateTime.Now;
            _context.SaveChanges();
            return Ok("Article has been updated");
        }

        //create an api delete the article
        [HttpDelete("deleteArticle_Ad")]
        public IActionResult DeleteArticle(int id)
        {
            var article = _context.ArticleTables.Find(id);
            if (article == null)
            {
                return NotFound();
            }
            _context.ArticleTables.Remove(article);
            _context.SaveChanges();
            return Ok("Article has been deleted");
        }

        //create an api for user to delete the article
        [HttpDelete("deleteArticle")]
        public IActionResult DeleteArticle(string email, string password, int id)
        {
            var user = _context.UserTables.Where(u => u.Email == email && u.Password == password).FirstOrDefault();
            var article = _context.ArticleTables.Find(id);
            
            if (article == null)
            {
                return NotFound();
            }
            if (user == null)
            {
                return NotFound();
            }
            else if(user.Userid != article.AuthorId)
            {
                return Unauthorized();
            }
            
            _context.ArticleTables.Remove(article);
            _context.SaveChanges();
            return Ok("Article has been deleted");
        }

        //create an api for user to like the article
        [HttpPost("likeArticle")]
        public IActionResult LikeArticle(int id, int user_id)
        {
            var article = _context.ArticleTables.Find(id);
            var user = _context.UserTables.Find(user_id);
            if (article == null || user == null)
            {
                return NotFound();
            }
            //save the article link to user who like the article
            user.ArticleTables.Add(article);
            //save the article to the database
            _context.SaveChanges();
            return Ok("Article has been liked");
        }

        //create an api for user to unlike the article
        [HttpDelete("unlikeArticle")]
        public IActionResult UnlikeArticle(int id, int user_id)
        {
            var article = _context.ArticleTables.Find(id);
            var user = _context.UserTables.Find(user_id);
            if (article == null || user == null)
            {
                return NotFound();
            }
            //save the article link to user who like the article
            user.ArticleTables.Remove(article);
            //save the article to the database
            _context.SaveChanges();
            return Ok("Article has been unliked");
        }
    }
}

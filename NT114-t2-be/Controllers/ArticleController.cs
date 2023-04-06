using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult GetArticle()
        {
            if (_context.ArticleTables == null)
            {
                return NotFound();
            }
            return Ok(_context.ArticleTables);
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
    }
}

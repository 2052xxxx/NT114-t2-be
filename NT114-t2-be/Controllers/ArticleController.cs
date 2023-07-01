using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NT114_t2_be.Models;
using System;
using static System.Net.Mime.MediaTypeNames;

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
            var article = await _context.ArticleTables.ToListAsync();
            var articleDtos = article.Select(a => new ArticleDto
            {
                ArticleId = a.ArticleId,
                ArticleTitle = a.ArticleTitle,
                Body = a.Body,
                AuthorId = a.AuthorId,
                AuthorName = "",
                Views = a.Views,
                Likes = a.Likes,
                Comments = a.Comments,
                LastUpdated = a.LastUpdated
                // Map other properties as needed
            }).ToList();
            foreach (var a in articleDtos)
            {

                if (string.IsNullOrEmpty(a.Body) || a.Body.Length <= 100)
                {
                    a.Body = a.Body;
                }
                else
                {
                    a.Body = a.Body.Substring(0, 100) + "...";
                }
                // find the author name by author id
                var author = _context.UserTables.Find(a.AuthorId);
                if (author == null)
                {
                    a.AuthorName = "[delete]";
                }
                else
                {
                    a.AuthorName = author.Username;
                }
            }

            return Ok(articleDtos);
        }

        //create an api for user to get all the user's articles
        [HttpGet("showUserArticle")]
        public IActionResult GetUserArticles(int userId)
        {
            var query = @"
                SELECT *
                FROM dbo.ArticleTable
                JOIN dbo.UserTable ON author_id = userid
                WHERE userid = {0};
            ";

            var userArticles = _context.ArticleTables
                .FromSqlRaw(query, userId)
                .ToList();

            var articleDtos = userArticles.Select(a => new ArticleDto
            {
                ArticleId = a.ArticleId,
                ArticleTitle = a.ArticleTitle,
                Body = a.Body,
                AuthorId = a.AuthorId,
                AuthorName = "",
                Views = a.Views,
                Likes = a.Likes,
                Comments = a.Comments,
                LastUpdated = a.LastUpdated
                // Map other properties as needed
            }).ToList();
            foreach (var article in articleDtos)
            {
               
                if (string.IsNullOrEmpty(article.Body) || article.Body.Length <= 100)
                {
                    article.Body = article.Body;
                }
                else
                {
                    article.Body = article.Body.Substring(0, 100) + "...";
                }
                // find the author name by author id
                var author = _context.UserTables.Find(article.AuthorId);
                if(author == null)
                {
                    article.AuthorName = "[delete]" ;
                }
                else
                {
                    article.AuthorName = author.Username;
                }
            }

            return Ok(articleDtos);
        }

        // create an api for user to get the article by id
        [HttpGet("showArticle/{id}")]
        public async Task<ActionResult<ArticleTable>> GetArticle(int id)
        {
            var article = await _context.ArticleTables.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            var articleDto = new ArticleDto {
                ArticleId = article.ArticleId,
                ArticleTitle = article.ArticleTitle,
                Body = article.Body,
                AuthorId = article.AuthorId,
                AuthorName = "",
                Views = article.Views,
                Likes = article.Likes,
                Comments = article.Comments,
                LastUpdated = article.LastUpdated
            };
            var author = _context.UserTables.Find(article.AuthorId);
            if (author == null)
            {
                articleDto.AuthorName = "[delete]";
            }
            else
            {
                articleDto.AuthorName = author.Username;
                article.Views++;
            }
            _context.SaveChanges();
            return Ok(articleDto);
        }

        //create an api for user to get the article but truncate the title and body
        [HttpGet("showArticle_Ad")]
        public async Task<ActionResult<IEnumerable<ArticleTable>>> GetArticle_Ad()
        {
            if(_context.ArticleTables == null)
            {
                return NotFound();
            }
            var articles = await _context.ArticleTables.ToListAsync();
            foreach(var article in articles)
            {               
                if (string.IsNullOrEmpty(article.ArticleTitle) || article.ArticleTitle.Length <= 20)
                {
                    article.ArticleTitle = article.ArticleTitle;
                }
                else
                {
                    article.ArticleTitle = article.ArticleTitle.Substring(0, 20) + "...";
                }
                if (string.IsNullOrEmpty(article.Body) || article.Body.Length <= 20)
                {
                    article.Body = article.Body;
                }
                else
                {
                    article.Body = article.Body.Substring(0, 20) + "...";
                }
            }
            return articles;
        }

        //create an api for user to post new article, the author of the article is the user who is existing in the database
        [HttpPost("postArticle")]
        public IActionResult PostArticle(PostArticle article)
        {            
            //save the article link to user who create the article
            var user = _context.UserTables.Find(article.AuthorId);
            if (user == null)
            {
                return NotFound();
            }   
            
            user.ArticleTables.Add(new ArticleTable
            {
                ArticleTitle = article.ArticleTitle,
                Body = article.Body,
                AuthorId = article.AuthorId,
                Views = 0,
                Likes = 0,
                Comments = 0,
                LastUpdated = DateTime.Now,
                Author = user
            });
            //save the article to the database
            _context.SaveChanges();
            return Ok("Post article successed");
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
        [HttpDelete("deleteArticle_Ad/{id}")]
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

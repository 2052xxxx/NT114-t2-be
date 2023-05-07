using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NT114_t2_be.Models;

namespace NT114_t2_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Nt114T2DbContext _context;
        public UserController(Nt114T2DbContext dbContext)
        {
            _context = dbContext;
        }

        [HttpGet("showUser")]
        public async Task<ActionResult<IEnumerable<UserTable>>> GetUser()
        {
            if(_context.UserTables == null)
            {
                return NotFound();
            }
            return await _context.UserTables.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserTable>> GetUserById(int id)
        {
            if (_context.UserTables == null)
            {
                return NotFound();
            }

            var user = await _context.UserTables.FirstOrDefaultAsync(x => x.Userid == id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpPost("signUp")]
        public async Task<ActionResult<UserTable>> PostNewUser(UserTable user)
        {
            _context.UserTables.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostNewUser), new {id = user.Userid}, user);
        }

        //create an api for user to edit their profile

        [HttpPut("editUser")]
        public async Task<IActionResult> PutEditUser(int id, UserTable user )
        {
            if (id != user.Userid)
            {
                return BadRequest("Invalid User Id");
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
            return Ok();
        }

        private bool UserExists(long id)
        {
            return (_context.UserTables?.Any(x => x.Userid == id)).GetValueOrDefault();
        }
                      
        //create an api for user to delete their profile including all the articles they have posted
        [HttpDelete("deleteUser")]
        public async Task<ActionResult<UserTable>> DeleteUser(int id)
        {
            var user = await _context.UserTables.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var article = await _context.ArticleTables.FirstOrDefaultAsync(x => x.AuthorId == id);
            if (article != null)
            {
                _context.ArticleTables.Remove(article);
                await _context.SaveChangesAsync();
            }
            _context.UserTables.Remove(user);

            var comment = await _context.CommentTables.FirstOrDefaultAsync(x => x.UserId == id);
            if (comment != null)
            {
                _context.CommentTables.Remove(comment);
                await _context.SaveChangesAsync();
            }
            await _context.SaveChangesAsync();               

            return user;
        }

        //how to link comment table to user table
        [HttpGet("showUsrArticle")]
        public async Task<ActionResult<IEnumerable<ArticleTable>>> GetArticle()
        {
            if (_context.ArticleTables == null)
            {
                return NotFound();
            }
            return await _context.ArticleTables.ToListAsync();
        }

        //create an api for users to sign in
        [HttpPost("signIn")]
        public async Task<ActionResult<UserTable>> SignIn(string email, string password)
        {
            var userSignIn = await _context.UserTables.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
            if (userSignIn == null)
            {
                return NotFound();
            }
            return userSignIn;
        }

        //create an api for users to sign out
        [HttpPost("signOut")]
        public async Task<ActionResult<UserTable>> SignOut(string email, string password)
        {
            var userSignOut = await _context.UserTables.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
            if (userSignOut == null)
            {
                return NotFound();
            }
            return userSignOut;
        }
    }
}

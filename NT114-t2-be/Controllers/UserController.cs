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

        //create an api for user to get the user but truncate the avatar, bio and registration date
        [HttpGet("showUser_Ad")]
        public async Task<ActionResult<IEnumerable<UserTable>>> GetUser_Ad()
        {
            if(_context.UserTables == null)
            {
                return NotFound();
            }
            var users = await _context.UserTables.ToListAsync();
            foreach(var user in users)
            {
                if (string.IsNullOrEmpty(user.Avatar) || user.Avatar.Length <= 20 && string.IsNullOrEmpty(user.Bio) || user.Bio.Length <= 20)
                {
                    user.Avatar = user.Avatar;
                    user.Bio = user.Bio;
                }
                else
                {
                    user.Avatar = user.Avatar.Substring(0, 20) + "...";
                    user.Bio = user.Bio.Substring(0, 20) + "...";
                }
            }
            return users;
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
        [HttpDelete("deleteUser/{id}")]
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

        //create an api for users to upload their avatar
        [HttpPost("uploadAvatar")]
        public async Task<ActionResult<UserTable>> UploadAvatar(IFormFile file)
        {
            //get the file name
            var fileName = Path.GetFileName(file.FileName);
            //get the file extension
            var fileExtension = Path.GetExtension(fileName);
            //get the file size
            var fileSize = file.Length;
            //get the file path
            //var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);
            ////get the file stream
            //using (var fileStream = new FileStream(filePath, FileMode.Create))
            //{
            //    await file.CopyToAsync(fileStream);
            //}
            return Ok();
        }

        [HttpPost("signIn")]
        public async Task<ActionResult<UserTable>> SignIn(Login login)
        {
            //create an api for users to sign in

            var log = await _context.UserTables.FirstOrDefaultAsync(x => x.Email.Equals(login.Email) && x.Password.Equals(login.Password));
            if (log == null)
            {
                return NotFound();
            }
            else
            {
                //set status value to 1
                log.Status = 1;
                _context.Entry(log).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            return Ok();
        }


        //create an api for users to sign out
        [HttpPost("signOut_Ad")]
        public async Task<ActionResult<UserTable>> SignOut(string email, string password)
        {
            var userSignOut = await _context.UserTables.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
            if (userSignOut == null)
            {
                return NotFound();
            }
            else
            {
                userSignOut.Status = 0;
                _context.Entry(userSignOut).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            return userSignOut;
        }

        // delete all users
        [HttpDelete("deleteAllUser")]
        public async Task<ActionResult<UserTable>> DeleteAllUser()
        {
            var user = await _context.UserTables.ToListAsync();
            if (user == null)
            {
                return NotFound();
            }
            _context.UserTables.RemoveRange(user);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}

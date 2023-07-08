using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NT114_t2_be.Models;
using NT114_t2_be.Services.UserServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NT114_t2_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Nt114T2DbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IHttpContextAccessor _contextAccessor;

        public UserController(
            Nt114T2DbContext dbContext, IConfiguration configuration, 
            IUserService userService, IWebHostEnvironment hostEnvironment, 
            IHttpContextAccessor contextAccessor)
        {
            _context = dbContext;
            _configuration = configuration;
            _userService = userService;
            _hostEnvironment = hostEnvironment;
            _contextAccessor = contextAccessor;
        }
        private async Task<UserTable> GetUserByEmail(string email)
        {
            return await _context.UserTables.FirstOrDefaultAsync(user => user.Email == email);
        }

        //public UserAuth userAuth = new UserAuth();
        private void createPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        private bool verifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
                //for (int index = 0; index < computedHash.Length; index++)
                //{
                //    if (computedHash[index] != storedHash[index]) return false;
                //}
            }
            //return true;
        }
        private string createToken(UserTable user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var key = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:TokenKey").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
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
              
                if ( string.IsNullOrEmpty(user.Bio) || user.Bio.Length <= 20)
                {
                    user.Bio = user.Bio;
                }
                else
                {
                    user.Bio = user.Bio.Substring(0, 20) + "...";
                }
               
            }
            return users;
        }

        //if api receive the token from the user, it will check the token and return the user information
        [HttpGet, Authorize]
        public ActionResult<UserTable> GetMe()
        {
            UserAuth userInfo = _userService.GetUserName();
            if (userInfo == null)
            {
                return NotFound();
            }
            var users = new List<UserTable>();
            if (userInfo != null && !string.IsNullOrEmpty(userInfo.Username) && !string.IsNullOrEmpty(userInfo.Email))
            {
                // Trích xuất danh sách thông tin người dùng từ database
                users = _context.UserTables.Where(x => x.Username == userInfo.Username && x.Email == userInfo.Email).ToList();
            }
            return Ok(users);
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

        [HttpPost("register")]
        public async Task<ActionResult<UserTable>> PostNewUser(UserTable user)
        {
            // check if username is already taken
            if (_context.UserTables.Any(x => x.Email == user.Email))
            {
                return BadRequest("Account already taken");
            }
            else
            {
                createPasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);
                user.Hash = passwordHash;
                user.Salt = passwordSalt;
                string token = createToken(user);
                user.Status = 1;
                _context.UserTables.Add(user);
                await _context.SaveChangesAsync();

                return Ok(token);
            }
            
        }


        //create an api for user to edit their profile
        //[HttpPut("editUser/{id}")]
        //public async Task<IActionResult> PutUser(int id, UserTable user)
        //{
        //    if (id != user.Userid)
        //    {
        //        return BadRequest();
        //    }
        //    var userTable = await _context.UserTables.FindAsync(id);
        //    if (userTable == null)
        //    {
        //        return NotFound();
        //    }
        //    userTable.Username = user.Username;
        //    userTable.Email = user.Email;
        //    userTable.Avatar = user.Avatar;
        //    userTable.Bio = user.Bio;
           
        //    userTable.RegistrationDate = user.RegistrationDate;
        //    _context.Entry(userTable).State = EntityState.Modified;
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!UserExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //    string token = createToken(userTable);
        //    return Ok(token);
        //}

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

            return Ok(user);
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


        [HttpPost("signIn")]
        public async Task<ActionResult<UserTable>> SignIn(Login login)
        {           
            
            var user = await GetUserByEmail(login.Email);
            if (user == null)
            {
                return BadRequest("Invalid user");
            }
            else
            {
                var passwordHash = user.Hash;
                var passwordSalt = user.Salt;
                if (!verifyPasswordHash(login.Password, passwordHash, passwordSalt))
                {
                    return BadRequest("Invalid Password");

                }
                else
                {
                    //set status value to 1
                    user.Status = 1;
                    string token = createToken(user);
                    _context.Entry(user).State = EntityState.Modified;                    
                    await _context.SaveChangesAsync();
                    return Ok(token);

                }
            }
            
        }
     

        //create an api for users to sign out

        [HttpPost("logout")]
        public async Task<ActionResult<UserTable>> SignOut(UserAuth logout)
        {
            var userSignOut = await _context.UserTables.FirstOrDefaultAsync(x => x.Email == logout.Email && x.Username == logout.Username);
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

        //[HttpPost("uploadImage")]
        //public async Task<string> SaveImage(IFormFile imageFile)
        //{
        //    string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');
        //    imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageFile.FileName);
        //    var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
        //    using (var fileStream = new FileStream(imagePath, FileMode.Create))
        //    {
        //        await imageFile.CopyToAsync(fileStream);
        //    }
        //    return imageName;
        //}

        [HttpPost("uploadImage/{id}")]
        public async Task<ActionResult> UploadImage(int id)
        {
            bool Results = false;
            // find user by id
            var user = await _context.UserTables.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            string imageUrl = string.Empty;
            string token = string.Empty;

            try
            {
                var _uploadedfile = Request.Form.Files;
                foreach (IFormFile source in _uploadedfile)
                {
                    string Filename = source.FileName;
                    string Filepath = GetFilePath(Filename);
                    if (!Directory.Exists(Filepath))
                    {
                        Directory.CreateDirectory(Filepath);
                    }

                    string imagepath = Filepath + "\\image.png";

                    if (System.IO.File.Exists(imagepath))
                    {
                        System.IO.File.Delete(imagepath);
                    }
                    using (FileStream stream = System.IO.File.Create(imagepath))
                    {
                        await source.CopyToAsync(stream);
                        Results = true;
                    }
                    imageUrl = getImageByUrl(Filename);
                }
            }
            catch (Exception ex)
            {

            }
            if (Results)
            {
                user.Avatar = imageUrl;
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                token = createToken(user);
            }
            return Ok(token);
        }


        [NonAction]
        private string UploadImage(IFormFile img)
        {
            // find user by id

            string imageUrl = string.Empty;

            try
            {
                var _uploadedfile = img;

                string Filename = img.FileName;
                string Filepath = GetFilePath(Filename);
                if (!Directory.Exists(Filepath))
                {
                    Directory.CreateDirectory(Filepath);
                }

                string imagepath = Filepath + "\\image.png";

                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                }
                using (FileStream stream = System.IO.File.Create(imagepath))
                {
                    img.CopyToAsync(stream);
                }
                imageUrl = getImageByUrl(Filename);

            }
            catch (Exception ex)
            {

            }

            return imageUrl;
        }

        //[HttpPost("test")]
        ////create an api for users to upload image
        [HttpPost("test")]
        public async Task<ActionResult> UploadImage()
        {
            bool Results = false;
            try
            {
                var _uploadedfile = Request.Form.Files;
                foreach (IFormFile source in _uploadedfile)
                {
                    string Filename = source.FileName;
                    string Filepath = GetFilePath(Filename);
                    if (!Directory.Exists(Filepath))
                    {
                        Directory.CreateDirectory(Filepath);
                    }
                    string imagepath = Filepath + "\\image.png";

                    if (System.IO.File.Exists(imagepath))
                    {
                        System.IO.File.Delete(imagepath);
                    }
                    using (FileStream stream = System.IO.File.Create(imagepath))
                    {
                        await source.CopyToAsync(stream);
                        Results = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return Ok(Results);
        }

        // create api for user to edit profile
        [HttpPut("editProfile")]
        public async Task<ActionResult<UserTable>> UploadImage([FromForm] UserDto user)
        {
            string token = string.Empty;
            var userEdit = await _context.UserTables.FindAsync(user.Userid);
            if (userEdit == null)
            {
                return NotFound();
            }
            else
            {
                if (user.Username != null)
                {
                    userEdit.Username = user.Username;
                }
                if (user.Realname != null)
                {
                    user.Realname = user.Realname;
                }
                if (user.avatar != null)
                {
                    userEdit.Avatar = UploadImage(user.avatar);
                }
                if (user.Bio != null)
                {
                    userEdit.Bio = user.Bio;
                }
               
                _context.Entry(userEdit).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                token = createToken(userEdit);
            }
            return Ok(token);
        }

        [NonAction]
        private string GetFilePath(string avatarCode)
        {
            return this._hostEnvironment.WebRootPath + "\\Images\\" + avatarCode;
        }

        [NonAction]
        private string getImageByUrl(string avatarCode)
        {
            string imageUrl = string.Empty;
            string hostUrl = "https://localhost:7015/";
            string filePath = GetFilePath(avatarCode);
            string imagePath = filePath + "\\image.png";
            if(!System.IO.File.Exists(imagePath)) {
                imageUrl = hostUrl + "/Images/noimage.png";
            }
            else
            {
                imageUrl = hostUrl + "/Images/"+ avatarCode+"/image.png";
            }
            return imageUrl;
        }

        // create put user api
       


    }
}

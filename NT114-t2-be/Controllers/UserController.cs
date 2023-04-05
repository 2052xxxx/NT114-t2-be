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

        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutEditUser(int id, UserTable user)
        //{
        //    if (id != user.Userid)
        //    {
        //        return BadRequest("Invalid User Id");
        //    }

        //    _context.Entry(user).State = EntityState.Modified;

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
            
        //    return Ok();
        //}


        //  [HttpGet]
        ////  [Route("RawData")]
        //  public IActionResult Get()
        //  {
        //      return Ok(_tables);
        //  }

        //  [HttpGet("{id:int}")]
        //  public IActionResult Get(int id)
        //  {
        //      var user = _tables.FirstOrDefault(x=>x.Userid == id);
        //      if (user == null)
        //          return BadRequest("Value null");
        //      return Ok(user);
        //  }

        //  [HttpPost]        
        //  public IActionResult Post(UserTable user)
        //  {
        //      _tables.Add(user);

        //      Console.WriteLine(_tables.Count);

        //      return CreatedAtAction("Get", nameof(Post), user);
        //  }

        //  [HttpPatch]
        //  public IActionResult Patch(int id, string name)
        //  {
        //      var user = _tables.First(x => x.Userid == id);
        //      if (user == null)
        //          return BadRequest("Invalid Id");
        //      user.Realname = name;

        //      return Ok(user);
        //  }

        //  [HttpDelete] public IActionResult Delete(int id)
        //  {
        //      var user = _tables.FirstOrDefault(x => x.Userid == id);
        //      if (user == null)
        //          return BadRequest("Invalid Id");
        //      _tables.Remove(user);
        //      return Ok(_tables);
        //  }

    }
}

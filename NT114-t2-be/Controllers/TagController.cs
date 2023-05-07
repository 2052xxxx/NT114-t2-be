using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NT114_t2_be.Models;

namespace NT114_t2_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly Nt114T2DbContext _context;
        public TagController(Nt114T2DbContext dbContext)
        {
            _context = dbContext;
        }

        [HttpGet("showTag")]
        public IActionResult GetTag()
        {
            return Ok("This is the tag");
        }

        //create an api for user to post new tag
        [HttpPost("postTag")]
        public IActionResult PostTag(string tagName)
        {
            //save the tag to the database
            _context.TagTables.Add(new TagTable
            {
                TagName = tagName
            });
            _context.SaveChanges();
            return Ok("Tag has been posted");
        }


        //create an api for user to delete tag
        [HttpDelete("deleteTag")]
        public IActionResult DeleteTag(int id)
        {
            var tag = _context.TagTables.Find(id);
            if (tag == null)
            {
                return NotFound();
            }
            _context.TagTables.Remove(tag);
            _context.SaveChanges();
            return Ok("Tag has been deleted");
        }

        
    }
}

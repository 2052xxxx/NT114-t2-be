using Microsoft.EntityFrameworkCore;

namespace NT114_t2_be.Models
{
    public class UserTableContext : DbContext
    {
        public UserTableContext(DbContextOptions<UserTableContext> options) : base(options)
        {
            
        }
        public DbSet<UserTable> Users { get; set; }
    }
}

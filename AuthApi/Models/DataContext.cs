using AuthAi.Models.Authentication;
using AuthApi.Models.BookReading;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthAi.Models
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            base.Database.Migrate();
        }
        public DbSet<ConfirmationCode> ConfirmationCodes { get; set; }

        public DbSet<Book> Books { get; set; }
    }
}

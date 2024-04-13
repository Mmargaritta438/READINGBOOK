using AuthAi.Models;

namespace AuthAi.Configuration
{
    public static class IdentityExtensions
    {
        public static void AddIdentityWithOptions(this WebApplicationBuilder builder)
        {
            builder.Services.AddIdentityCore<ApplicationUser>(opts =>
                {
                    opts.User.RequireUniqueEmail = true;
                    opts.SignIn.RequireConfirmedEmail = true;
                    opts.Password.RequireDigit = true;
                    opts.Password.RequireLowercase = true;
                })
                .AddEntityFrameworkStores<DataContext>();
        }
    }
}

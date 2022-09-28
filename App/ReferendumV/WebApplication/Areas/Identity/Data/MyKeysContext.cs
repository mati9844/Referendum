using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;

namespace WebApplication.Areas.Identity.Data
{
    public class MyKeysContext : DbContext
    {
        public MyKeysContext()
        {
        }

        //Add-Migration AddDataProtectionKeys -Context MyKeysContext
        // A recommended constructor overload when using EF Core 
        // with dependency injection.
        public MyKeysContext(DbContextOptions<MyKeysContext> options)
            : base(options) { }

        // This maps to the table that stores keys.
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public DbSet<VerifyPhoneNumber> verifyPhoneNumbers { get; set; }



    }
}

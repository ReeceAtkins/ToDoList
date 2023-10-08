using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using To_Do_List.Models;
using Task = To_Do_List.Models.Task;

namespace To_Do_List.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Task> Tasks { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
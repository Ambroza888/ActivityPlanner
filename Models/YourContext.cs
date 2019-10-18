using Microsoft.EntityFrameworkCore;


namespace c_sharp_proj.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users {get;set;}
        public DbSet<_Activity> Activities {get;set;}
        public DbSet<Association> Associations {get;set;}
    }
}
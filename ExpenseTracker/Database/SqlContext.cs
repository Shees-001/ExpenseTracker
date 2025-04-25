using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Database
{
    public class SqlContext:DbContext
    {
        public SqlContext(DbContextOptions<SqlContext> options): base(options)
        {

        } 
        public DbSet<Categories> tblCategories { get; set; }
        public DbSet<Users> Users { get; set; }
     }
}

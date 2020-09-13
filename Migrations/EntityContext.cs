using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System.Linq;
using BankAccountWebAPI;

namespace BankAccountWebAPI
{
    public class Context : DbContext
    {
        public DbSet<Account> BankAccounts { get; set; }
        public DbSet<SingleAccountOperation> SingleAccOperations { get; set; }
        public DbSet<DoubleAccountOperation> DoubleAccOperations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=bankmanagement.db");   
    }    
}
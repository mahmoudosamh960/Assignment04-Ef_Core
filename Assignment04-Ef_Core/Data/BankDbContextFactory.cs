using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NationalBankManagementSystem.Data;

namespace Assignment04_Ef_Core.Data
{
    public class BankDbContextFactory : IDesignTimeDbContextFactory<BankDbContext>
    {
        public BankDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BankDbContext>();

            optionsBuilder.UseSqlServer(
    "Server=DESKTOP-37EVQ3K\\MSSQLSERVER01;Database=NationalBankDB;Trusted_Connection=True;TrustServerCertificate=True;"
);

            return new BankDbContext(optionsBuilder.Options);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using NationalBankManagementSystem.Models;

namespace NationalBankManagementSystem.Data
{
    public class BankDbContext : DbContext
    {
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<CustomerAccount> CustomerAccounts { get; set; }

        public BankDbContext(DbContextOptions<BankDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Branch>()
                .HasKey(b => b.Code);

            modelBuilder.Entity<Manager>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Account>()
                .HasKey(a => a.AccountNumber);

            modelBuilder.Entity<Customer>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Transaction>()
                .HasKey(t => t.TransactionNumber);

            modelBuilder.Entity<CustomerAccount>()
                .HasKey(ca => new
                {
                    ca.CustomerId,
                    ca.AccountNumber
                });

            modelBuilder.Entity<Branch>()
                .HasOne(b => b.Manager)
                .WithOne(m => m.Branch)
                .HasForeignKey<Manager>(m => m.BranchCode);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Branch)
                .WithMany(b => b.Accounts)
                .HasForeignKey(a => a.BranchCode);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountNumber);

            
            modelBuilder.Entity<CustomerAccount>()
                .HasOne(ca => ca.Customer)
                .WithMany(c => c.CustomerAccounts)
                .HasForeignKey(ca => ca.CustomerId);

            modelBuilder.Entity<CustomerAccount>()
                .HasOne(ca => ca.Account)
                .WithMany(a => a.CustomerAccounts)
                .HasForeignKey(ca => ca.AccountNumber);

            modelBuilder.Entity<Branch>().HasData(
        new Branch
        {
            Code = "CAI-01",
            Name = "Cairo Main Branch",
            Address = "Cairo",
            PhoneNumber = "01000000001"
        },
        new Branch
        {
            Code = "ALX-01",
            Name = "Alexandria Branch",
            Address = "Alexandria",
            PhoneNumber = "01000000002"
        }
    );

            modelBuilder.Entity<Manager>().HasData(
                new Manager
                {
                    Id = 1,
                    FullName = "Ahmed Ali",
                    Email = "ahmed@bank.com",
                    PhoneNumber = "01000000003",
                    BranchCode = "CAI-01"
                },
                new Manager
                {
                    Id = 2,
                    FullName = "Mohamed Hassan",
                    Email = "mohamed@bank.com",
                    PhoneNumber = "01000000004",
                    BranchCode = "ALX-01"
                }
            );
        }
    }
}

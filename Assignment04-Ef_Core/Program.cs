using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using NationalBankManagementSystem.Data;
using NationalBankManagementSystem.Models;

namespace Assignment04_Ef_Core
{
    internal class Program
    {
        static DbContextOptions<BankDbContext> GetOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<BankDbContext>();
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\MSSQLLocalDB;Database=NationalBankDB;Trusted_Connection=True;TrustServerCertificate=True;");
            return optionsBuilder.Options;
        }

     

        static string ReadRequired(string label)
        {
            while (true)
            {
                Console.Write(label);
                string? s = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(s)) return s;
                Console.WriteLine("  Value cannot be empty. Please try again.");
            }
        }

        static string ReadDigits(string label)
        {
            while (true)
            {
                string s = ReadRequired(label);
                if (s.All(char.IsDigit)) return s;
                Console.WriteLine("  Digits only. Please try again.");
            }
        }

        static string ReadEmail(string label)
        {
            while (true)
            {
                string s = ReadRequired(label);
                try
                {
                    var m = new MailAddress(s);
                    if (m.Address == s && s.Contains('.')) return s;
                }
                catch { }
                Console.WriteLine("  Invalid email format. Please try again.");
            }
        }

        static DateTime ReadDate(string label)
        {
            while (true)
            {
                string s = ReadRequired(label);
                if (DateTime.TryParseExact(s, "yyyy-MM-dd",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out DateTime d)
                    && d < DateTime.Today)
                    return d;
                Console.WriteLine("  Invalid date. Use yyyy-MM-dd (a past date).");
            }
        }

        static int ReadInt(string label)
        {
            while (true)
            {
                Console.Write(label);
                if (int.TryParse(Console.ReadLine(), out int v)) return v;
                Console.WriteLine("  Invalid number. Please try again.");
            }
        }

        static int ReadChoice(string label, int min, int max)
        {
            while (true)
            {
                int v = ReadInt(label);
                if (v >= min && v <= max) return v;
                Console.WriteLine($"  Please choose between {min} and {max}.");
            }
        }

     

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("========================================");
                Console.WriteLine("       National Bank - Management");
                Console.WriteLine("========================================");
                Console.WriteLine("  1) Add a new Customer");
                Console.WriteLine("  2) Open a new Account for a Customer");
                Console.WriteLine("  3) Update Account Status (Active / Closed)");
                Console.WriteLine("  4) Remove an Account from a Customer");
                Console.WriteLine("  5) List all Customers (with accounts)");
                Console.WriteLine("  0) Exit");
                Console.WriteLine("----------------------------------------");
                Console.Write("  Enter choice: ");

                string? choice = Console.ReadLine()?.Trim();

                if (choice == "0") return;

                try
                {
                    switch (choice)
                    {
                        case "1": AddCustomer(); break;
                        case "2": OpenAccount(); break;
                        case "3": UpdateAccountStatus(); break;
                        case "4": RemoveAccount(); break;
                        case "5": ListCustomers(); break;
                        default:
                            Console.WriteLine("Invalid choice. Please enter 0-5.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Error: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"Details: {ex.InnerException.Message}");
                }

                Console.WriteLine();
                Console.WriteLine("Press any key to return to the menu...");
                Console.ReadKey(true);
            }
        }

       
        static void AddCustomer()
        {
            using var context = new BankDbContext(GetOptions());

            Console.Clear();
            Console.WriteLine("--- Add New Customer ---");

            string fullName = ReadRequired("Full Name      : ");
            string nationalId = ReadDigits("National ID    : ");
            DateTime dob = ReadDate("Date of Birth (yyyy-MM-dd) : ");
            string email = ReadEmail("Email          : ");
            string phone = ReadDigits("Phone          : ");
            string address = ReadRequired("Address        : ");

            Console.WriteLine("Customer Type:");
            Console.WriteLine("    1) Individual");
            Console.WriteLine("    2) Business");
            int typeChoice = ReadChoice("  Choice: ", 1, 2);
            string customerType = typeChoice == 1 ? "Individual" : "Business";

            var customer = new Customer
            {
                FullName = fullName,
                NationalId = nationalId,
                DateOfBirth = dob,
                Email = email,
                PhoneNumber = phone,
                Address = address,
                CustomerType = customerType
            };

            context.Customers.Add(customer);
            context.SaveChanges();

            Console.WriteLine();
            Console.WriteLine($"Customer created successfully. CustomerId = {customer.Id}");
        }

   

        static void OpenAccount()
        {
            using var context = new BankDbContext(GetOptions());

            Console.Clear();
            Console.WriteLine("--- Open New Account ---");

            string accountNumber = ReadRequired("Account Number : ");

            if (context.Accounts.Any(a => a.AccountNumber == accountNumber))
            {
                Console.WriteLine("Account number already exists.");
                return;
            }

            Console.WriteLine("Account Type:");
            Console.WriteLine("    1) Savings");
            Console.WriteLine("    2) Current");
            Console.WriteLine("    3) Business");
            int typeChoice = ReadChoice("  Choice: ", 1, 3);
            string accountType = typeChoice switch
            {
                1 => "Savings",
                2 => "Current",
                _ => "Business"
            };

            string branchCode = ReadRequired("Branch Code    : ");
            int customerId = ReadInt("Customer Id    : ");

            Console.WriteLine("Ownership Role:");
            Console.WriteLine("    1) Primary");
            Console.WriteLine("    2) CoHolder");
            int roleChoice = ReadChoice("  Choice: ", 1, 2);
            string ownershipType = roleChoice == 1 ? "Primary" : "CoHolder";

            Console.WriteLine($"Validating branch '{branchCode}' and customer #{customerId}...");

            if (!context.Branches.Any(b => b.Code == branchCode))
            {
                Console.WriteLine($"Branch '{branchCode}' not found.");
                return;
            }

            if (!context.Customers.Any(c => c.Id == customerId))
            {
                Console.WriteLine($"Customer #{customerId} not found.");
                return;
            }

            var account = new Account
            {
                AccountNumber = accountNumber,
                AccountType = accountType,
                BranchCode = branchCode,
                CurrentBalance = 0,
                OpeningDate = DateTime.Now
            };

            var link = new CustomerAccount
            {
                CustomerId = customerId,
                AccountNumber = accountNumber,
                OwnershipType = ownershipType,
                OwnershipStartDate = DateTime.Now,
                AccountStatus = "Active"
            };

            context.Accounts.Add(account);
            context.CustomerAccounts.Add(link);
            context.SaveChanges();

            Console.WriteLine(
                $"Account '{accountNumber}' created and linked to customer {customerId} as {ownershipType} owner.");
        }

       

        static void UpdateAccountStatus()
        {
            using var context = new BankDbContext(GetOptions());

            Console.Clear();
            Console.WriteLine("--- Update Account Status ---");

            string accountNumber = ReadRequired("Account Number : ");
            int customerId = ReadInt("Customer Id    : ");

            var link = context.CustomerAccounts.FirstOrDefault(ca =>
                ca.AccountNumber == accountNumber && ca.CustomerId == customerId);

            if (link == null)
            {
                Console.WriteLine("This account is not linked to this customer.");
                return;
            }

            Console.WriteLine("New Status:");
            Console.WriteLine("    1) Active");
            Console.WriteLine("    2) Closed");
            int statusChoice = ReadChoice("  Choice: ", 1, 2);

            link.AccountStatus = statusChoice == 1 ? "Active" : "Closed";
            context.SaveChanges();

            Console.WriteLine($"Status updated to {link.AccountStatus}.");
        }


        static void RemoveAccount()
        {
            using var context = new BankDbContext(GetOptions());

            Console.Clear();
            Console.WriteLine("--- Remove Account From Customer ---");

            string accountNumber = ReadRequired("Account Number : ");
            int customerId = ReadInt("Customer Id    : ");

            var link = context.CustomerAccounts.FirstOrDefault(ca =>
                ca.AccountNumber == accountNumber && ca.CustomerId == customerId);

            if (link == null)
            {
                Console.WriteLine("Ownership link not found.");
                return;
            }

            context.CustomerAccounts.Remove(link);
            context.SaveChanges();

            Console.WriteLine();
            Console.WriteLine("  Ownership link deleted.");

            bool hasOtherOwners = context.CustomerAccounts
                .Any(ca => ca.AccountNumber == accountNumber);

            if (!hasOtherOwners)
            {
                var account = context.Accounts
                    .FirstOrDefault(a => a.AccountNumber == accountNumber);

                if (account != null)
                {
                    context.Accounts.Remove(account);
                    context.SaveChanges();
                    Console.WriteLine(
                        $"  That was the last owner - account '{accountNumber}' was also removed.");
                }
            }
        }


        static void ListCustomers()
        {
            using var context = new BankDbContext(GetOptions());

            Console.Clear();
            Console.WriteLine("--- All Customers ---");

            var customers = context.Customers
                .Include(c => c.CustomerAccounts)
                    .ThenInclude(ca => ca.Account)
                        .ThenInclude(a => a.Branch)
                .OrderBy(c => c.Id)
                .ToList();

            if (customers.Count == 0)
            {
                Console.WriteLine("No customers found.");
                return;
            }

            foreach (var c in customers)
            {
                Console.WriteLine($"  #{c.Id} {c.FullName} ({c.CustomerType})");

                if (!c.CustomerAccounts.Any())
                {
                    Console.WriteLine("      (no accounts)");
                    continue;
                }

                foreach (var ca in c.CustomerAccounts)
                {
                    var a = ca.Account;
                    Console.WriteLine(
                        $"      {a.AccountNumber,-10} {a.AccountType,-9} " +
                        $"Balance: {a.CurrentBalance,12:N2}  " +
                        $"{ca.OwnershipType,-9} {ca.AccountStatus,-7} @ {a.Branch.Name}");
                }
            }
        }
    }
}
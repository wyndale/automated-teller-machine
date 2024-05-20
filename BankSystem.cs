using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

namespace BankSystem
{
    public class CustomerType
    {
        public enum Customer_Type
        {
            Individual,
            Company
        }

        public Customer_Type type { get; set; }

        public CustomerType(Customer_Type type)
        {
            this.type = type;
        }
    }

    public interface ICustomerBankDatabase
    {
        void AddCustomer(Customer customer);
        Customer FindCustomer(string fullname, int password);
        IEnumerable<Customer> GetCustomers();
        void SaveCustomers();
    }

    public class BankCustomerDatabase : ICustomerBankDatabase
    {
        private List<Customer> customers = new List<Customer>();
        private readonly string _filepath = "bankdatabase.json";

        public BankCustomerDatabase()
        {
            LoadCustomers();
        }

        public void AddCustomer(Customer customer)
        {
            customers.Add(customer);
            SaveCustomers();
        }

        public Customer FindCustomer(string fullname, int password)
        {
            return customers.FirstOrDefault(c => c.FullName == fullname && c.Password == password);
        }

        public IEnumerable<Customer> GetCustomers()
        {
            return customers;
        }

        public void SaveCustomers()
        {
            string json = JsonConvert.SerializeObject(customers, Formatting.Indented);
            File.WriteAllText(_filepath, json);
        }

        public void LoadCustomers()
        {
            if (File.Exists(_filepath))
            {
                string json = File.ReadAllText(_filepath);
                customers = JsonConvert.DeserializeObject<List<Customer>>(json) ?? new List<Customer>();
            }
        }
    }

    public class Customer
    {
        private string fullname;
        private string email;
        private int password;
        private CustomerType.Customer_Type type;
        private decimal balance;
        private List<Transaction> transactions;

        public string FullName { get => fullname; set => fullname = value; }
        public string Email { get => email; set => email = value; }
        public int Password { get => password; set => password = value; }
        public CustomerType.Customer_Type Type { get => type; set => type = value; }
        public decimal Balance { get => balance; set => balance = value; }
        public List<Transaction> Transactions { get => transactions; set => transactions = value; }

        public Customer(string fullname, string email, int password, CustomerType.Customer_Type type, decimal balance)
        {
            FullName = fullname;
            Email = email;
            Password = password;
            Type = type;
            Balance = balance;
            Transactions = new List<Transaction>();
        }
    }

    public class Transaction
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }

        public Transaction(string type, decimal amount)
        {
            Date = DateTime.Now;
            Type = type;
            Amount = amount;
        }
    }

    public class Bank
    {
        private readonly ICustomerBankDatabase _database;

        public ICustomerBankDatabase CustomerDatabase => _database;

        public Bank(ICustomerBankDatabase database)
        {
            _database = database;
        }

        public void AddCustomer(string fullname, string email, int password, CustomerType.Customer_Type type, decimal balance)
        {
            var customer = new Customer(fullname, email, password, type, balance);
            CustomerDatabase.AddCustomer(customer);
        }

        public Customer FindCustomer(string fullname, int password)
        {
            return CustomerDatabase.FindCustomer(fullname, password);
        }

        public string MaskEmail(string email)
        {
            var atIndex = email.IndexOf('@');
            if (atIndex > 3)
            {
                return email.Substring(0, 3) + new string('*', atIndex - 3) + email.Substring(atIndex);
            }
            return new string('*', atIndex) + email.Substring(atIndex);
        }

        public void DisplayEmail(string fullname, int password)
        {
            var customer = _database.FindCustomer(fullname, password);
            if (customer != null)
            {
                Console.WriteLine("\n");
                string maskedEmail = MaskEmail(customer.Email);
                Console.WriteLine($"Account type: {customer.Type}");
                Console.WriteLine($"Full name: {customer.FullName}");
                Console.WriteLine($"Email: {maskedEmail}");
            }
        }
    }

    public class DepositAccount : Bank
    {
        public DepositAccount(ICustomerBankDatabase database) : base(database)
        { }

        public void Deposit(Customer customer, decimal amount)
        {
            customer.Balance += amount;
            customer.Transactions.Add(new Transaction("Deposit", amount));
            CustomerDatabase.SaveCustomers();
            Console.WriteLine("\n\n________________________________________\n");
            Console.WriteLine($"\nDeposit successfully!\n\nNew balance: {customer.Balance}");
            Console.Write("\n________________________________________\n");
        }

        public void Withdraw(Customer customer, decimal amount)
        {
            if (customer.Balance >= amount)
            {
                customer.Balance -= amount;
                customer.Transactions.Add(new Transaction("Withdrawal", amount));
                CustomerDatabase.SaveCustomers();
                Console.WriteLine("\n\n________________________________________\n");
                Console.WriteLine($"\nWithdraw successfully!\n\nNew balance: {customer.Balance}");
                Console.Write("\n________________________________________\n");
            }
            else
            {
                throw new InvalidOperationException("Insufficient funds.");
            }
        }
    }

    public class LoanAccount : Bank
    {
        private const decimal InterestRateIndividual = 0.05m;
        private const decimal InterestRateCompany = 0.04m;

        public LoanAccount(ICustomerBankDatabase database) : base(database)
        { }

        public void ApplyForLoan(Customer customer, decimal amount)
        {
            customer.Balance += amount;
            customer.Transactions.Add(new Transaction("Loan", amount));
            CustomerDatabase.SaveCustomers();
            Console.WriteLine("\n\n________________________________________\n");
            Console.WriteLine($"\nApply for loan successful!");
            Console.Write("\n________________________________________\n");
        }

        public void MakeLoanPayment(Customer customer, decimal amount)
        {
            decimal interestRate = customer.Type == CustomerType.Customer_Type.Individual ? InterestRateIndividual : InterestRateCompany;
            decimal interest = amount * interestRate;
            customer.Balance -= (amount + interest);
            customer.Transactions.Add(new Transaction("Loan Payment", amount));
            CustomerDatabase.SaveCustomers();
            Console.WriteLine("\n\n________________________________________\n");
            Console.WriteLine($"\nLoan payment successful!");
            Console.Write("\n________________________________________\n");
        }
    }

    public class MortgageAccount : Bank
    {
        private const decimal InterestRateIndividual = 0.04m;
        private const decimal InterestRateCompany = 0.03m;

        public MortgageAccount(ICustomerBankDatabase database) : base(database)
        { }

        public void ApplyForMortgage(Customer customer, decimal amount)
        {
            customer.Balance += amount;
            customer.Transactions.Add(new Transaction("Mortgage", amount));
            CustomerDatabase.SaveCustomers();
            Console.WriteLine("\n\n________________________________________\n");
            Console.WriteLine($"\nApply for mortgage loan successful!");
            Console.Write("\n________________________________________\n");
        }

        public void MakeMortgagePayment(Customer customer, decimal amount)
        {
            decimal interestRate = customer.Type == CustomerType.Customer_Type.Individual ? InterestRateIndividual : InterestRateCompany;
            decimal interest = amount * interestRate;
            customer.Balance -= (amount + interest);
            customer.Transactions.Add(new Transaction("Mortgage Payment", amount));
            CustomerDatabase.SaveCustomers();
            Console.WriteLine("\n\n________________________________________\n");
            Console.WriteLine($"\nPayment for mortgage loan successful!");
            Console.Write("\n________________________________________\n");
        }
    }

    public class BankTransactionHistory : Bank
    {
        public BankTransactionHistory(ICustomerBankDatabase database) : base(database)
        { }

        public void DisplayHistory(string fullname, int password)
        {
            var customer = FindCustomer(fullname, password);
            if (customer != null)
            {
                Console.WriteLine($"\n\nTransaction history of {fullname}\n");
                foreach (var transaction in customer.Transactions)
                {
                    Console.WriteLine($"{transaction.Date}: {transaction.Type} of {transaction.Amount:C}");
                }
            }
        }
    }
}

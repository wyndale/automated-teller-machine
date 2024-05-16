using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TellerMachine
{
    public class Card
    {
        public enum CardType
        {
            Debit,
            ATM,
            PrepaidDebit,
            ContactLessDebit,
            InternationalDebit
        }

        public CardType Type { get; set; }

        public Card(CardType type)
        {
            Type = type;
        }
    }

    public interface ICustomerDatabase
    {
        void AddCustomer(Customer customer);
        Customer FindCustomer(ulong accountNum);
        IEnumerable<Customer> GetCustomers();
    }

    public class Customer
    {
        private ulong accountNum;
        private string fullname;
        private string email;
        private int pincode;
        private Card cardType;
        private decimal balance;

        public ulong AccountNumber { get => accountNum; set => accountNum = value; }
        public string FullName { get => fullname; set => fullname = value; }
        public string Email { get => email; set => email = value; }
        public int PinCode { get => pincode; set => pincode = value; }
        public Card CardType { get => cardType; set => cardType = value; }
        public decimal Balance { get => balance; set => balance = value; }

        public Customer(string fullname, string email, ulong accountNum, int pincode, Card cardType, decimal balance)
        {
            FullName = fullname;
            Email = email;
            AccountNumber = accountNum;
            PinCode = pincode;
            CardType = cardType;
            Balance = balance;
        }
    }

    public class CustomerDatabase : ICustomerDatabase
    {
        private List<Customer> _customers;

        public CustomerDatabase()
        {
            _customers = new List<Customer>();
        }

        public void AddCustomer(Customer customer)
        {
            _customers.Add(customer);
        }

        public Customer FindCustomer(ulong accountNum)
        {
            return _customers.FirstOrDefault(c => c.AccountNumber == accountNum);
        }

        public IEnumerable<Customer> GetCustomers()
        {
            return _customers;
        }
    }

    public class ATM
    {
        private readonly ICustomerDatabase _customerDatabase;

        public ATM(ICustomerDatabase customerDatabase)
        {
            _customerDatabase = customerDatabase;
        }

        public void AddCustomer(string fullname, string email, ulong accountNum, int pincode, Card cardType, decimal balance)
        {
            var customer = new Customer(fullname, email, accountNum, pincode, cardType, balance);
            _customerDatabase.AddCustomer(customer);
        }

        public Customer FindCustomer(ulong accountNumber)
        {
            return _customerDatabase.FindCustomer(accountNumber);
        }

        public string MaskAccountNumber(ulong accountNum)
        {
            string accountNumStr = accountNum.ToString();
            if (accountNumStr.Length > 2)
            {
                return accountNumStr.Substring(0, 2) + new string('*', accountNumStr.Length - 2);
            }
            return accountNumStr;
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

        public void DisplayMaskInfo(ulong accountNumber)
        {
            var customer = _customerDatabase.FindCustomer(accountNumber);
            if (customer != null)
            {
                string maskedAccountNumber = MaskAccountNumber(customer.AccountNumber);
                string maskedEmail = MaskEmail(customer.Email);
                Console.WriteLine($"\nCard type: {customer.CardType.Type}");
                Console.WriteLine($"Full name: {customer.FullName}");
                Console.WriteLine($"Email: {maskedEmail}");
                Console.WriteLine($"Account number: {maskedAccountNumber}");
            }
        }

    }

    public delegate void Receipt(string message);

    public interface IWithdrawal
    {
        void CashWithdrawal(decimal amount, ulong accountNumber);
        void TransactionDetails(ulong accountNumber);
    }

    public interface IAccountBalance
    {
        void AccountBalance(ulong accountNumber);
    }

    public class Withdraw : ATM, IWithdrawal
    {
        public event Receipt withdrawalReceipt;

        public Withdraw(ICustomerDatabase customerDatabase) : base(customerDatabase)
        {
        }

        public void CashWithdrawal(decimal amount, ulong accountNumber)
        {
            var customer = FindCustomer(accountNumber);
            if (customer != null && customer.Balance >= amount)
            {
                customer.Balance -= amount;
                TransactionDetails(accountNumber);
            }
            else
            {
                Console.WriteLine("Insufficient funds!");
            }
        }

        public void TransactionDetails(ulong accountNumber)
        {
            var customer = FindCustomer(accountNumber);
            if (customer != null)
            {
                Console.WriteLine("\n_____________________________________________");
                withdrawalReceipt?.Invoke($"\n\nCash withdrawn successfully!\n");
                Console.WriteLine($"Date withdraw: {DateTime.Now}");
                Console.WriteLine($"\nNew Balance: {customer.Balance:C}");
                Console.WriteLine("\n_____________________________________________");
            }
        }
    }

    public class CheckAccountBalance : ATM, IAccountBalance
    {
        public CheckAccountBalance(ICustomerDatabase customerDatabase) : base(customerDatabase)
        {
        }

        public void AccountBalance(ulong accountNumber)
        {
            var customer = FindCustomer(accountNumber);
            if (customer != null)
            {
                Console.WriteLine("\n____________________________________");
                Console.WriteLine($"\nBalance: {customer.Balance:C}");
                Console.WriteLine("\n____________________________________");
            }
            else
            {
                Console.WriteLine("Customer not found!");
            }
        }
    }

    public class TransferMoney : ATM
    {
        public TransferMoney(ICustomerDatabase customerDatabase) : base(customerDatabase)
        {
        }

        public void Transfer(ulong fromAccount, ulong toAccount, decimal amount)
        {
            var sender = FindCustomer(fromAccount);
            var receiver = FindCustomer(toAccount);

            if (sender != null && receiver != null && sender.Balance >= amount)
            {
                sender.Balance -= amount;
                receiver.Balance += amount;
                Console.WriteLine($"Transfer successful! {amount} transferred from {fromAccount} to {toAccount}.");
            }
            else
            {
                Console.WriteLine("Transfer failed. Check account details or balance.");
            }
        }
    }

    public class TransactionHistory : ATM
    {
        public TransactionHistory(ICustomerDatabase customerDatabase) : base(customerDatabase)
        {
        }

        public void DisplayHistory(ulong accountNumber)
        {
            // Assuming a simple display function for now, as we haven't stored transactions.
            var customer = FindCustomer(accountNumber);
            if (customer != null)
            {
                Console.WriteLine($"Displaying transaction history for account: {accountNumber} (placeholder, as no actual transactions are recorded)");
            }
            else
            {
                Console.WriteLine("Customer not found!");
            }
        }
    }
}

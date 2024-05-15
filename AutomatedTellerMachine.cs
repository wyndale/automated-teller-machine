using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellerMachine
{
    public class Card
    {
        public enum CardType
        {
            Debit,
            ATMOnly,
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

    public class ATM
    {
        private DataSet customersData;
        private decimal balance;
        private Card cardType;

        public decimal Balance { get => balance; set => balance = value; }
        public Card CardType { get => cardType; set => cardType = value; }

        public ATM()
        {
            this.customersData = new DataSet("Customers Information");

            DataTable customersTable = new DataTable("Customers");
            customersTable.Columns.Add("Card type", typeof(Card));
            customersTable.Columns.Add("Fullname", typeof(string));
            customersTable.Columns.Add("Email", typeof(string));
            customersTable.Columns.Add("Account number", typeof(ulong));
            customersTable.Columns.Add("Pin code", typeof(int));
            customersTable.Columns.Add("Balance", typeof(decimal));

            this.customersData.Tables.Add(customersTable);
        }

        public void AddCustomer(string fullname, string email, ulong accountNum, int pincode, decimal balance, Card cardType)
        {
            DataTable customersTable = customersData.Tables["Customers"];
            DataRow row = customersTable.NewRow();

            row["Card type"] = cardType;
            row["Full name"] = fullname;
            row["Email address"] = email;
            row["Account number"] = accountNum;
            row["Pin code"] = pincode;
            row["Balance"] = balance;
            Balance = balance;
            CardType = cardType;

            customersTable.Rows.Add(row);
        }

        public DataRow FindCustomer(ulong accountNum)
        {
            DataTable customersTable = customersData.Tables["Customers"];
            DataRow[] rows = customersTable.Select($"Account number = {accountNum}");
            return rows.Length > 0 ? rows[0] : null;
        }

        public string MaskData(ulong accountNum)
        {
            string accountNumStr = accountNum.ToString();
            return Regex.Replace(accountNumStr, @"(?<=^\d{2})\d", "*");
        }

        public void DisplayMaskInfo()
        {
            DataTable customersTable = customersData.Tables["Customers"];
            foreach (DataRow row in customersTable.Rows)
            {
                string masked = MaskData((ulong)row["Account number"]);
                Console.Write($"\nCard type: {row["Card type"]}\n");
                Console.WriteLine($"Full name: {row["Full name"]}\nEmail: {row["Email address"]}\nAccount number: {row["Account number"]}" +
                    $"\nPin code: {row["Pin code"]}\nBalance: {row["Balance"]}");
            }
        }
    }

    public interface IWithdrawal
    {
        void CashWithdrawal(decimal amount);
        void WithdrawalReceipt();
    }

    public interface IAccountBalance
    {
        void AccountBalance();
    }

    public class Withdraw : ATM, IWithdrawal
    {
        public Withdraw()
        {
        }

        public void CashWithdrawal(decimal amount)
        {
            if (Balance >= amount)
            {
                Balance -= amount;
            }
            else
            {
                throw new InvalidOperationException("Insufficient funds!");
            }
        }

        public void WithdrawalReceipt()
        {

        }
    }

    public class CheckAccountBalance : ATM, IAccountBalance
    {


        public void AccountBalance()
        {
            Console.Write($"Balance: {Balance}");
        }
    }

    public class TransferMoney
    {


    }

    public class TransactionHistory
    {

    }
}

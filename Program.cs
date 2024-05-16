using System;
using TellerMachine;

class Program
{
    static void Main()
    {
        CustomerDatabase customerDatabase = new CustomerDatabase();

        ATM atm = new ATM(customerDatabase);
        Withdraw withdraw = new Withdraw(customerDatabase);
        CheckAccountBalance checkBalance = new CheckAccountBalance(customerDatabase);
        TransferMoney transfer = new TransferMoney(customerDatabase);
        TransactionHistory history = new TransactionHistory(customerDatabase);

        atm.AddCustomer("Wendel Lapura", "lapurawendel95@gmail.com", 12345678, 2004, new Card(Card.CardType.Debit), 1000000.00m);
        atm.AddCustomer("Jane Smith", "jane.smith@example.com", 9876543210, 5678, new Card(Card.CardType.ATM), 50000.00m);

        bool operateIfyes = true;

        while (operateIfyes)
        {
            try
            {
                Console.Write("Enter account number\n\n");
                Console.Write("> ");
                ulong accountNumber = ulong.Parse(Console.ReadLine());

                var customer = customerDatabase.FindCustomer(accountNumber);

                if (customer != null)
                {
                    Console.Write("\nEnter pin code\n\n> ");
                    int pinCode = int.Parse(Console.ReadLine());

                    if (customer.PinCode == pinCode)
                    {
                        atm.DisplayMaskInfo(accountNumber);

                        Console.WriteLine("\n\nSelect operation:\n\n 1. Withdraw 2. Check Balance 3. Transfer Money 4. Transaction History\n\n");
                        Console.Write("> ");
                        int operation = int.Parse(Console.ReadLine());

                        switch (operation)
                        {
                            case 1:
                                Console.Write("\n\nEnter amount to withdraw: ");
                                decimal amount = decimal.Parse(Console.ReadLine());
                                withdraw.withdrawalReceipt += HandleWithdrawalReceipt;
                                withdraw.CashWithdrawal(amount, accountNumber);
                                withdraw.withdrawalReceipt -= HandleWithdrawalReceipt;
                                break;
                            case 2:
                                checkBalance.AccountBalance(accountNumber);
                                break;
                            case 3:
                                Console.Write("Enter destination account number: ");
                                ulong toAccount = ulong.Parse(Console.ReadLine());
                                Console.Write("Enter amount to transfer: ");
                                decimal transferAmount = decimal.Parse(Console.ReadLine());
                                transfer.Transfer(accountNumber, toAccount, transferAmount);
                                break;
                            case 4:
                                history.DisplayHistory(accountNumber);
                                break;
                            default:
                                Console.WriteLine("\nInvalid operation selected.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nIncorrect pin code!");
                    }
                }
                else
                {
                    Console.WriteLine("\nAccount number not found!");
                }

                Console.Write("\n\nWant to operate again?  (yes or no)\n");
                Console.Write("\n> ");
                string response = Console.ReadLine().ToLower();

                if (response == "no")
                {
                    operateIfyes = false;
                }
                else if (response != "yes")
                {
                    throw new InvalidOperationException("\nInvalid input. Please enter 'yes' or 'no' only!\n");
                }
            }
            catch
            {
                Console.WriteLine($"\nSomething went wrong!\n");
            }
            Console.WriteLine();
        }

        Console.ReadKey();
    }

    static void HandleWithdrawalReceipt(string message)
    {
        Console.WriteLine(message);
    }
}

using System;
using TellerMachine;
using BankSystem;

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
        atm.AddCustomer("Jane Smith", "jane.smith@example.com", 98765, 5678, new Card(Card.CardType.ATM), 50000.00m);

        BankCustomerDatabase bankdatabase = new BankCustomerDatabase();

        Bank bank = new Bank(bankdatabase);
        DepositAccount depositAccount = new DepositAccount(bankdatabase);
        LoanAccount loanAccount = new LoanAccount(bankdatabase);
        MortgageAccount mortgageAccount = new MortgageAccount(bankdatabase);
        BankTransactionHistory bankHistory = new BankTransactionHistory(bankdatabase);

        bank.AddCustomer("Wyndale Ocero", "wendellapura@gmail.com", 082004, CustomerType.Customer_Type.Individual, 1000000000m);
        bank.AddCustomer("Wendel Tabid", "enchanteurtsukuba88@gmail.com", 200408, CustomerType.Customer_Type.Company, 9999999999m);

        bool operations = true;
        bool continueSession = true;

        while (continueSession)
        {

            Console.WriteLine("Select operation:\n\n1. Bank Operation\n2. Automated Teller Machine\n3. Exit\n\n");
            Console.Write("> ");
            int inputOperation = int.Parse(Console.ReadLine());
            Console.WriteLine();
            switch (inputOperation)
            {
                case 1:
                    Bank();
                    break;
                case 2:
                    Teller();
                    break;
                case 3:
                    Console.WriteLine("\nExiting...\n");
                    return;
                default:
                    Console.WriteLine("Invalid number of operation!");
                    break;
            }

            void Bank()
            {
                try
                {
                    Console.WriteLine("\nBANK SYSTEM");
                    Console.Write("\nEnter full name\n\n> ");
                    string fullname = Console.ReadLine();
                    Console.Write("\nEnter password\n\n> ");
                    int password = int.Parse(Console.ReadLine());

                    var bankCustomer = bankdatabase.FindCustomer(fullname, password);

                    if (bankCustomer != null && bankCustomer.Password == password)
                    {
                        bank.DisplayEmail(fullname, password);
                        while (operations)
                        {
                            Console.WriteLine("\n\nSelect bank operation:\n\n1. Deposit\n2. Loan\n3. Mortgage\n4. Transaction History\n5. Exit\n");
                            Console.Write("> ");
                            int bank_operation = int.Parse(Console.ReadLine());

                            switch (bank_operation)
                            {
                                case 1:
                                    Console.WriteLine("\nChoose an option:");
                                    Console.WriteLine("1. Deposit");
                                    Console.WriteLine("2. Withdraw");
                                    Console.Write("\n> ");
                                    int depositOption = int.Parse(Console.ReadLine());
                                    if (depositOption == 1)
                                    {
                                        Console.Write("\nEnter amount to deposit: ");
                                        decimal depositAmount = decimal.Parse(Console.ReadLine());
                                        depositAccount.Deposit(bankCustomer, depositAmount);
                                    }
                                    else if (depositOption == 2)
                                    {
                                        Console.Write("\nEnter amount to withdraw: ");
                                        decimal withdrawAmount = decimal.Parse(Console.ReadLine());
                                        try
                                        {
                                            depositAccount.Withdraw(bankCustomer, withdrawAmount);
                                        }
                                        catch (InvalidOperationException ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid option.");
                                    }
                                    break;
                                case 2:
                                    Console.WriteLine("\nChoose an option:");
                                    Console.WriteLine("1. Apply for Loan");
                                    Console.WriteLine("2. Make Loan Payment");
                                    Console.Write("\n> ");
                                    int loanOption = int.Parse(Console.ReadLine());
                                    if (loanOption == 1)
                                    {
                                        Console.Write("\nEnter amount to loan: ");
                                        decimal loanAmount = decimal.Parse(Console.ReadLine());
                                        loanAccount.ApplyForLoan(bankCustomer, loanAmount);
                                    }
                                    else if (loanOption == 2)
                                    {
                                        Console.Write("\nEnter amount to pay for loan: ");
                                        decimal loanPaymentAmount = decimal.Parse(Console.ReadLine());
                                        loanAccount.MakeLoanPayment(bankCustomer, loanPaymentAmount);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid option.");
                                    }
                                    break;
                                case 3:
                                    Console.WriteLine("\nChoose an option:");
                                    Console.WriteLine("1. Apply for Mortgage");
                                    Console.WriteLine("2. Make Mortgage Payment");
                                    Console.Write("\n> ");
                                    int mortgageOption = int.Parse(Console.ReadLine());
                                    if (mortgageOption == 1)
                                    {
                                        Console.Write("\nEnter amount to mortgage: ");
                                        decimal mortgageAmount = decimal.Parse(Console.ReadLine());
                                        mortgageAccount.ApplyForMortgage(bankCustomer, mortgageAmount);
                                    }
                                    else if (mortgageOption == 2)
                                    {
                                        Console.Write("\nEnter amount to pay for mortgage: ");
                                        decimal mortgagePaymentAmount = decimal.Parse(Console.ReadLine());
                                        mortgageAccount.MakeMortgagePayment(bankCustomer, mortgagePaymentAmount);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid option.");
                                    }
                                    break;
                                case 4:
                                    bankHistory.DisplayHistory(fullname, password);
                                    break;
                                case 5:
                                    Console.WriteLine("\nExiting...\n");
                                    return;
                                default:
                                    Console.WriteLine("Invalid operation!");
                                    break;
                            }

                            Console.Write("\n\nWant to transact again?  (yes or no)\n");
                            Console.Write("\n> ");
                            string response = Console.ReadLine().ToLower();

                            if (response == "no")
                            {
                                operations = false;
                            }
                            else if (response != "yes")
                            {
                                throw new InvalidOperationException("\nInvalid input. Please enter 'yes' or 'no' only!\n");
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid name or password");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("\n");
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine();
            }

            void Teller()
            {
                try
                {
                    Console.WriteLine("\nAUTOMATED TELLER MACHINE");
                    Console.Write("\n\nEnter account number\n\n");
                    Console.Write("> ");
                    ulong accountNumber = ulong.Parse(Console.ReadLine());

                    var customer = customerDatabase.FindCustomer(accountNumber);

                    if (customer != null)
                    {
                        Console.Write("\nEnter pin code\n\n> ");
                        int pinCode = int.Parse(Console.ReadLine());

                        if (customer.PinCode == pinCode)
                        {
                            while (operations)
                            {
                                atm.DisplayMaskInfo(accountNumber);

                                Console.WriteLine("\n\nSelect operation:\n\n 1. Withdraw 2. Check Balance 3. Transfer Money 4. Transaction History 5. Exit\n\n");
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
                                        Console.Write("\nEnter the destination account number: ");
                                        ulong toAccount = ulong.Parse(Console.ReadLine());
                                        atm.DisplayMaskInfo(toAccount);
                                        Console.Write("\nEnter amount to transfer: ");
                                        decimal transferAmount = decimal.Parse(Console.ReadLine());
                                        transfer.Transfer(accountNumber, toAccount, transferAmount);
                                        break;
                                    case 4:
                                        history.DisplayHistory(accountNumber);
                                        break;
                                    case 5:
                                        Console.WriteLine("\nExiting...\n");
                                        return;
                                    default:
                                        Console.WriteLine("\nInvalid operation selected.");
                                        break;
                                }

                                Console.Write("\n\nWant to transact again?  (yes or no)\n");
                                Console.Write("\n> ");
                                string response = Console.ReadLine().ToLower();

                                if (response == "no")
                                {
                                    operations = false;
                                }
                                else if (response != "yes")
                                {
                                    throw new InvalidOperationException("\nInvalid input. Please enter 'yes' or 'no' only!\n");
                                }
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
                }
                catch
                {
                    Console.WriteLine($"\nWrong account number!\n");
                }
                Console.WriteLine();
            }
        }
        Console.ReadKey();
    }

    static void HandleWithdrawalReceipt(string message)
    {
        Console.WriteLine(message);
    }
}

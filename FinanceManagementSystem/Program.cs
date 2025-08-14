using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    // Core model using record type for immutability
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // Interface for payment behavior
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // Concrete implementations of ITransactionProcessor
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[BANK TRANSFER] Processing ${transaction.Amount:F2} transaction for {transaction.Category}");
            Console.WriteLine($"Transfer completed via traditional banking system on {transaction.Date:yyyy-MM-dd}");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[MOBILE MONEY] Processing ${transaction.Amount:F2} transaction for {transaction.Category}");
            Console.WriteLine($"Mobile payment completed instantly on {transaction.Date:yyyy-MM-dd}");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[CRYPTO WALLET] Processing ${transaction.Amount:F2} transaction for {transaction.Category}");
            Console.WriteLine($"Blockchain transaction recorded on {transaction.Date:yyyy-MM-dd}");
        }
    }

    // Base Account class
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied to Account {AccountNumber}. New balance: ${Balance:F2}");
        }
    }

    // Sealed specialized account class
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) 
            : base(accountNumber, initialBalance)
        {
        }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine($"Insufficient funds in Savings Account {AccountNumber}. " +
                                $"Required: ${transaction.Amount:F2}, Available: ${Balance:F2}");
                return;
            }

            Balance -= transaction.Amount;
            Console.WriteLine($"Savings Account {AccountNumber} - Transaction processed successfully. " +
                            $"Updated balance: ${Balance:F2}");
        }
    }

    // Main finance application class
    public class FinanceApp
    {
        private List<Transaction> transactions = new List<Transaction>();

        public void Run()
        {
            Console.WriteLine("=== Finance Management System ===\n");

            // Step i: Instantiate SavingsAccount
            var savingsAccount = new SavingsAccount("SAV-001", 1000.00m);
            Console.WriteLine($"Created Savings Account: {savingsAccount.AccountNumber} with initial balance: ${savingsAccount.Balance:F2}\n");

            // Step ii: Create three Transaction records
            var transaction1 = new Transaction(1, DateTime.Now.AddDays(-2), 150.75m, "Groceries");
            var transaction2 = new Transaction(2, DateTime.Now.AddDays(-1), 89.50m, "Utilities");
            var transaction3 = new Transaction(3, DateTime.Now, 45.25m, "Entertainment");

            // Step iii: Create processors and process transactions
            var mobileProcessor = new MobileMoneyProcessor();
            var bankProcessor = new BankTransferProcessor();
            var cryptoProcessor = new CryptoWalletProcessor();

            // Process Transaction 1 with MobileMoneyProcessor
            Console.WriteLine("--- Processing Transaction 1 ---");
            mobileProcessor.Process(transaction1);
            Console.WriteLine();

            // Process Transaction 2 with BankTransferProcessor
            Console.WriteLine("--- Processing Transaction 2 ---");
            bankProcessor.Process(transaction2);
            Console.WriteLine();

            // Process Transaction 3 with CryptoWalletProcessor
            Console.WriteLine("--- Processing Transaction 3 ---");
            cryptoProcessor.Process(transaction3);
            Console.WriteLine();

            // Step iv: Apply each transaction to the SavingsAccount
            Console.WriteLine("--- Applying Transactions to Savings Account ---");
            savingsAccount.ApplyTransaction(transaction1);
            savingsAccount.ApplyTransaction(transaction2);
            savingsAccount.ApplyTransaction(transaction3);
            Console.WriteLine();

            // Step v: Add all transactions to transactions list
            transactions.Add(transaction1);
            transactions.Add(transaction2);
            transactions.Add(transaction3);

            // Display summary
            Console.WriteLine("--- Transaction Summary ---");
            Console.WriteLine($"Total transactions processed: {transactions.Count}");
            decimal totalAmount = 0;
            foreach (var txn in transactions)
            {
                Console.WriteLine($"ID: {txn.Id}, Date: {txn.Date:yyyy-MM-dd}, Amount: ${txn.Amount:F2}, Category: {txn.Category}");
                totalAmount += txn.Amount;
            }
            Console.WriteLine($"Total transaction amount: ${totalAmount:F2}");
            Console.WriteLine($"Final account balance: ${savingsAccount.Balance:F2}");

            // Demonstrate insufficient funds scenario
            Console.WriteLine("\n--- Testing Insufficient Funds Scenario ---");
            var largeTransaction = new Transaction(4, DateTime.Now, 2000.00m, "Large Purchase");
            savingsAccount.ApplyTransaction(largeTransaction);
        }
    }

    // Main program entry point
    class Program
    {
        static void Main(string[] args)
        {
            // Step i: Create an instance of FinanceApp and call Run()
            var financeApp = new FinanceApp();
            financeApp.Run();
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
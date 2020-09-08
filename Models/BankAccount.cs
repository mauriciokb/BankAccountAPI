using System.Collections.Generic;

namespace BankAccountWebAPI
{
    /// <summary>
    /// Represents a simple bank account
    /// </summary>
    public class BankAccount
    {    
        public int BankAccountId { get; private set; }

        public string OwnerName { get; private set; }

        public decimal Balance { get; private set; }

        public BankAccount(int bankAccountId, string ownerName, decimal balance)
        {
            BankAccountId = bankAccountId;
            OwnerName = ownerName;
            Balance = balance;
        }
        public BankAccount(string OwnerName)
        {
            this.OwnerName = OwnerName;
            this.Balance = 0;
        }

        public void Deposit(decimal amount)
        {
            this.Balance += amount;
        }

        public bool Widthdraw(decimal amount)
        {
            if(this.Balance - amount >= 0)
            {
                this.Balance -= amount;
                return true;
            }

            return false;
        }
    }
}
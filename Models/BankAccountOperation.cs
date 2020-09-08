using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccountWebAPI
{
    /// <summary>
    /// Encapsulates all data required to perform single bank account operations like depoist or withdraw
    /// </summary>
    public class SingleAccountOperation
    {
        public SingleAccountOperation(){}

        public SingleAccountOperation(BankAccount bankAccount)
        {
            this.PrimaryBankAcc = bankAccount;
            this.PrimaryBankAccId = bankAccount.BankAccountId;
        }

        public int Id { get; set; }

        public decimal Amount { get; set; }

        public decimal TaxAmount { get; set; }

        public System.DateTime ExecutionTimeStamp { get; set; }

        public OperationType OperationType { get; set; }
        
        [ForeignKey("PrimaryBankAcc")]
        public int PrimaryBankAccId { get; set; }

        public virtual BankAccount PrimaryBankAcc { get; set; }
    
    }

    /// <summary>
    /// Encapsulates all data required to perform double bank account operations like transfer
    /// </summary>
    public class DoubleAccountOperation : SingleAccountOperation
    {
        public DoubleAccountOperation(){}

        public DoubleAccountOperation(BankAccount primaryBankAcc, BankAccount secondaryBankAcc)
        : base(primaryBankAcc)
        {
            this.SecondaryBankAcc = secondaryBankAcc;
            this.SecondaryBankAccId = SecondaryBankAccId;
        }
        
        [ForeignKey("SecondaryBankAcc")]
        public int SecondaryBankAccId { get; set; }
        
        public virtual BankAccount SecondaryBankAcc { get; set; }

        public decimal ExtraTaxAmount { get; set; }
    }

    public enum OperationType
    {
        DEPOSIT,
        WIDTHDRAW,
        TRANSFERENCE,
    };
    }
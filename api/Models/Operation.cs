using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccountWebAPI
{
    /// <summary>
    /// Encapsulates all data required to perform single bank account operations like depoist or withdraw
    /// </summary>
    public class SingleAccountOperation
    {
        public SingleAccountOperation(){}

        public SingleAccountOperation(Account account, decimal amount, OperationType operationType)
        {
            this.PrimaryAcc = account;
            this.PrimaryAccId = account.AccountId;
            this.Amount = amount;
            this.OperationType = operationType;
        }

        public int Id { get; private set; }

        public decimal Amount { get; private set; }

        public OperationType OperationType { get; private set; }

        [ForeignKey("PrimaryAcc")]
        public int PrimaryAccId { get; private set; }

        public virtual Account PrimaryAcc { get; private set; }

        public decimal TaxAmount { get; set; }

        public System.DateTime ExecutionTimeStamp { get; set; }
    
    }

    /// <summary>
    /// Encapsulates all data required to perform double bank account operations like transfer
    /// </summary>
    public class DoubleAccountOperation : SingleAccountOperation
    {
        public DoubleAccountOperation(){}

        public DoubleAccountOperation(Account primaryAcc, Account secondaryAcc, decimal amount, OperationType operationType)
        : base(primaryAcc, amount, operationType)
        {
            this.SecondaryAcc = secondaryAcc;
            this.SecondaryAccId = secondaryAcc.AccountId;
        }
        
        [ForeignKey("SecondaryAcc")]
        public int SecondaryAccId { get; private set; }
        
        public virtual Account SecondaryAcc { get; private set; }

        public decimal ExtraTaxAmount { get; set; }
    }

    public enum OperationType
    {
        DEPOSIT,
        WIDTHDRAW,
        TRANSFERENCE,
    };
}
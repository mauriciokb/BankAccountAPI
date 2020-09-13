using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccountWebAPI
{
    /// <summary>
    /// Encapsulates all data required to perform single bank account operations like depoist or withdraw
    /// </summary>
    public class SingleAccountOperation
    {
        public SingleAccountOperation(){}

        public SingleAccountOperation(Account account)
        {
            this.PrimaryAcc = account;
            this.PrimaryAccId = account.AccountId;
        }

        public int Id { get; set; }

        public decimal Amount { get; set; }

        public decimal TaxAmount { get; set; }

        public System.DateTime ExecutionTimeStamp { get; set; }

        public OperationType OperationType { get; set; }
        
        [ForeignKey("PrimaryAcc")]
        public int PrimaryAccId { get; set; }

        public virtual Account PrimaryAcc { get; set; }
    
    }

    /// <summary>
    /// Encapsulates all data required to perform double bank account operations like transfer
    /// </summary>
    public class DoubleAccountOperation : SingleAccountOperation
    {
        public DoubleAccountOperation(){}

        public DoubleAccountOperation(Account primaryAcc, Account secondaryAcc)
        : base(primaryAcc)
        {
            this.SecondaryAcc = secondaryAcc;
            this.SecondaryAccId = secondaryAcc.AccountId;
        }
        
        [ForeignKey("SecondaryAcc")]
        public int SecondaryAccId { get; set; }
        
        public virtual Account SecondaryAcc { get; set; }

        public decimal ExtraTaxAmount { get; set; }
    }

    public enum OperationType
    {
        DEPOSIT,
        WIDTHDRAW,
        TRANSFERENCE,
    };
    }
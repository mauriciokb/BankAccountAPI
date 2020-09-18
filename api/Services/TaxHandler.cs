
namespace BankAccountWebAPI
{
 
    public class WithdrawTaxApplier : ITaxApplier<SingleAccountOperation>
    {
        public void Apply(SingleAccountOperation op)
        {
            op.TaxAmount = 4m;
        }
    }

    public class DepositTaxApplier : ITaxApplier<SingleAccountOperation>
    {        
        public void Apply(SingleAccountOperation op)
        {
            op.TaxAmount = System.Math.Floor((op.Amount * 0.01m) * 100m) / 100m;
         
        }
    }

    public class TransferenceTaxApplier : ITaxApplier<DoubleAccountOperation>
    {
        public void Apply(DoubleAccountOperation op)
        {      
            op.TaxAmount = 1m;
            op.ExtraTaxAmount = 0m;
        }
    }
}

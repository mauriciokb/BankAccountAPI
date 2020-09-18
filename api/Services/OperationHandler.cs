namespace BankAccountWebAPI
{

    /// <summary>
    /// Class responsible for executing withdraw operations
    /// </summary>
    public class WidthdrawHandler : IOperation<SingleAccountOperation>
    {
        public bool Execute(SingleAccountOperation opData)
        {
            decimal ammountToWithdraw = opData.Amount + opData.TaxAmount;

            opData.ExecutionTimeStamp = System.DateTime.Now;

            return opData.PrimaryAcc.Widthdraw(ammountToWithdraw);        
        }
    }

    /// <summary>
    /// Class responsible for executing deposit operations
    /// </summary>
    public class DepositHandler : IOperation<SingleAccountOperation>
    {
        public bool Execute(SingleAccountOperation opData)
        {
            decimal ammountToDeposit = opData.Amount + opData.TaxAmount;

            opData.PrimaryAcc.Deposit(ammountToDeposit);

            opData.ExecutionTimeStamp = System.DateTime.Now;

            return true;
        }
    }

    /// <summary>
    /// Class responsible for executing transfer operations
    /// </summary>
    public class TransferenceHandler : IOperation<DoubleAccountOperation>
    {
        public bool Execute(DoubleAccountOperation opData)
        {
            decimal ammountToWithdraw = opData.Amount + opData.TaxAmount;

            // Withdraws from the primary (source) account
            bool withdrawResult = opData.PrimaryAcc.Widthdraw(ammountToWithdraw);

            if (withdrawResult)
            {
                decimal ammountToDeposit = opData.Amount + opData.ExtraTaxAmount;

                // Deposits the same amount (plus taxes) to the secondary (destination) account
                opData.SecondaryAcc.Deposit(ammountToDeposit);

                opData.ExecutionTimeStamp = System.DateTime.Now;

                return true;
            }

            return false;
        }
    }
}
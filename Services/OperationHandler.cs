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

        return opData.PrimaryBankAcc.Widthdraw(ammountToWithdraw);
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

        opData.PrimaryBankAcc.Deposit(ammountToDeposit);

        return true;
    }
}

/// <summary>
/// Class responsible for executing transfer operations
/// </summary>
public class TransferHandler : IOperation<DoubleAccountOperation>
{
    public bool Execute(DoubleAccountOperation opData)
    {
        decimal ammountToWithdraw = opData.Amount + opData.TaxAmount;

        // Withdraws from the primary (source) account
        bool withdrawResult = opData.PrimaryBankAcc.Widthdraw(ammountToWithdraw);

        if(withdrawResult)
        {
            decimal ammountToDeposit = opData.Amount + opData.ExtraTaxAmount;

            // Deposits the same amount (plus taxes) to the secondary (destination) account
            opData.SecondaryBankAcc.Deposit(ammountToDeposit);
            return true;
        }

        return false;
    }
}
}
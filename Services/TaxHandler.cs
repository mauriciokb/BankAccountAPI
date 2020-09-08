
namespace BankAccountWebAPI
{
/// <summary>
/// Decorates single account operation handlers to calculate and apply taxes
/// </summary>
public class SingleAccountOpTaxHandler : IOperation<SingleAccountOperation>
{
    private IOperation<SingleAccountOperation> decoratedOpHandler;

    public SingleAccountOpTaxHandler(IOperation<SingleAccountOperation> opHandler)
    {
        decoratedOpHandler = opHandler;
    }

    public bool Execute(SingleAccountOperation op)
    {
        decimal amount = op.Amount;
        decimal tax = 0m;

        switch(op.OperationType)
        {
            case OperationType.WIDTHDRAW:
                tax = 4m;
            break;
            case OperationType.DEPOSIT:
                // Ensures that tax will be rounded downwards with 2 decimal places
                tax = System.Math.Floor((amount * 0.01m) * 100m) / 100m;
            break;
        }

        op.TaxAmount = tax;

        return decoratedOpHandler.Execute(op);
    }
}

/// <summary>
/// Decorates double account operation handlers to calculate and apply taxes
/// </summary>
public class DoubleAccountOpTaxHandler : IOperation<DoubleAccountOperation>
{
    private IOperation<DoubleAccountOperation> decoratedOpHandler;

    public DoubleAccountOpTaxHandler(IOperation<DoubleAccountOperation> op)
    {
        decoratedOpHandler = op;
    }

    public bool Execute(DoubleAccountOperation op)
    {
        decimal ammount = op.Amount;
        decimal primaryAccountTax = 0m;
        decimal secondaryAccountTax = 0m;

        switch(op.OperationType)
        {
            case OperationType.TRANSFERENCE:
                primaryAccountTax = 1m;
                secondaryAccountTax = 0m;
            break;
            default: 
                return false;
        }

        op.TaxAmount = primaryAccountTax;
        op.ExtraTaxAmount = secondaryAccountTax;

        return decoratedOpHandler.Execute(op);
    }
}
}

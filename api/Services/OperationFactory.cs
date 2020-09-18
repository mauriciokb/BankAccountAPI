using System.Collections.Generic;

namespace BankAccountWebAPI
{
    class WithdrawOperation : IOperationCreator<SingleAccountOperation>
    {
        public SingleAccountOperation Create(List<Account> accounts, decimal amount)
        {
            SingleAccountOperation op = new SingleAccountOperation(accounts[0]);
            op.Amount = amount;
            op.OperationType = OperationType.WIDTHDRAW;

            return op;
        }
    }

    class DepositOperation : IOperationCreator<SingleAccountOperation>
    {
        public SingleAccountOperation Create(List<Account> accounts, decimal amount)
        {
            SingleAccountOperation op = new SingleAccountOperation(accounts[0]);
            op.Amount = amount;
            op.OperationType = OperationType.DEPOSIT;

            return op;
        }
    }

    class TransferenceOperation : IOperationCreator<DoubleAccountOperation>
    {
        public DoubleAccountOperation Create(List<Account> accounts, decimal amount)
        {
            DoubleAccountOperation op = new DoubleAccountOperation(accounts[0], ((List<Account>)accounts)[1]);
            op.Amount = amount;
            op.OperationType = OperationType.TRANSFERENCE;

            return op;
        }
    }
}
using System.Collections.Generic;

namespace BankAccountWebAPI
{
    class WithdrawOperation : IOperationCreator<SingleAccountOperation>
    {
        public SingleAccountOperation Create(List<Account> accounts, decimal amount)
        {
            SingleAccountOperation op = new SingleAccountOperation(accounts[0], amount, OperationType.WIDTHDRAW);

            return op;
        }
    }

    class DepositOperation : IOperationCreator<SingleAccountOperation>
    {
        public SingleAccountOperation Create(List<Account> accounts, decimal amount)
        {
            SingleAccountOperation op = new SingleAccountOperation(accounts[0], amount, OperationType.DEPOSIT);

            return op;
        }
    }

    class TransferenceOperation : IOperationCreator<DoubleAccountOperation>
    {
        public DoubleAccountOperation Create(List<Account> accounts, decimal amount)
        {
            DoubleAccountOperation op = new DoubleAccountOperation(accounts[0], accounts[1], amount, OperationType.TRANSFERENCE);
  
            return op;
        }
    }
}
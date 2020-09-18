using System.Collections.Generic;

namespace BankAccountWebAPI
{
   
    public class CompleteOperation<T>
    {
        IDataReader dbReader;

        IOperationCreator<T> opCreator;

        IOperation<T> opHandler;

        ITaxApplier<T> taxApplier;

        IPersistor<T> opPersistor;


        public CompleteOperation(IDataReader dbReader, 
                                 IOperationCreator<T> opCreator, 
                                 IOperation<T> opHandler, 
                                 ITaxApplier<T> taxApplier,
                                 IPersistor<T> opPersistor)
        {
            this.dbReader = dbReader;
            this.opCreator = opCreator;
            this.opHandler = opHandler;
            this.taxApplier = taxApplier;
            this.opPersistor = opPersistor;
        }

        public List<Account> Execute(List<int> accIds, decimal amount)
        {
            List<Account> accounts = new List<Account>();

            // Fetches the account(s)
            for (int i = 0; i < accIds.Count; i++)
            {
                Account acc = dbReader.GetAccountById(accIds[i]);
                if(acc == null)
                {
                    throw new System.Collections.Generic.KeyNotFoundException("Account (" + accIds[i] + ") not found.");
                }

                accounts.Add(acc);
            }

            // Creates the operation
            T operation = opCreator.Create(accounts, amount);

            // Applies taxes
            taxApplier.Apply(operation);

            // Executes the operation
            if (!opHandler.Execute(operation))
            {
                throw new System.InvalidOperationException("Unable to perform this operation.");
            }

            // Persists the operation
            opPersistor.Persist(operation);

            return accounts;
        }
    }

    public class Withdraw : CompleteOperation<SingleAccountOperation>
    {
        public Withdraw(IDataReader dataReader, IPersistor<SingleAccountOperation> opPersistor)
        : base(dataReader,
               new WithdrawOperation(),
               new WidthdrawHandler(),
               new WithdrawTaxApplier(),
               opPersistor)
        {}
    }

    public class Deposit : CompleteOperation<SingleAccountOperation>
    {
        public Deposit(IDataReader dataReader, IPersistor<SingleAccountOperation> opPersistor)
        : base(dataReader,
               new DepositOperation(),
               new DepositHandler(),
               new DepositTaxApplier(),
               opPersistor)
        { }
    }

    public class Transference : CompleteOperation<DoubleAccountOperation>
    {
        public Transference(IDataReader dataReader, IPersistor<DoubleAccountOperation> opPersistor)
        : base(dataReader,
               new TransferenceOperation(),
               new TransferenceHandler(),
               new TransferenceTaxApplier(),
               opPersistor)
        { }
    }
}
using System.Collections.Generic;

namespace BankAccountWebAPI
{
    public interface IOperationCreator<T>
    {
        T Create(List<Account> accounts, decimal amount);
    }

    public interface IOperation<T>
    {
        bool Execute(T op);
    }

    public interface ITaxApplier<T>
    {
        void Apply(T data);
    }

    public interface IPersistor<T>
    {
        bool Persist(T data);
    }

    public interface IDataReader
    {
        Account GetAccountById(int id);

        System.Collections.Generic.List<SingleAccountOperation> GetOperationsByAccountId(int id);
    }

}

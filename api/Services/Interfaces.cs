namespace BankAccountWebAPI
{
    public interface IOperation<T>
    {
        bool Execute(T op);
    }

    public interface ITaxApplier<T>
    {
        bool Apply(T data);
    }

    public interface IPersist<T>
    {
        bool Persist(T data);
    }

    public interface IReadData
    {
        bool GetAccountById(int id, out Account data);
  
        bool GetOperationsByAccountId(int id, out System.Collections.Generic.List<SingleAccountOperation> operations);
    }
}

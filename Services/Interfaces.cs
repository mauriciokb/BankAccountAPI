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

    public interface IRead<T>
    {
        bool Read(int id, out T data);
    }
}

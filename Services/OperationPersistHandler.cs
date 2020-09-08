namespace BankAccountWebAPI
{
    /// <summary>
    /// Base class for handling the persistence of the data 
    /// (operations history and bank account balance) 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OpPersistHandlerBase<T> : IOperation<T>
    {        
        private IOperation<T> opHandler;
        private IPersist<T> persist;

        public OpPersistHandlerBase(IOperation<T> opHandler, IPersist<T> persist)
        {
            this.opHandler = opHandler;
            this.persist = persist;
        }

        public bool Execute(T data)
        {
            if(opHandler.Execute(data))
            {
                if(persist.Persist(data))
                {
                   return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Decorates single account operation handlers to persist succeeded operations
    /// </summary>
    public class SingleAccountOpPersistHandler : OpPersistHandlerBase<SingleAccountOperation>
    {
        public SingleAccountOpPersistHandler(IOperation<SingleAccountOperation> opHandler, 
                                             IPersist<SingleAccountOperation> opPersistObj) 
                                             : base(opHandler, opPersistObj)
        {

        }
    }

    /// <summary>
    /// Decorates double account operation handlers to persist succeeded operations
    /// </summary>
    public class DoubleAccountOpPersistHandler : OpPersistHandlerBase<DoubleAccountOperation>
    {
        public DoubleAccountOpPersistHandler(IOperation<DoubleAccountOperation> opHandler, 
                                             IPersist<DoubleAccountOperation> opPersistObj) 
                                             : base(opHandler, opPersistObj)
        {

        }
    }
}
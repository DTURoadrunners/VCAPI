namespace VCAPI.Repository.MySQL.Exceptions
{
    [System.Serializable]
    public class DuplicateEntryException : System.Exception
    {
        public DuplicateEntryException() { }
        public DuplicateEntryException(string message) : base(message) { }
        public DuplicateEntryException(string message, System.Exception inner) : base(message, inner) { }
        protected DuplicateEntryException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
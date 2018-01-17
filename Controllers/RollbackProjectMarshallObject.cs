namespace VCAPI.Controllers
{
    /// <summary>
    /// All Rollback functions require a RollbackProjectMarshallObject, 
    /// this contains the revision you want to go back to and a comment for the coresponding log.
    /// </summary>
    public class RollbackProjectMarshallObject
    {   public string comment;
        public int revision;
    }
}
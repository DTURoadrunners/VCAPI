namespace VCAPI.Repository
{
    /// <summary>
    /// The rank of a user determines if a user can do a certain thing.
    /// </summary>
    public enum RANK
    {
            PROHIBITED = 1,
            GUEST = 2,
            STUDENT = 3,
            ADMIN = 4,
            SUPERADMIN = 5
    }
}
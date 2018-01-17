namespace VCAPI.Repository
{
    /// <summary>
    /// The rank of a user determines if a user can do a certain thing.
    /// A guest can only watch
    /// A student can modify and add categories, componentTypes and components
    /// An admin can add people to a project
    /// A superadmin can modify and add projects
    /// Every rank can do everything previous rank(s) can do
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
namespace VCAPI.Repository
{
    /// <summary>
<<<<<<< HEAD
    /// The rank of a user determines if a user can do a certain thing.
    /// A guest can only watch
    /// A student can modify and add categories, componentTypes and components
    /// An admin can add people to a project
    /// A superadmin can modify and add projects
    /// Every rank can do everything previous rank(s) can do
=======
    /// The rank of a user determines if a user can do a certain thing. Every rank inherits the previous rank. 
>>>>>>> 40f3a4ecf23eeee2363c52d31520564ad7bfe820
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
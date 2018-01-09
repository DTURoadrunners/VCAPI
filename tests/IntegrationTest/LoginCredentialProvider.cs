using static VCAPI.Controllers.LoginController;

namespace tests.IntegrationTest
{
    public static class LoginCredentialProvider
    {
        public static LoginCredentials GetSuperAdmin()
        {
            return new LoginCredentials(){
                    username = "Hello",
                    password = "Sesame"
            };
        }

        
    }
}
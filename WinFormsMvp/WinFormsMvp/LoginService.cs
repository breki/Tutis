namespace WinFormsMvp
{
    public class LoginService : ILoginService
    {
        public bool AreCredentialsValid(string userName, string password)
        {
            return true;
        }
    }
}
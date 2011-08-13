using System;

namespace WinFormsMvp
{
    public interface ILoginService
    {
        bool AreCredentialsValid(string userName, string password);
    }
}
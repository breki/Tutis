using System;

namespace WinFormsMvp
{
    public class LoginPresenter
    {
        public LoginPresenter(ILoginView view, ILoginService loginService)
        {
            this.view = view;
            this.loginService = loginService;
            view.LoginButtonClicked += OnLoginButtonClicked;
        }

        public void Run()
        {
            view.Run();
        }

        private void OnLoginButtonClicked(object sender, EventArgs e)
        {
            if (loginService.AreCredentialsValid(view.UserName, view.Password))
                view.Close();

            // some error handling logic
        }

        private readonly ILoginView view;
        private readonly ILoginService loginService;
    }
}
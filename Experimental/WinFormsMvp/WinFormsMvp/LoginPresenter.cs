using System;

namespace WinFormsMvp
{
    public class LoginPresenter : DialogPresenterBase<ILoginView>
    {
        public LoginPresenter(ILoginView view, ILoginService loginService)
            : base(view)
        {
            this.loginService = loginService;
            view.LoginButtonClicked += OnLoginButtonClicked;
        }

        private void OnLoginButtonClicked(object sender, EventArgs e)
        {
            if (loginService.AreCredentialsValid(View.UserName, View.Password))
                View.Close();

            // some error handling logic
        }

        private readonly ILoginService loginService;
    }
}
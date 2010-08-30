using System;
using System.Windows.Forms;
using Castle.Facilities.Startable;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace WinFormsMvp
{
    public class WinFormsMvpInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<StartableFacility>();

            container.Register(Component.For<ILoginView>().ImplementedBy<LoginForm>());
            container.Register(Component.For<LoginPresenter>()
                .StartUsingMethod(x => x.Run));

            container.Register(Component.For<ILoginService>().ImplementedBy<LoginService>());
        }
    }
}
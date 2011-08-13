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
            WinFormsMvpFacility mvcFacility = new WinFormsMvpFacility();

            container.AddFacility(mvcFacility.GetType().FullName, mvcFacility);

            mvcFacility
                .MainForm<MainAppPresenter, IMainAppView, MainForm>()
                .Dialog<LoginPresenter, ILoginView, LoginForm>()
                .Document<MyDocumentPresenter, IMyDocumentView, MyDocumentForm>();

                //.RegisterAllOfMvc(typeof(MainForm).Assembly);

            //container.Register(Component.For<IMainAppView>().ImplementedBy<MainForm>().LifeStyle.Transient);
            //container.Register(Component.For<MainAppPresenter>()
            //    .StartUsingMethod(x => x.Run).LifeStyle.Transient);

            //container.Register(Component.For<ILoginView>().ImplementedBy<LoginForm>().LifeStyle.Transient);
            //container.Register(Component.For<LoginPresenter>().LifeStyle.Transient);

            container.Register(Component.For<ILoginService>().ImplementedBy<LoginService>());

            mvcFacility.ApplicationCanStart();
        }
    }
}
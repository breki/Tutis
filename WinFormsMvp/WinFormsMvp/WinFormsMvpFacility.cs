using System;
using Castle.Facilities.Startable;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;

namespace WinFormsMvp
{
    public class WinFormsMvpFacility : AbstractFacility
    {
        public void ApplicationCanStart()
        {
            DialogsRunner dialogsRunner = new DialogsRunner(Kernel);
            Kernel.Register(Component.For<IDialogsRunner>().Instance(dialogsRunner));

            DocumentsFactory documentsFactory = new DocumentsFactory(Kernel);
            Kernel.Register(Component.For<IDocumentsFactory>().Instance(documentsFactory));
        }

        protected override void Init()
        {
            Kernel.ComponentRegistered += ComponentRegistered;
            Kernel.ComponentUnregistered += ComponentUnregistered;

            Kernel.AddFacility<StartableFacility>();
        }

        private void ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
        }

        private void ComponentUnregistered(string key, IHandler handler)
        {
        }

        public WinFormsMvpFacility MainForm<TPresenter, TView, TViewImpl>()
            where TPresenter : MainAppPresenter
            where TView : IMainAppView
            where TViewImpl : TView
        {
            Kernel.Register(Component.For<TView, TViewImpl>().ImplementedBy<TViewImpl>()
                                .LifeStyle.Transient);
            Kernel.Register(Component.For<TPresenter>()
                .StartUsingMethod(x => x.Run).LifeStyle.Transient);

            return this;
        }

        public WinFormsMvpFacility Dialog<TPresenter, TView, TViewImpl>()
            where TPresenter : DialogPresenterBase<TView>
            where TView : IDialogView
            where TViewImpl : TView
        {
            Kernel.Register(Component.For<TView, TViewImpl>().ImplementedBy<TViewImpl>()
                                .LifeStyle.Transient);
            Kernel.Register(Component.For<TPresenter>().LifeStyle.Transient);
            return this;
        }

        public WinFormsMvpFacility Document<TPresenter, TView, TViewImpl>()
            where TPresenter : DocumentPresenterBase<TView>
            where TView : IDocumentView
            where TViewImpl : TView
        {
            Kernel.Register(Component.For<TView, TViewImpl>().ImplementedBy<TViewImpl>()
                                .LifeStyle.Transient);
            Kernel.Register(Component.For<TPresenter>().LifeStyle.Transient);
            return this;
        }
    }
}
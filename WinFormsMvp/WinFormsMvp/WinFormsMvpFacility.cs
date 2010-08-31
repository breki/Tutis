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
            DialogRunner dialogRunner = new DialogRunner(Kernel);
            Kernel.Register(Component.For<IDialogRunner>().Instance(dialogRunner));
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
    }
}
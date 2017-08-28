using System;
using Ninject;
using System.Web.Mvc;
using System.Web.Routing;
using CinemaServer.Models.Context;
using CinemaServer.Models.Abstract;

namespace CinemaServer.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel ninjectKernel;
        public NinjectControllerFactory()
        {
            ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null
              ? null
              : (IController)ninjectKernel.Get(controllerType);
        }
        private void AddBindings()
        {
            ninjectKernel.Bind<ICinemaRepository>().To<EFCinemaRepository>();
        }
    }
}
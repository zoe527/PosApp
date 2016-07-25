﻿using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Routing;
using Autofac;
using Autofac.Integration.WebApi;

namespace PosApp
{
    public class Bootstrap
    {
        IContainer m_container;

        internal ILifetimeScope CreateLifetimeScope()
        {
            return m_container.BeginLifetimeScope();
        }

        public void Initialize(HttpConfiguration httpConfiguration)
        {
            RegisterRoute(httpConfiguration.Routes);
            RegisterFilters(httpConfiguration.Filters);
            BuildContainers(httpConfiguration);
        }

        void BuildContainers(HttpConfiguration httpConfiguration)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            containerBuilder.RegisterModule(new PosAppModule());
            m_container = containerBuilder.Build();
            httpConfiguration.DependencyResolver = new AutofacWebApiDependencyResolver(m_container);
        }

        static void RegisterFilters(HttpFilterCollection filters)
        {
            filters.Add(new PosAppExceptionFilter());
        }

        static void RegisterRoute(HttpRouteCollection routes)
        {
            routes.MapHttpRoute(
                "get receipt",
                "receipt",
                new
                {
                    controller = "Receipt",
                    action = "BuildReceipt"
                },
                new
                {
                    httpMethod = new HttpMethodConstraint(HttpMethod.Post)
                });
        }
    }
}
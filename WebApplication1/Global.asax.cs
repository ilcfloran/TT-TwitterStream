using Microsoft.AspNet.SignalR;
using System;
using System.Web.Http;

namespace WebApplication1
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(1150);

        }
    }
}

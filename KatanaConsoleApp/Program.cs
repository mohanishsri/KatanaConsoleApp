using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using System.IO;

namespace KatanaConsoleApp
{
    using System.Web.Http;
    using AppFunc = Func<IDictionary<string, object>, Task>;    

    public class Program
    {

        static void Main(string[] args)
        {
            string url = "http://localhost:8081";


            using (WebApp.Start<Startup>(url))
            {

                Console.WriteLine("Started");

                Console.Read();

                Console.WriteLine("Stopped");
            }

        }
    }

    public static class AppBuilderExtensions
    {
        public static void UseHelloWorld(this IAppBuilder app)
        {
            app.Use<HelloWorldComponent>();
        }
    }

    public class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                //app.Use(async (environment, next) =>
                //{
                //    foreach(var pair in environment.Environment)
                //    {
                //        Console.WriteLine("{0}:{1}",pair.Key, pair.Value);
                //    }

                //    await next();
                //});
                
                app.Use(async (environment, next) =>
                {
                    Console.WriteLine("Requesting :" + environment.Request.Path);

                    await next();

                    Console.WriteLine("Response:" + environment.Response.StatusCode);
                });


                ConfigureWebApi(app);

                app.UseHelloWorld();

                //app.us
                //app.Use<HelloWorldComponent>();
                // app.UseWelcomePage();
                //app.Run(ctx =>
                //{
                //    return ctx.Response.WriteAsync("Hello world");
                //});
            }

            private void ConfigureWebApi(IAppBuilder app)
            {
                var config = new HttpConfiguration();
                config.Routes.MapHttpRoute("DefaultApi", 
                            "api/{controller}/{id}", new { id = RouteParameter.Optional });

                app.UseWebApi(config);
            }
        }
       

        public class HelloWorldComponent
        {
            AppFunc _next;

            public HelloWorldComponent(AppFunc next) 
            {
                _next = next;
            }

            public  Task Invoke(IDictionary<string, object> enviroment)
            {
                var response = enviroment["owin.ResponseBody"] as Stream;


                using (var writer = new StreamWriter(response))
                {

                    return writer.WriteAsync("Hello!!");
                }
            }
        }

    }


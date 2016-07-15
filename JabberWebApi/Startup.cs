using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.Security;
using System.Net.Http.Formatting;
using System.Web.Http;
using Microsoft.Owin.StaticFiles.ContentTypes;

namespace JabberWebApi
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            
            HttpConfiguration config = new HttpConfiguration();

            //config.Formatters.Insert(0, new JsonpFormatter());
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings =
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            appBuilder.UseWebApi(config);

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".json"] = "application/json";
            appBuilder.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = provider
            });
        }
    }
} 

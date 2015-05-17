using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WebApiRouteTester
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config, string routeName = "", string routeTemplate = "", object routeDefaults = null)
		{
			if (string.IsNullOrWhiteSpace(routeName))
			{
				routeName = "DefaultApi";
			}

			if (string.IsNullOrWhiteSpace(routeTemplate))
			{
				routeTemplate = "api/{controller}/{id}";
			}

			if (routeDefaults == null)
			{
				routeDefaults = new { id = RouteParameter.Optional };
			}


			// Web API routes
			config.MapHttpAttributeRoutes();
			config.Routes.MapHttpRoute(routeName, routeTemplate, routeDefaults);

			var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
			settings.Converters.Add(new StringEnumConverter());
			config.Formatters.JsonFormatter.SerializerSettings = settings;
			config.Formatters.Add(new BsonMediaTypeFormatter());
		}
	}
}

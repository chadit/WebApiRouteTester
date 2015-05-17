using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using Moq;

namespace WebApiRouteTester
{
	public static class WebApiRouteTester
	{
		/// <summary>
		/// Routes the request.
		/// </summary>
		/// <param name="config">The config.</param>
		/// <param name="request">The request.</param>
		/// <returns>Inbformation about the route.</returns>
		public static RouteInfo RouteRequest(HttpConfiguration config, HttpRequestMessage request)
		{
			// create context
			var controllerContext = new HttpControllerContext(config, new Mock<IHttpRouteData>().Object, request);

			// get route data
			var routeData = config.Routes.GetRouteData(request);
			RemoveOptionalRoutingParameters(routeData.Values);

			request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;
			controllerContext.RouteData = routeData;

			// get controller type
			var controllerDescriptor = new DefaultHttpControllerSelector(config).SelectController(request);
			controllerContext.ControllerDescriptor = controllerDescriptor;

			// get action name
			var actionMapping = new ApiControllerActionSelector().SelectAction(controllerContext);

			var info = new RouteInfo(controllerDescriptor.ControllerType, actionMapping.ActionName);

			foreach (var param in actionMapping.GetParameters())
			{
				info.Parameters.Add(param.ParameterName);
			}

			return info;
		}

		#region | Extensions |

		/// <summary>
		/// Determines that a URL maps to a specified controller.
		/// </summary>
		/// <typeparam name="TController">The type of the controller.</typeparam>
		/// <param name="fullDummyUrl">The full dummy URL.</param>
		/// <param name="action">The action.</param>
		/// <param name="parameterNames">The parameter names.</param>
		/// <returns></returns>
		public static bool DoesRouteMapTo<TController>(this string fullDummyUrl, string action, params string[] parameterNames)
		{
			return DoesRouteMapTo<TController>(fullDummyUrl, action, HttpMethod.Get, parameterNames);
		}

		/// <summary>
		/// Determines that a URL maps to a specified controller.
		/// </summary>
		/// <typeparam name="TController">The type of the controller.</typeparam>
		/// <param name="fullDummyUrl">The full dummy URL.</param>
		/// <param name="action">The action.</param>
		/// <param name="httpMethod">The HTTP method.</param>
		/// <param name="parameterNames">The parameter names.</param>
		/// <returns></returns>
		/// <exception cref="System.Exception"></exception>
		public static bool DoesRouteMapTo<TController>(this string fullDummyUrl, string action, HttpMethod httpMethod, params string[] parameterNames)
		{
			return DoesRouteMapTo<TController>(fullDummyUrl, action, httpMethod, string.Empty, string.Empty, null, parameterNames);
		}

		/// <summary>
		/// Determines that a URL maps to a specified controller.
		/// </summary>
		/// <typeparam name="TController">The type of the controller.</typeparam>
		/// <param name="fullDummyUrl">The full dummy URL.</param>
		/// <param name="action">The action.</param>
		/// <param name="httpMethod">The HTTP method.</param>
		/// <param name="parameterNames">The parameter names.</param>
		/// <returns></returns>
		/// <exception cref="System.Exception"></exception>
		public static bool DoesRouteMapTo<TController>(this string fullDummyUrl, string action, HttpMethod httpMethod, string routeName, string routeTemplate, object routeDefaults, params string[] parameterNames)
		{
			var request = new HttpRequestMessage(httpMethod, fullDummyUrl);
			var config = new HttpConfiguration();
			WebApiConfig.Register(config, routeName, routeTemplate, routeDefaults);
			config.EnsureInitialized();
			var route = RouteRequest(config, request);

			var controllerName = typeof(TController).Name;
			if (route.Controller.Name != controllerName)
				throw new Exception($"The specified route '{fullDummyUrl}' does not match the expected controller '{controllerName}'");

			if (route.Action.ToLowerInvariant() != action.ToLowerInvariant())
				throw new Exception($"The specified route '{fullDummyUrl}' does not match the expected action '{action}'");

			if (parameterNames.Any())
			{
				if (route.Parameters.Count != parameterNames.Count())
					throw new Exception(
						$"The specified route '{fullDummyUrl}' does not have the expected number of parameters - expected '{parameterNames.Count()}' but was '{route.Parameters.Count}'");

				foreach (var param in parameterNames)
				{
					if (!route.Parameters.Contains(param))
						throw new Exception(
							$"The specified route '{fullDummyUrl}' does not contain the expected parameter '{param}'");
				}
			}

			return true;
		}

		#endregion

		#region | Private Methods |

		/// <summary>
		/// Removes the optional routing parameters.
		/// </summary>
		/// <param name="routeValues">The route values.</param>
		private static void RemoveOptionalRoutingParameters(IDictionary<string, object> routeValues)
		{
			var optionalParams = routeValues
				.Where(x => x.Value == RouteParameter.Optional)
				.Select(x => x.Key)
				.ToList();

			foreach (var key in optionalParams)
			{
				routeValues.Remove(key);
			}
		}

		#endregion
	}

	/// <summary>
	/// Route information
	/// </summary>
	public class RouteInfo
	{
		#region | Construction |

		/// <summary>
		/// Initializes a new instance of the <see cref="RouteInfo"/> class.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		public RouteInfo(Type controller, string action)
		{
			Controller = controller;
			Action = action;
			Parameters = new List<string>();
		}

		#endregion

		public Type Controller { get; private set; }
		public string Action { get; private set; }
		public List<string> Parameters { get; private set; }
	}
}
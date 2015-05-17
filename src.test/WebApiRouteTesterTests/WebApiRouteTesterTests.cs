using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockClass;
using WebApiRouteTester;

namespace WebApiRouteTesterTests
{
	[TestClass]
	public class WebApiRouteTesterTests : HttpApplication
	{
		[TestInitialize]
		public void TestInit()
		{
			var controller = new MockUserController {Configuration = new HttpConfiguration()};
			controller.Configuration.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional }
			);
		}

		[TestMethod]
		public void MockUserController_VerifyGet_True()
		{
			const string requestUrl = "http://localhost/user";
			Assert.IsTrue(requestUrl.DoesRouteMapTo<MockUserController>("Get", HttpMethod.Get));
		}

		[TestMethod]
		public void MockUserController_VerifyGetById_True()
		{
			const string requestUrl = "http://localhost/user/1234";
			Assert.IsTrue(requestUrl.DoesRouteMapTo<MockUserController>("Get", HttpMethod.Get));
		}

		[TestMethod]
		public void MockUserController_VerifyPost_True()
		{
			const string requestUrl = "http://localhost/user";
			Assert.IsTrue(requestUrl.DoesRouteMapTo<MockUserController>("Create", HttpMethod.Post));
		}

		[TestMethod]
		public void MockUserController_VerifyPut_True()
		{
			const string requestUrl = "http://localhost/user";
			Assert.IsTrue(requestUrl.DoesRouteMapTo<MockUserController>("Update", HttpMethod.Put));
		}

		[TestMethod]
		public void MockUserController_VerifyDelete_True()
		{
			const string requestUrl = "http://localhost/user";
			Assert.IsTrue(requestUrl.DoesRouteMapTo<MockUserController>("Delete", HttpMethod.Delete));
		}

		[TestMethod]
		public void MockUserController_VerifyPatch_True()
		{
			const string requestUrl = "http://localhost/user/1234";
			Assert.IsTrue(requestUrl.DoesRouteMapTo<MockUserController>("Patch", new HttpMethod("PATCH")));
		}

	}
}

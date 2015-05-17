# WebApiRouteTester
Helps test webapi URL routes to verify if they are valid

Example of Basic Validation

    [TestMethod]
		public void MockUserController_VerifyGet_True()
		{
			const string requestUrl = "http://localhost/user";
			Assert.IsTrue(requestUrl.DoesRouteMapTo<MockUserController>("Get", HttpMethod.Get));
		}


if you need to override the default WebApi convension do the following

    [TestMethod]
		public void MockUserController_VerifyGet_True()
		{
			const string requestUrl = "http://localhost/user";
			Assert.IsTrue(requestUrl.DoesRouteMapTo<MockUserController>("Get", HttpMethod.Get, "DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional }));
		}

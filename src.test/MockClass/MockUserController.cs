using System.Web.Http;

namespace MockClass
{
	public class MockUserController : ApiController
	{
		[Route("user"), HttpGet]
		public void Get(){}

		[Route("user/{userId}"), HttpGet]
		public void Get([FromUri]int userId) {}

		[Route("user"), HttpPost]
		public void Create(){}

		[Route("user"), HttpPut]
		public void Update() { }

		[Route("user"), HttpDelete]
		public void Delete() { }

		[Route("user/{userId}"), HttpPatch]
		public void Patch([FromUri]int userId) { }
	}
}

using Microsoft.AspNetCore.Http;
using Moq;
using R4;

namespace Tests.Static;

[DisableAutoDiscover]
public class HttpContextAccessorFake : IHttpContextAccessor, ISingletonService
{
	public HttpContext HttpContext { get; set; }
		= new DefaultHttpContext();

	public HttpContextAccessorFake()
	{
		HttpContext.Request.Host = new HostString("testhost");

		var sessionMock = new Mock<ISession>();
		sessionMock.Setup(x => x.Id).Returns("_fake_session_id");
		HttpContext.Session = sessionMock.Object;
	}
}

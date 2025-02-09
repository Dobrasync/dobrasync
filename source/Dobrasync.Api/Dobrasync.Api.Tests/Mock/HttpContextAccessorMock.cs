using Microsoft.AspNetCore.Http;

namespace Dobrasync.Api.Tests.Mock;

public class HttpContextAccessorMock : IHttpContextAccessor
{
    public HttpContext? HttpContext { get; set; }
}
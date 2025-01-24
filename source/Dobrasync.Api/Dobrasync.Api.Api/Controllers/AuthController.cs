using Dobrasync.Api.Api.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Dobrasync.Api.Api.Controllers;

[SwaggerTag("Auth")]
public class AuthController : BaseController
{
    [HttpGet("login/classic")]
    [SwaggerOperation(
        OperationId = nameof(LoginClassicAsync)
    )]
    public ActionResult LoginClassicAsync()
    {
        return Ok("Test");
    }
}
using Dobrasync.Api.Api.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Dobrasync.Api.Api.Controllers;

[SwaggerTag("Environment")]
public class EnvironmentController : BaseController
{
    [HttpGet("test")]
    [SwaggerOperation(
        OperationId = nameof(Test)
    )]
    public ActionResult Test()
    {
        return Ok("Test");
    }
}
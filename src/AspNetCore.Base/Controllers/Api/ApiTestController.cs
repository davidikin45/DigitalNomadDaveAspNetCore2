using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Controllers.Api
{
    [ApiVersion("1.0")]
    [Route("api/test")]
    public class ApiTestController : ApiControllerBase
    {
        [Route("checkid/{id}")]
        [HttpGet]
        public IActionResult CheckId(int id)
        {
            if (id > 100)
            {
                return ApiErrorMessage("We cannot use IDs greater than 100.");
            }
            return Ok(id);
        }

        [Route("unauthorized")]
        [HttpGet]
        public IActionResult Unauthorized_401()
        {
            return Unauthorized();
        }

        [Route("challenge")]
        [HttpGet]
        public IActionResult ChallengeDefault()
        {
            return Challenge();
        }

        [Route("challenge-bearer")]
        [HttpGet]
        public IActionResult Challenge_Bearer()
        {
            return Challenge(JwtBearerDefaults.AuthenticationScheme);
        }

        [Route("forbid")]
        [HttpGet]
        public IActionResult ForbidDefault()
        {
            return Forbid();
        }

        [Route("forbid-bearer")]
        [HttpGet]
        public IActionResult Forbid_Bearer()
        {
            return Forbid(JwtBearerDefaults.AuthenticationScheme);
        }

        [Route("forbidden")]
        [HttpGet]
        public IActionResult Forbidden_403()
        {
            return Forbidden();
        }

        [Route("ok")]
        [HttpGet]
        public IActionResult OK_200()
        {
            return Ok();
        }

        [Route("not-found")]
        [HttpGet]
        public IActionResult NotFound_404()
        {
            return NotFound();
        }

        [Route("throw-exception")]
        [HttpGet]
        public IActionResult ThrowException()
        {
            throw new Exception("");
        }

        [Route("cancel-operation")]
        [HttpGet]
        public async Task<IActionResult> CancelOperation()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            await Task.Run(async () => await Task.Delay(10000), cts.Token);

            return Ok();
        }
    }
}

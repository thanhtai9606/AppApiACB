using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using App.Filters;

namespace App.Controllers
{
    // [Route("api/[controller]")]
    // [ApiController]
    ///[ApiKeyAuth]
    //[Authorize(Roles = "Admin")]
    public class SecreteController : ControllerBase
    {
        [HttpGet, Route("Index")]
        public string Index()
        {
            return "Hello World";
        }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        [HttpGet("list")]
        public ActionResult<IEnumerable<object>> ListUsers()
        {
            // demo static; în proiect real folosești DB
            var users = new[] {
                new { Id=1, Username="alice" },
                new { Id=2, Username="bob" }
            };
            return Ok(users);
        }
    }
}

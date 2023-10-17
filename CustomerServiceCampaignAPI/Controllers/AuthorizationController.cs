using CustomerServiceCampaignAPI.Authorization;
using CustomerServiceCampaignAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerServiceCampaignAPI.Controllers
{
    [ApiController]
    [Route("/authorization")]
    public class AuthorizationController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public AuthorizationController(ApplicationDbContext db)
        {
            _db = db;
        }

        /*[HttpGet]
        public IActionResult Jwt()
        {
            return new ObjectResult(JwtToken.GenerateJwtToken());
        }*/

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] string email)
        {
            var agent = _db.Agents.FirstOrDefault(a => a.Email == email);

            if (agent == null)
            {
                return Unauthorized();
            }

            string token = JwtToken.GenerateJwtToken(agent);

            return Ok(new { token });
        }

    }
}

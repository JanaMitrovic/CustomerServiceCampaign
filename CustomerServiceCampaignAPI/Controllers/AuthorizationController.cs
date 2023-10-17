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

        [HttpGet]
        public IActionResult Jwt()
        {
            return new ObjectResult(JwtToken.GenerateJwtToken());
        }

    }
}

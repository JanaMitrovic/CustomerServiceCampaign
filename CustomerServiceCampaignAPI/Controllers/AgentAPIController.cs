using CustomerServiceCampaignAPI.Data;
using CustomerServiceCampaignAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerServiceCampaignAPI.Controllers
{
    [Route("api/agentAPI")]
    [ApiController]
    public class AgentAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public AgentAPIController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Agent>> GetAgents()
        {
            return Ok(_db.Agents.ToList());
        }

        [HttpGet("{id:int}", Name = "GetAgentbyId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Agent> GetAgent(int id)
        {
            if (id == 0)
            {
                ModelState.AddModelError("CustomError", "Id cannot be 0!");
                return BadRequest(ModelState);
            }

            var villa = _db.Agents.FirstOrDefault(u => u.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            return Ok(villa);
        }

        [HttpPost(Name = "GetAgentByEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Agent> GetAgentByEmail([FromBody] string email)
        {
            if (email == null || email == "")
            {
                ModelState.AddModelError("CustomError", "Email must be passed!");
                return BadRequest(ModelState);
            }

            var agent = _db.Agents.FirstOrDefault(u => u.Email == email);

            if (agent == null)
            {
                return NotFound();
            }

            return Ok(agent);
        }

    }
}

﻿using CustomerServiceCampaignAPI.Data;
using CustomerServiceCampaignAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerServiceCampaignAPI.Controllers
{
    [Route("api/campaignAPI")]
    [ApiController]
    public class CampaignAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public CampaignAPIController(ApplicationDbContext db)
        {
            _db = db;
        }

        /*[HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<CampaignDTO>> GetCampaigns()
        {
            return Ok(_db.Campaigns.ToList());
        }*/

        /*[HttpGet("{id:int}", Name = "GetCampaign")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<AgentDTO> GetCampaign(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var campaign = _db.Campaigns.FirstOrDefault(u => u.Id == id);

            if (campaign == null)
            {
                return NotFound();
            }

            return Ok(campaign);
        }*/

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("/createCampaign")]
        public ActionResult<Campaign> CreatePurchase([FromBody] Campaign campaign)
        {
            if (campaign == null)
            {
                return BadRequest(campaign);
            }
            if (campaign.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Campaign model = new()
            {
                Id = campaign.Id,
                StartDate = campaign.StartDate,
                EndDate = campaign.StartDate.AddDays(6)
            };

            _db.Campaigns.Add(model);
            _db.SaveChanges();

            return CreatedAtAction(nameof(CreatePurchase), model);
        }

        /*[HttpDelete("{id:int}", Name = "DeleteCampaign")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteCampaign(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var campaign = _db.Campaigns.FirstOrDefault(a => a.Id == id);
            if (campaign == null)
            {
                return NotFound();
            }

            _db.Campaigns.Remove(campaign);
            _db.SaveChanges();
            return NoContent();
        }*/
    }
}

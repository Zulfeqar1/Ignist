﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ignist.Models;
using Ignist.Data;
using Microsoft.Azure.Cosmos;

namespace Ignist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicationsController : ControllerBase 
    {
        private readonly IPublicationsRepository _publicationsRepository;

        public PublicationsController(IPublicationsRepository publicationsRepository)
        {
            _publicationsRepository = publicationsRepository;
        }

        // Get all Publications
        [HttpGet]
        public async Task<ActionResult<List<Publication>>> GetAllPublications()
        {
            var publications = await _publicationsRepository.GetAllPublicationsAsync();
            return Ok(publications);
        }

        // Finding a publication with the specific Id
        [HttpGet("{id}")]
        public async Task<ActionResult<Publication>> GetPublication(string id, [FromQuery] string UserId) // Endret til string fordi Cosmos DB bruker string IDs
        {
            var publication = await _publicationsRepository.GetPublicationByIdAsync(id, UserId);
            if (publication is null)
            {
                return NotFound("Publication not found.");
            }

            return Ok(publication);
        }


        //Creating new Publications
        [HttpPost]
        public async Task<ActionResult<Publication>> AddPublication(Publication publication)
        {
            await _publicationsRepository.AddPublicationAsync(publication);
            return CreatedAtAction(nameof(GetPublication), new { id = publication.Id }, publication);
        }

        // Updating a Publication
        [HttpPut("{id}")]
        public async Task<ActionResult<Publication>> UpdatePublication(string id, Publication updatedPublication)
        {
            try
            {
                await _publicationsRepository.UpdatePublicationAsync(updatedPublication);
                return NoContent();
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound("Publication not found.");
            }
        }

        // Deleting a publication
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublication(string id, [FromQuery] string UserId)
        {
            try
            {
                await _publicationsRepository.DeletePublicationAsync(id, UserId);
                return NoContent();
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound("Publication not found.");
            }
        }
    }
}

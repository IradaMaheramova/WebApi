using CityInfo.API.Models;
using CityInfo.API.Sevices;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/pointsofinterest")]
    public class PointsOfInterestsController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestsController> _logger;
        private readonly IMailService _mailService;

        public PointsOfInterestsController(ILogger<PointsOfInterestsController> logger,
            IMailService mailService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }

        [HttpGet]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                _logger.LogInformation("City was not found");
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
                return NotFound();

            var pointsOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);

            if (pointsOfInterest == null)
                return NotFound();

            return Ok(pointsOfInterest);
        }

        [HttpPost]
        public IActionResult CreatePointOfInterest(int cityId, 
            [FromBody]PointsOfInterestsForCreationDto pointOfInterest)
        {
            if(pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError(
                    "Description",
                    "Name and description should be different."
                    );
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
                return NotFound();

            // demo to be improved
            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
                c => c.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointsOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute(
                "GetPointOfInterest",
                new { cityId, id = finalPointOfInterest.Id},
                finalPointOfInterest);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id,
          [FromBody]PointsOfInterestsForUpdatingDto pointOfInterest)
        {
            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError(
                    "Description",
                    "Name and description should be different."
                    );
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
                return NotFound();

            var pointsOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);

            if (pointsOfInterestFromStore == null)
                return NotFound();

            pointsOfInterestFromStore.Name = pointOfInterest.Name;
            pointsOfInterestFromStore.Description = pointOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
         [FromBody] JsonPatchDocument<PointsOfInterestsForUpdatingDto> patchDoc)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
                return NotFound();

            var pointsOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);

            if (pointsOfInterestFromStore == null)
                return NotFound();

            var pointOfInterestToPath =
                new PointsOfInterestsForUpdatingDto()
                {
                    Name = pointsOfInterestFromStore.Name,
                    Description = pointsOfInterestFromStore.Description
                };

            patchDoc.ApplyTo(pointOfInterestToPath, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pointOfInterestToPath.Description == pointOfInterestToPath.Name)
            {
                ModelState.AddModelError(
                    "Description",
                    "Name and description should be different."
                    );
            }

            if (!TryValidateModel(pointOfInterestToPath))
            {
                return BadRequest(ModelState);
            };

            pointsOfInterestFromStore.Name = pointOfInterestToPath.Name;
            pointsOfInterestFromStore.Description = pointOfInterestToPath.Description;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
                return NotFound();

            var pointsOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);

            if (pointsOfInterestFromStore == null)
                return NotFound();

            city.PointsOfInterest.Remove(pointsOfInterestFromStore);

            _mailService.Send("Point pf interesr deleted.", $"Point of interest {pointsOfInterestFromStore.Name}");

            return NoContent();
        }

    }
}

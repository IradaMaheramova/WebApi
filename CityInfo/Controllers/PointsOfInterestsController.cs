using AutoMapper;
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
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestsController(ILogger<PointsOfInterestsController> logger,
            IMailService mailService, ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ??
                throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public IActionResult GetPointsOfInterest(int cityId, int id)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                _logger.LogInformation($"City with id {cityId} was not found.");
                return NotFound();
            }

            var pointsOfInterestForCity = _cityInfoRepository.GetPointOfInterest(cityId);

            return Ok(_mapper.Map<IEnumerable<PointsOfInterestDto>>(pointsOfInterestForCity));
        }

        [HttpGet("{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                _logger.LogInformation($"City with id {cityId} was not found.");
                return NotFound();
            }

            var pointsOfInterestForCity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);

            if (pointsOfInterestForCity == null)
                return NotFound();

            return Ok(_mapper.Map<PointsOfInterestDto>(pointsOfInterestForCity));
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

            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();

            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            _cityInfoRepository.AddPointOfInterestsForCity(cityId, finalPointOfInterest);
            _cityInfoRepository.Save();

            var createdPointOfInterest = _mapper.Map <PointsOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute(
                "GetPointOfInterest",
                new { cityId, id = createdPointOfInterest.Id},
                createdPointOfInterest);
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

            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();

            var pointsOfInterestForCity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);

            if (pointsOfInterestForCity == null)
                return NotFound();

            _mapper.Map(pointOfInterest, pointsOfInterestForCity);

            _cityInfoRepository.UpdatePointOfInterestForCity(cityId, pointsOfInterestForCity);

            _cityInfoRepository.Save();
            
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
         [FromBody] JsonPatchDocument<PointsOfInterestsForUpdatingDto> patchDoc)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();

            var pointsOfInterestForCity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);

            if (pointsOfInterestForCity == null)
                return NotFound();

           var pointOfInterestToPatch = _mapper.Map<PointsOfInterestsForUpdatingDto>(pointsOfInterestForCity);

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError(
                    "Description",
                    "The provided description should be different from the name.");
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, pointsOfInterestForCity);

            _cityInfoRepository.UpdatePointOfInterestForCity(cityId, pointsOfInterestForCity);

            _cityInfoRepository.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();

            var pointsOfInterestForCity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);

            if (pointsOfInterestForCity == null)
                return NotFound();

            _cityInfoRepository.DeletePointOfInterestForCity(pointsOfInterestForCity);

            _cityInfoRepository.Save();

            _mailService.Send("Point of interesr deleted.", $"Point of interest {pointsOfInterestForCity.Name}");

            return NoContent();
        }

    }
}

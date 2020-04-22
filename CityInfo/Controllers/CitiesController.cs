using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Sevices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ??
                throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public IActionResult GetCities()
        {
            var cityEntities = _cityInfoRepository.GetCities();

            //var results = new List<CityWithoutPointsOfInteresDto>();

            //foreach (var cityEntity in cityEntities)
            //{
            //    results.Add(new CityWithoutPointsOfInteresDto
            //    {
            //        Id = cityEntity.Id,
            //        Description = cityEntity.Description,
            //        Name = cityEntity.Name
            //    });
            //}

            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInteresDto>>(cityEntities));
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool  includePointOfInterest)
        {
            var city = _cityInfoRepository.GetCity(id, includePointOfInterest);

            if (city == null)
                return NotFound();

            if (includePointOfInterest)
            {
                //var cityResult = new CityDto
                //{
                //    Id = city.Id,
                //    Description = city.Description,
                //    Name = city.Name
                //};

                //foreach (var point in city.PointsOfInterest)
                //{
                //    cityResult.PointsOfInterest.Add(
                //        new PointsOfInterestDto()
                //    {
                //        Id = point.Id,
                //        Description = point.Description,
                //        Name = point.Name
                //    });
                //}
                return Ok(_mapper.Map<CityDto>(city));
            }

            //var cityWithoutPointsOfInterest = new CityWithoutPointsOfInteresDto()
            //{
            //    Id = city.Id,
            //    Description = city.Description,
            //    Name = city.Name

            //}; 

            return Ok(_mapper.Map<CityWithoutPointsOfInteresDto>(city));
        }
    }
}

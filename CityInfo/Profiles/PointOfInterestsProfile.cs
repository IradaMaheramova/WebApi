using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Profiles
{
    public class PointOfInterestsProfile : Profile
    {
        public PointOfInterestsProfile()
        {
            CreateMap<Entities.PointOfInterest, Models.PointsOfInterestDto>();
            CreateMap<Models.PointsOfInterestsForCreationDto, Entities.PointOfInterest>();
            CreateMap<Models.PointsOfInterestsForUpdatingDto, Entities.PointOfInterest>()
                .ReverseMap();
        }
    }
}

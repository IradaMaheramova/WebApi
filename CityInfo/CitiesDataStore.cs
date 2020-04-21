using CityInfo.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public static CitiesDataStore Current { get; } = new CitiesDataStore();

        public List<CityDto> Cities { get; set; }

        public CitiesDataStore()
        {
            Cities = new List<CityDto>()
            {
                 new CityDto()
                {
                    Id = 1,
                    Name = "NY",
                    Description = "Large city",

                    PointsOfInterest =  new List<PointsOfInterestDto>()
                    {
                        new PointsOfInterestDto()
                        {
                            Id = 1,
                            Name = "Central Park",
                            Description = "Interesting"
                        },

                        new PointsOfInterestDto()
                        {
                            Id = 2,
                            Name = "Empire State Building",
                            Description = "Interesting"
                        }
                    }
                },

                 new CityDto()
                {
                    Id = 2,
                    Name = "Antwerp",
                    Description = "Belgium",
                     PointsOfInterest =  new List<PointsOfInterestDto>()
                    {
                        new PointsOfInterestDto()
                        {
                            Id = 1,
                            Name = "Cathedral",
                            Description = "Interesting"
                        }
                    }
                }
            };
        }
    }
}

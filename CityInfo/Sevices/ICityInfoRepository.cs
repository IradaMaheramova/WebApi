using CityInfo.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Sevices
{
    public interface ICityInfoRepository
    {
        IEnumerable<City> GetCities();
        City GetCity(int cityId, bool includePointOfInterest);
        IEnumerable<PointOfInterest> GetPointOfInterest(int cityId);
        PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInteresrtId);
        bool CityExists(int cityId);
    }
}

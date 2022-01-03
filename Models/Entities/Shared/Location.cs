using api.Models.Entities.Shared;
using api.Models.ViewModels.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Entities
{
    public class Location : BaseEntity
    {
        public string PostalCode { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string City { get; set; }
        public string Voivodeship { get; set; }
        public string County { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        
        public int HouseId { get; set; }
        public House House { get; set; }

        public Location() { }

        public Location(AddLocationViewModel model)
        {
            PostalCode = model.PostalCode;
            Street = model.Street;
            Number = model.Number;
            City = model.City;
            Voivodeship = model.Voivodeship;
            County = model.County;
            Latitude = model.Latitude;
            Longitude = model.Longitude;
        }

        public void UpdateLocation(AddLocationViewModel model)
        {
            PostalCode = model.PostalCode;
            Street = model.Street;
            Number = model.Number;
            City = model.City;
            Voivodeship = model.Voivodeship;
            County = model.County;
            Latitude = model.Latitude;
            Longitude = model.Longitude;
        }
    }
}

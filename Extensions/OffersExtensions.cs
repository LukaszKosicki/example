using API.Models.Entity.Categories;
using API.Models.Entity.Offers;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace API.Extensions
{
    public static class OffersExtensions
    {
        public static List<Offer> GetOffersByCategoryId(this List<Category> categories, int categoryId) =>
            categoryId != 0 ? categories.FirstOrDefault(c => c.Id == categoryId).Offers.ToList()
            : categories.FirstOrDefault(c => c.ParentCategoryId == null).Offers.ToList();

        public static List<Offer> GetOffersAtDIstance(this List<Offer> offers, int distance, double lng, double lat)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var myLocation = geometryFactory.CreatePoint(new Coordinate(lat, lng));
     
            return offers.Where(o => o.Location.Cordinates.ProjectTo(2855).Distance(myLocation.ProjectTo(2855)) <= distance * 1000)
                .ToList();
        }
          
        public static List<Offer> GetOffersAtDsc(this List<Offer> offers, string dsc)
        {
            return offers.Where(o => o.Title.Contains(dsc) || o.Description.Contains(dsc)).ToList();
        }
    }
}

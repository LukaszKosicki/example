using api.Models.Entities.Auth;
using api.Models.Entities.HouseMap;
using api.Models.Entities.Rooms;
using api.Models.Entities.Shared;
using api.Models.ViewModels.House;
using api.Models.ViewModels.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Entities
{
    public enum ConstructionStages
    {
        Papierologia = 0,
        StanZerowy = 1,
        StanSurowyOtwarty = 2,
        StanSurowyZamknięty = 3,
        RobotyWykończeniowe = 4,
        ZakończenieBudowy = 5,
        UrządzanieWnętrz = 6,
        Zamieszkany = 7
    }

    public class House : BaseEntity
    {
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public float LandArea { get; set; }

        public bool ChangesInProject { get; set; }
        public ConstructionStages ConstructionStage { get; set; }
        public Location Location { get; set; }
        public List<HouseImage> Images { get; set; }
        public List<Room> Rooms { get; set; }
        public List<HouseComment> Comments { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }

        public House() { }

        public House(string projectName, string description, float landArea, bool changesInProject, int constructionStage)
        {
            ProjectName = projectName;
            Description = description;
            LandArea = landArea;
            ChangesInProject = changesInProject;
            ConstructionStage = (ConstructionStages)constructionStage;
        }

        public void UpdateHouse(UpdateHouseViewModel model)
        {
            ProjectName = model.ProjectName;
            Description = model.Description;
            LandArea = model.LandArea;
            ChangesInProject = model.ChangesInProject;
            ConstructionStage = (ConstructionStages)model.ConstructionStage;

            if (Location.Latitude != model.Location.Latitude || Location.Longitude != model.Location.Longitude)
            {
                Location.UpdateLocation(model.Location);
            }
        }

        public void AddLocation(AddLocationViewModel model)
        {
            Location = new Location(model);
        }

        public void AddImages(List<HouseImage> images)
        {
            Images = images;
        }
    }
}

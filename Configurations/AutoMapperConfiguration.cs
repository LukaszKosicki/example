using api.Models.Entities;
using api.Models.Entities.Auth;
using api.Models.Entities.Chat;
using api.Models.Entities.Rooms;
using api.Models.Entities.Shared;
using api.Models.ViewModels.Auth;
using api.Models.ViewModels.Chat;
using api.Models.ViewModels.Comment;
using api.Models.ViewModels.Home;
using api.Models.ViewModels.House;
using api.Models.ViewModels.Hubs.Chat;
using api.Models.ViewModels.ImageViewModels;
using api.Models.ViewModels.Room;
using api.Models.ViewModels.Users;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Configurations
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            //home
            CreateMap<House, GetHousesViewModel>()
                .ForMember(m => m.City, opt => opt.MapFrom(vw => vw.Location.City))
                .ForMember(m => m.UserName, opt => opt.MapFrom(vw => vw.User.UserName))
                .ForMember(m => m.Image, opt => opt.MapFrom(vm => vm.Images.FirstOrDefault()));
            CreateMap<Room, Models.ViewModels.Home.GetRoomsViewModel>()
                .ForMember(m => m.UserName, opt => opt.MapFrom(vm => vm.User.UserName))
                .ForMember(m => m.Image, opt => opt.MapFrom(vm => vm.Images.FirstOrDefault()));

            //house
            CreateMap<House, HousesListViewModel>()
                .ForMember(m => m.Longitude, opt => opt.MapFrom(vw => vw.Location.Longitude))
                .ForMember(m => m.Latitude, opt => opt.MapFrom(vw => vw.Location.Latitude))
                .ForMember(m => m.UserName, opt => opt.MapFrom(vw => vw.User.UserName));
            CreateMap<House, UserHousesListViewModel>()
                .ForMember(m => m.Image, opt => opt.MapFrom(vm => vm.Images.FirstOrDefault()))
                .ForMember(m => m.UserName, opt => opt.MapFrom(vw => vw.User.UserName));
            CreateMap<House, GetHouseViewModel>()
                .ForMember(m => m.Longitude, opt => opt.MapFrom(vw => vw.Location.Longitude))
                .ForMember(m => m.Latitude, opt => opt.MapFrom(vw => vw.Location.Latitude))
                .ForMember(m => m.Voivodeship, opt => opt.MapFrom(vm => vm.Location.Voivodeship))
                .ForMember(m => m.City, opt => opt.MapFrom(vm => vm.Location.City))
                .ForMember(m => m.UserName, opt => opt.MapFrom(vw => vw.User.UserName));
            CreateMap<House, ListHouseNamesViewModel>();

            //rooms
            CreateMap<Room, Models.ViewModels.Room.GetRoomsViewModel>()
                .ForMember(m => m.Image, opt => opt.MapFrom(vm => vm.Images.FirstOrDefault()))
                .ForMember(m => m.UserName, opt => opt.MapFrom(vw => vw.User.UserName));
            CreateMap<Room, GetRoomViewModel>()
                .ForMember(m => m.HouseId, opt => opt.MapFrom(vm => vm.House != null ? vm.House.Id : 0))
                .ForMember(m => m.HouseName, opt => opt.MapFrom(vm => vm.House != null ? vm.House.ProjectName : "Brak domu"))
                .ForMember(m => m.UserName, opt => opt.MapFrom(vw => vw.User.UserName));
            CreateMap<Room, HouseRoomsViewModel>()
                .ForMember(m => m.Image, opt => opt.MapFrom(vm => vm.Images.FirstOrDefault()));

            //image
            CreateMap<Image, GetImageViewModel>();

            //user
            CreateMap<RegistrationViewModel, User>();
            CreateMap<User, UserViewModel>()
                .ForMember(m => m.HousesCount, opt => opt.MapFrom(vw => vw.Houses.Count))
                .ForMember(m => m.Id, opt => opt.MapFrom(vm => vm.Id.ToString()));
            CreateMap<User, GetUserViewModel>()
                .ForMember(m => m.HousesCount, opt => opt.MapFrom(vw => vw.Houses.Count))
                .ForMember(m => m.RoomsCount, opt => opt.MapFrom(vm => vm.Rooms.Count))
                .ForMember(m => m.UserId, opt => opt.MapFrom(vm => vm.Id.ToString()));

            //comment
            CreateMap<Comment, GetCommentsViewModel>()
                .ForMember(m => m.CreatedDate, opt => opt.MapFrom(vm => vm.CreatedDate.ToString("g")))
                .ForMember(m => m.Username, opt => opt.MapFrom(vm => vm.User.UserName));

            //chat
            CreateMap<Message, GetMessageViewModel>();
            CreateMap<Participant, GetConversationViewModel>()
                .ForMember(m => m.LastMessage, opt => opt.MapFrom(vm => vm.Conversation.LastMessage.Content))
                .ForMember(m => m.ParticipantName, opt => opt.MapFrom(vm => vm.Conversation.Participants
                .SingleOrDefault(p => p.User.UserName != vm.User.UserName).User.UserName))
                .ForMember(m => m.LastMessageDate, opt => opt.MapFrom(vm => vm.Conversation.LastMessage.CreatedDate.ToString("g")))
                .ForMember(m => m.LastMessageAuthor, opt => opt.MapFrom(vm => vm.Conversation.LastMessage.User.UserName));


            //hub
            CreateMap<SendMessageViewModel, HubMessageViewModel>();
        }
    }
}

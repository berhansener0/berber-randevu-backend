using AutoMapper;
using BerberRandevuAPI.DTOs;
using BerberRandevuAPI.Models;
using BerberRandevuAPI.Models.DTOs;

namespace BerberRandevuAPI.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Appointment, AppointmentReadDTO>().ReverseMap();
            CreateMap<Barber, BarberDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
        }
    }
}

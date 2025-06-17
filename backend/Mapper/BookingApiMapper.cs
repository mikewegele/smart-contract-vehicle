using AutoMapper;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Model;

namespace SmartContractVehicle.Mapper
{
    public class BookingApiMapper : Profile
    {
        public BookingApiMapper()
        {
            CreateMap<Reservation, ReservationTO>();
            CreateMap<ReservationTO, Reservation>();
        }
    }
}
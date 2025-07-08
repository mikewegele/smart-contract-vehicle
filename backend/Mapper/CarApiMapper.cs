using AutoMapper;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Model;

namespace SmartContractVehicle.Mapper
{
    public class CarApiMapper : Profile
    {
        public CarApiMapper()
        {
            CreateMap<Car, CarTO>()
                // Company (from Trim -> Model -> Producer)
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Trim.Model.Producer.Name))
                .ForMember(dest => dest.CompanyLogoPath, opt => opt.MapFrom(src => src.Trim.Model.Producer.ImagePath))

                // Model
                .ForMember(dest => dest.ModelName, opt => opt.MapFrom(src => src.Trim.Model.Name))

                // Trim
                .ForMember(dest => dest.TrimName, opt => opt.MapFrom(src => src.Trim.Name))
                .ForMember(dest => dest.FueltypeName, opt => opt.MapFrom(src => src.Trim.Fuel.Name))
                .ForMember(dest => dest.DrivetrainName, opt => opt.MapFrom(src => src.Trim.Drivetrain.Name))
                .ForMember(dest => dest.TrimImagePath, opt => opt.MapFrom(src => src.Trim.ImagePath))

                // Car details
                .ForMember(dest => dest.CarId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CurrentPosition, opt => opt.MapFrom(src => src.CurrentPosition))
                .ForMember(dest => dest.RemainingReach, opt => opt.MapFrom(src => src.RemainingReach))
                .ForMember(dest => dest.Colour, opt => opt.MapFrom(src => src.Colour))
                .ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src.SeatNumbers))
                .ForMember(dest => dest.PricePerMinute, opt => opt.MapFrom(src => src.PricePerMinute))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Id));

            CreateMap<FuelType, FuelTypeTO>();
            CreateMap<FuelTypeTO, FuelType>();

            CreateMap<Drivetrain, DrivetrainTO>();
            CreateMap<DrivetrainTO, Drivetrain>();

            CreateMap<CarStatus, CarStatusTO>();
            CreateMap<CarStatusTO, CarStatus>();


            
            CreateMap<VehicleTrim, VehicleTrimTO>()
                .ForMember(dst => dst.Model, opt => opt.MapFrom(src => src.Model.Id))
                .ForMember(dst => dst.Cars, opt=> opt.MapFrom(src => src.Cars.Select(c => c.Id)))
                .ForMember(dst => dst.FuelId, opt => opt.MapFrom(src => src.Fuel.Id))
                .ForMember(dst => dst.DrivetrainId, opt => opt.MapFrom(src => src.Drivetrain.Id));

            CreateMap<VehicleModel, VehicleModelTO>()
                .ForMember(dst => dst.Trims, opt => opt.MapFrom(src => src.Trims.Select(t => t.Id)))
                .ForMember(dst => dst.ProducerId, opt => opt.MapFrom(src => src.Producer.Id));

            CreateMap<AutomotiveCompany, AutomotiveCompanyTO>()
                .ForMember(dst => dst.Models, opt => opt.MapFrom(src => src.Models.Select(m => m.Id)));
        }
    }
}
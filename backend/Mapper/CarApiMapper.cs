using AutoMapper;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Model;

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
            .ForMember(dest => dest.CurrentPosition, opt => opt.MapFrom(src => src.CurrentPosition))
            .ForMember(dest => dest.RemainingReach, opt => opt.MapFrom(src => src.RemainingReach))
            .ForMember(dest => dest.Colour, opt => opt.MapFrom(src => src.Colour))
            .ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src.SeatNumbers))
            .ForMember(dest => dest.PricePerMinute, opt => opt.MapFrom(src => src.PricePerMinute));
    }
}

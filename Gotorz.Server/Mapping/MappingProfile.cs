using AutoMapper;
using Gotorz.Server.Models;
using Gotorz.Shared.DTOs;

/// <summary>
/// AutoMapper profile for mapping between domain models and DTOs.
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    /// Configures bidirectional mappings between models and DTOs.
    /// </summary>
    public MappingProfile()
    {
        CreateMap<AirportDto, Airport>().ReverseMap();

        // Structure from ChatGPT. Customized for this project.
        CreateMap<FlightDto, Flight>()
            .ForMember(dest => dest.DepartureAirportId, opt => opt.MapFrom(src => src.DepartureAirport.AirportId))
            .ForMember(dest => dest.ArrivalAirportId, opt => opt.MapFrom(src => src.ArrivalAirport.AirportId))
            .ForMember(dest => dest.DepartureAirport, opt => opt.Ignore())
            .ForMember(dest => dest.ArrivalAirport, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.DepartureAirport, opt => opt.MapFrom(src => src.DepartureAirport))
            .ForMember(dest => dest.ArrivalAirport, opt => opt.MapFrom(src => src.ArrivalAirport));

        CreateMap<FlightTicketDto, FlightTicket>()
            .ForMember(dest => dest.FlightId, opt => opt.MapFrom(src => src.Flight.FlightId))
            .ForMember(dest => dest.HolidayPackageId, opt => opt.MapFrom(src => src.HolidayPackage.HolidayPackageId))
            .ForMember(dest => dest.Flight, opt => opt.Ignore())
            .ForMember(dest => dest.HolidayPackage, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.Flight, opt => opt.MapFrom(src => src.Flight))
            .ForMember(dest => dest.HolidayPackage, opt => opt.MapFrom(src => src.HolidayPackage));

        CreateMap<HolidayPackageDto, HolidayPackage>().ReverseMap();

        CreateMap<UserDto, ApplicationUser>().ReverseMap();

        CreateMap<HolidayBookingDto, HolidayBooking>()
            .ForMember(dest => dest.HolidayPackageId, opt => opt.MapFrom(src => src.HolidayPackage.HolidayPackageId))
            .ForMember(dest => dest.HolidayPackage, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.UserId))
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.HolidayPackage, opt => opt.MapFrom(src => src.HolidayPackage))
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer));

        CreateMap<TravellerDto, Traveller>()
            .ForMember(dest => dest.HolidayBooking, opt => opt.Ignore()).ReverseMap();

        CreateMap<HotelDto, Hotel>().ReverseMap();

        CreateMap<HotelRoomDto, HotelRoom>()
            .ForMember(dest => dest.Hotel, opt => opt.Ignore()).ReverseMap();

        CreateMap<HotelBookingDto, HotelBooking>()
            .ForMember(dest => dest.HolidayPackageId, opt => opt.MapFrom(src => src.HolidayPackageDto.HolidayPackageId))
            .ForMember(dest => dest.HolidayPackage, opt => opt.Ignore())
            .ForMember(dest => dest.HotelRoomId, opt => opt.MapFrom(src => src.HotelRoom.HotelRoomId))
            .ForMember(dest => dest.HotelRoom, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.HolidayPackageDto, opt => opt.MapFrom(src => src.HolidayPackage))
            .ForMember(dest => dest.HotelRoom, opt => opt.MapFrom(src => src.HotelRoom));
    }
}
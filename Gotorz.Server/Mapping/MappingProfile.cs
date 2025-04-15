using AutoMapper;
using Gotorz.Server.Models;
using Gotorz.Shared.DTO;

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
            .ForMember(dest => dest.Flight, opt => opt.Ignore()).ReverseMap();
    }
}
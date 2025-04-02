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
        CreateMap<Airport, AirportDto>().ReverseMap();
        CreateMap<Flight, FlightDto>()
            .ForMember(dest => dest.DepartureAirport, opt => opt.MapFrom(src => src.DepartureAirport))
            .ForMember(dest => dest.ArrivalAirport, opt => opt.MapFrom(src => src.ArrivalAirport))
            .ReverseMap()
            .ForMember(dest => dest.DepartureAirportId, opt => opt.MapFrom(src => src.DepartureAirport.AirportId))
            .ForMember(dest => dest.ArrivalAirportId, opt => opt.MapFrom(src => src.ArrivalAirport.AirportId));
    }
}
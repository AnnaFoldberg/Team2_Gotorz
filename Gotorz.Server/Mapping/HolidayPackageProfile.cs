using AutoMapper;
using Gotorz.Server.Models;
using Gotorz.Shared.DTO;
namespace Gotorz.Server.Mapping
{
    public class HolidayPackageProfile : Profile
    {
        public HolidayPackageProfile()
        {
            CreateMap<HolidayPackageDto, HolidayPackage>();

            CreateMap<HolidayPackage, HolidayPackageDto>();
        }
    }
}

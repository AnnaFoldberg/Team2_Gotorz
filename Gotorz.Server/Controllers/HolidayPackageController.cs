using AutoMapper;
using Gotorz.Server.DataAccess;
using Gotorz.Server.Models;
using Gotorz.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Gotorz.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HolidayPackageController : ControllerBase
    {
        private readonly IHolidayPackageRepository _holidayRepository;
        private IMapper _mapper;


        /// <summary>
        /// Initializes a new instance of the <see cref="HolidayPackageController"/> class.
        /// </summary>
        /// <param name="holidayRepository">The repository for managing holiday package entities.</param>
        /// <param name="mapper">The mapper for converting between DTOs and domain models.</param>
        public HolidayPackageController(IHolidayPackageRepository holidayRepository, IMapper mapper)
        {
            _holidayRepository = holidayRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all holiday packages from the database and returns them as a collection of DTOs.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a list of <see cref="HolidayPackageDto"/> objects.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HolidayPackageDto>>> GetAllAsync()
        {
            var all = await _holidayRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<HolidayPackageDto>>(all);

            return Ok(result);
        }
        

        /// <summary>
        /// Creates a new holiday package based on the provided DTO.
        /// </summary>
        /// <param name="dto">The data transfer object containing holiday package details.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
        [HttpPost]
        public async Task<ActionResult<HolidayPackageDto>> Create(HolidayPackageDto dto)
        {
            var package = _mapper.Map<HolidayPackage>(dto);
            await _holidayRepository.AddAsync(package);
            var packageDto = _mapper.Map<HolidayPackageDto>(package);

            return Ok(packageDto);
        }

        /// <summary>
        /// Retrieves a holiday package by its URL-friendly title and returns it as a DTO.
        /// </summary>
        /// <param name="url">The URL-encoded title used to identify the holiday package.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing the <see cref="HolidayPackageDto"/> if found; otherwise, <c>NotFound</c>.
        /// </returns>
        [HttpGet("{url}")]
        public async Task<ActionResult<HolidayPackageDto>> GetByUrl(string url)
        {
            var package = await _holidayRepository.GetByUrlAsync(url);
            if (package == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<HolidayPackageDto>(package));
        }
    }
}

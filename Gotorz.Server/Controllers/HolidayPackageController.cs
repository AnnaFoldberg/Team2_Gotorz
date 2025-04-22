using AutoMapper;
using Gotorz.Server.DataAccess;
using Gotorz.Server.Models;
using Gotorz.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Gotorz.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HolidayPackageController : ControllerBase
    {
        private readonly ISimpleKeyRepository<HolidayPackage>? _repository;
        private IMapper _mapper;

        public HolidayPackageController(ISimpleKeyRepository<HolidayPackage> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HolidayPackageDto>>> GetAll()
        {
            var all = await _repository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<HolidayPackageDto>>(all);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<HolidayPackageDto>> Create(CreateHolidayPackageDto dto)
        {
            var package = _mapper.Map<HolidayPackage>(dto);
            var created = await _repository.AddAsync(package);
            var resultDto = _mapper.Map<HolidayPackageDto>(created);


            return CreatedAtAction(nameof(GetAll), new { id = created.HolidayPackageId }, resultDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HolidayPackageDto>> GetById(int id)
        {
            var package = await _repository.GetByKeyAsync(id);
            if (package == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<HolidayPackageDto>(package));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, HolidayPackageDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var package = await _repository.GetByKeyAsync(id);
            if (package is null)
                return NotFound();

            package.Title = dto.Title;
            package.Description = dto.Description;

            await _repository.UpdateAsync(package);

            return NoContent();
        }
    }
}

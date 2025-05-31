using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Model;
using SmartContractVehicle.Service;

namespace SmartContractVehicle.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CarController(CarService carService, IMapper mapper) : ControllerBase
    {
        private readonly CarService _carService = carService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<ActionResult<List<CarTO>>> Query()
        {
            var carModels = await _carService.GetAllCarsAsync();
            var carDtos = _mapper.Map<List<CarTO>>(carModels);

            return Ok(carDtos);
        }
    }
}

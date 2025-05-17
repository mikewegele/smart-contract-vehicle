using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartContractVehicle.Data;
using SmartContractVehicle.Models;


namespace SmartContractVehicle.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]/")]
    public class AdressController(AppDbContext db) : ControllerBase
    {
        private readonly AppDbContext _db = db;

        [HttpGet(Name = "Get")]
        public IActionResult Get(int id)
        {
            var adress = _db.Adresses.Find(id);
            return Ok(adress);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            db.Adresses.Where(a => a.Id == id).ExecuteDelete();
            return Ok();
        }

        [HttpPost]
        public IActionResult Post(Adress adress)
        {
            if (ModelState.IsValid)
            {
                _db.Adresses.Add(adress);
                _db.SaveChanges();
                return Ok(adress);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPatch]
        public IActionResult Update(Adress adress)
        {
            if (ModelState.IsValid)
            {
                _db.Adresses.Update(adress);
                return Ok(adress);
            }

            return BadRequest();
        }
    }

}
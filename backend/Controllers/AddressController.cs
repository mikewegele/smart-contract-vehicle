using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartContractVehicle.Data;
using SmartContractVehicle.Model;

namespace SmartContractVehicle.Controllers;

[ApiController]
[Route("api/[controller]/[action]/")]
public class AddressController(AppDbContext db) : ControllerBase
{
    private readonly AppDbContext _db = db;

    [HttpGet(Name = "Get")]
    public IActionResult Get(int id)
    {
        var address = _db.Addresses.Find(id);
        return Ok(address);
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        _db.Addresses.Where(a => a.Id == id).ExecuteDelete();
        return Ok();
    }

    [HttpPost]
    public IActionResult Post(Address address)
    {
        if (ModelState.IsValid)
        {
            _db.Addresses.Add(address);
            _db.SaveChanges();
            return Ok(address);
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpPatch]
    public IActionResult Update(Address address)
    {
        if (ModelState.IsValid)
        {
            _db.Addresses.Update(address);
            return Ok(address);
        }

        return BadRequest();
    }
}
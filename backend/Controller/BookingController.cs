using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartContractVehicle.Data;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Model;

namespace SmartContractVehicle.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class BookingController(AppDbContext db, IMapper mapper) : ControllerBase
    {
        private readonly AppDbContext _db = db;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// This function will block the car from getting reserved by any one but the one who blocked it
        /// </summary>
        /// <param name="carId">The GUID of the car that shall be blocked.</param>
        /// <param name="ct">A CancellationToken if the Request is cancelled.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<CarTO>> BlockCar(Guid carId, CancellationToken ct)
        {
            var car = _db.Cars.Find(carId);
            if (car is null) return NotFound("No car with this ID was found.");

            if (car.Status.Id != (int)CarStatuses.Available)
                return Conflict("This car is already in use.");

            var userId = User.Claims.First(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti).Value;

            var user = _db.Users.Find(userId);
            if (user is null) return NotFound("The user was not found.");

            car.SetStatus(_db.CarStatuses.Find((int)CarStatuses.Pending));

            var reservation = new Reservation(userId);


            reservation.BlockCar(db, car);

            await _db.SaveChangesAsync(ct);
            return Ok(_mapper.Map<CarTO>(car));
        }

        [HttpPost]
        public async Task<ActionResult<Reservation>> ReserveCar(Guid carId, string transactionId, CancellationToken ct)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(transactionId);
            ArgumentException.ThrowIfNullOrEmpty(transactionId);

            var userId = User.Claims.First(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti).Value;

            var user = _db.Users.Find(userId);
            if (user is null) return NotFound("The user was not found.");

            var car = _db.Cars.Find(carId);
            if (car == null) return NotFound("Car not found.");

            if (car.Status.Id != (int)CarStatuses.Available)
                return Conflict("Car is not available.");

            // TODO Check transaction Id on the blockchain

            var reservation = new Reservation(car.Id, user.Id, car.PricePerMinute, transactionId);
            await _db.Reservations.AddAsync(reservation, ct);

            car.SetStatus(_db.CarStatuses.Find((int)CarStatuses.Reserved));
            _db.Cars.Update(car);
            await _db.SaveChangesAsync(ct);


            return Ok(reservation);

        }

        [HttpPost]
        public async Task<ActionResult> UnlockCar(Reservation userReservation, CancellationToken ct)
        {
            // First we get the user that tries to unlock the car
            var userId = User.Claims.First(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti).Value;
            var loggedInUser = _db.Users.Find(userId);
            if (loggedInUser is null) return NotFound("The user was not found.");

            // Then we check if the reservation the user provided is valid
            var databaseReservation = _db.Reservations.Find(userReservation.Id);
            if (databaseReservation == null) return NotFound("This reservation is not saved in the system.");

            if (databaseReservation.RentorId != loggedInUser.Id) return Conflict("You must provide a reservation of your own.");



            return Ok();
        }
    }
}

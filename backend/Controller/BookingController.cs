using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartContractVehicle.Data;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Model;
using Microsoft.EntityFrameworkCore;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;

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
        public async Task<ActionResult<ReservationTO>> BlockCar(Guid carId, CancellationToken ct)
        {   
            var userId = User.Claims.First(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti).Value;
            var user = _db.Users.Find(userId);
            if (user is null) return NotFound("The user was not found.");

            var car = _db.Cars.Find(carId);
            if (car is null) return NotFound("No car with this ID was found.");

            if (car.Status.Id != (int)CarStatuses.Available)
                return Conflict("This car is already in use.");

            var reservation = new Reservation(userId);

            reservation.BlockCar(db, car);

            _db.Add(reservation);

            await _db.SaveChangesAsync(ct);
            return Ok(_mapper.Map<ReservationTO>(reservation));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationTO>>> GetAllReservations(CancellationToken ct)
        {
            var reservations = await _db.Reservations.ToListAsync(ct);
            var reservationDTOs = _mapper.Map<IEnumerable<ReservationTO>>(reservations);
            return Ok(reservationDTOs);
        }

        [HttpPost]
        public async Task<ActionResult<ReservationTO>> ReserveCar(ReservationTO reservation, CancellationToken ct)
        {
            var userId = User.Claims.First(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti).Value;
            var user = _db.Users.Find(userId);
            if (user is null) return NotFound("User not found.");

            var dbReservation = _db.Reservations.Find(reservation.Id);
            if (dbReservation is null) return NotFound("Reservation not found.");

            var transacId = reservation.BlockchainTransactionId;

            var rpcUrl = "http://127.0.0.1:7545";
            var web3 = new Web3(rpcUrl);

            Transaction tx = null;
            try
            {
                tx = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transacId);
            }
            catch (Exception ex)
            {
                return BadRequest("Error fetching transaction: " + ex.Message);
            }

            if (tx == null)
            {
                return BadRequest("Transaction not found or invalid.");
            }

            dbReservation.BlockchainTransactionId = transacId;
            dbReservation.ReserveCar(_db);

            await _db.SaveChangesAsync(ct);
            return Ok(reservation);
        }

        [HttpPost]
        public async Task<ActionResult> UnlockCar(ReservationTO userReservation, CancellationToken ct)
        {
            // First we get the user that tries to unlock the car
            var userId = User.Claims.First(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti).Value;
            var loggedInUser = _db.Users.Find(userId);
            if (loggedInUser is null) return NotFound("The user was not found.");

            // Then we check if the reservation the user provided is valid
            var dBReservation = _db.Reservations.Find(userReservation.Id);
            if (dBReservation == null) return NotFound("This reservation is not saved in the system.");

            // Check that this is still a valid Reservation
            if (dBReservation.ReservationCancelled || dBReservation.ReservationCompleted)
                return BadRequest("This Reservation was either cancelled or already completed. Please try again.");

            // Check that the person that reserved is the same as the one requesting the opening
            if (dBReservation.RentorId != loggedInUser.Id) return BadRequest("You must provide a reservation of your own.");

            // TODO Send Notifcation to the car to open

            // TODO Create new Ride Object and share it with the user, we later can use this to save ride info etc

            return Ok();
        }
    }
}

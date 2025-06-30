using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using SmartContractVehicle.Data;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Model;
using SmartContractVehicle.Service;

namespace SmartContractVehicle.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class BookingController(AppDbContext db, IMapper mapper, ICarCommandService carCommandService) : ControllerBase
    {
        private readonly AppDbContext _db = db;
        private readonly IMapper _mapper = mapper;
        private readonly ICarCommandService  _carCommandService = carCommandService;

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
            var userId = User.Claims.First(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti).Value;

            var reservations = await _db.Reservations.Where(r => r.RentorId == userId).ToListAsync(ct);
            var reservationDTOs = _mapper.Map<IEnumerable<ReservationTO>>(reservations);
            return Ok(reservationDTOs);
        }

        [HttpPost]
        public async Task<ActionResult<string>> ReserveCar(ReservationTO reservation, CancellationToken ct)
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

            var message = $"Reservation confirmed with transaction {transacId} in block {tx.BlockNumber.Value}.";
            return Ok(message);
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
            var car = _db.Cars.Find(dBReservation.ReservedCarId);
            if (car is null) return NotFound("The car reserved doesn't seem to exist.");

            var answer = await _carCommandService.SendUnlockCommandAsync(car.VIN);
            if (!answer.Success) return BadRequest(answer.Message);
            // TODO Create new Ride Object and share it with the user, we later can use this to save ride info etc

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<ReservationTO>> FinishDriving(Guid reservationId, CancellationToken ct)
        {
            var userId = User.Claims.First(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti).Value;
            var user = await _db.Users.FindAsync([userId], ct);
            if (user == null)
                return NotFound("User not found.");

            var reservation = await _db.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId, ct);
            if (reservation == null)
                return NotFound("Reservation not found.");

            if (reservation.RentorId != user.Id)
                return BadRequest("You can only finish your own reservation.");

            var car = await _db.Cars.FindAsync([reservation.ReservedCarId], ct);
            if (car == null)
                return NotFound("Associated car not found.");

            var answer = await _carCommandService.SendLockCommandAsync(car.VIN);
            if (!answer.Success) return BadRequest(answer.Message);

            car.SetStatus(await _db.CarStatuses.FindAsync([(int)CarStatuses.Available], ct), reservation);
            car.ActiveReservation = null;
            reservation = await _db.Reservations.FindAsync([reservationId], ct);
            if (reservation != null)
            {
                _db.Reservations.Remove(reservation);
                await _db.SaveChangesAsync();
            }
            return Ok(reservation);
        }



    }
}

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

        [HttpPost]
        public async Task<ActionResult<ReservationTO>> ReserveCar(ReservationTO reservation, CancellationToken ct)
        {

            var userId = User.Claims.First(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti).Value;

            var user = _db.Users.Find(userId);
            if (user is null) return NotFound("The user was not found.");

            var dbReservation = _db.Reservations.Find(reservation.Id);
            if (dbReservation is null) return NotFound("Reserveration unknown.");

            // TODO Check transaction Id on the blockchain if the transaction is non valid, cancle the reservation
            var transacId = reservation.BlockchainTransactionId;

            dbReservation.BlockchainTransactionId = transacId;

            dbReservation.ReserveCar(db);
            
            await _db.SaveChangesAsync(ct);
            return Ok(reservation);
        }

        [HttpPost]
        public async Task<ActionResult> StartRide(ReservationTO userReservation, CancellationToken ct)
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

            // TODO Create new Booking / Ride Object and share it with the user, we later can use this to save ride info etc

            return Ok();
        }

        [HttpPost]
        public ActionResult EndRideProcedureInit()
        {
            // TODO Check if car is stationary (and empty)
            

            // TODO Try to lock car 


            return Ok(); // bzw.: inkl RideLedger/Booking object
        }

        // Für unsere Buchung müssen wir speichern:
        // StartOrt, StartZeit, User, Fahrzeug, Preis p. Minute, EndZeit, EndOrt, TransactionIdBlockchain, (dynamisch Fahrtdauer & Preis berechnen) fahrtpreis => ppm * (endzeit - startzeit);

        [HttpPost]
        public ActionResult EndRideProcedureFinalize() // in: RideTO userRide inkl. TransaktionsId
        {
            // DB Query dbRide = db.Find(userRide.Id)

            // dbRide.EndRide(); 
            // EndRide : Funktion aus ride / booking objekt
              
            // TODO Check if ride is paid for (blockchain)

            // Finalize Booking object // Endzeit, End Ort, 
            // Auto Verfügbar machen
            // Telemtriedaten updaten etc.

            return Ok();
        }
    }
}

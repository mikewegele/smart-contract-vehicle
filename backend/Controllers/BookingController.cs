using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using NetTopologySuite.Geometries;
using SmartContractVehicle.Data;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Model;
using SmartContractVehicle.Service;

namespace SmartContractVehicle.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class BookingController(AppDbContext db, IMapper mapper, CarCommandService carCommandService, ILogger<BookingController> logger, TelemetryService telemetryService) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;
    private readonly CarCommandService _carCommandService = carCommandService;
    private readonly ILogger<BookingController> _logger = logger;
    private readonly TelemetryService _telemetryService = telemetryService;

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
        _logger.LogInformation("User {UserId} is attempting to block car {CarId}", userId, carId);

        var user = await _db.Users.FindAsync([userId], ct);
        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} not found while trying to block car {CarId}", userId, carId);
            return NotFound("The user was not found.");
        }

        var car = await _db.Cars.FindAsync([carId], ct);
        if (car is null)
        {
            _logger.LogWarning("Car with ID {CarId} not found for blocking request by user {UserId}", carId, userId);
            return NotFound("No car with this ID was found.");
        }

        if (car.Status.Id != (int)CarStatuses.Available)
        {
            _logger.LogWarning("Car {CarId} is not available (current status: {StatusId}). Block request by user {UserId} denied.", car.Id, car.Status.Id, userId);
            return Conflict("This car is already in use.");
        }

        var reservation = new Reservation(userId);
        reservation.BlockCar(_db, car);
        _db.Add(reservation);

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Car {CarId} successfully blocked by user {UserId} with reservation {ReservationId}", car.Id, userId, reservation.Id);
        return Ok(_mapper.Map<ReservationTO>(reservation));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservationTO>>> GetAllReservations(CancellationToken ct)
    {
        var userId = User.Claims.First(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti).Value;
        _logger.LogInformation("User {UserId} is requesting all their reservations.", userId);

        var reservations = await _db.Reservations.Where(r => r.RentorId == userId).ToListAsync(ct);
        var reservationDTOs = _mapper.Map<IEnumerable<ReservationTO>>(reservations);

        _logger.LogInformation("Successfully retrieved {Count} reservations for user {UserId}", reservationDTOs.Count(), userId);
        return Ok(reservationDTOs);
    }

    [HttpPost]
    public async Task<ActionResult<string>> ReserveCar(ReservationTO reservation, CancellationToken ct)
    {
        var userId = User.Claims.First(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti).Value;
        _logger.LogInformation("User {UserId} is attempting to reserve car with reservation {ReservationId} and transaction {TransactionId}", userId, reservation.Id, reservation.BlockchainTransactionId);

        var user = await _db.Users.FindAsync([userId], ct);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found while trying to reserve with reservation {ReservationId}", userId, reservation.Id);
            return NotFound("User not found.");
        }

        var dbReservation = await _db.Reservations.FindAsync([reservation.Id], ct);
        if (dbReservation is null)
        {
            _logger.LogWarning("Reservation {ReservationId} not found for user {UserId}", reservation.Id, userId);
            return NotFound("Reservation not found.");
        }

        var transacId = reservation.BlockchainTransactionId;
        var rpcUrl = "http://127.0.0.1:7545";
        var web3 = new Web3(rpcUrl);

        Transaction? tx = null;
        try
        {
            _logger.LogDebug("Fetching transaction {TransactionId} from blockchain for user {UserId}", transacId, userId);
            tx = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transacId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching transaction {TransactionId} from blockchain for user {UserId}", transacId, userId);
            return BadRequest("Error fetching transaction: " + ex.Message);
        }

        if (tx == null)
        {
            _logger.LogWarning("Blockchain transaction {TransactionId} not found or invalid for user {UserId}", transacId, userId);
            return BadRequest("Transaction not found or invalid.");
        }

        dbReservation.BlockchainTransactionId = transacId;
        dbReservation.ReserveCar(_db);

        await _db.SaveChangesAsync(ct);

        var message = $"Reservation confirmed with transaction {transacId} in block {tx.BlockNumber.Value}.";
        _logger.LogInformation("Reservation {ReservationId} successfully confirmed for user {UserId} with transaction {TransactionId}", dbReservation.Id, userId, transacId);
        return Ok(message);
    }

    [HttpPost]
    public async Task<ActionResult> UnlockCar(ReservationTO userReservation, CancellationToken ct)
    {
        var userId = User.Claims.First(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti).Value;
        _logger.LogInformation("User {UserId} is attempting to unlock car with reservation {ReservationId}", userId, userReservation.Id);

        var loggedInUser = await _db.Users.FindAsync([userId], ct);
        if (loggedInUser is null)
        {
            _logger.LogWarning("User {UserId} not found while trying to unlock car with reservation {ReservationId}", userId, userReservation.Id);
            return NotFound("The user was not found.");
        }

        var dBReservation = await _db.Reservations.FindAsync([userReservation.Id], ct);
        if (dBReservation == null)
        {
            _logger.LogWarning("Reservation {ReservationId} not found for unlock request by user {UserId}", userReservation.Id, userId);
            return NotFound("This reservation is not saved in the system.");
        }

        if (dBReservation.ReservationCancelled || dBReservation.ReservationCompleted)
        {
            _logger.LogWarning("User {UserId} attempted to unlock with an invalid reservation {ReservationId} (Cancelled: {Cancelled}, Completed: {Completed})", userId, dBReservation.Id, dBReservation.ReservationCancelled, dBReservation.ReservationCompleted);
            return BadRequest("This Reservation was either cancelled or already completed. Please try again.");
        }

        if (dBReservation.RentorId != loggedInUser.Id)
        {
            _logger.LogWarning("User {UserId} attempted to unlock reservation {ReservationId} belonging to another user {OwnerId}", loggedInUser.Id, dBReservation.Id, dBReservation.RentorId);
            return BadRequest("You must provide a reservation of your own.");
        }

        var car = await _db.Cars.FindAsync([dBReservation.ReservedCarId], ct);
        if (car is null)
        {
            _logger.LogWarning("Car with ID {CarId} for reservation {ReservationId} not found during unlock attempt by user {UserId}", dBReservation.ReservedCarId, dBReservation.Id, userId);
            return NotFound("The car reserved doesn't seem to exist.");
        }

        var initialState = new TelemetryTO
        {
            CurrentPosition = new Point(car.CurrentPosition.X, car.CurrentPosition.Y) { SRID = car.CurrentPosition.SRID },
            RemainingReach = car.RemainingReach,
            CurrentSpeed = 0, // Car is stationary when unlocked
            Heading = 0       // Default heading
        };

        _logger.LogInformation("Sending unlock and sync command to car {VIN} for user {UserId}", car.VIN, userId);

        var answer = await _carCommandService.SendUnlockAndSyncCommandAsync(car.VIN, initialState);
        if (!answer.Success)
        {
            _logger.LogError("Failed to send unlock and sync command to car {VIN} for user {UserId}. Reason: {Reason}", car.VIN, userId, answer.Message);
            return BadRequest(answer.Message);
        }

        _logger.LogInformation("Successfully sent unlock and sync command to car {VIN} for user {UserId}", car.VIN, userId);
        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult<ReservationTO>> FinishDriving(Guid reservationId, CancellationToken ct)
    {
        var userId = User.Claims.First(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti).Value;
        _logger.LogInformation("User {UserId} is attempting to finish driving for reservation {ReservationId}", userId, reservationId);

        var user = await _db.Users.FindAsync([userId], ct);
        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found while trying to finish driving for reservation {ReservationId}", userId, reservationId);
            return NotFound("User not found.");
        }

        var reservation = await _db.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId, ct);
        if (reservation == null)
        {
            _logger.LogWarning("Reservation {ReservationId} not found for finish driving request by user {UserId}", reservationId, userId);
            return NotFound("Reservation not found.");
        }

        if (reservation.RentorId != user.Id)
        {
            _logger.LogWarning("User {UserId} attempted to finish reservation {ReservationId} belonging to another user {OwnerId}", user.Id, reservation.Id, reservation.RentorId);
            return BadRequest("You can only finish your own reservation.");
        }

        var car = await _db.Cars.FindAsync([reservation.ReservedCarId], ct);
        if (car == null)
        {
            _logger.LogWarning("Car with ID {CarId} for reservation {ReservationId} not found during finish driving attempt by user {UserId}", reservation.ReservedCarId, reservation.Id, user.Id);
            return NotFound("Associated car not found.");
        }

        _logger.LogInformation("Attempting to retrieve final telemetry for car {VIN} before locking.", car.VIN);
        var finalTelemetry = _telemetryService.GetCarState(car.VIN);

        if (finalTelemetry != null)
        {
            _logger.LogInformation("Final telemetry found for car {VIN}. Updating database state. Position: ({Lat}, {Lon}), Reach: {Reach}", car.VIN, finalTelemetry.CurrentPosition.Y, finalTelemetry.CurrentPosition.X, finalTelemetry.RemainingReach);
            car.CurrentPosition = finalTelemetry.CurrentPosition;
            car.RemainingReach = finalTelemetry.RemainingReach;
        }
        else
        {
            _logger.LogWarning("Could not retrieve final telemetry for car {VIN}. Database state will not be updated with the final driving position.", car.VIN);
        }

        _logger.LogInformation("Sending lock command to car {VIN} for user {UserId} to finish driving session.", car.VIN, user.Id);
        var answer = await _carCommandService.SendLockCommandAsync(car.VIN);
        if (!answer.Success)
        {
            _logger.LogError("Failed to send lock command to car {VIN} for user {UserId}. Reason: {Reason}", car.VIN, user.Id, answer.Message);
            return BadRequest(answer.Message);
        }

        car.SetStatus(await _db.CarStatuses.FindAsync([(int)CarStatuses.Available], ct), reservation);
        car.ActiveReservation = null;

        _db.Reservations.Remove(reservation);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Successfully finished driving for user {UserId} and reservation {ReservationId}. Car {VIN} is now locked and available.", user.Id, reservationId, car.VIN);
        return Ok(_mapper.Map<ReservationTO>(reservation));
    }
}

// SPDX-License-Identifier: MIT
pragma solidity ^0.8.19;

contract CarRental {

    event CarRented(
        string carId,
        address indexed renter,
        string userId,
        uint256 timestamp,
        uint256 value
    );

    event CarReservationCancelled(
        string carId,
        address indexed renter,
        string userId,
        uint256 timestamp
    );

    event CarDriven(
        string carId,
        address indexed driver,
        string userId,
        uint256 timestamp
    );

    event CarReturned(
        string carId,
        address indexed driver,
        string userId,
        uint256 timestamp,
        uint256 value
    );

    function rentCar(string memory carId, string memory userId) public payable {
        require(msg.value > 0, "No Ether sent");
        emit CarRented(carId, msg.sender, userId, block.timestamp, msg.value);
    }

    function cancelReservation(string memory carId, string memory userId) public {
        emit CarReservationCancelled(carId, msg.sender, userId, block.timestamp);
    }

    function driveCar(string memory carId, string memory userId) public {
        emit CarDriven(carId, msg.sender, userId, block.timestamp);
    }

    function returnCar(string memory carId, string memory userId) public payable {
        require(msg.value > 0, "Payment required to finish driving");
        emit CarReturned(carId, msg.sender, userId, block.timestamp, msg.value);
    }

    function helloWorld() public pure returns (string memory) {
        return "Hello, world!";
    }
}
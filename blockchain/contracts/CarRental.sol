// SPDX-License-Identifier: MIT
pragma solidity ^0.8.19;

contract CarRental {

    event CarRented(
        string carId,
        address indexed renter,
        string userId,
        uint256 timestamp
    );

    event CarReservationCancelled(
        string carId,
        address indexed renter,
        string userId,
        uint256 timestamp
    );


    function rentCar(string memory carId, string memory userId) public payable {
        require(msg.value > 0, "No Ether sent");
        emit CarRented(carId, msg.sender, userId, block.timestamp);
    }

    function cancelReservation(string memory carId, string memory userId) public {
        emit CarReservationCancelled(carId, msg.sender, userId, block.timestamp);
    }

    function helloWorld() public pure returns (string memory) {
        return "Hello, world!";
    }
}
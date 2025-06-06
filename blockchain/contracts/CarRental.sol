// SPDX-License-Identifier: MIT
pragma solidity ^0.8.19;

contract SimpleCarRental {
    address public owner;

    constructor() {
        owner = msg.sender;
    }

    struct Car {
        uint256 id;
        string model;
        uint256 pricePerDay;
        bool isAvailable;
        address renter;
    }

    uint256 public nextCarId = 1;
    mapping(uint256 => Car) public cars;

    event CarAdded(uint256 carId, string model, uint256 pricePerDay);
    event CarRented(uint256 carId, address renter, uint256 daysRented);
    event CarReturned(uint256 carId, address renter);

     function helloWorld() public pure returns (string memory) {
            return "Hello, world!";
        }

    function addCar(string memory model, uint256 pricePerDay) public {
        cars[nextCarId] = Car(nextCarId, model, pricePerDay, true, address(0));
        emit CarAdded(nextCarId, model, pricePerDay);
        nextCarId++;
    }

    function rentCar(uint256 carId, uint256 numberOfDays) public payable {
        Car storage car = cars[carId];
        car.isAvailable = false;
        car.renter = msg.sender;
        emit CarRented(carId, msg.sender, numberOfDays);
    }

    function returnCar(uint256 carId) public {
        Car storage car = cars[carId];
        car.isAvailable = true;
        car.renter = address(0);
        emit CarReturned(carId, msg.sender);
    }

    function getCar(uint256 carId) public view returns (Car memory) {
        return cars[carId];
    }

    function withdraw() public {
        payable(owner).transfer(address(this).balance);
    }
}

// SPDX-License-Identifier: MIT
pragma solidity ^0.8.19;

contract CarRental {
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

    modifier onlyOwner() {
        require(msg.sender == owner, "Only owner can do this");
        _;
    }

    modifier onlyRenter(uint256 carId) {
        require(cars[carId].renter == msg.sender, "You didn't rent this car");
        _;
    }

    function addCar(string memory model, uint256 pricePerDay) public onlyOwner {
        cars[nextCarId] = Car(nextCarId, model, pricePerDay, true, address(0));
        emit CarAdded(nextCarId, model, pricePerDay);
        nextCarId++;
    }

    function rentCar(uint256 carId, uint256 numberOfDays) public payable {
        Car storage car = cars[carId];
        require(car.isAvailable, "Car not available");
        uint256 totalCost = car.pricePerDay * numberOfDays;
        require(msg.value >= totalCost, "Not enough ETH sent");

        car.isAvailable = false;
        car.renter = msg.sender;

        emit CarRented(carId, msg.sender, numberOfDays);
    }

    function returnCar(uint256 carId) public onlyRenter(carId) {
        Car storage car = cars[carId];
        car.isAvailable = true;
        car.renter = address(0);

        emit CarReturned(carId, msg.sender);
    }

    function getCar(uint256 carId) public view returns (Car memory) {
        return cars[carId];
    }

    function withdraw() public onlyOwner {
        payable(owner).transfer(address(this).balance);
    }
}

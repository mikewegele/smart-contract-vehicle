// SPDX-License-Identifier: MIT
pragma solidity ^0.8.19;

contract SimpleCarRental {

    struct Car {
        uint256 id;
        string model;
        uint256 pricePerDay;
        bool isAvailable;
        address renter;
    }

    uint256 public nextCarId = 1;
    uint256 public totalCars;
    bool public initialized;

    mapping(uint256 => Car) public cars;

    event CarAdded(uint256 carId, string model, uint256 pricePerDay);
    event CarRented(uint256 carId, address renter, string transaktionnummer, bool transactionSucceeded);
    event CarReturned(uint256 carId, address renter);

    function helloWorld() public pure returns (string memory) {
        return "Hello, world!";
    }

    modifier onlyIfNotInitialized() {
        require(!initialized, "Cars already initialized");
        _;
    }

    function initializeDefaultCars(
        uint256[] memory ids,
        string[] memory models,
        uint256[] memory prices
    ) public onlyIfNotInitialized {
        require(
            ids.length == models.length && models.length == prices.length,
            "Mismatched array lengths"
        );

        for (uint i = 0; i < ids.length; i++) {
            cars[ids[i]] = Car({
                id: ids[i],
                model: models[i],
                pricePerDay: prices[i],
                isAvailable: true,
                renter: address(0)
            });
            totalCars++;
        }

        initialized = true;
    }

    function rentCar(string carId, string userId, uint256 pricePerMinute) public payable {
        uint256 totalPrice = (pricePerMinute / 2) * 15;
        require(msg.value >= totalPrice, "Not enough Ether sent");
        car.renter = msg.sender;
        emit CarRented(carId, msg.sender, "hash", true);
    }
}

# Decentralized Carsharing Platform

This project is a **web-based carsharing platform** that uses **crypto wallets** and **smart contracts** to manage
vehicle reservations, access, and payments transparently and securely.

# Project Setup

This project consists of a **backend** built with **ASP.NET Core** and **PostgreSQL**, and a **frontend** built with *
*React**.

## Prerequisites

Ensure you have the following installed before proceeding:

- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or higher)
- [Node.js](https://nodejs.org/) (version 14 or higher)
- [PostgreSQL](https://www.postgresql.org/) (version 12 or higher)

### Optional (if you're using Visual Studio):

- [Visual Studio](https://visualstudio.microsoft.com/) with the **.NET Core Development** workload.
- **PostgreSQL Extension** in Visual Studio for easier database management.

---

## Running the Project Components

This project uses multiple scripts to manage the database, backend, blockchain, and frontend.

### Important scripts overview

- **`npm run drop-all-tables`**  
  Removes all tables from the database. Use with caution, as this deletes all data.

- **`npm run database`**  
  Starts the PostgreSQL database using Docker Compose. This should be running before starting the backend.

- **`npm run backend`**  
  Starts the backend server built with ASP.NET Core. It handles communication with the database and provides the API for
  the frontend.

- **`npm run smart-contract`**  
  Starts the local blockchain environment (Ganache) for smart contract development and testing.

- **`npm run frontend`**  
  Builds and runs the React frontend, deploys smart contracts, generates TypeScript types, and serves the app in your
  browser.

- **`npm run car-clients`**
  Build and runs the cars Client. Remeber to set the VINs Array in the appsettings.json to the ones in your database.
  Also needs the backend running to function.  

---

## Recommended start sequence

Open separate terminal windows/tabs and run these commands in order:

1. **Start the database:**
   ```bash
   npm run database
   ```

2. **Start the backend:**

   ```bash
   npm run backend
   ```
3. **Start the car clients:**
   ```bash
   npm run car-client
   ```

5. **Start the blockchain environment:**

   ```bash
   npm run smart-contract
   ```
6. **Start the frontend:**

   ```bash
   npm run frontend
   ```

Make sure Docker is running and the database is accessible before starting the backend. Running the commands in separate
terminals allows all services to operate concurrently.

---

## Detailed manual setup (if you prefer manual steps)

### Database Setup (PostgreSQL with Docker)

   ```bash
   cd stack/
   docker-compose up -d
   ```

### Backend Setup (ASP.NET Core)

   ```bash
   cd backend/
   dotnet restore
   dotnet build
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   dotnet run
   ```

Backend will be available on `http://localhost:5147`.
The Map for the connected Cars will be available on `http://localhost:5147/Map`.

### Frontend Setup (React)

   ```bash
   cd frontend/
   npm install
   npm run dev
   ```

Frontend will be available on `http://localhost:5173`.

---

## Key Features

- 🔐 User verification via connected crypto wallet
- 📄 Smart contract-based reservations
- 💸 Pay-per-use model (e.g. unlock fee, per-minute pricing)
- 📍 Geolocation-based vehicle availability
- ⏱️ Auto reservation timeouts (e.g. 15 min grace period)

## Tech Stack

## Architecture

This project uses a **hybrid architecture**:

- Smart contracts handle payments, rights, and on-chain reservation logic.
- Off-chain backend handles user logic, geolocation, timers, and sensitive data.
- Web frontend provides the interface for searching, reserving, and accessing vehicles.

## Project Structure

## Local Development

## License

# Decentralized Carsharing Platform

This project is a **web-based carsharing platform** that uses **crypto wallets** and **smart contracts** to manage vehicle reservations, access, and payments transparently and securely.

# Project Setup

This project consists of **backend** built with **ASP.NET Core** and **PostgreSQL**, and a **frontend** built with **React**.

## Prerequisites

Ensure you have the following installed before proceeding:

- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or higher)
- [Node.js](https://nodejs.org/) (version 14 or higher)
- [PostgreSQL](https://www.postgresql.org/) (version 12 or higher)

### Optional (if you're using Visual Studio):
- [Visual Studio](https://visualstudio.microsoft.com/) with the **.NET Core Development** workload.
- **PostgreSQL Extension** in Visual Studio for easier database management.

---

## First Terminal: Database Setup (PostgreSQL Database)
 
1. Navigate to the `stack` directory:

    ```bash
    cd stack/
    ```

2. Run the  `docker-compose.yml` :

    ```bash
    docker-compose up -d
    ```

   This will start a PostgreSQL container and make it available at `localhost:5432`.

## Second Terminal: Backend Setup (ASP.NET Core)

1. Navigate to the `backend` directory:

    ```bash
    cd backend/
    ```

2. Install the required dependencies and build the project:

    ```bash
    dotnet restore
    dotnet build
    ```

3. Create and apply the database migrations:

    ```bash
    dotnet ef migrations add InitialCreate
    dotnet ef database update
    ```

   This will create the required database schema based on your `DbContext`.

4. Start the backend API:

    ```bash
    dotnet run
    ```

   The API should now be running on `http://localhost:5147` by default (you can check the port in the terminal output).

---

## Third Terminal: Frontend Setup (React)

1. Navigate to the `frontend` directory:

    ```bash
    cd frontend/
    ```

2. Install the dependencies:

    ```bash
    npm install
    ```

3. Run the frontend application:

    ```bash
    npm run dev
    ```

   The React app should now be running on `http://localhost:5173` by default.

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

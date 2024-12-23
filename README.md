
# Mortgage Pricing System with CQRS

This project implements a **Mortgage Pricing System** using the **CQRS (Command Query Responsibility Segregation)** pattern. The system separates the read and write responsibilities to optimize scalability, performance, and maintainability.

## Project Overview

The Mortgage Pricing System is designed to:
- Allow users to fetch mortgage pricing details (Query).
- Enable updates to mortgage pricing policies (Command).

The system is built with ASP.NET Core and follows best practices in microservice architecture. CQRS principles are applied to ensure clear separation of concerns between read and write operations.

## Features

### Query Side:
- Retrieve optimized pricing details based on mortgage policies.
- Read model optimized for fast, efficient queries.

### Command Side:
- Update mortgage pricing policies (e.g., interest rates, credit score requirements).
- Write model ensures business rules are enforced during updates.

### Additional Features:
- Supports eventual consistency between read and write models.
- Scalable architecture for handling high read-to-write ratios.
- Ready for integration with different databases for read and write operations.

## Technology Stack

- **Framework**: ASP.NET Core
- **Database**: SQL Server (for write operations), Optional NoSQL/Denormalized Database for read.
- **Pattern**: CQRS
- **Languages**: C#

## Architecture

1. **Command Side**:
   - Processes write operations (e.g., create, update, delete).
   - Enforces business rules.
   - Updates the database.

2. **Query Side**:
   - Handles read operations.
   - Provides read-optimized data.
   - Can utilize caching or a denormalized database for performance.

3. **Eventual Consistency**:
   - Ensures the read database reflects write changes asynchronously.

## Getting Started

### Prerequisites

- .NET 6.0 SDK
- SQL Server
- Any modern code editor (e.g., Visual Studio, Visual Studio Code)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/your-repo/mortgage-cqrs.git
   cd mortgage-cqrs
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Update database connection strings in `appsettings.json`.

4. Apply migrations and seed data (if applicable):
   ```bash
   dotnet ef database update
   ```

5. Run the application:
   ```bash
   dotnet run
   ```

6. Access the API via `https://localhost:5001`.

## Usage

### Query API
- Endpoint: `/api/mortgage/pricing`
- Method: `GET`
- Description: Fetch mortgage pricing details.

### Command API
- Endpoint: `/api/mortgage/update-policy`
- Method: `PUT`
- Description: Update mortgage pricing policies.

## Project Structure

- `Commands/`: Command-side handlers and models.
- `Queries/`: Query-side handlers and models.
- `Controllers/`: API endpoints for query and command operations.
- `Infrastructure/`: Database context and configuration.

## Future Enhancements

- Add caching to the query side for better performance.
- Introduce messaging for eventual consistency using RabbitMQ or Azure Service Bus.
- Expand the read database with a NoSQL implementation for scalability.
- Add unit and integration tests for robust coverage.

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.

---

## Contributing

Contributions are welcome! Please submit a pull request or create an issue for any suggestions or improvements.

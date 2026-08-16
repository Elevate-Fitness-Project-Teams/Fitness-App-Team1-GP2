# Elevate Fitness Platform

Elevate is a comprehensive, microservices-based fitness and health application. It is designed around Domain-Driven Design (DDD) principles and leverages an event-driven architecture to provide a seamless user experience for tracking workouts, nutrition, functional capacity, and receiving AI-driven health recommendations.

## 🏗 Architecture

The platform is built using **ASP.NET Core (Web APIs)** and follows a microservices architecture. Each service is independent, managing its own domain and database to ensure scalability and loose coupling.

### Key Technologies
- **Framework:** .NET 7/8 (C#)
- **Data Access:** Entity Framework Core (EF Core)
- **Database:** SQL Server (Containerized, per-service logical databases)
- **Messaging:** RabbitMQ (Event-driven architecture for notifications & progress tracking)
- **Architecture Patterns:** CQRS (MediatR), Domain-Driven Design (DDD)
- **Security:** JWT Authentication, Data Protection API (DPAPI)
- **Containerization:** Docker & Docker Compose

## 🧩 Microservices

1. **AuthenticationService:** Manages user registration, login, and issues JWT tokens for secure API access.
2. **UserProfileService:** Handles user demographic data, profile pictures, and personal preferences.
3. **NutritionService:** Provides meal recommendations, calculates calorie targets, and aggregates the user's home feed. Connects to the SmartCoach service.
4. **ProgressTrackingService:** Logs user workouts and tracks historical progress over time.
5. **SmartCoachService:** An AI-powered service that aggregates data (nutrition, capacity, progress) to provide personalized, AI-driven health and workout recommendations.
6. **FCEService:** Manages Functional Capacity Evaluations to assess user fitness levels safely.
7. **NotificationService:** Listens to RabbitMQ events (e.g., `progress.events`, `achievement_earned`) and dispatches notifications to users.
8. **FitnessApp / WorkoutService:** Core service handling workout routines and exercise executions.
9. **FitnessApp.Shared:** A shared library containing cross-cutting concerns, such as JWT authentication middleware, custom exceptions, and extensions.

## 🚀 Getting Started

The easiest way to run the entire backend stack locally is using Docker Compose.

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running.
- .NET SDK (if running services locally outside of Docker).

### Running the Infrastructure
From the root directory of the project, run:

```bash
docker-compose up -d
```

This command will spin up:
- SQL Server (`elevate-sqlserver`) on port `1434`
- RabbitMQ (`elevate-rabbitmq`) on ports `5673` (AMQP) & `15673` (Management UI)
- All the .NET microservices on their respective ports (`8085` to `8089`).

*Note: Database migrations should be automatically applied on startup, or handled through the respective service pipelines.*

## 🔒 Authentication Flow
The system uses centralized JWT authentication. To test endpoints in Swagger (available at `/swagger` on most services in the Development environment), you first need to authenticate via the `AuthenticationService`, retrieve the JWT token, and authorize using the Bearer scheme.

## 🤝 Contributing
- **Branching Strategy:** Ensure all new features are branched off `DEV`.
- **Commits:** Follow conventional commit standards (e.g., `feat:`, `fix:`, `docs:`).
- **PRs:** Submit Pull Requests against the `DEV` branch for code review.

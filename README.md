# Employee Evaluation System - Backend

This is the backend service for the Employee Evaluation System, designed for scalability, maintainability, and security.

## Table of Contents

- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Environment Variables](#environment-variables)
- [Docker Setup](#docker-setup)
- [Scripts](#scripts)
- [API Documentation](#api-documentation)
- [Contributing](#contributing)
- [License](#license)

## Technologies Used

- **Node.js**: JavaScript runtime environment
- **Express.js**: Web framework for Node.js
- **MongoDB**: NoSQL database
- **Mongoose**: ODM for MongoDB
- **JWT**: Authentication and authorization
- **bcrypt**: Password hashing
- **dotenv**: Environment variable management
- **Swagger**: API documentation
- **Docker**: Containerization for deployment

## Project Structure

```
/Backend
│
├── src/
│   ├── config/
│   ├── controllers/
│   ├── middlewares/
│   ├── models/
│   ├── routes/
│   ├── services/
│   └── utils/
├── db/
│   └── init/           # Database initialization scripts
├── tests/
├── .env
├── Dockerfile
├── docker-compose.yml
├── package.json
└── README.md
```

## Getting Started

1. **Clone the repository:**
    ```bash
    git clone https://github.com/yourusername/EmployeeEvaluationSystem.git
    cd EmployeeEvaluationSystem/Backend
    ```

2. **Install dependencies:**
    ```bash
    npm install
    ```

3. **Configure environment variables:**
    - Copy `.env.example` to `.env` and update the values as needed.

4. **Run the application:**
    ```bash
    npm start
    ```

## Environment Variables

- `PORT`: Server port
- `MONGODB_URI`: MongoDB connection string
- `JWT_SECRET`: Secret key for JWT

## Docker Setup

1. **Build and start services:**
    ```bash
    docker-compose up --build
    ```

2. **Database Initialization:**
    - Initialization scripts are located in `db/init/` and will be executed automatically when the MongoDB container starts.

3. **Stopping services:**
    ```bash
    docker-compose down
    ```

## Scripts

- `npm start`: Start the server
- `npm run dev`: Start the server in development mode
- `npm test`: Run tests

## API Documentation

API documentation is available via Swagger at `/api-docs` after starting the server.

## Contributing

Contributions are welcome! Please open issues or submit pull requests for improvements.

## License

This project is licensed under the MIT License.
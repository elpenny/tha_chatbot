# ChatBot Application

A full-stack AI chatbot application built with Angular frontend and ASP.NET Core backend, featuring real-time streaming responses and conversation management.

## Features

### 🤖 Core Chat Functionality
- **Real-time streaming responses** - Messages appear character by character as they're generated
- **Conversation management** - Create, view, and switch between multiple conversations
- **Message cancellation** - Stop response generation mid-stream
- **Conversation history** - All messages are persisted and retrievable

### 👍 User Interaction
- **Message rating system** - Rate bot responses with thumbs up/down
- **Responsive design** - Works on desktop and mobile devices
- **Conversation sidebar** - Easy navigation between chat sessions
- **Auto-scroll** - Messages automatically scroll to keep latest visible

### 🛠 Technical Features
- **Clean architecture** - Separation of concerns with Domain/Application/Infrastructure layers
- **Real-time streaming** - Server-Sent Events (SSE) for live response streaming
- **Database persistence** - All conversations and ratings stored in SQL Server
- **Docker support** - Full containerization for easy deployment
- **API documentation** - Swagger/OpenAPI integration

## Architecture

- **Frontend**: Angular 20 with Angular Material UI components
- **Backend**: ASP.NET Core 9.0 Web API with Clean Architecture
- **Database**: SQL Server with Entity Framework Core
- **Communication**: RESTful API with Server-Sent Events for streaming

## Running the Application

### 🚀 Option 1: Docker (Recommended)

The easiest way to run the complete application:

```bash
# Clone the repository
git clone <repository-url>
cd tha_chatbot

# Run the full application stack
docker compose --profile fullApp up

# The application will be available at:
# - Frontend: http://localhost:4200
# - Backend API: http://localhost:8080
# - Swagger UI: http://localhost:8080/swagger
```

### 💻 Option 2: Local Development

#### Prerequisites
- Node.js 22+
- .NET 9.0 SDK  
- SQL Server (LocalDB or full instance)

#### Database Setup
```bash
cd ChatBotServer
dotnet ef database update --project ChatBotServer.API
```

#### Backend
```bash
cd ChatBotServer
dotnet build
dotnet run --project ChatBotServer.API
```
Backend will be available at: http://localhost:8080

#### Frontend
```bash
cd ChatBotClient
npm install
npm start
```
Frontend will be available at: http://localhost:4200

## API Documentation

Swagger UI is available at:
- **Local development**: http://localhost:8080/swagger
- **Docker**: http://localhost:8080/swagger

## Project Structure

```
├── ChatBotClient/              # Angular frontend
│   ├── src/app/
│   │   ├── components/         # UI components
│   │   ├── api/               # API services
│   │   └── models/            # TypeScript models
│   └── Dockerfile
├── ChatBotServer/             # .NET backend
│   ├── ChatBotServer.API/     # Web API layer
│   ├── ChatBotServer.Application/  # Business logic
│   ├── ChatBotServer.Domain/  # Domain models
│   ├── ChatBotServer.Infrastructure/  # Data access
│   └── Dockerfile
└── compose.yaml               # Docker Compose configuration
```

## Key Technologies

- **Frontend**: Angular 20, Angular Material, TypeScript, RxJS
- **Backend**: ASP.NET Core 9.0, Entity Framework Core, MediatR
- **Database**: SQL Server, Entity Framework migrations
- **Containerization**: Docker, Docker Compose
- **API**: RESTful services, Server-Sent Events, OpenAPI/Swagger

## Connecting to Azure AI Foundry

The application is designed to easily integrate with real AI models. To connect to Azure AI Foundry:

### 1. Azure AI Foundry Setup

1. **Create an Azure AI Foundry project** in the Azure portal
2. **Deploy a model** (e.g., GPT-4, GPT-3.5-turbo) in your project
3. **Get the endpoint URL and API key** from your deployment

### 2. Backend Configuration

1. **Add Azure AI configuration** to `appsettings.json`:
```json
{
  "AzureAI": {
    "Endpoint": "https://your-endpoint.openai.azure.com/",
    "ApiKey": "your-api-key",
    "DeploymentName": "your-deployment-name"
  }
}
```

### 3. Environment Variables (Recommended)

For production, use environment variables instead of storing keys in appsettings.json:

```bash
# Local development
export AzureAI__Endpoint="https://your-endpoint.openai.azure.com/"
export AzureAI__ApiKey="your-api-key"
export AzureAI__DeploymentName="your-deployment-name"
```

**Docker environment variables**:
```yaml
# In compose.yaml
environment:
  - AzureAI__Endpoint=https://your-endpoint.openai.azure.com/
  - AzureAI__ApiKey=your-api-key
  - AzureAI__DeploymentName=your-deployment-name
```

### 4. Features with Real AI

Once connected to Azure AI Foundry, the application will:
- Generate actual AI responses instead of Lorem Ipsum
- Maintain conversation context across messages
- Support the same streaming, rating, and cancellation features
- Store all real conversations in the database

## Development Notes

- The application uses a fake chat service by default that generates Lorem Ipsum responses
- Switch to `AzureAIChatBotService` to connect to real AI models
- Database migrations run automatically in Docker
- CORS is configured to allow frontend-backend communication
- All conversations and ratings are persisted to the database
- The architecture supports easy switching between fake and real AI services
# Chatbot Backend Infrastructure

This project contains the database infrastructure for the chatbot application, including:

- Entity models
- DbContext
- Repository interfaces and implementations
- Database migrations

## Entity Framework Core Setup

The project is configured to use SQL Server through Entity Framework Core. 

## Creating Migrations

To create a new migration, run the following command from the solution directory:

```
dotnet ef migrations add [MigrationName] --project chatbot_backend.Infrastructure --startup-project chatbot_backend.API
```

## Applying Migrations

To apply migrations to the database, run:

```
dotnet ef database update --project chatbot_backend.Infrastructure --startup-project chatbot_backend.API
```

## Models

The infrastructure includes the following entity models:

- `ChatConversation`: Represents a chat conversation session
- `ChatMessage`: Represents individual messages in a conversation

## Repositories

The repository pattern is implemented with both generic and specific repositories:

- `IRepository<T>`: Generic repository interface
- `IChatConversationRepository`: For conversation-specific operations
- `IChatMessageRepository`: For message-specific operations

## Configuration

Database connection string is configured in the API project's `appsettings.json` file.

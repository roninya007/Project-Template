# Project Configuration

## Stack
- Frontend: React 18 + TypeScript (in /src/client)
- Backend: .NET 8 Web API REST (in /src/api)
- Testing: xUnit for .NET, Jest + React Testing Library for React
- CI/CD: GitHub Actions

## Agent Roles
- **Frontend Agent**: Works only in /src/client. React components, pages, hooks.
- **Backend Agent**: Works only in /src/api. .NET controllers, services, REST endpoints.
- **Unit Test Agent**: Writes tests for any code produced by other agents.
- **CI/CD Agent**: Manages .github/workflows. Build, test, deploy pipelines.

## Conventions
- REST APIs use versioned routes: /api/v1/resource
- All .NET services must have XML doc comments for Swagger
- React components use functional style with hooks only
- Use /clear between agent role switches to conserve token budget (Pro plan)
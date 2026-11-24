# LevelUp

Gamificeret planlægningsplatform – svendeprøveprojekt.

## Struktur

```
backend/   ASP.NET Core Web API (.NET 10) – LevelUp.slnx
frontend/  React + TypeScript (Vite)
```

## Kom i gang

Krav: .NET 10 SDK og Node.js 22+.

### Backend

```bash
dotnet run --project backend/LevelUp.Api --launch-profile http
```

API'et kører på http://localhost:5105. Tjek: http://localhost:5105/api/health
OpenAPI-dokumentet ligger på http://localhost:5105/openapi/v1.json (kun i Development).

### Frontend

```bash
cd frontend
npm install
npm run dev
```

Frontenden kører på http://localhost:5173 og kalder API'et via `VITE_API_URL` i `frontend/.env.development`.

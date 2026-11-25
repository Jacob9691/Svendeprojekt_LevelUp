# LevelUp

Gamificeret planlægningsplatform – svendeprøveprojekt.

## Struktur

```
backend/             ASP.NET Core Web API (.NET 10) – LevelUp.slnx
frontend/            React + TypeScript (Vite)
docker-compose.yml   SQL Server 2022 til udvikling
```

## Kom i gang

Krav: .NET 10 SDK, Node.js 22+ og Docker Desktop.

### Database (SQL Server i Docker)

1. Kopiér `.env.example` til `.env` og vælg en stærk adgangskode til `MSSQL_SA_PASSWORD`.
2. Start databasen:

   ```bash
   docker compose up -d
   ```

   SQL Server kører på `localhost,14330` (port 14330, så den ikke konflikter med en lokal SQL Server på 1433).
   Data gemmes i Docker-volumen `levelup-sqldata` og overlever genstart.

3. Gem connection string i user-secrets (samme adgangskode som i `.env`). Den ligger uden for repoet:

   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,14330;Database=LevelUp;User Id=sa;Password=<din-adgangskode>;TrustServerCertificate=True" --project backend/LevelUp.Api
   ```

Når API'et starter i Development, kører det automatisk nye migrations og opretter demo-data.

### Backend

```bash
dotnet run --project backend/LevelUp.Api --launch-profile http
```

API'et kører på http://localhost:5105. Tjek: http://localhost:5105/api/health

Swagger UI ligger på **http://localhost:5105/swagger**, hvor alle endpoints kan ses og afprøves.
Det rå OpenAPI-dokument ligger på http://localhost:5105/openapi/v1.json. Begge findes kun i Development.

### Frontend

```bash
cd frontend
npm install
npm run dev
```

Frontenden kører på http://localhost:5173 og kalder API'et via `VITE_API_URL` i `frontend/.env.development`.

## API-endpoints

Alle endpoints kan afprøves i Swagger UI eller med [LevelUp.Api.http](backend/LevelUp.Api/LevelUp.Api.http).

| Metode | Endpoint | Beskrivelse |
| --- | --- | --- |
| GET | `/api/tasks?from=&to=` | Brugerens opgaver, evt. for én uge |
| GET | `/api/tasks/{id}` | Én opgave |
| POST | `/api/tasks` | Opret opgave |
| PUT | `/api/tasks/{id}` | Rediger opgave |
| DELETE | `/api/tasks/{id}` | Slet opgave |
| POST | `/api/tasks/{id}/complete` | Gennemfør opgave – giver XP og points |
| GET | `/api/shop/items` | Genstande i shoppen |
| POST | `/api/shop/items/{id}/purchase` | Køb genstand |
| GET | `/api/inventory` | Brugerens inventar |
| POST | `/api/inventory/{id}/equip` | Udstyr genstand |
| POST | `/api/inventory/{id}/unequip` | Tag genstand af |
| GET / PUT | `/api/profile` | Profil, level, XP og points / skift visningsnavn |
| GET | `/api/profile/transactions` | Historik over points |
| GET / PUT | `/api/profile/settings` | Indstillinger for påmindelser |

Fejl returneres som ProblemDetails: `400` ved valideringsfejl, `404` hvis ressourcen ikke findes,
og `409` hvis en regel brydes (f.eks. for få points eller opgaven allerede er belønnet).

> **Midlertidigt:** Alle requests behandles som demo-brugeren (`DevCurrentUserService`).
> Det udskiftes, når login og JWT sættes op.

## Database og migrations

Tabellerne er defineret med Entity Framework Core i `backend/LevelUp.Api/Data/AppDbContext.cs` og
navngives i snake_case som i ERD'en (`app_user`, `total_xp` osv.). Migrations ligger i `Data/Migrations`.

`dotnet-ef` er installeret som lokalt værktøj. Hent det første gang med:

```bash
dotnet tool restore
```

Ny migration efter ændringer i modellerne:

```bash
dotnet ef migrations add <Navn> --project backend/LevelUp.Api --output-dir Data/Migrations
```

Anvend migrations manuelt (sker også automatisk ved opstart i Development):

```bash
dotnet ef database update --project backend/LevelUp.Api
```

Nulstil databasen helt (sletter alle data):

```bash
docker compose down -v
```

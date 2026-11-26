# LevelUp

Gamificeret planlægningsplatform – svendeprøveprojekt.

## Struktur

```
backend/             ASP.NET Core Web API (.NET 10) – LevelUp.slnx
frontend/            React + TypeScript (Vite)
docker-compose.yml   SQL Server 2022 og Mailpit (test-mailserver) til udvikling
```

## Kom i gang

Krav: .NET 10 SDK, Node.js 22+ og Docker Desktop.

### Database og mail (Docker)

1. Kopiér `.env.example` til `.env` og vælg en stærk adgangskode til `MSSQL_SA_PASSWORD`.
2. Start SQL Server og Mailpit:

   ```bash
   docker compose up -d
   ```

   SQL Server kører på `localhost,14330` (port 14330, så den ikke konflikter med en lokal SQL Server på 1433).
   Data gemmes i Docker-volumen `levelup-sqldata` og overlever genstart.

   Mailpit fanger alle e-mails API'et sender (bekræftelses- og loginkoder). Læs dem på **http://localhost:8025**.

3. Gem hemmelighederne i user-secrets. De ligger uden for repoet:

   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,14330;Database=LevelUp;User Id=sa;Password=<adgangskode-fra-.env>;TrustServerCertificate=True" --project backend/LevelUp.Api
   ```

   ```bash
   dotnet user-secrets set "Jwt:SigningKey" "<tilfældig tekst på mindst 32 tegn>" --project backend/LevelUp.Api
   ```

   Valgfrit – opretter en bekræftet demo-bruger (`demo@levelup.local`) med denne adgangskode:

   ```bash
   dotnet user-secrets set "DemoUser:Password" "<adgangskode>" --project backend/LevelUp.Api
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

Login i frontenden (`frontend/src/auth`):

- `AuthProvider` gemmer access token og bruger i `localStorage`, så man forbliver logget ind ved genindlæsning,
  og logger automatisk ud, når tokenet udløber.
- axios sender tokenet med på alle kald (`api/client.ts`). Svarer API'et `401` på et kald med token, logges brugeren ud.
- `RequireAuth` sender brugere uden login til `/login` og tilbage til den side, de prøvede at åbne, efter login.
- Sider: `/opret` → `/bekraeft` (kode) → `/login` (adgangskode, derefter engangskode).

## Login og sikkerhed

Login sker i to trin med ASP.NET Core Identity og JWT:

1. **Registrering** – `POST /api/auth/register` opretter kontoen og sender en 6-cifret kode via e-mail eller SMS.
   Kontoen bekræftes med `POST /api/auth/confirm`. Man kan ikke logge ind før kontoen er bekræftet.
2. **Login trin 1** – `POST /api/auth/login` tjekker e-mail og adgangskode og sender en engangskode
   via brugerens valgte 2FA-metode. Svaret indeholder et kortlivet `twoFactorToken` (10 min).
3. **Login trin 2** – `POST /api/auth/login/2fa` med `twoFactorToken` og koden returnerer et `accessToken` (JWT, 60 min).
4. Alle andre endpoints kræver headeren `Authorization: Bearer <accessToken>`. I Swagger bruges knappen **Authorize**.

Beskyttelse:

- Adgangskoder hashes af Identity (PBKDF2). Krav: mindst 8 tegn med stort og lille bogstav og et tal.
- Efter 5 forkerte forsøg (adgangskode eller kode) låses kontoen i 5 minutter.
- `/api/auth/*` er begrænset til 10 kald pr. minut pr. IP-adresse (60 i Development).
- Hver service filtrerer på bruger-id'et fra tokenet, så brugere kun kan se og ændre egne data.
- SMS sendes endnu ikke rigtigt – koden skrives i API'ets log (`LoggingSmsSender`).

## API-endpoints

Alle endpoints kan afprøves i Swagger UI eller med [LevelUp.Api.http](backend/LevelUp.Api/LevelUp.Api.http).

| Metode | Endpoint | Beskrivelse |
| --- | --- | --- |
| POST | `/api/auth/register` | Opret konto – sender bekræftelseskode |
| POST | `/api/auth/confirm` | Bekræft konto med kode |
| POST | `/api/auth/confirm/resend` | Send ny bekræftelseskode |
| POST | `/api/auth/login` | Login trin 1 – sender engangskode |
| POST | `/api/auth/login/2fa` | Login trin 2 – returnerer JWT |
| POST | `/api/auth/login/2fa/resend` | Send ny engangskode |
| GET | `/api/tasks?from=&to=` | Brugerens opgaver, evt. for én uge |
| GET | `/api/tasks/{id}` | Én opgave |
| POST | `/api/tasks` | Opret opgave – med `repeat` og `repeatUntil` oprettes en gentagende serie |
| PUT | `/api/tasks/{id}` | Rediger opgave |
| DELETE | `/api/tasks/{id}?following=` | Slet opgave – med `following=true` også senere opgaver i serien |
| POST | `/api/tasks/{id}/complete` | Gennemfør opgave – giver XP og points |
| GET | `/api/achievements` | Alle achievements med fremskridt (låser nye op, hvis de er nået) |
| GET | `/api/challenges` | Udfordringer og brugerens status |
| POST | `/api/challenges/{id}/start` | Start udfordring – lægger opgaverne i kalenderen |
| DELETE | `/api/challenges/{id}` | Opgiv udfordring – fjerner ikke-gennemførte opgaver |
| GET | `/api/shop/items` | Genstande i shoppen |
| POST | `/api/shop/items/{id}/purchase` | Køb genstand |
| GET | `/api/inventory` | Brugerens inventar |
| POST | `/api/inventory/{id}/equip` | Udstyr genstand |
| POST | `/api/inventory/{id}/unequip` | Tag genstand af |
| GET / PUT | `/api/profile` | Profil, level, XP og points / skift visningsnavn |
| GET | `/api/profile/transactions` | Historik over points |
| GET / PUT | `/api/profile/settings` | Påmindelser (kanal og minutter før), tidszone og 2FA-metode |
| POST | `/api/profile/phone` | Send SMS-kode til nyt telefonnummer |
| POST | `/api/profile/phone/confirm` | Bekræft og gem telefonnummer |

Fejl returneres som ProblemDetails: `400` ved valideringsfejl, `401` ved forkert login eller manglende token,
`403` hvis kontoen er låst eller ikke bekræftet, `404` hvis ressourcen ikke findes, `409` hvis en regel brydes
(f.eks. for få points), og `429` ved for mange kald. Login-fejl har et `code`-felt
(`accountNotConfirmed`, `lockedOut`, `invalidCode`, `twoFactorTokenExpired`), som frontenden kan reagere på.

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

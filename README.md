
```md
# Payroll Approval Management System

Et backend-system for håndtering av lønnsprosesser, inkludert generering av lønn, godkjenning og opprettelse av lønnsslipper (PDF).

Prosjektet er utviklet med ASP.NET Core 8, PostgreSQL, Entity Framework Core og Docker, og følger prinsippene for lagdelt arkitektur.

---

## Oversikt

Formålet med prosjektet er å utvikle en backend-løsning for håndtering av lønnsprosesser:

Login → Generer lønn → Godkjenn lønn → Generer lønnsslipp → PDF

Systemet inkluderer:

- JWT-basert autentisering  
- Rollebasert tilgang til API  
- Håndtering av ansatte og avdelinger  
- Generering av lønn  
- Godkjenningsflyt  
- Generering av lønnsslipp som PDF  
- PostgreSQL database med EF Core  
- Docker-basert oppsett  
- Automatisk seed-data for testing og demo  

---

## Arkitektur

Løsningen er bygget etter prinsippene for Clean Architecture:

```

API
Application
Domain
Infrastructure

````

| Lag | Ansvar |
|---|---|
| Domain | Entiteter, enums og forretningsregler |
| Application | Forretningslogikk og tjenester |
| Infrastructure | Database, EF Core og repositories |
| API | Controllere, DTO-er, autentisering og Swagger |

Strukturen sørger for at forretningslogikk er adskilt fra både API og database.

---

## Teknologistack

| Område | Teknologi |
|---|---|
| Backend | ASP.NET Core 8 |
| Språk | C# |
| Database | PostgreSQL |
| ORM | Entity Framework Core |
| Autentisering | JWT Bearer |
| Dokumentasjon | Swagger / OpenAPI |
| PDF-generering | QuestPDF |
| Logging | Serilog |
| Container | Docker + Docker Compose |
| Testing | xUnit |

---

## Domene

Systemet består av følgende hovedentiteter:

- Department (Avdeling)  
- Employee (Ansatt)  
- PayrollStructure (Lønnsstruktur)  
- Payroll (Lønn)  
- Approval (Godkjenning)  
- Payslip (Lønnsslipp)  


---

## Forretningsregler

Systemet håndhever følgende regler:

- En ansatt kan kun ha én aktiv lønnsstruktur  
- Det kan ikke opprettes flere lønninger for samme ansatt samme måned  
- Lønn kan ikke endres etter godkjenning  
- Lønnsslipp kan kun genereres etter at lønn er godkjent  

---

## Starte prosjektet (Docker)

Kjør fra rotmappen:

```bash
docker compose down -v
docker compose up --build
````

Åpne Swagger:

```
http://localhost:8080/swagger
```

`docker compose down -v` brukes for å resette databasen før testing eller demo.

---

## Seed-data (Development)

Ved oppstart i Development-modus legges følgende demo-data automatisk inn:

* 1 avdeling: Engineering
* 1 ansatt: Demo Employee
* 1 lønnsstruktur:

    * Grunnlønn: 50000
    * Bonus: 5000
    * Trekk: 1500

Dette gjør at systemet kan testes uten manuell opprettelse av data.

---

## Demo

Den enkleste måten å demonstrere systemet på er via `demo.html`.

### Fremgangsmåte:

1. Start systemet med Docker
2. Åpne `demo.html`
3. Klikk gjennom:

* Login
* Load Employee
* Generate Payroll
* Approve Payroll
* Open Payslip PDF

### Flyt:

```
Login → Employee → Payroll → Approval → PDF
```

---

## Autentisering

API-et bruker JWT (JSON Web Token) for autentisering.

Beskyttede endepunkter krever gyldig token.

---

## Prosjektstruktur

```
src/
  Domain
  Application
  Infrastructure
  Api
```

---

## Testing

Kjør tester:

```bash
dotnet test
```

---

## Utfordringer og læring

Under utviklingen møtte vi flere utfordringer:

* Oppsett av PostgreSQL med Docker
* Håndtering av connection strings mellom miljøer
* JWT-konfigurasjon og autentisering
* CORS-problemer ved bruk av nettleserbasert demo
* Kobling mellom API og database via repositories
* Håndtering av duplikat lønn per måned
* Nedlasting av PDF fra beskyttede endepunkter
* Testing krevde jevnlig reset av database for å unngå duplikatdata under demo
* Samarbeid via Git og pull requests

Disse utfordringene ga bedre forståelse for backend-utvikling, API-design og feilsøking.

---

## KI-bruk

KI-verktøy ble brukt som støtte i prosjektet, blant annet til:

* Feilsøking og debugging
* Diskusjon rundt arkitektur
* Strukturering av kode og dokumentasjon

All kode er gjennomgått, tilpasset og testet av gruppen. Løsningen reflekterer gruppens egne valg og forståelse.

---

## Videre arbeid

Mulige forbedringer:

* Utvide testdekning (spesielt integrasjonstester)
* Lage en full frontend-løsning
* Håndtere eksisterende lønn bedre (oppdatering/regenerering)
* Utvide rollebasert tilgang
* Lage rapporter og historikk for lønn

---

## Team

| Område                    | Ansvar                        |
| ------------------------- | ----------------------------- |
| Forretningslogikk         | Domene og tjenester           |
| API og sikkerhet          | Controllere, JWT og Swagger   |
| Database og infrastruktur | PostgreSQL, EF Core og Docker |

---

## Konklusjon

Prosjektet viser hvordan en backend-løsning for lønnshåndtering kan bygges med fokus på struktur, sikkerhet og forretningsregler.

Systemet inkluderer:

* Lagdelt arkitektur
* Sikker API med autentisering
* Databaseintegrasjon
* Håndheving av forretningsregler
* PDF-generering

Løsningen kan kjøres med Docker og testes via Swagger eller en enkel demo-side.



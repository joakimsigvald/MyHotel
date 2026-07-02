# MyHotel

Booking system for small/local hotels. .NET 10 backend with event sourcing (Marten + PostgreSQL), React frontend.

## Architecture

```
frontend/     React + Vite + TypeScript (dev server proxies /api → backend)
src/Contract/ Thin DTOs, service interfaces and results — no dependencies
src/Core/     Events, aggregates and business logic; implements Contract,
              defines repository ports that Infra implements
src/Entry/    Minimal API endpoints: HTTP in, Contract services, HTTP out
src/Infra/    Marten: repositories and projections, implements Core's ports
src/Host/     Startup, dependency injection, Marten wiring, dev seed data
```

**Event sourcing / CQRS:** All state changes are stored as events (`RoomRegistered`,
`ReservationMade`, `ReservationCancelled`) in Marten's event store. Read models
(`Room`, `Reservation`) are Marten *inline projections*: they are updated in the same
PostgreSQL transaction as the event append, so a command that returns success is
immediately visible to queries — no eventual consistency for the UI to handle, no
WebSockets needed.

**API:**

| Method | Route | Purpose |
|---|---|---|
| GET | `/api/rooms` | List rooms |
| POST | `/api/rooms` | Register a room |
| GET | `/api/reservations` | List reservations |
| POST | `/api/reservations` | Make a reservation (validates overlap) |
| POST | `/api/reservations/{id}/cancel` | Cancel a reservation |
| GET | `/api/reservations/{id}/history` | Full event stream of one reservation |

## Prerequisites

- .NET 10 SDK
- Node.js 20+
- PostgreSQL 15+ — either locally, or via Docker: `docker compose up -d`
  (expects database `myhotel`, user/password `myhotel`/`myhotel` on port 5432;
  see `src/Host/appsettings.Development.json`)

## Run (development)

Terminal 1 — backend on http://localhost:5000 (Marten creates its schema automatically,
and four rooms are seeded on first start):

```
dotnet run --project src/Host
```

Terminal 2 — frontend on http://localhost:5173:

```
cd frontend
npm run dev
```

## Notes

- Core stays free of the Marten package because event→aggregate mapping is done by
  explicit projection classes in Infra (`RoomProjection`, `ReservationProjection`)
  that delegate to the aggregates. Marten 9 dispatches conventional `Apply`/`Create`
  methods via a compile-time source generator, so these projection classes must be
  declared `partial` and live in an assembly referencing the Marten NuGet package —
  otherwise the API fails at startup with `InvalidProjectionException:
  No source-generated dispatcher found`.

## Known simplifications (to revisit)

- **Double-booking race:** the overlap check and the event append are not guarded
  against concurrent writers booking the same room simultaneously. Fine for a demo;
  real fix is a transactional guard (e.g. serializable isolation or an exclusion
  constraint on the projection table).
- No authentication, no multi-hotel tenancy, prices are plain decimals with a
  hardcoded € in the UI.

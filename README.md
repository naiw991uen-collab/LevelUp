<div align="center">

<img src="LevelUp.PL/wwwroot/images/logo.svg" alt="LEVEL UP" width="120">

# LEVEL UP — PlayStation Lounge Management System

**An offline cashier app for PlayStation lounges that runs straight from a USB flash drive.**

Plug in the drive, double-click `LevelUp.exe`, and the browser opens on the login page. Nothing to install.

**Live demo:** [levelup-ps.runasp.net](http://levelup-ps.runasp.net)

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4)
![EF Core](https://img.shields.io/badge/EF%20Core-9-6DB33F)
![SQLite](https://img.shields.io/badge/SQLite-offline-003B57?logo=sqlite&logoColor=white)
![xUnit](https://img.shields.io/badge/tests-xUnit-5E2750)
![UI](https://img.shields.io/badge/UI-Arabic%20RTL-orange)

</div>

---

## ✨ Features

| Area | What it does |
|---|---|
| 🎮 **Devices board** | Live card for every room: start, switch Single ↔ Multi, pause, resume, close and pay. Timers tick in the browser. |
| 🧾 **Itemized bill** | Each mode segment is shown with its time, rate and cost, plus the drinks and the grand total. |
| 🥤 **Drinks** | Add or remove drinks before or during a session. The price is snapshotted when the drink is added. |
| 📜 **Sessions log** | Finished sessions with filters by business day, mode (Single / Multi / Mixed) and room. |
| 📊 **Dashboards** | Separate Single and Multi dashboards: today's income, play hours, session counts, rooms running now, and a per-room breakdown. |
| 💰 **Rate history** | Changing the hourly rate affects new sessions only. Running segments keep their original rate. |
| 🏠 **Rooms & drinks admin** | Add, rename and enable or disable rooms; add drinks, change prices and mark them unavailable. Nothing is hard-deleted. |
| 🔐 **Login** | ASP.NET Core Identity, sign in by user name. |

## 📐 Business rules

- **Billing per started minute, per segment:** `round(ceil(seconds / 60) × hourlyRate / 60)`.
- **Rate snapshot:** each segment stores the rate that was in effect when it started.
- **Switch mode** closes the current segment and opens a new one at the current rate of the new mode.
- **Pause** closes the open segment and shows the bill. Paused time is **not** billed.
- **Close (paid)** ends the session and frees the room.
- **The business day starts at 06:00**, so a session at 02:00 belongs to the previous day.
- **Drinks revenue counts toward the mode the session started with.**
- A room with an open session can't be disabled. A disabled room can't start a session. An unavailable drink can't be added.

## 🏗️ Architecture

A classic 3-layer solution. Each layer depends only on the one below it.

```
LevelUp.PL.slnx
├── LevelUp.DAL     Entities, Enums, LevelUpDbContext (Identity), Fluent API configurations,
│                   Migrations, Generic + specific repositories, Unit of Work, Data seeding
├── LevelUp.BLL     Services per feature (Classes / Interfaces / ViewModel / Validators / Mapping),
│                   Result & Result<T>, BillingCalculator
├── LevelUp.PL      MVC Controllers, Razor Views (Arabic RTL), wwwroot, Serilog, startup
└── LevelUp.Tests   xUnit tests for the billing calculation
```

**Patterns and conventions**

- Every service returns `Result` / `Result<T>` instead of throwing for business errors.
- Thin controllers: call the service, set a TempData message, redirect (Post/Redirect/Get).
- Services and repositories are registered by naming convention with **Scrutor**.
- One **AutoMapper** profile and one **FluentValidation** validator per feature, with Arabic error messages.
- **Serilog** levels: Information for success, Warning for a rejected business rule, Error for exceptions.
- All actions are plain HTML forms. JavaScript is used only for the live timers.

## 🧰 Tech stack

| | |
|---|---|
| Runtime | .NET 9, ASP.NET Core MVC |
| Data | EF Core 9 + SQLite, ASP.NET Core Identity |
| Validation & mapping | FluentValidation 12, AutoMapper 16 |
| DI | Scrutor 7 |
| Logging | Serilog |
| Tests | xUnit |

## 🗂️ Domain model

| Entity | Key fields |
|---|---|
| `Room` | Name, DefaultMode, IsActive |
| `Drink` | Name, Price, IsAvailable |
| `ModeRate` | Mode, HourlyRate, EffectiveFrom |
| `GameSession` | RoomId, StartedAt, EndedAt, StartMode, Status, BusinessDay |
| `SessionSegment` | GameSessionId, Mode, HourlyRate (snapshot), StartedAt, EndedAt |
| `SessionDrink` | GameSessionId, RoomId, DrinkId, Quantity, UnitPrice (snapshot) |

Enums: `GameMode { Single, Multi }`, `SessionStatus { Running, Stopped, Closed }`.

## 🚀 Getting started

**Requirements:** .NET 9 SDK, Windows.

```bash
git clone https://github.com/naiw991uen-collab/LevelUp.git
cd LevelUp
```

Create `LevelUp.PL/appsettings.Local.json` (it is git-ignored) with the admin password.
It needs at least 6 characters, with an uppercase letter, a lowercase letter, a digit and a symbol:

```json
{
  "AdminSeed": {
    "Password": "<your-admin-password>"
  }
}
```

```bash
dotnet run --project LevelUp.PL
```

The app runs at `http://localhost:5080` and opens the browser automatically.
On first start it applies the migrations and seeds:

- the default hourly rates for Single and Multi
- one admin user with user name `admin` and the password from `appsettings.Local.json`

### Run the tests

```bash
dotnet test
```

## 💾 Run from a USB flash drive

Build one self-contained executable (the target PC doesn't need .NET):

```bash
dotnet publish LevelUp.PL -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
```

1. Copy the `publish` folder to the flash drive.
2. Double-click `LevelUp.exe`. A console window opens and the browser opens on the login page.
3. `levelup.db` and the `Logs` folder are created **next to the exe**, on the flash drive.
4. Closing the console window stops the app.

> ⚠️ **Back up `levelup.db` every day.** When you update the app, replace the program files only and never overwrite the drive's `levelup.db`.

## 📁 Project structure

```
LevelUp.PL/
├── Controllers/   Account, Device, Session, Dashboard, Drink, Room
├── Views/         Razor views + shared _Layout (header, sidebar, theme)
├── Filters/       RunningCountFilter (running-sessions badge in the sidebar)
├── Extentions/    Migrate + seed, Serilog, open browser on start
└── wwwroot/       css, js (timer.js), fonts, images
```

## 📄 License

This project was built for a private PlayStation lounge. All rights reserved.

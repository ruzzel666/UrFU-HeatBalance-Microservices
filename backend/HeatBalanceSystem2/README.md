# HeatBalanceSystem (учебный проект)

Многопользовательская автоматизированная информационная система (веб‑приложение ASP.NET Core) для расчёта теплового баланса:

- конвейерной сушильной печи
- камерной сушильной печи
- электрической сушильной печи
- сушильного барабана

## Состав решения

- `ProductWebApp` — веб‑приложение (Razor Pages) + Identity + EF Core  
  - по умолчанию **SQLite (локальный файл)** для простого запуска  
  - PostgreSQL поддерживается (переключение настройкой)
- `HeatBalance.Math` — математическая библиотека (модели входных данных + расчёт)
- `HeatBalance.Math.Tests` — регрессионные тесты математической библиотеки (xUnit)

## Требования

- Visual Studio 2022
- .NET SDK 9.0
- PostgreSQL 14+ (опционально, если хотите хранить данные в PostgreSQL)

## Настройка

1) Откройте решение `HeatBalanceSystem.sln`.

2) По умолчанию приложение использует **SQLite** и создаёт файл БД:

- `ProductWebApp/App_Data/heat_balance.db`

Если хотите **PostgreSQL**:

- в `ProductWebApp/appsettings.json` поставьте `Database:Provider = "Postgres"`
- задайте `ConnectionStrings:Postgres` (или `ConnectionStrings:DefaultConnection`)

3) (Опционально) Настройте сидирование администратора:

- `Seed:AdminEmail`
- `Seed:AdminPassword`

## Миграции БД

Миграции нужны **только для PostgreSQL**. В проекте используется **локальный** инструмент `dotnet-ef` (см. `.config/dotnet-tools.json`).

Команды (из корня решения):

```bash
dotnet tool restore
dotnet tool run dotnet-ef database update --project "ProductWebApp\ProductWebApp.csproj" --startup-project "ProductWebApp\ProductWebApp.csproj"
```

## Запуск

```bash
.\run-dev.ps1
```

После запуска:

- зарегистрируйте пользователя
- создайте набор исходных данных в `/Cabinet`
- выполните расчёт и экспортируйте отчёт в PDF

## Документация

- `docs/IDEF0.md` — функциональная модель (IDEF0)
- `docs/UserGuide.md` — руководство пользователя
- `docs/AdminGuide.md` — руководство администратора


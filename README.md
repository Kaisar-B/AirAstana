# ✈️ AirAstana Flight Status API

Test Web API для управления статусами авиарейсов с **JWT авторизацией**, **кэшированием** и **логированием**.
---
## 🔗 ✅ Выполненные требования ТЗ

- ✅ **.NET 6, EF Core Code First**
- ✅ **SOLID, KISS, DRY принципы**
- ✅ **MediatR + CQRS паттерны**
- ✅ **FluentValidation**
- ✅ **Serilog**
- ✅ **Redis**
- ✅ **JWT авторизация**
- ✅ **Swagger документация**
- ✅ **DDD архитектура (4 слоя)**
- ✅ **Role-based доступ**
- ✅ **Фильтрация + сортировка**
- ✅ **Кэширование (чтение из кэша, запись в БД)**
- ✅ **Unit + Integration тесты**
  
---

## 🚀 Технологии

- **.NET 8 | ASP.NET Core Web API**
- **Entity Framework Core 8** (Code First, PostgreSQL)
- **MediatR** (CQRS паттерн)
- **FluentValidation** (валидация)
- **Serilog** (логирование)
- **Redis** (кэширование)
- **JWT Bearer** (авторизация)
- **Swagger / OpenAPI** (документация)
- **Unit & Integration Tests** (тестирование)



### How it will look on GitHub

**🔐 Авторизация**

POST `/api/auth/login`

Body:
```json
{
  "username": "admin",
  "password": "admin123"
}

---

## 🏗 Архитектура — Clean Architecture (DDD)

Проект построен на основе принципов **Clean Architecture** и **Domain Driven Design (DDD)**.

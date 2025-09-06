# Keel

**Keel** is a lightweight set of **.NET 8** building blocks for enterprise apps.  
It gives you a clean Kernel and minimal Web glue so you don’t keep rewriting
the same primitives on every project.

- Domain primitives: `Result<T>`, `Error`, `Maybe<T>`, `Guard`
- DDD skeleton: `ValueObject`, `Entity`, `AggregateRoot`, Domain Events (contracts)
- Querying: `Specification<T>`, paging (`PageRequest/Response`), `MultiSortOptions`,
  `RangeFilter<T>`, advanced `FilterOptions` (Equals / Contains / Between) with safe mapping
- Persistence contracts: `IRepository<T,TKey>`, `IUnitOfWork`
- Exceptions: domain & infrastructure (`DomainException`, `NotFoundException`,
  `ConcurrencyException`, `ValidationException`, `UnauthorizedException`, …)
- Web adapters: global exception middleware → **RFC 7807 ProblemDetails**
  (enriched with `traceId` & `correlationId`), `Result → ActionResult/IResult` mappers
- Operation Context: single injectable abstraction for `IClock`, current user,
  correlation id and tenant id (with ASP.NET Core middleware + claims/headers adapters)
- Outbox **abstractions** (contracts only)
- Unit tests covering the critical paths

> The goal is **reuse with minimal coupling**: keep the Kernel provider-agnostic and
let Infrastructure wire it to EF, buses, etc.

---

## Solution layout

src/
Keel.Kernel/ # Provider-agnostic primitives & contracts
Keel.Web/ # ASP.NET Core adapters (middleware, mappers, context)
tests/
Keel.UnitTests/ # xUnit tests for Kernel + Web glue

---

## Requirements

- .NET SDK **8.0**
- ASP.NET Core (for `Keel.Web` consumers)

---

## Getting started

### 1) Add project references

Reference `Keel.Kernel` and (if you build Web APIs) `Keel.Web` from your application:

```xml
<ItemGroup>
  <ProjectReference Include="..\src\Keel.Kernel\Keel.Kernel.csproj" />
  <ProjectReference Include="..\src\Keel.Web\Keel.Web.csproj" />
</ItemGroup>
```

### 2) Wire up Web middleware (ProblemDetails + OperationContext)

```xml
// Program.cs
using Keel.Web.Extensions;
using Keel.Kernel.Abstractions.Time;

// Provide a clock (simple system clock)
builder.Services.AddSingleton<IClock, SystemClock>();

// ProblemDetails + exception handling
builder.Services.AddKeelProblemDetails();

// Operation context (Guid user/tenant ids in this example)
builder.Services.AddKeelOperationContext<Guid, Guid>(opts =>
{
    opts.ParseUserId   = s => Guid.TryParse(s, out var g) ? g : null;
    opts.ParseTenantId = s => Guid.TryParse(s, out var g) ? g : null;
});

var app = builder.Build();

app.UseKeelExceptionHandling();           // global exception → ProblemDetails (with trace/correlation)
app.UseKeelOperationContext<Guid, Guid>(); // correlation + current user + tenant

app.MapControllers();
app.Run();
```

### 3) Return Result from handlers

```xml
// Application service
public async Task<Result<EmployeeDto>> GetByIdAsync(Guid id, CancellationToken ct)
{
    var employee = await _repo.FindAsync(id, ct);
    if (employee is null) return Result<EmployeeDto>.Failure("Employees.NotFound", "Employee not found.");
    return Result<EmployeeDto>.Success(EmployeeDto.From(employee));
}

// Controller (MVC)
[HttpGet("{id:guid}")]
public IActionResult Get(Guid id) => _service.GetByIdAsync(id).Result.ToActionResult(HttpContext);

// Minimal API
app.MapGet("/employees/{id:guid}", async (Guid id, IEmployeeService svc) =>
{
    var result = await svc.GetByIdAsync(id);
    return result.ToIResult();
});
```

### 4) Build safe, portable queries with Specifications

```xml
using Keel.Kernel.Core.Querying;

public sealed record EmployeeListRequest(
    FilterOptions Filters,
    MultiSortOptions Sort,
    PageRequest Page);

public sealed class EmployeesSpec : Specification<Employee>
{
    public EmployeesSpec(EmployeeListRequest req)
    {
        // Filters (whitelisted)
        this.ApplyFilters(req.Filters, map => map
            .ForContains("firstName",  e => e.FirstName)
            .ForContains("lastName",   e => e.LastName)
            .ForEquals("departmentId", e => e.DepartmentId, FilterParsers.TryGuid)
            .ForBetween("createdOn",   e => e.CreatedOnUtc, FilterParsers.TryDateTimeOffset)
        );

        // Sorting (whitelisted)
        var sortMap = new Dictionary<string, Expression<Func<Employee, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["firstName"] = e => e.FirstName,
            ["lastName"]  = e => e.LastName,
            ["createdOn"] = e => e.CreatedOnUtc
        };
        this.ApplySorts(req.Sort, sortMap, defaultOrder: new[] { (Key: (Expression<Func<Employee, object>>)(e => e.LastName), Desc: false) });

        // Paging
        SetPaging(req.Page.Skip, req.Page.Take);
    }
}
```
---

## Highlights

- Standard errors everywhere
  Throw domain exceptions where it makes sense; otherwise return Result. The web layer
  maps both to ProblemDetails consistently.

- Provider-agnostic queries
  Specification<T> + FilterOptions/MultiSortOptions/RangeFilter<T> build pure expression trees.
  Your repository is free to apply them with EF, Dapper, or custom stores.

- Single context for cross-cutting concerns
  IOperationContext<TUserKey,TTenantId> exposes IClock, current user, correlation id, and tenant id.
  The ASP.NET Core adapter populates it from claims/headers.

- Contracts, not implementations
  Kernel ships interfaces for Repository/UnitOfWork and Outbox. Implement them in your Infra layer.

  ---

## Running tests

```xml
dotnet test
```
Tests cover paging, advanced filters/sorts, specification composition, middleware behavior,
ProblemDetails mapping, operation context setup, and validation flow.

---

## Roadmap / ideas

- Optional EF Core adapters (value converters, spec evaluator)
- Outbox store sample (SQL) with leasing/backoff
- Packaging as NuGet (Kernel/Web)
- Optional strongly-typed IDs (Guid/long) adapters (you can skip if you prefer simplicity)

---

## Contributing

Issues and PRs are welcome. Please keep PRs focused and covered by tests.
Coding style follows standard .NET analyzer rules with nullable enabled.

---

## License

Choose a license that fits your needs (MIT recommended).
If you add a LICENSE file, mention it here.

```xml
If you want, I can also generate a minimal `SystemClock` implementation or add a “How to plug Keel into an existing API” tutorial section.
```

using Carter;
using Scalar.AspNetCore;
using Serilog;
using WebAPI;
using WebAPI.Common.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((services, logging) => logging
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Carter finds every slice endpoint (ICarterModule) in this assembly. Validators are excluded:
// they are registered by AddFeatures() and run through ValidationDecorator, not by Carter.
builder.Services.AddCarter(
    new DependencyContextAssemblyCatalog(typeof(Program).Assembly),
    configurator => configurator.WithEmptyValidators());

// Binding failures (invalid JSON, missing body, wrong value type) throw BadHttpRequestException in every
// environment, so GlobalExceptionHandler turns them into a uniform 400 ProblemDetails instead of an empty 400.
builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);

// AddControllers() used to register the authorization services implicitly; without it,
// app.UseAuthorization() fails at startup unless they are registered explicitly.
builder.Services.AddAuthorization();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails(options =>
    options.CustomizeProblemDetails = context =>
        context.ProblemDetails.Extensions["correlationId"] = context.HttpContext.TraceIdentifier);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddFeatures();
builder.Services.AddDatabase(
    builder.Configuration.GetConnectionString("School")
    ?? throw new InvalidOperationException("The School connection string is required."),
    builder.Configuration.GetValue<bool>("Database:EnableSensitiveDataLogging"));

var app = builder.Build();
await app.Services.MigrateDatabaseAsync(app.Lifetime.ApplicationStopping);

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();               // OpenAPI document: /openapi/v1.json
    app.MapScalarApiReference();    // Interactive UI that reads it: /scalar
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHealthChecks("/health");

// Every slice endpoint goes through one group so the ProblemDetails logging filter applies to all of them.
app.MapGroup(string.Empty)
    .AddEndpointFilter<ProblemDetailsLoggingFilter>()
    .MapCarter();

app.Run();

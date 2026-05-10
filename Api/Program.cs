using Api.Extensions;
using Api.Middlewares;
using Application.Contracts.Repositories;
using Application.Extensions;
using Hangfire;
using Infrastructure.Extensions;
using Infrastructure.Jobs;
using Infrastructure.Persistence;
using Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddPresentationServices();
builder.AddApplicationServices();
builder.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        logger.LogInformation("Applying pending migrations (if any)...");
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("Migrations applied successfully.");

        var seedingData = services.GetRequiredService<ISeeder>();
        await seedingData.SeedAsync();
        logger.LogInformation("Database seeding completed successfully!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        throw;
    }
}

var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();

recurringJobManager.AddOrUpdate<IZoomOAuthStateRepository>("cleanup-zoom-oauth-states",
    repo => repo.DeleteAllExpiredAndUsedStatesAsync(),
    Cron.Hourly);

recurringJobManager.AddOrUpdate<QuizDeadlineReminderJob>("quiz-deadline-reminders",
    job => job.SendQuizDeadlineRemindersAsync(),
    Cron.Daily);

recurringJobManager.AddOrUpdate<AssignmentDeadlineReminderJob>("assignment-deadline-reminders",
    job => job.SendAssignmentDeadlineRemindersAsync(),
    Cron.Daily);


// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
    app.UseHangfireDashboard();
}
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors();
app.UseHealthChecks("/health");
app.UseAuthentication();
app.UseMiddleware<SessionMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
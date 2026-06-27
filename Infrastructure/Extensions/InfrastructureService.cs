using Application.Contracts.Externals;
using Application.Contracts.Files;
using Application.Contracts.Zoom;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.Health;
using Infrastructure.Jobs;
using Infrastructure.Repositories;
using Infrastructure.Seeders;
using Infrastructure.Services;
using Infrastructure.Services.AuthServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure.Extensions
{
    public static class InfrastructureService
    {
        public static void AddInfrastructureServices(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddDbContextPool<AppDbContext>(options =>
            {
                var connectionString = BuildPostgresConnectionString(configuration);

                var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                dataSourceBuilder.EnableDynamicJson();
                var dataSource = dataSourceBuilder.Build();

                options.UseNpgsql(dataSource);
            });
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                var redisUrl = Environment.GetEnvironmentVariable("REDIS_URL")
                    ?? configuration.GetConnectionString("RedisConnection");

                if (redisUrl?.StartsWith("redis://") == true)
                {
                    var uri = new Uri(redisUrl);
                    var password = uri.UserInfo.Split(':').LastOrDefault();
                    redisUrl = $"{uri.Host}:{uri.Port},password={password},ssl=false";
                }

                options.Configuration = redisUrl;
            });

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(90);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddHybridCache();

            builder.Services.AddOptions<JwtOptions>()
                .BindConfiguration(nameof(JwtOptions))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<MailOptions>()
                .BindConfiguration(nameof(MailOptions))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<BunnyOptions>()
                .BindConfiguration(nameof(BunnyOptions))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<AiOptions>()
                .BindConfiguration(nameof(AiOptions))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<Common.Options.FileOptions>()
                .BindConfiguration(nameof(Common.Options.FileOptions))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<Common.Options.ZoomOptions>()
                .BindConfiguration(nameof(Common.Options.ZoomOptions))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddHangfire(config =>
            {
                var connectionString = BuildPostgresConnectionString(configuration);

                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(options =>
                        options.UseNpgsqlConnection(connectionString));
            });

            builder.Services.AddHangfireServer();

            builder.Services.AddHealthChecks()
                   .AddNpgSql(BuildPostgresConnectionString(configuration))
                   .AddHangfire(options => options.MinimumAvailableServers = 1)
                   .AddRedis(Environment.GetEnvironmentVariable("REDIS_URL")
                       ?? configuration.GetConnectionString("RedisConnection")!)
                   .AddCheck<MailHealthCheck>("mail service");

            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.AddScoped<ITokenProvider, TokenProvider>();
            builder.Services.AddScoped<IRefreshRepository, RefreshRepository>();

            builder.Services.AddScoped<ISeeder, Seeder>();
            builder.Services.AddScoped<IPlanRepository, PlanRepository>();
            builder.Services.AddScoped<ITenantRepository, TenantRepository>();
            builder.Services.AddScoped<ITenantUserRepository, TenantUserRepository>();
            builder.Services.AddScoped<IFileRepository, FileRepository>();
            builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            builder.Services.AddHttpClient<IFileService, FileService>();
            builder.Services.AddHttpClient<IExternalService, ExternalService>();
            builder.Services.AddScoped<ITenantMemberRepository, TenantMemberRepository>();
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<ITenantRoleRepository, TenantRoleRepository>();
            builder.Services.AddScoped<ITenantInviteRepository, TenantInviteRepository>();
            builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
            builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
            builder.Services.AddScoped<IModuleItemRepository, ModuleItemRepository>();
            builder.Services.AddScoped<IZoomService, ZoomService>();
            builder.Services.AddScoped<IZoomIntegrationRepository, ZoomIntegrationRepository>();
            builder.Services.AddScoped<IZoomOAuthStateRepository, ZoomOAuthStateRepository>();
            builder.Services.AddScoped<ILiveSessionRepository, LiveSessionRepository>();
            builder.Services.AddScoped<ITenantPageRepository, TenantPageRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
            builder.Services.AddScoped<ILessonRepository, LessonRepository>();
            builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
            builder.Services.AddScoped<ITenantWebsiteSettingsRepository, TenantWebsiteSettingsRepository>();
            builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
            builder.Services.AddScoped<IQuizRepository, QuizRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IAttemptRepository, AttemptRepository>();
            builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
            builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<ICourseInviteRepository, CourseInviteRepository>();
            builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            builder.Services.AddScoped<IDiscussionRepository, DiscussionRepository>();
            builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
            builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
            builder.Services.AddScoped<IStudentSubscriptionRepository, StudentSubscriptionRepository>();
            builder.Services.AddScoped<IStudentSubjectRepository, StudentSubjectRepository>();
            builder.Services.AddScoped<IStudentStreakRepository, StudentStreakRepository>();
            builder.Services.AddScoped<IFriendRepository, FriendRepository>();
            builder.Services.AddScoped<ILessonViewRepository, LessonViewRepository>();
            builder.Services.AddScoped<IStudentGradeRepository, StudentGradeRepository>();
            builder.Services.AddScoped<IFlashCardRepository, FlashCardRepository>();
            builder.Services.AddScoped<IStudentQuizRepository, StudentQuizRepository>();
            builder.Services.AddScoped<QuizDeadlineReminderJob>();
            builder.Services.AddScoped<AssignmentDeadlineReminderJob>();
            builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
            builder.Services.AddScoped<ICourseProgressRepository, CourseProgressRepository>();
            builder.Services.AddScoped<ITenantPageVisitRepository, TenantPageVisitRepository>();
        }
        public static string BuildPostgresConnectionString(IConfiguration configuration)
        {
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            if (!string.IsNullOrWhiteSpace(databaseUrl))
            {
                var uri = new Uri(databaseUrl);
                var userInfo = uri.UserInfo.Split(':');

                return new NpgsqlConnectionStringBuilder
                {
                    Host = uri.Host,
                    Port = uri.Port,
                    Username = userInfo[0],
                    Password = userInfo[1],
                    Database = uri.AbsolutePath.Trim('/'),
                    SslMode = SslMode.Require
                }.ToString();
            }

            return configuration.GetConnectionString("DefaultConnection")!;
        }
    }
}
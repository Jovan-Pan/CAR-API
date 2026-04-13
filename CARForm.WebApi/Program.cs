using Contracts.Repository.MasterData;
using Microsoft.AspNetCore.Localization;
using Microsoft.OpenApi.Models;
using Quartz;
using Repository.MasterData;
using Services;
using Services.CAR;
using Services.Contracts.CAR;
using WebApi;
using WebApi.DependecyInjection;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var configuration = builder.Configuration;
builder.Services.Configure<AppSettings>(configuration);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = configuration.GetValue<string>("AppName"),
        Version = configuration.GetValue<string>("BuildVersion")
    });
});

//configure for master data api
builder.Services.AddMasterApiHandler(configuration);
//configure business logic
builder.Services.AddBusinessLogic();
//configure data access
builder.Services.AddDataAccess();
//configure jwt
builder.Services.AddJwtConfiguration(configuration);
//Configure CORS
builder.Services.AddCorsConfiguration(configuration);
//configure nlog
builder.Services.AddNLogConfigurations();
//configure MemCache
builder.Services.AddMemCacheConfigurations();
//configure Localization
builder.Services.AddLocalization();

builder.Services.AddScoped<IMDMRepository, MDMRepository>();
builder.Services.AddScoped<ISendMailSettingService, SendMailSettingService>();

int executeHour = builder.Configuration.GetValue<int>("cronSchedule", 8);

string cronSchedule = $"0 0 {executeHour} * * ?";

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("DailyCheckJob");
    q.AddJob<DailyCheckJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("DailyCheckJob-trigger")
        //.WithCronSchedule(cronSchedule, x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))));
        .WithCronSchedule(cronSchedule));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

//For ExcelReaderFactory
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("DefaultCors");

app.UseMiddleware<LogRequestMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<JwtMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

//for localization
var supportedCultures = new[] { "en", "zh" };
var requestLocalizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

requestLocalizationOptions.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());

app.UseRequestLocalization(requestLocalizationOptions);

app.MapControllers();

app.Run();

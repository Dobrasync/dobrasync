using System.Text.Json.Serialization;
using Asp.Versioning;
using Asp.Versioning.Conventions;
using Dobrasync.Api.Api.Middleware;
using Dobrasync.Api.BusinessLogic.Services;
using Dobrasync.Api.Database.DB;
using Dobrasync.Api.Shared.Appsettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

#region Builder
#region Bootstrap
var builder = WebApplication.CreateBuilder(args);
#endregion
#region Appsettings
builder.Services.AddOptions<AppsettingsAS>()
    .Bind(builder.Configuration)
    .ValidateDataAnnotations()
    .ValidateOnStart();

AppsettingsAS appsettings = new AppsettingsAS();
builder.Configuration.Bind(appsettings);
#endregion
#region API Explorer

builder.Services.AddEndpointsApiExplorer();

#endregion
#region Swagger

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Dobrasync API", Version = "v1" });
    opt.EnableAnnotations();
});

#endregion
#region Route

builder.Services.Configure<RouteOptions>(opt => { opt.LowercaseUrls = true; });

#endregion
#region Versioning

builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1);
    opt.ReportApiVersions = true;
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ApiVersionReader = new UrlSegmentApiVersionReader();
    opt.UnsupportedApiVersionStatusCode = 404;
}).AddMvc(opt => { opt.Conventions.Add(new VersionByNamespaceConvention()); }).AddApiExplorer(opt =>
{
    opt.GroupNameFormat = "'v'VVV";
    opt.SubstituteApiVersionInUrl = true;
});

#endregion
#region Controllers

builder.Services.AddControllers();

builder.Services.AddControllersWithViews()
    .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

#endregion
#region DB Context
builder.Services.AddDbContext<DobrasyncContext>(opt =>
{
    opt.UseMySQL(appsettings.Database.ConnectionString);
});
#endregion
#region Http Context
builder.Services.AddHttpContextAccessor();
#endregion
#region Common Services

ServiceRegister.RegisterCommonServices(builder.Services);

#endregion
#endregion
#region App
var app = builder.Build();

#region DevSwagger

if (appsettings.Dev.EnableApiExplorer)
{
    app.UseSwagger();
    app.UseSwaggerUI(config => { config.SwaggerEndpoint("/swagger/v1/swagger.json", "Dobrasync API"); });
}

#endregion
#region HTTPS Redirect

if (appsettings.Deployment.EnableHttpsRedirection)
{
    app.UseHttpsRedirection();
}

#endregion
#region Controllers

app.MapControllers();

#endregion
#region Cors
app.UseCors(opt =>
{
    opt.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .WithOrigins(appsettings.Deployment.CorsOrigins.ToArray());
});
#endregion
#region Middleware

app.UseMiddleware<ExceptionInterceptorMiddleware>();

#endregion
#region Database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DobrasyncContext>();
    
    dbContext.Database.EnsureCreated();
}
#endregion

#endregion

app.Run();
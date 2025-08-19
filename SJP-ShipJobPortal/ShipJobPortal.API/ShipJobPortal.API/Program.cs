using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ShipJobPortal.Application.Services;
using ShipJobPortal.Domain.Interfaces;
using ShipJobPortal.Infrastructure.Repositories;
using ShipJobPortal.Infrastructure.DataHelpers;
using ShipJobPortal.Infrastructure.Helpers;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Data;
using Microsoft.Data.SqlClient;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Infrastructure.DataAccessLayer;
using ShipJobPortal.Domain.Settings;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendOnly", policyBuilder =>
        policyBuilder
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            );
});

//builder.Services.Configure<IConfiguration>(builder.Configuration);  //new

var key = Encoding.UTF8.GetBytes(builder.Configuration["TokenSettings:TokenKey"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["TokenSettings:Issuer"],
        ValidAudience = builder.Configuration["TokenSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        RoleClaimType = ClaimTypes.Role
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication failed: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully: " + context.Principal.Identity?.Name);
            return Task.CompletedTask;
        }
    };
});


// Register Controllers
builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(ShipJobPortal.Application.Mappings.MappingProfile).Assembly);

// Swagger Configuration with JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SJP- Ship Job Portal API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your JWT.\n\nExample: \"Bearer abc123\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
// After ShipHire
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new SqlConnection(connectionString);
});

// Add a factory for ShipHireDB (only when needed)
builder.Services.AddScoped<Func<string, IDbConnection>>(sp =>
{
    return (dbName) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        var connectionString = config.GetConnectionString(dbName);
        return new SqlConnection(connectionString);
    };
});

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 104857600; // 100 MB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});


//before shiphire
//builder.Services.AddScoped<IDbConnection>(sp =>
//{
//    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//    return new SqlConnection(connectionString);
//});

//builder.Services.AddScoped<Func<IDbConnection>>(_ =>
//{
//    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//    return () => new SqlConnection(connectionString);
//});

// Dependency Injection
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IJobPostRepository, JobPostRepository>();
builder.Services.AddScoped<IEncryptionService, EncryptionHelper>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IGetListRepository, GetListRepository>();
builder.Services.AddScoped<IJobApplyRepository, JobApplyRepository>();
builder.Services.AddScoped<IOtpRepositories, OtpRepositories>();
builder.Services.AddScoped<IMatchingRepository, MatchingRepository>();
builder.Services.AddScoped<IShiphireRepository, ShiphireRepository>();
builder.Services.AddScoped<IAdvertismentRepository, AdvertismentRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepositoy>();




builder.Services.AddScoped<ILoginService, LoginServices>();
builder.Services.AddScoped<IModuleService, ModuleService>();
builder.Services.AddScoped<IJobPostService, JobPostService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IDbExceptionLogger, DbExceptionLogger>();
builder.Services.AddScoped<IGetListService, GetListService>();
builder.Services.AddScoped<IJobApplyService, JobApplyService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IMatchingService, MatchingService>();
builder.Services.AddScoped<IAdvertisementService, AdvertisementService>();
builder.Services.AddScoped<ITokenService, TokenService>();


//builder.Services.AddScoped<LoginServices>();
//builder.Services.AddScoped<RoleService>();
//builder.Services.AddScoped<CompanyService>();
//builder.Services.AddScoped<ModuleService>();
//builder.Services.AddScoped<ProfileService>();
//builder.Services.AddScoped<JobPostService>();

builder.Services.AddScoped<DataAccess>();
builder.Services.AddScoped<IDataAccess_Improved, DataAccess_Improved>();
//builder.Services.AddSingleton<EncryptionHelper>();
builder.Services.AddScoped<ReturnDapper>();
builder.Services.AddScoped<DbExceptionLogger>();

builder.Services.AddHttpClient();

var app = builder.Build();

app.UseMiddleware<ShipJobPortal.API.Middlewares.ExceptionMiddleware>();

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//for IIS
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/SJPAPI/swagger/v1/swagger.json", "SJP - Ship Job Portal API V1");
    c.RoutePrefix = "swagger"; // So Swagger UI is accessible at /SJPAPI/swagger/
});




app.UseHttpsRedirection();
app.UseCors("AllowFrontendOnly");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

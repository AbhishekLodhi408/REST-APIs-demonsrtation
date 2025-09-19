using Application1.Data;
using Application1.Repositries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography.X509Certificates;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyReactApp", policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.WithOrigins("*");
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Management APIs", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                      "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                      "Example: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6...\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "auth2",
                Name="Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }

    });
});

builder.Services.AddDbContext<Student_Course_DBContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("OurConnectionString")));

builder.Services.AddScoped<IStudentRepository, StudentRepositry>();
builder.Services.AddScoped<ICourseRepositry, CourseRepositry>();

string tokenKeyString = builder.Configuration.GetSection("AppSetting:TokenKey").Value;
SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKeyString)); 
TokenValidationParameters validationParams = new TokenValidationParameters()
{
    IssuerSigningKey = tokenKey,
    ValidateIssuer = false,
    ValidateIssuerSigningKey = false,
    ValidateAudience = false
};

//Adding Athentication and overriding the OnChallange event
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = validationParams;

        options.Events = new JwtBearerEvents 
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    message = "You are not authorized to access this resource."
                });
                return context.Response.WriteAsync(result);

            }
        };
    });

//Adding Policy for Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.RequireRole("Admin");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("MyReactApp");

app.UseAuthentication();

//Adding custom middleware for 403 response 
app.Use(async (context, next) =>
{
    await next();

    if(context.Response.StatusCode == StatusCodes.Status403Forbidden)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
        {
              message = "You don't have permission for this action."
        }));
    }
});

app.UseAuthorization();

app.MapControllers();

app.Run();

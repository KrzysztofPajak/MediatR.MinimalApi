using MediatR.MinimalApi.Extensions;
using MediatRSampleWebApplication.Commands.Roles;
using MediatRSampleWebApplication.EndpointFilters;
using MediatRSampleWebApplication.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddTransient<ValidationFilter>();

// Register MediatR services
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

var app = builder.Build();

app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Register MediatR endpoints (based on attributes Endpoint)
app.MapMediatREndpoints(typeof(Program).Assembly);


// Register MediatR endpoints (based on manual registration)
app.MapPostWithMediatR<CreateRole.CreateRoleCommand, Role>("/role/create")
    .WithDisplayName("CreateRole")
    .WithOpenApi(x =>
    {
        x.Tags = [new OpenApiTag() { Name = "Role" }];
        return x;
    });

app.MapGetWithMediatR<GetRoles.GetRolesCommand, IList<Role>>("/role/get")
    .WithDisplayName("GetRoles")
    .WithOpenApi(x =>
    {
        x.Tags = [new OpenApiTag() { Name = "Role" }];
        return x;
    });

app.MapGetWithMediatR<GetRoleById.GetRoleByIdCommand, Role>("/role/get/{id}")
    .WithDisplayName("GetRoleById")
    .WithOpenApi(x =>
    {
        x.Tags = [new OpenApiTag() { Name = "Role" }];
        return x;
    });

app.MapPutWithMediatR<UpdateRole.UpdateRoleCommand, Role>("/role/update/{Id}")
    .WithDisplayName("UpdateRole")
    .WithOpenApi(x =>
    {
        x.Tags = [new OpenApiTag() { Name = "Role" }];
        return x;
    });

app.MapDeleteWithMediatR<DeleteRole.DeleteRoleCommand, bool>("/role/delete/{Id}")
    .WithDisplayName("DeleteRole")
    .WithOpenApi(x =>
    {
        x.Tags = [new OpenApiTag() { Name = "Role" }];
        return x;
    });

app.Run();

public partial class Program { }
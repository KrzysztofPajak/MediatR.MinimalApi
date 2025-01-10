using FluentValidation;
using MediatR.MinimalApi.Extensions;
using MediatRSampleWebApplication.Commands.Companies;
using MediatRSampleWebApplication.Commands.Roles;
using MediatRSampleWebApplication.EndpointFilters;
using MediatRSampleWebApplication.Models;
using MediatRSampleWebApplication.Queries.Companies;
using MediatRSampleWebApplication.Transformers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();

builder.Services.AddOpenApi("v1", options =>
{
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();

});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

//ignore null values in json serialization
builder.Services.Configure<JsonOptions>(options =>
 {
     options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
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
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
});

//Register MediatR services for Minimal API
builder.Services.MinimalApiMediatRExtensions();

var app = builder.Build();

app.UseStatusCodePages();
app.MapOpenApi();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("OpenApi Playground");
        options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

// Register MediatR endpoints (based on attributes Endpoint)
app.MapMediatREndpoints(typeof(Program).Assembly);


// Register MediatR endpoints (based on manual registration)
app.MapPostWithMediatR<CreateRole.CreateRoleCommand, Role>("/role/create")
    .WithDisplayName("CreateRole")
    .WithTags("Role");

app.MapGetWithMediatR<GetRoles.GetRolesCommand, IList<Role>>("/role/get")
    .WithDisplayName("GetRoles")
    .WithTags("Role");

app.MapGetWithMediatR<GetRoleById.GetRoleByIdCommand, Role>("/role/get/{id}")
    .WithDisplayName("GetRoleById")
    .WithTags("Role");

app.MapPutWithMediatR<UpdateRole.UpdateRoleCommand, Role>("/role/update/{Id}")
    .WithDisplayName("UpdateRole")
    .WithTags("Role");

app.MapDeleteWithMediatR<DeleteRole.DeleteRoleCommand, bool>("/role/delete/{Id}")
    .WithDisplayName("DeleteRole")
    .WithTags("Role");


app.MapPostWithMediatR<CreateCompany.CreateCompanyCommand, Company>("/company/create")
    .WithDisplayName("CreateCompany")
    .WithTags("Company");


app.MapGetWithMediatR<GetCompany.Companies, IList<Company>>("/company/get")
    .WithDisplayName("GetCompanies")
    .WithTags("Company");

app.Run();

public partial class Program { }
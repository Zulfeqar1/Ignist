using Ignist.Data;
using Ignist.Data.Services;
using System.Text;
using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;



var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

#region MY Code


builder.Services.AddSingleton((provider) =>
{
    var configuration = provider.GetRequiredService<IConfiguration>(); 
    var EndpointUri = configuration["CosmosDbSettings:EndpointUri"];
    var PrimaryKey = configuration["CosmosDbSettings:PrimaryKey"];
    var databaseName = configuration["CosmosDbSettings:DatabaseName"];

    var cosmosClientOptions = new CosmosClientOptions
    {
        ApplicationName = databaseName,
        ConnectionMode = ConnectionMode.Gateway
    };
    var loggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddConsole(); 
    });

    var cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, cosmosClientOptions);

    return cosmosClient;

});
#endregion

builder.Services.AddControllers();

//JWT token Singleton
builder.Services.AddSingleton<JwtTokenService>(sp =>
    new JwtTokenService(sp.GetRequiredService<IConfiguration>()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Authetesering
builder.Services.AddScoped<ICosmosDbService, CosmosDbService>();
builder.Services.AddSingleton<PasswordHelper>();
#endregion

#region Legg til autentiseringstjenester
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero
    };
});
#endregion


#region My Code
builder.Services.AddScoped<IPublicationsRepository, PublicationsRepository>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

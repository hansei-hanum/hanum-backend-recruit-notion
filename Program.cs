using Hanum.Core.Authentication;
using Hanum.Core.Helpers;
using Hanum.Core.Middleware;
using Hanum.Core.Protos;
using Hanum.Core.Services;
using Hanum.Recruit.Contracts.Services;
using Hanum.Recruit.Models.DTO.Requests;
using Hanum.Recruit.Services;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

services.AddHanumLogging();

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddHanumSwaggerGen();

// gRPC Client
services.AddHanumAuthGrpcClient(
	configuration.GetConnectionString("AuthService.gRPC")!);

// Authentication Handler
services.AddAuthentication()
	.AddHanumCommonAuthScheme();

// Services
services.AddHanumUserService();

// Notion Service
services.AddNotionClient(options => {
	options.AuthToken = configuration.GetConnectionString("NotionAPIKey")!;
});

// Memory Cache
services.AddMemoryCache();

services.AddSingleton<IDepartmentService, DepartmentService>();
services.AddSingleton<IApplicationService, ApplicationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseHanumExceptionHandler();
app.MapControllers();

app.Run();

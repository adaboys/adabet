using System.Reflection;
using App;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

internal class Program {
	private static void Main(string[] args) {
		// Step 1. Configure services.
		var builder = WebApplication.CreateBuilder(args);
		var services = builder.Services;
		var config = builder.Configuration;

		// Register appsettings.json which be bind with TOptions.
		// Ref: https://stackoverflow.com/questions/40470556/actually-read-appsettings-in-configureservices-phase-in-asp-net-core
		_ = services.Configure<AppSetting>(config.GetSection(AppSetting.SECTION_APP));

		// Allow cross origin header
		services.AddCors(options => {
			options.AddPolicy(name: AppConst.CORS_POLICY_ALLOW_ANY, builder => {
				builder.AllowAnyOrigin().AllowAnyHeader();
			});
		});

		// Configure DI (dependecy injection)
		// - Singleton: IoC container will create and share a single instance of a service throughout the application's lifetime.
		// - Transient: The IoC container will create a new instance of the specified service type every time you ask for it.
		// - Scoped: IoC container will create an instance of the specified service type once per request and will be shared in a single request.
		// Ref: https://www.tutorialsteacher.com/core/aspnet-core-introduction
		services
			.AddScoped<BootCommand>()
			.AddScoped<UpdateCommand202304>()

			.AddScoped<AuthService>()
			.AddScoped<AuthTokenService>()
			.AddScoped<UserService>()
			.AddScoped<UserDao>()
			.AddScoped<AppService>()
			.AddScoped<UserWalletService>()
			.AddScoped<SportService>()
			.AddScoped<UserSportService>()
			.AddScoped<BlockfrostHookService>()
			.AddScoped<SportPredictionService>()
			.AddScoped<CurrencyService>()
			.AddScoped<UserFavoriteService>()
			.AddScoped<UserCoinTxService>()

			.AddScoped<UserComponent>()
			.AddScoped<MailComponent>()
			.AddScoped<RedisComponent>()

			.AddScoped<SystemDao>()
			.AddScoped<ApiNodejsRepo>()
			.AddScoped<CardanoNodeRepo>()
			.AddScoped<BetsapiRepo>()
			.AddScoped<BlockfrostRepo>()

			.AddControllers();

		// Our app setting
		var appSetting = config.GetSection(AppSetting.SECTION_APP).Get<AppSetting>()!;
		var isDevelopment = appSetting.environment == AppSetting.ENV_DEVELOPMENT;
		var isStaging = appSetting.environment == AppSetting.ENV_STAGING;
		var isProduction = appSetting.environment == AppSetting.ENV_PRODUCTION;

		Console.WriteLine($"----> Enable cronjob: {appSetting.taskMode.enableCronJob}");
		Console.WriteLine($"----> Enable command: {appSetting.taskMode.enableCommand}");

		// Config database connections
		services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(appSetting.database.appdb));
		services.AddDbContextPool<CasinoDbContext>(options => options.UseMySQL(appSetting.database.casinoDb));

		// Config JWT Authentication
		services.ConfigureJwtAuthenticationDk(appSetting);

		// [Redis] Config in-memory cache
		services.ConfigureRedisDk(appSetting);

		// [Logging] Use Serilog for logging
		builder.ConfigureSerilogDk();

		// [Quartz] For cronjob
		if (appSetting.taskMode.enableCronJob) {
			services.ConfigureQuartzDk(config, appSetting);
		}

		// [Swagger] For api doc
		if (!isProduction || appSetting.taskMode.enableCommand) {
			services.AddSwaggerGen(option => {
				// Support generating api-doc
				var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

				// Add JWT authorization Bearer token
				// Ref: https://www.c-sharpcorner.com/article/how-to-add-jwt-bearer-token-authorization-functionality-in-swagger/
				option.SwaggerDoc("v1", new OpenApiInfo {
					Title = "JWTToken_Auth_API",
					Version = "v1"
				});
				option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() {
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
				});
				option.AddSecurityRequirement(new OpenApiSecurityRequirement {
					{
						new OpenApiSecurityScheme {
							Reference = new OpenApiReference {
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						new string[] {}
					}
				});
			});

			// For what?
			services.AddEndpointsApiExplorer();
		}

		// Use signalR for realtime actions (communication, notification, ...)
		// We customize id-provider to let signalR create/lookup userId from our specified claim.
		services.AddSignalR();
		services.AddSingleton<IUserIdProvider, CustomSignalrUserIdProvider>();


		// Step 2. Configure app.
		var app = builder.Build();

		// Use https at remote (except development) env
		if (!isDevelopment) {
			app.UseHttpsRedirection();
		}

		// [Swagger] for api doc at non-production env
		if (!isProduction || appSetting.taskMode.enableCommand) {
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		// Allow store request-log (for eg,. api/auth/login response 200...) with Serilog
		if (!isProduction) {
			app.UseSerilogRequestLogging();
		}

		// Use exception page at non-production env
		if (!isProduction) {
			app.UseDeveloperExceptionPage();
		}

		// Customize api response for unhandled exception
		if (isProduction) {
			app.UseExceptionHandler("/api/error");
		}
		else {
			app.UseExceptionHandler("/api/error-development");
		}

		// Use routing
		app.UseRouting();

		// Use cors to allow request from web client.
		// Note: MUST place this after Routing and before Authentication.
		// Ref: https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-6.0
		app.UseCors(AppConst.CORS_POLICY_ALLOW_ANY);

		// Add authentication middleware (authenticate with JWT)
		app.UseAuthentication();

		// Requires authenticated access via [Authenticate] annotation
		app.UseAuthorization();

		// Add StaticFileMiddleware to pipeline, allow access static-file inside `wwwroot`.
		// For eg,. file at `public/html/helloworld.html` can be accessed via https://localhost:8080/html/helloworld.html
		app.UseStaticFiles();

		// Map controllers, hubs,...
		app.UseEndpoints(endpoints => {
			endpoints.MapControllers();

			endpoints.MapHub<NotificationHub>("/hub/notification");
		});

		app.Run();
	}
}

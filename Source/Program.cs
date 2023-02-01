using System.Reflection;
using App;
using Microsoft.EntityFrameworkCore;
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
			.AddScoped<AuthService>()
			.AddScoped<AuthTokenService>()
			.AddScoped<UserService>()
			.AddScoped<UserDao>()
			.AddScoped<AppService>()
			.AddScoped<UserWalletService>()
			.AddScoped<SportService>()
			.AddScoped<UserSportService>()
			.AddScoped<SystemWalletDao>()
			.AddScoped<UserComponent>()
			.AddScoped<MailComponent>()
			.AddScoped<RedisComponent>()
			.AddScoped<ApiNodejsRepo>()
			.AddScoped<OnebetRepo>()
			.AddScoped<CardanoNodeRepo>()
			.AddScoped<BetsapiRepo>()
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

		// [Swagger] Use swagger for api doc
		if (!isProduction) {
			services.AddSwaggerGen(option => {
				// Support generating api-doc
				var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
			});
			// services.AddEndpointsApiExplorer();
		}


		// Step 2. Configure app.
		var app = builder.Build();

		// Use https at remote (except development) env
		if (!isDevelopment) {
			app.UseHttpsRedirection();
		}

		// [Swagger] for api doc at non-production env
		if (!isProduction) {
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

		// Declare it to auto mapping route in controller classes
		app.UseEndpoints(endpoints => {
			endpoints.MapControllers();
		});

		app.Run();
	}
}

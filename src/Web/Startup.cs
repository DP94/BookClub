using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.Util.Internal.PlatformServices;
using Aws.DynamoDbLocal;
using Aws.Services;
using Core.Services;

namespace BookClub;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSingleton<IBookService, BookService>();
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<IBookDynamoDbStorageService, BookDynamoDbStorageService>();
        services.AddSingleton<IUserDynamoDbStorageService, UserDynamoDbStorageService>();
        var awsOptions = new AWSOptions()
        {
            Region = RegionEndpoint.EUWest2
        };

        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (env == "LOCAL")
        {
            new LocalDynamoDbSetup().SetupDynamoDb();
            awsOptions.Credentials = new BasicAWSCredentials("x", "x");
            awsOptions.DefaultClientConfig.ServiceURL = "http://localhost:8000";
        }
        services.AddDefaultAWSOptions(awsOptions);
        
        services.AddAWSService<IAmazonDynamoDB>(awsOptions);
        services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
        });
        services.AddHttpContextAccessor();
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseCors();
        app.Use((context, next) =>
        {
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            return next.Invoke();
        });
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}
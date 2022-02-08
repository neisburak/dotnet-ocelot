using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.LoadBalancer.LoadBalancers;
using Ocelot.Middleware;
using Ocelot.Responses;
using Ocelot.Values;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("SecretKey"))),
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Configuration
    .AddJsonFile("ocelot.json")
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json");

builder.Services.AddOcelot()
    .AddDelegatingHandler<RequestLogger>()
    .AddCacheManager(settings => settings.WithDictionaryHandle())
    .AddCustomLoadBalancer<CustomLoadBalancer>();

var app = builder.Build();

app.UseAuthentication();

app.UseOcelot();

app.Run();


public class RequestLogger : DelegatingHandler
{
    private readonly ILogger<RequestLogger> _logger;

    public RequestLogger(ILogger<RequestLogger> logger)
    {
        _logger = logger;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Request [{request.Method}] - {request.RequestUri?.PathAndQuery} : {request.RequestUri?.Port}");
        return base.SendAsync(request, cancellationToken);
    }
}

public class CustomLoadBalancer : ILoadBalancer
{
    private readonly Func<Task<List<Service>>> _services;
    private readonly object _lock = new();
    private int _last;

    public CustomLoadBalancer(Func<Task<List<Service>>> services)
    {
        _services = services;
    }

    public async Task<Response<ServiceHostAndPort>> Lease(HttpContext httpContext)
    {
        var services = await _services();
        lock (_lock)
        {
            if (_last >= services.Count)
            {
                _last = 0;
            }

            var next = services[_last];
            _last++;
            return new OkResponse<ServiceHostAndPort>(next.HostAndPort);
        }
    }

    public void Release(ServiceHostAndPort hostAndPort) { }
}
using BlogApp.Business.Services.Abstract;
using BlogApp.Business.Services.Concrete;
using BlogApp.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using BlogApp.Middleware;
using BlogApp.Filters;
using BlogApp.DataAccess.Repositories.Abstract;
using BlogApp.DataAccess.Repositories.Concrete;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"];

// Normal MVC servislerini ekliyoruz
builder.Services.AddControllers(options =>
{
    // Global exception filter'ı ekle
    options.Filters.Add<GlobalExceptionFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Blog API",
        Version = "v1"
    });

    // XML belge dosyasını yükle
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token'ınızı buraya 'Bearer ' ön ekiyle birlikte girin. Örnek: Bearer xxxxx.yyyyy.zzzzz"
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

// Repository'leri ekle
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
// Service'leri ekle
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<ICommentService, CommentManager>();
builder.Services.AddScoped<IBlogPostService, BlogPostManager>();
builder.Services.AddScoped<IFotoService, FotoService>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();

builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        return new BadRequestObjectResult(new { Success = false, Errors = errors });
    };
});

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "isAdmin" && (c.Value == "true" || c.Value == "True"))));

    // You can add other policies here if needed
    // options.AddPolicy("EditorPolicy", policy => policy.RequireRole("Editor"));
});

// HttpContextAccessor ekle
builder.Services.AddHttpContextAccessor();

// Add AutoMapper (if needed later)
// builder.Services.AddAutoMapper(typeof(Program));

// CORS Configuration (Example)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins(
                "http://localhost:4200",
                "http://localhost:5115",
                "https://localhost:7100",
                "http://localhost:63681",
                "https://localhost:44368"
            )
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
// Admin yetkilendirme middleware'ini authentication ve authorization arasına ekleyin
app.UseAdminAuthorization();
app.UseAuthorization();  

app.MapControllers();

app.Run();

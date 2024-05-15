using FastLane.Context;
using FastLane.Controllers;
using FastLane.Entities;
using FastLane.Repository.Airport;
using FastLane.Repository.Customer;
using FastLane.Repository.Employee;
using FastLane.Repository.Order;
using FastLane.Repository.Order_Detail;
using FastLane.Repository.Role;
using FastLane.Repository.Service;
using FastLane.Repository.Status;
using FastLane.Repository.User;
using FastLane.Service;
using FastLane.Service.Account;
using FastLane.Service.Customer;
using FastLane.Service.Email;
using FastLane.Service.Employee;
using FastLane.Service.Excel;
using FastLane.Service.Order;
using FastLane.Service.Pagination;
using FastLane.Service.Passport;
using FastLane.Service.Role;
using FastLane.Service.Service;
using FastLane.Service.Status;
using FastLane.Service.User;
using FastLane.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// The dynamic variable is to configure CORS
var allowedOrigin = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Add service
builder.Services.AddScoped<PasswordHelper>();
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPaginationService, Pagination>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IPassportDataExtractor, PassportDataExtractor>();
builder.Services.AddHttpClient<IPassportDataExtractor, PassportDataExtractor>();
//Add repository
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IAirportRepository, AirportRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrder_DetailRepository, Order_DetailRepository>();
builder.Services.AddScoped<IStatusRepository, StatusRepository>();

// Config CORS to VueJs call API
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()
               .WithExposedHeaders("Content-Disposition");
    });
});

//Config JWT and cookie to make authenication API
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie("Cookies")
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ClockSkew = TimeSpan.Zero,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:SecretKey"])),
    };
});


// Add authorization
builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure NSwag for Swagger documentation
builder.Services.AddSwaggerDocument(config =>
{
    config.DocumentName = "v1";
    config.PostProcess = document =>
    {
        document.Info.Title = "API Manage";
        document.Info.Version = "v1";
        document.Info.Contact = new NSwag.OpenApiContact
        {
            Name = "PhamTienDat",
            Email = "Phamtiendat246@gmail.com"
        };
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseOpenApi(config =>
{
    config.Path = "/swagger-ui";
});

app.UseSwaggerUi(config =>
{
    config.Path = "/api";
    config.DocumentPath = "/swagger-ui";
});


//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("MyPolicy");

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();

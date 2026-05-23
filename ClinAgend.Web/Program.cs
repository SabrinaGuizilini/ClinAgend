using ClinAgend.Web.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ClinAgend.Data.Models;
using ClinAgend.Data;
using ClinAgend.Core.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using MudBlazor.Services;
using ClinAgend.Web;
using ClinAgend.Core.Interfaces;
using ClinAgend.Web.Services;
using MudBlazor;
using ClinAgend.Data.Interfaces;
using ClinAgend.Data.Repositories;
using ClinAgend.Models.Enums;
using ClinAgend.Models.Settings;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var culture = new CultureInfo("pt-BR");

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/access-denied";
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true;
});

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IEmailTemplateService,EmailTemplateService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IClinicService, ClinicService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IScheduleValidatorService, ScheduleValidatorService>();
builder.Services.AddScoped<IScheduleBlockService, ScheduleBlockService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// AuthenticationStateProvider baseado no Identity
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AppClaimsPrincipalFactory>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthorizationCore();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DefaultOnly", policy =>
        policy.RequireClaim("UserType", nameof(UserType.Default)));

    options.AddPolicy("MasterOnly", policy =>
        policy.RequireClaim("UserType", nameof(UserType.Master)));

    options.AddPolicy("DoctorOnly", policy =>
        policy.RequireClaim("UserType", nameof(UserType.Doctor)));

    options.AddPolicy("SuperAdminOnly", policy =>
        policy.RequireClaim("UserType", nameof(UserType.SuperAdmin)));

    options.AddPolicy("DefaultAndMaster", policy =>
       policy.RequireClaim("UserType",
       nameof(UserType.Default),
       nameof(UserType.Master)));

    options.AddPolicy("NotSuperAdmin", policy =>
        policy.RequireClaim("UserType", 
        nameof(UserType.Default),
        nameof(UserType.Master),
        nameof(UserType.Doctor)));
});


builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 6000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

builder.Services.AddRazorPages();

builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddTransient<MudLocalizer, DictionaryMudLocalizer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        context.Database.Migrate();

        Console.WriteLine("Migrations aplicadas com sucesso.");

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = services.GetRequiredService<IConfiguration>();

        await DbInitializer.SeedUsers(userManager, configuration);

        Console.WriteLine("Seed executado com sucesso.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erro na inicialização:");
        Console.WriteLine(ex);
    }
}

Console.WriteLine("Timezone local: " + TimeZoneInfo.Local.DisplayName);
Console.WriteLine("Timezone ID: " + TimeZoneInfo.Local.Id);
Console.WriteLine("DateTime.Now: " + DateTime.Now);
Console.WriteLine("DateTime.UtcNow: " + DateTime.UtcNow);


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection(); 
app.UseStaticFiles(); 
app.UseRouting();

app.UseAuthentication();
app.UseStatusCodePagesWithRedirects("/404");
app.UseAntiforgery();
app.UseAuthorization();

app.MapRazorPages();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();

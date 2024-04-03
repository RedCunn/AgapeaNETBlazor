using Agapea_Blazor.Server.Models;
using Agapea_Blazor.Server.Models.Services;
using Agapea_Blazor.Server.Models.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ----- configuracion de identity con EF --------------------------------
    
String _DBConnectionString= builder.Configuration.GetConnectionString("BlazorSqlServerConnectionString");
String _AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;  
    
    //1º paso : configurar cadena de conexion q va a usar el DbContext para volcar cambios en migraciones y recup.datos
builder.Services.AddDbContext<AppDBContext>((DbContextOptionsBuilder opt) =>
{
    opt.UseSqlServer(_DBConnectionString, (SqlServerDbContextOptionsBuilder opt) => { opt.MigrationsAssembly(_AssemblyName); } );
});

    //2º paso : configuro servicios de Identity : UserManager y SignInManager 

builder.Services.AddIdentity<MiClienteIdentity, IdentityRole>(
                    (IdentityOptions opts) =>
                    {
                        //opciones configuracion UserManager...
                        opts.Password = new PasswordOptions
                        {
                            RequireDigit = true,
                            RequireUppercase = true,
                            RequireLowercase = true,
                            RequiredLength = 6
                        };
                        opts.User = new UserOptions
                        {
                            RequireUniqueEmail = true
                        };

                        //opciones configuracion SignInManager...
                        opts.SignIn = new SignInOptions
                        {
                            RequireConfirmedEmail = true
                        };
                        opts.Lockout = new LockoutOptions
                        {
                            AllowedForNewUsers = false,
                            MaxFailedAccessAttempts = 5,
                            DefaultLockoutTimeSpan = TimeSpan.FromHours(3)
                        };
                    }
                )
                .AddEntityFrameworkStores<AppDBContext>(); // <-- saltara exception a la hora de validar emails cuando Identity genera token de activacion
                                                           //porque no existe proveedor de tokens configurado : .AddTokenProvider<> 

//-------------------------------------------------------------------------

//____CONFIGURACION SERVICIO GENERACION JWT UNA VEZ QUE IDENTITY OK___
// ---> con esto estoy estableciendo que el esquema de autenticacion sea JWT, sino por defecto sería con Cookies

byte[] _signBytes = System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:signature"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    (JwtBearerOptions opts) =>
                    {
                        opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidateIssuer = true, // <-- validar si el jwt ha sido generado desde mi servidor "issuer"
                            ValidateLifetime = true, // <-- validar la fecha de expiracion del jwt (claim "exp")
                            ValidateIssuerSigningKey = true, // <-- validar si el token ha sido firmado por el servidor 
                            ValidateAudience = false, // <-- validar subdominios para los que es valido el jwt (claim "audience")
                            ValidIssuer = builder.Configuration["JWT:issuer"],
                            IssuerSigningKey = new SymmetricSecurityKey(_signBytes)
                        };
                    } 
                );//<---- con el segundo metodo configuramos la comprobacion de los claims de los JWT recibidos desde los clientes blazor

//_________INYECCION SERVICIO ENVIO EMAIL A TRAVES DE MAILJET___________
builder.Services.AddScoped<IClienteCorreo, MailJetService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

//--------------------- meto en la pipeline los middleware para la autentificacion y autorizacion usando identity --------------
app.UseAuthentication();
app.UseAuthorization();
//------------------------------------------------------------------------------------------------------------------------------


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

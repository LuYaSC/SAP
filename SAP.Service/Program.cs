using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SAP.Repository.SAPRepository;
using SAP.Repository.SAPRepository.Entities;
using SAP.RuleEngine.AuthenticationService;
using SAP.RuleEngine.CollaboratorService;
using SAP.RuleEngine.KidService;
using SAP.RuleEngine.ParentService;
using SAP.RuleEngine.TypeBusinessService;
using System.Security.Principal;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IPrincipal>(provider => provider.GetService<IHttpContextAccessor>().HttpContext.User);

#region ServiceType Based
builder.Services.AddScoped<ITypeBusinessService<Turn>, TypeBusinessService<Turn>>();
builder.Services.AddScoped<ITypeBusinessService<Modality>, TypeBusinessService<Modality>>();
builder.Services.AddScoped<ITypeBusinessService<Room>, TypeBusinessService<Room>>();
builder.Services.AddScoped<ITypeBusinessService<City>, TypeBusinessService<City>>();
builder.Services.AddScoped<ITypeBusinessService<BranchOffice>, TypeBusinessService<BranchOffice>>();
builder.Services.AddScoped<ITypeBusinessService<PaymentType>, TypeBusinessService<PaymentType>>();
builder.Services.AddScoped<ITypeBusinessService<Relationship>, TypeBusinessService<Relationship>>();
builder.Services.AddScoped<ITypeBusinessService<DocumentType>, TypeBusinessService<DocumentType>>();
#endregion

builder.Services.AddScoped<IKidService, KidService>();
builder.Services.AddScoped<IParentService, ParentService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ICollaboratorService, CollaboratorService>();

string mySqlConnectionStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SAPContext>(options => options.UseNpgsql(mySqlConnectionStr));
builder.Services.AddIdentityCore<User>(
        //options =>
        //{
        //    options.SignIn.RequireConfirmedAccount = false;
        //}
        )
     .AddRoles<Role>()
     .AddEntityFrameworkStores<SAPContext>();
//jwt
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Adding Jwt Bearer  
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsDevPolicy", builder =>
    {
        builder.WithOrigins("*")
            .WithMethods("POST, GET")
            .AllowAnyHeader();
    });

});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CorsDevPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;




// Here we define how tokens will be.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


string key = "lkjh3k4jh3k4jh32049i32-0e9-f0ewifledkjhflewkjhrkejwhr";



builder.Services.AddAuthentication("Bearer").AddJwtBearer( opt =>
{
    var signatureKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

    var signatureCredential = new SigningCredentials(signatureKey, SecurityAlgorithms.HmacSha256Signature);

    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = signatureKey,
    };


});


builder.Services.AddAuthorization();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Get Endpoint without no Authorizatio
app.MapGet("/", () => $"Hello World!!");
// Get Endpoint -- you need auth first.
app.MapGet("/hello", (ClaimsPrincipal user) => $"Hello {user.Identity?.Name}").RequireAuthorization();

// The Auth endpoint ;)
app.MapGet("/login/{user}/{pass}", (string user, string pass) =>
    {
            if ( user == "admin" && pass == "1234")
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var byteKey = Encoding.UTF8.GetBytes(key);
            var Tokendes = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                   {
                       new Claim ( ClaimTypes.Name, user),

                   }),
                Expires = DateTime.UtcNow.AddMonths(1),
                SigningCredentials = new SigningCredentials( new SymmetricSecurityKey(byteKey),
                         SecurityAlgorithms.HmacSha256Signature)
            };
            // Here we create the token based on the prevoius params.
            var token = tokenHandler.CreateToken(Tokendes);
            return tokenHandler.WriteToken(token);
            
        }
            else
        {
            return "Invalid User or Password!.";
        }
    });
     
app.Run();


using QuickBite.Auth.API.Extensions;
using QuickBite.Auth.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
<<<<<<< HEAD
=======

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowQuickBiteFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
>>>>>>> 44243b21b06a86f89e76746c8c98acb44cd4dab3
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

<<<<<<< HEAD
=======
app.UseCors("AllowQuickBiteFrontend");
>>>>>>> 44243b21b06a86f89e76746c8c98acb44cd4dab3
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

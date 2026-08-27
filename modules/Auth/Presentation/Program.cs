using LxzdBxy.Backend.Application;
using LxzdBxy.Backend.Infrastructure;
using LxzdBxy.Backend.Presentation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddWebService(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API V1");
        options.ConfigObject.AdditionalItems["withCredentials"] = true;

    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
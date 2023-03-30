using Common.Utility;
using I18NTool.I18N;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseI18NMiddleware();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

//app.UseI18NEndpoint("/resources/I18N/I18NTool", I18NEntity.GetJsonResource);
app.UseEndpoints(endpoint =>
{
    endpoint.MapGet("/resources/I18N/I18NTool", async (requestContext) =>
    {
        requestContext.Response.ContentType = "application/javascript";
        await requestContext.Response.WriteAsync(I18NEntity.GetJsonResource(requestContext));
        await requestContext.Response.Body.FlushAsync();
    });
});

app.Run();

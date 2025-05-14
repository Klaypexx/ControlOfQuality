using OrderApi.Repository;
using OrderApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Регистрируем контроллеры
builder.Services.AddControllers();

// Регистрируем репозиторий как Singleton (чтобы данные сохранялись между запросами)
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();

// Регистрируем сервисы платежей и уведомлений
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Регистрируем OrderService как фасад для работы с заказами
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
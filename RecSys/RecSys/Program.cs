using Microsoft.EntityFrameworkCore;
using RecSys.Models;
using RecSys.Data;
using RecSys.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 添加内存缓存服务
        builder.Services.AddMemoryCache();

        // 其他服务注册保持不变
        builder.Services.AddRazorPages();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHttpClient();
        builder.Services.AddDbContextPool<RecSysContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("RecSysConnectionString")),
            poolSize: 128);
        builder.Services.AddScoped<ISupervisorRecommendationService, SupervisorRecommendationService>();
        builder.Services.AddScoped<ILecturerService, LecturerService>();
        builder.Services.AddHttpClient<ILecturerService, LecturerService>(client =>
        {
            //client.BaseAddress = new Uri("http://localhost:5000");
            client.BaseAddress = new Uri(
                builder.Configuration["FLASK_API_URL"] ?? "http://flaskapi:5000");
        });
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(20); // Session timeout
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        var app = builder.Build();

        // 中间件配置保持不变
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.UseSession();
        app.MapRazorPages();
        app.Run();
    }
}
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using TinyBI.Engine.JsonModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TinyBI.WasmHost
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            var app = builder.Build();

            JsRuntime = app.Services.GetRequiredService<IJSRuntime>();

            await JsRuntime.InvokeAsync<string>("notifyBlazorReady");

            await app.RunAsync();
        }

        private static IJSRuntime JsRuntime;

        private static Schema Demo = new Schema(typeof(DemoSchema.NxgSchema));

        [JSInvokable]
        public async static Task<string> Query(string queryJson)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            };

            var parsed = JsonSerializer.Deserialize<QueryJson>(queryJson, jsonOptions);
            string sql = null;

            try
            {
                var query = new Query(parsed, Demo);

                var filterParams = new EmbeddedFilterParameters();
                sql = query.ToSql(filterParams, new Filter[0], 10);

                return await JsRuntime.InvokeAsync<string>("querySql", sql);
            }
            catch (Exception x)
            {
                x = x.GetBaseException();
                return JsonSerializer.Serialize(new
                {
                    message = x.Message,
                    stackTrace = x.StackTrace,
                    input = parsed,
                    sql
                });
            }
        }
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Todo.API.Extensions
{
    public static class ApplicationInit
    {
        public static void ConfigureTodo(this IApplicationBuilder app)
        {
            app.UseCors();

            app.UseSwagger();

            app.UseSwaggerUI(options =>
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Management Analysis API v1")
            );
        }
    }
}

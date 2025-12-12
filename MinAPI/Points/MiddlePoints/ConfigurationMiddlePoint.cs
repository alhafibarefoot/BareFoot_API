namespace MinAPI.Points.MiddlePoints
{
    public static class ConfigurationMiddlePoint
    {
        public static void RegisterMiddlewares(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(options =>
                {
                    options.RouteTemplate = "swagger/{documentName}/swagger.json";
                });

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "BareFoot API V1");
                    options.RoutePrefix = "swagger";
                    // options.RoutePrefix = string.Empty;  //direct root
                    options.InjectStylesheet("/css/swagger.css");
                });
            }
            if (app.Environment.IsStaging())
            {
                // your code here
            }
            if (app.Environment.IsProduction())
            {
                // your code here
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors("MyAllowedOrigins");

            app.UseOutputCache();
        }
    }
}

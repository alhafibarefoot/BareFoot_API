using Microsoft.AspNetCore.Mvc.ModelBinding;
using MinAPI.Data.Models;
using Newtonsoft.Json;

namespace MinAPI.Data.Bindings
{
    public class PostModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var json = await new StreamReader(
                bindingContext.HttpContext.Request.Body
            ).ReadToEndAsync();
            var varPost = JsonConvert.DeserializeObject<Post>(json);
            bindingContext.Result = ModelBindingResult.Success(varPost);
        }
    }
}

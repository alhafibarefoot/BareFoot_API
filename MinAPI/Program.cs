using MinAPI.Endpoints;
using MinAPI.Extensions;

//****************************************************************************************************************************
var builder = WebApplication.CreateBuilder(args);

//******************************************************* Services  Zone *****************************************************

builder.RegisterServices();

//****************************************************************************************************************************

var app = builder.Build();

//********************************************* Middle Points Zone( HTTP request pipeline) ***********************************

app.RegisterMiddlewares();

//******************************************************* End Points Zone ****************************************************


app.MapGroup("/hello").MapHello().WithTags("Hello");
app.MapGroup("/demo").MapDemo().WithTags("Demo");
app.MapGroup("/staticpost").MapStaticPost().WithTags("StaticPostNews");
app.MapGroup("/dbcontext").MapDBConextPost().WithTags("DBContextPostNews");
app.MapGroup("/automapper").MapAutoMapperPost().WithTags("AutoMapperPostNews");
app.MapGroup("/api/test").MapTestEndpoints().WithTags("Testing");

//****************************************************************************************************************************

app.Run();

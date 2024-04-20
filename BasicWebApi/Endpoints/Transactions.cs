using Application.Exceptions;
using Carter;

namespace BasicWebApi.Api.Endpoints;

public class Transactions : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/transactions");

        group.WithTags("Transactions");

        group.MapGet("/", () =>
        {
            throw new CustomException("oh nao um erro");
        });
    }
}
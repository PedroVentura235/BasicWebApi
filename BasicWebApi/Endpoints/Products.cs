using Application.Exceptions;
using BasicWebApi.Application.Features.Products;
using Carter;
using MediatR;

namespace BasicWebApi.Api.Endpoints;

public class Products : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/products");

        group.WithTags("Products");

        group.MapGet("/", async (CreateProductRequest request, IMediator mediator) =>
        {
            var response = await mediator.Send(request);
            return response.Match(
              resultValue => Results.Ok(resultValue),
              error => Results.Content(content: error.Message, statusCode: (int)error.StatusCode));
        }).Produces<Guid>();
    }
}
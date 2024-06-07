using Microsoft.AspNetCore.Http;
using warehouse_management_application.Items;
using warehouse_management_core.DTO_s;

namespace warehouse_management_api.Endpoints;

public static class ItemEndPoint
{

    public static void MapItemEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("api/items", CreateItem).WithTags("Item");
        routes.MapPost("api/items/", GetItem).WithTags("Item");
    }

    public async static Task<IResult> CreateItem(ItemDTO item, ItemService service, HttpContext context)
    {
        try
        {
            await service.CreatePortfolio(portfolio.Name);
            return Results.Created($"api/items/{item.Id}", item);
        }
        catch (SimilarItemNameException ex)
        {
            logger.LogError(ex, "Failed to create portfolio due to similar name.");
            return Results.BadRequest($"A portfolio with the name '{item.Name}' already exists.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating the portfolio.");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    public async static Task<IResult> GetItem(ItemDTO item, ItemService service, HttpContext context)
    {
        try
        {
            var items = await service.GetItems();
            return Results.Ok(items);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while getting the portfolios.");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
} 
}

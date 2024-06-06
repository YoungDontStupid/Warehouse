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
        var items = await;
    }
    public async static Task<IResult> GetItem(ItemDTO item, ItemService service, HttpContext context)
    {

    }

}

using LevelUp.Api.Dtos;
using LevelUp.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.Api.Controllers;

[ApiController]
[Route("api/shop")]
public class ShopController(IShopService shopService) : ControllerBase
{
    [HttpGet("items")]
    public async Task<ActionResult<List<ShopItemDto>>> GetItems() =>
        await shopService.GetItemsAsync();

    /// <summary>Køber en genstand, hvis brugeren har nok points og ikke allerede ejer den.</summary>
    [HttpPost("items/{id:int}/purchase")]
    public async Task<ActionResult<PurchaseResponse>> Purchase(int id) =>
        await shopService.PurchaseAsync(id);
}

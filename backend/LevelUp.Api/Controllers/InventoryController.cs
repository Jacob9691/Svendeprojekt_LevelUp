using LevelUp.Api.Dtos;
using LevelUp.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.Api.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController(IInventoryService inventoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<InventoryItemDto>>> GetInventory() =>
        await inventoryService.GetInventoryAsync();

    /// <summary>Udstyrer karakteren med genstanden. Andre genstande i samme kategori tages af.</summary>
    [HttpPost("{id:int}/equip")]
    public async Task<ActionResult<InventoryItemDto>> Equip(int id) =>
        await inventoryService.EquipAsync(id);

    [HttpPost("{id:int}/unequip")]
    public async Task<ActionResult<InventoryItemDto>> Unequip(int id) =>
        await inventoryService.UnequipAsync(id);
}

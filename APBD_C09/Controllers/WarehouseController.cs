using APBD_C09.Models.DTOs;
using APBD_C09.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_C09.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IWarehouseService _warehouseService;
    private readonly IOrderService _orderService;
    private readonly IProductWarehouseService _productWarehouseService;

    public WarehouseController(IProductService productService, IWarehouseService warehouseService, 
                               IOrderService orderService, IProductWarehouseService productWarehouseService)
    {
        _productService = productService;
        _warehouseService = warehouseService;
        _orderService = orderService;
        _productWarehouseService = productWarehouseService;
    }

    [HttpPost]
    public async Task<IActionResult> InsertIntoProductWarehouse([FromBody] ProductWarehouseDTO productWarehouse)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        if (! await _productService.DoesProductExistAsync(productWarehouse.IdProduct)) 
            return NotFound($"Product with given id ({productWarehouse.IdProduct}) does not exist");
        
        if (! await _warehouseService.DoesWarehouseExistAsync(productWarehouse.IdWarehouse))
            return NotFound($"Warehouse with given id ({productWarehouse.IdWarehouse}) does not exist");
        
        if (! await _orderService.HasProductBeenOrderedAsync(productWarehouse.IdProduct, productWarehouse.Amount, productWarehouse.CreatedAt))
            return NotFound("Given product hasn't been ordered");
        
        if ( await _productWarehouseService.HasOrderBeenRealisedAsync(productWarehouse.IdProduct))
            return NotFound("Given order has already been realised");

        try
        {
            await _orderService.UpdateFulfilledAtAsync(productWarehouse.IdProduct);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }

        int orderId = await _orderService.GetOrderIdAsync(productWarehouse.IdProduct, productWarehouse.Amount, productWarehouse.CreatedAt);
        int generatedId;
        
        try
        {
            generatedId = await _productWarehouseService.InsertOrderAsync(productWarehouse.IdWarehouse,
                productWarehouse.IdProduct, orderId, productWarehouse.Amount);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
        
        return Ok(generatedId);
    }
}
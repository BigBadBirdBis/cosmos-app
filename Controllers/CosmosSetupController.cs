using CosmosApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CosmosApp.Controllers
{
    [Route("Setup")]
    public class CosmosSetupController(CosmosSetupService cosmosSetupService) : Controller
    {
        private readonly CosmosSetupService _cosmosSetupService = cosmosSetupService;

        [HttpPost("CosmosDbContainers")]
        public async Task<IActionResult> PostCosmosDbContainers()
        {
            await _cosmosSetupService.CreateAllContainersAsync();
            return Ok();   
        }

        [HttpDelete("CosmosDbContainers")]
        public async Task<IActionResult> DeleteCosmosDbContainers()
        {
            await _cosmosSetupService.DeleteAllContainersAsync();
            return Ok();
        }
    }
}

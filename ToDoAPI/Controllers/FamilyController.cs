using Microsoft.AspNetCore.Mvc;
using ToDoAPI.DTOs;
using ToDoAPI.Models;
using ToDoAPI.Services.Interfaces;

namespace ToDoAPI.Controllers
{
    [ApiController]
    [Route("api/families")]
    public class FamilyController : Controller
    {
        private readonly IFamilyService familyService;
        public FamilyController(IFamilyService familyService)
        {
            this.familyService = familyService;
        }

        [HttpPost]
        public async Task<ActionResult<Family>> CreateFamily([FromBody] CreateFamilyRequestDto createFamilyRequestDto)
        {
            await this.familyService.CreateFamily(createFamilyRequestDto.UserEmail, createFamilyRequestDto.FamilyName, createFamilyRequestDto.InvitedUserEmail);
            return Ok(createFamilyRequestDto.FamilyName);
        }
    }
}

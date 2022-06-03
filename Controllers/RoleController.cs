using System.Security.Claims;
using ExcelManipulation.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExcelManipulation.Controllers;
public class RoleController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    public RoleController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;

    }

    public async Task<String> CreateRole()
    {
        if (!await _roleManager.RoleExistsAsync(Roles.Admin.ToString()))
        {
            await _roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await _roleManager.CreateAsync(new IdentityRole(Roles.Default.ToString()));
            return "Roles Added Succesfully";
        }
        return "Roles are already initialized";
    }

    public async Task ChangeRole()
    {
        var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (await _userManager.IsInRoleAsync(user, Roles.Admin.ToString()))
        {
            await _userManager.RemoveFromRoleAsync(user, Roles.Admin.ToString());
        }
        else
        {
            await _userManager.AddToRoleAsync(user, Roles.Admin.ToString());
        }
    }

    public async Task<String> AddToDefault()
    {
        var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (await _userManager.IsInRoleAsync(user, Roles.Default.ToString()))
        {
            await _userManager.AddToRoleAsync(user, Roles.Default.ToString());
            return "User added to default role";
        }

        return "User already exist in default role";

    }


}
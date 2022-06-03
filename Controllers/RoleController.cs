using System.Security.Claims;
using ExcelManipulation.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExcelManipulation.Controllers;
public class RoleController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    public RoleController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
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

    public async Task<string> ChangeRole()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);
        string message = string.Empty;
        if (await _userManager.IsInRoleAsync(user, Roles.Admin.ToString()))
        {
            await _userManager.RemoveFromRoleAsync(user, Roles.Admin.ToString());
            
            message =  "Your role is default!";
        }
        else
        {
            await _userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            message =  "Your role is Admin!";
        }

        await _signInManager.RefreshSignInAsync(user);
        return message;
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
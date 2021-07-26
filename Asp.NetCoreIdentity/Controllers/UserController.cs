using Asp.NetCoreIdentity.Context;
using Asp.NetCoreIdentity.Entities;
using Asp.NetCoreIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asp.NetCoreIdentity.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ApplicationContext _context;

        public UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, ApplicationContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //var query = _userManager.Users;
            //var users = _context.Users.Join(_context.UserRoles, user => user.Id, userrole => userrole.UserId, (user, userrole) => new
            //{
            //    user,
            //    userrole
            //}).Join(_context.Roles, two => two.userrole.RoleId, role => role.Id, (two, role) => new { two.user, two.userrole, role }).Where(x => x.userrole.RoleId != 3).Select(x => new AppUser()
            //{
            //    Id = x.user.Id,
            //    AccessFailedCount = x.user.AccessFailedCount,
            //    ConcurrencyStamp = x.user.ConcurrencyStamp,
            //    Email = x.user.Email,
            //    Gender = x.user.Gender,
            //    ImagePath = x.user.ImagePath,
            //    LockoutEnabled = x.user.LockoutEnabled,
            //    LockoutEnd = x.user.LockoutEnd,
            //    NormalizedEmail = x.user.NormalizedEmail,
            //    NormalizedUserName = x.user.NormalizedUserName,
            //    PasswordHash = x.user.PasswordHash,
            //    PhoneNumber = x.user.PhoneNumber,
            //    UserName = x.user.UserName

            //}).ToList();
            ////var users = await _userManager.GetUsersInRoleAsync("Member");
            //return View(users);
            List<AppUser> filteredUsers = new();
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                    filteredUsers.Add(user);
            }
            return View(filteredUsers);
        }
        public IActionResult Create()
        {
            return View(new UserAdminCreateModel());
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserAdminCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    Email = model.Email,
                    Gender = model.Gender,
                    UserName = model.UserName
                };
                var result = await _userManager.CreateAsync(user, model.UserName + "123");
                if (result.Succeeded)
                {
                    var memberRole = await _roleManager.FindByNameAsync("Member");
                    if (memberRole == null)
                    {
                        await _roleManager.CreateAsync(new()
                        {
                            Name = "Member",
                            CreatedTime = DateTime.Now,
                        });
                    }
                    return RedirectToAction("Index");
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View(model);
        }
        public async Task<IActionResult> AssignRole(int id)
        {
            var user = _userManager.Users.SingleOrDefault(x => x.Id == id);
            var userRoles = await _userManager.GetRolesAsync(user);
            var roles = _roleManager.Roles.ToList();
            RoleAssignSendModel model = new();
            List<RoleAssignListModel> list = new();
            foreach (var role in roles)
            {
                list.Add(new()
                {
                    Name = role.Name,
                    RoleId = role.Id,
                    Exist = userRoles.Contains(role.Name)
                });
            }
            model.Roles = list;
            model.UserId = id;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AssignRole(RoleAssignSendModel model)
        {
            var user = _userManager.Users.SingleOrDefault(x=>x.Id == model.UserId);
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var item in model.Roles)
            {
                if (item.Exist)
                {
                    if (userRoles.Contains(item.Name)) 
                        await _userManager.AddToRoleAsync(user, item.Name);
                }
                else
                {
                    if (userRoles.Contains(item.Name))
                        await _userManager.RemoveFromRoleAsync(user, item.Name);
                }
            }
            return RedirectToAction("Index");
        }
    }
}

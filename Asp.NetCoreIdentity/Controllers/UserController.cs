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
            var query = _userManager.Users;
            var users = _context.Users.Join(_context.UserRoles, user => user.Id, userrole => userrole.UserId, (user, userrole) => new
            {
                user,
                userrole
            }).Join(_context.Roles, two => two.userrole.RoleId, role => role.Id, (two, role) => new { two.user, two.userrole, role }).Where(x => x.userrole.RoleId != 3).select(x => new appuser()
            {
                id = x.user.id,
                accessfailedcount = x.user.accessfailedcount,
                concurrencystamp = x.user.concurrencystamp,
                email = x.user.email,
                gender = x.user.gender,
                imagepath = x.user.imagepath,
                lockoutenabled = x.user.lockoutenabled,
                lockoutend = x.user.lockoutend,
                normalizedemail = x.user.normalizedemail,
                normalizedusername = x.user.normalizedusername,
                passwordhash = x.user.passwordhash,
                phonenumber = x.user.phonenumber,
                username = x.user.username

            }).tolist();
            //var users = await _userManager.GetUsersInRoleAsync("Member");
            return View(users);
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
                    if (memberRole==null)
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
    }
}

﻿using Asp.NetCoreIdentity.Context;
using Asp.NetCoreIdentity.Entities;
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
        private readonly ApplicationContext _conext;

        public UserController(UserManager<AppUser> userManager, ApplicationContext conext)
        {
            _userManager = userManager;
            _conext = conext;
        }

        public async Task<IActionResult> Index()
        {
            var query = _userManager.Users;
            var users = _conext.Users.Join(_conext.UserRoles, user => user.Id, userRole => userRole.UserId, (user, userRole) => new
            {
                user,
                userRole
            }).Join(_conext.Roles,two=>two.userRole.RoleId,role=>role.Id,(two,role)=>new { two.user,two.userRole,role}).Where(x => x.userRole.RoleId != 3).Select(x => new AppUser()
            {
                Id = x.user.Id,
                AccessFailedCount = x.user.AccessFailedCount,
                ConcurrencyStamp = x.user.ConcurrencyStamp,
                Email = x.user.Email,
                Gender = x.user.Gender,
                ImagePath = x.user.ImagePath,
                LockoutEnabled = x.user.LockoutEnabled,
                LockoutEnd = x.user.LockoutEnd,
                NormalizedEmail = x.user.NormalizedEmail,
                NormalizedUserName = x.user.NormalizedUserName,
                PasswordHash = x.user.PasswordHash,
                PhoneNumber = x.user.PhoneNumber,
                UserName = x.user.UserName

            }).ToList();

            var usersII = await _userManager.GetUsersInRoleAsync("Member");

            return View();
        }
    }
}

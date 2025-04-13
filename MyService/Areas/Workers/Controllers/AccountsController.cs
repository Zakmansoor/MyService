﻿using Domin.Entity;
using Infarstuructre.Data;
using Infarstuructre.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyService.Areas.Workers.Controllers
{
    [Area("Workers")]

    [Authorize]
    public class AccountsController : Controller
    {
        #region Declaration
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly MyServiceDbContext _context;
        #endregion

        #region Constructor
        public AccountsController(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, MyServiceDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        #endregion

        #region Method
       

       
        [Authorize(Roles = "Admin")]
        public IActionResult Registers()
        {
            var model = new RegisterViewModel
            {
                NewRegister = new NewRegister(),
                Roles = _roleManager.Roles.OrderBy(x => x.Name).ToList(),
                Users = _context.VwUsers.OrderBy(x => x.Role).ToList() 
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]

        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Registers(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var file = HttpContext.Request.Form.Files;
                if (file.Count() > 0)
                {
                    string ImageName = Guid.NewGuid().ToString() + Path.GetExtension(file[0].FileName);
                    var fileStream = new FileStream(Path.Combine(@"wwwroot/", Helper.PathSaveImageuser, ImageName), FileMode.Create);
                    file[0].CopyTo(fileStream);
                    model.NewRegister.ImageUser = ImageName;
                }
                else
                {
                    model.NewRegister.ImageUser = model.NewRegister.ImageUser;
                }
                var user = new ApplicationUser
                {
                    Id = model.NewRegister.Id,
                    Name = model.NewRegister.Name,
                    UserName = model.NewRegister.Email,
                    Email = model.NewRegister.Email,
                    ActiveUser = model.NewRegister.ActiveUser,
                    ImageUser = model.NewRegister.ImageUser
                };
                if (user.Id == null)
                {
                    //Craete
                    user.Id = Guid.NewGuid().ToString();
                    var result = await _userManager.CreateAsync(user, model.NewRegister.Password);
                    if (result.Succeeded)
                    {
                        //Succsseded
                        var Role = await _userManager.AddToRoleAsync(user, Helper.Roles.Admin.ToString());
                        if (Role.Succeeded)
                            SessionMsg(Helper.Success, Resource.ResourceWeb.lbSave, Resource.ResourceWeb.lbNotSavedMsgUserRole);
                        else
                            SessionMsg(Helper.Error, Resource.ResourceWeb.lbNotSaved, Resource.ResourceWeb.lbNotSavedMsgUser);
                    }
                    else //Not Successeded
                        SessionMsg(Helper.Error, Resource.ResourceWeb.lbNotSaved, Resource.ResourceWeb.lbNotUpdateMsgUser);
                }
                else
                {
                    //Update
                    var userUpdate = await _userManager.FindByIdAsync(user.Id);
                    userUpdate.Id = model.NewRegister.Id;
                    userUpdate.Name = model.NewRegister.Name;
                    userUpdate.UserName = model.NewRegister.Email;
                    userUpdate.Email = model.NewRegister.Email;
                    userUpdate.ActiveUser = model.NewRegister.ActiveUser;
                    userUpdate.ImageUser = model.NewRegister.ImageUser;

                    var result = await _userManager.UpdateAsync(userUpdate);
                    if (result.Succeeded)
                    {
                        var oldRole = await _userManager.GetRolesAsync(userUpdate);
                        await _userManager.RemoveFromRolesAsync(userUpdate, oldRole);
                        var AddRole = await _userManager.AddToRoleAsync(userUpdate, model.NewRegister.RoleName);
                        if (AddRole.Succeeded)
                            SessionMsg(Helper.Success, Resource.ResourceWeb.lbUpdate, Resource.ResourceWeb.lbNotUpdateMsgUserRole);
                        else
                            SessionMsg(Helper.Error, Resource.ResourceWeb.lbNotUpdate, Resource.ResourceWeb.lbNotUpdateMsgUserRole);
                    }
                    else // Not Successeded
                        SessionMsg(Helper.Error, Resource.ResourceWeb.lbNotUpdate, Resource.ResourceWeb.lbNotUpdateMsgUser);
                }
                return RedirectToAction("Registers", "Accounts");
            }
            return RedirectToAction("Registers", "Accounts");
        }

        [Authorize(Roles = "Admin")]


        public async Task<IActionResult> DeleteUser(string userId)
        {
            var User = _userManager.Users.FirstOrDefault(x => x.Id == userId);

            if (User.ImageUser != null && User.ImageUser != Guid.Empty.ToString())
            {
                var PathImage = Path.Combine(@"wwwroot/", Helper.PathImageuser, User.ImageUser);
                if (System.IO.File.Exists(PathImage))
                    System.IO.File.Delete(PathImage);
            }
            if ((await _userManager.DeleteAsync(User)).Succeeded)
                return RedirectToAction("Registers", "Accounts");

            return RedirectToAction("Registers", "Accounts");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorize(Roles = "SuperAdmin,User")]
        public async Task<IActionResult> ChangePassword(RegisterViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.ChangePassword.Id);
            if (user != null)
            {
                await _userManager.RemovePasswordAsync(user);
                var AddNewPassword = await _userManager.AddPasswordAsync(user, model.ChangePassword.NewPassword);
                if (AddNewPassword.Succeeded)
                    SessionMsg(Helper.Success, Resource.ResourceWeb.lbSave, Resource.ResourceWeb.lbMsgSavedChangePassword);
                else
                    SessionMsg(Helper.Error, Resource.ResourceWeb.lbNotSaved, Resource.ResourceWeb.lbMsgNotSavedChangePassword);

                return RedirectToAction(nameof(Registers));
            }

            return RedirectToAction(nameof(Registers));

        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            
       {
                var user = await _userManager.FindByNameAsync(model.Eamil);
                if (!(await _userManager.IsInRoleAsync(user, Helper.Roles.Admin.ToString())))
                {
                    ViewBag.ErrorLogin = true;
                    return View(model);
                }
                var Result = await _signInManager.PasswordSignInAsync(model.Eamil,
                    model.Password, model.RememberMy, false);
                if (Result.Succeeded)
                    return RedirectToAction("Index", "Home", new { Area = "Workers" });
                else
                    ViewBag.ErrorLogin = false;
            }
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        private void SessionMsg(string MsgType, string Title, string Msg)
        {
            HttpContext.Session.SetString(Helper.MsgType, MsgType);
            HttpContext.Session.SetString(Helper.Title, Title);
            HttpContext.Session.SetString(Helper.Msg, Msg);
        }
        #endregion
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
using CarImagesWeb.Models;
using CarImagesWeb.Services;
using CarImagesWeb.ViewModels.AdministrationViewModels;
using CarImagesWeb.ViewModels.RoleViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarImagesWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly ITagsHandler _tagsHandler;
        private readonly IRoleTagRepository _roleTagRepository;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdministrationController(RoleManager<UserRole> roleManager, UserManager<ApplicationUser> userManager,
            ITagsHandler tagsHandler, IRoleTagRepository roleTagRepository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _tagsHandler = tagsHandler;
            _roleTagRepository = roleTagRepository;
        }

        public IActionResult ListUsers()
        {
            return View(_userManager.Users);
        }

        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            var editUserViewModel = new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = await _userManager.GetRolesAsync(user)
            };
            
            return View(editUserViewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user != null)
            {
                user.Email = model.Email;
                user.UserName = model.UserName;

                var results = await _userManager.UpdateAsync(user);

                if (results.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in results.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var results = await _userManager.DeleteAsync(user);

                if (results.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in results.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                
            }
            return RedirectToAction("ListUsers");
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            
            var userRole = new UserRole {Name = model.RoleName};
            var result = await _roleManager.CreateAsync(userRole);

            if (result.Succeeded) return RedirectToAction("ListRoles", "Administration");

            foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);

            return View(model);
        }

        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null) return NotFound();

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in await _userManager.Users.ToListAsync())
                if (await _userManager.IsInRoleAsync(user, role.Name))
                    model.Users.Add(user.UserName);

            model.Tags = await _tagsHandler.TagsInRole(role);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null) return NotFound();

            role.Name = model.RoleName;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded) return RedirectToAction("ListRoles");

            foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null) return NotFound();

            var model = new List<UserRoleViewModel>();

            foreach (var user in await _userManager.Users.ToListAsync())
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    Username = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                    userRoleViewModel.IsSelected = true;
                else
                    userRoleViewModel.IsSelected = false;

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(string roleId, List<UserRoleViewModel> model)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null) return NotFound();

            foreach (var userRoleViewModel in model)
            {
                var user = await _userManager.FindByIdAsync(userRoleViewModel.UserId);

                switch (userRoleViewModel.IsSelected)
                {
                    case true when !await _userManager.IsInRoleAsync(user, role.Name):
                        await _userManager.AddToRoleAsync(user, role.Name);
                        break;
                    case false when await _userManager.IsInRoleAsync(user, role.Name):
                        await _userManager.RemoveFromRoleAsync(user, role.Name);
                        break;
                    default:
                        continue;
                }
            }

            return RedirectToAction("EditRole", new {Id = roleId});
        }
        
        public async Task<IActionResult> DeleteRole(string id)
        {
            var user = await _roleManager.FindByIdAsync(id);
            if (user != null)
            {
                var results = await _roleManager.DeleteAsync(user);

                if (results.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in results.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                
            }
            return RedirectToAction("ListRoles");
        }
        
        [HttpGet]
        public async Task<IActionResult> EditTagsInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null) return NotFound();

            var model = new List<RoleTagViewModel>();

            foreach (Tag tag in await _tagsHandler.GetTagsAsync())
            {
                var roleTagViewModel = new RoleTagViewModel
                {
                    Tag = tag,
                    TagId = tag.Id
                };

                if (await _tagsHandler.IsTagInRole(tag.Id, role))
                    roleTagViewModel.IsSelected = true;
                else
                    roleTagViewModel.IsSelected = false;

                var tagRoleMapping = await _roleTagRepository.GetAsync(rt => rt.UserRoleId == role.Id && rt.TagId == tag.Id);
                if (tagRoleMapping != null)
                {
                    roleTagViewModel.AllowDownload = tagRoleMapping.AllowDownload;
                    roleTagViewModel.AllowUpload = tagRoleMapping.AllowUpload;
                }
                model.Add(roleTagViewModel);
            }

            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditTagsInRole(string roleId, List<RoleTagViewModel> model)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null) return NotFound();

            foreach (var roleTagViewModel in model)
            {
                var tagId = roleTagViewModel.TagId;
                var tag = await _tagsHandler.GetTagAsync(tagId);
                switch (roleTagViewModel.IsSelected)
                {
                    case true when !await _tagsHandler.IsTagInRole(tag, role):
                        await _tagsHandler.AddTagToRoleAsync(tag, role, roleTagViewModel.AllowUpload, roleTagViewModel.AllowDownload);
                        break;
                    case false when await _tagsHandler.IsTagInRole(tag, role):
                        await _tagsHandler.RemoveTagFromRoleAsync(tag, role);
                        break;
                    case true when await _tagsHandler.IsTagInRole(tag, role):
                        await _tagsHandler.UpdateTagToRole(tag, role, roleTagViewModel.AllowUpload, roleTagViewModel.AllowDownload);
                        break;
                    default:
                        continue;
                }
            }

            return RedirectToAction("EditRole", new {Id = roleId});
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var passwordResetLink = Url.Action("ResetPassword", "Account", new {email = user.Email, token = token}, Request.Scheme);
                return Redirect(passwordResetLink);
            }

            return Json("user not found");
        }
    }
}
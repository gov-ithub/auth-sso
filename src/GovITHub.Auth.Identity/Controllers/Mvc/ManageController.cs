using GovITHub.Auth.Common.Models;
using GovITHub.Auth.Common.Services;
using GovITHub.Auth.Identity.Helpers;
using GovITHub.Auth.Identity.Models.ManageViewModels;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GovITHub.Auth.Identity.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly IStringLocalizer<ManageController> _localizer;

        public ManageController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailService emailService,
        ISmsSender smsSender,
        ILoggerFactory loggerFactory,
        IStringLocalizer<ManageController> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<ManageController>();
            _localizer = localizer;
        }

        //
        // GET: /Manage/Index
        [HttpGet]
        public async Task<IActionResult> Index(ManageMessageId? message = null)
        {
            string userMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";
            if (!string.IsNullOrWhiteSpace(userMessage))
                ViewData["StatusMessage"] = _localizer[userMessage];

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await _userManager.GetLoginsAsync(user);
            var otherLogins = _signInManager.GetExternalAuthenticationSchemes().Where(auth => userLogins.All(ul => auth.AuthenticationScheme != ul.LoginProvider)).ToList();
            IEnumerable<string> emails = await GetUserEmails(user);
            IEnumerable<string> phones = await GetUserPhones(user);
            var model = new IndexViewModel
            {
                HasPassword = await _userManager.HasPasswordAsync(user),
                PhoneNumbers = phones,
                Emails = emails,
                TwoFactor = await _userManager.GetTwoFactorEnabledAsync(user),
                Logins = await _userManager.GetLoginsAsync(user),
                BrowserRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
            };
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel account)
        {
            ManageMessageId? message = ManageMessageId.Error;
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.RemoveLoginAsync(user, account.LoginProvider, account.ProviderKey);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    message = ManageMessageId.RemoveLoginSuccess;
                }
            }
            return RedirectToAction(nameof(ManageLogins), new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public IActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var mainPhoneNo = await _userManager.GetPhoneNumberAsync(user);
            IdentityResult ir = string.IsNullOrEmpty(mainPhoneNo) ?
                await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber) :
                await _userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.PhoneNumber, model.PhoneNumber));
            if (ir.Succeeded)
            {
                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.AddPhoneSuccess });
            }
            else
            {
                _logger.LogError("Error setting user main phone. Reason {0}", ir.Errors.Select(t => t.Description).Aggregate((s, t) => string.Format("{0};{1}", s, t)));
                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
            }
        }

        public IActionResult AddEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmail(AddEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var mainEmail = await _userManager.GetEmailAsync(user);
            IdentityResult ir = string.IsNullOrEmpty(mainEmail) ?
                await _userManager.SetEmailAsync(user, model.Email) :
                await _userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Email, model.Email));
            if (ir.Succeeded)
            {
                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.AddEmailSuccess });
            }
            else
            {
                _logger.LogError("Error setting user email. Reason {0}", ir.Errors.Select(t => t.Description).Aggregate((s, t) => string.Format("{0};{1}", s, t)));
                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
            }
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation(1, "User enabled two-factor authentication.");
            }
            return RedirectToAction(nameof(Index), "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, false);
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation(2, "User disabled two-factor authentication.");
            }
            return RedirectToAction(nameof(Index), "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        [HttpGet]
        public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
            // Send an SMS to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.AddPhoneSuccess });
                }
            }
            // If we got this far, something failed, redisplay the form
            ModelState.AddModelError(string.Empty, _localizer["Failed to verify phone number"]);
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePhoneNumber(AddPhoneNumberViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                if (user != null)
                {
                    var mainPhoneNo = await _userManager.GetPhoneNumberAsync(user);
                    IdentityResult result =
                            string.Compare(model.PhoneNumber, mainPhoneNo, true) == 0 ?
                                await _userManager.SetPhoneNumberAsync(user, null) :
                                await RemovePhoneNumberClaim(user, model.PhoneNumber);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index), new { Message = ManageMessageId.RemovePhoneSuccess });
                    }
                }
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveEmail(AddEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                if (user != null)
                {
                    var mainEmail = await _userManager.GetEmailAsync(user);
                    IdentityResult result =
                            string.Compare(model.Email, mainEmail, true) == 0 ?
                                await _userManager.SetEmailAsync(user, null) :
                                await RemoveEmailClaim(user, model.Email);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index), new { Message = ManageMessageId.RemoveEmailSuccess });
                    }
                }
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        //
        // GET: /Manage/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User changed their password successfully.");
                    return Request.IsAjaxRequest() ?
                        (IActionResult)Json(new { message = _localizer[Convert.ToString(ManageMessageId.ChangePasswordSuccess)] }) :
                        RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);
                return Request.IsAjaxRequest() ?
                    (IActionResult)Json(new { message = "Erori, erori" }) :
                    View(model);
            }
            return Request.IsAjaxRequest() ?
                (IActionResult)Json(new { message = _localizer[Convert.ToString(ManageMessageId.Error)] }) :
                RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        //
        // GET: /Manage/SetPassword
        [HttpGet]
        public IActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        [ActionNameAttribute("EditProfile")]
        [HttpGet]
        public async Task<ActionResult> EditProfileAsync()
        {
            var user = await GetCurrentUserAsync();
            var userClaims = await _userManager.GetClaimsAsync(user);
            EditProfileViewModel model = userClaims.ToViewModel();
            return View(model);
        }

        [ActionNameAttribute("EditProfile")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfileAsync(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                // update all user claims                
                var result = await UpdateUserClaims(user, model.ToClaims());
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        //GET: /Manage/ManageLogins
        [HttpGet]
        public async Task<IActionResult> ManageLogins(ManageMessageId? message = null)
        {
            string messageText =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.AddLoginSuccess ? "The external login was added."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            if (!string.IsNullOrEmpty(messageText))
                ViewData["StatusMessage"] = _localizer[messageText];
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await _userManager.GetLoginsAsync(user);
            var otherLogins = _signInManager.GetExternalAuthenticationSchemes().Where(auth => userLogins.All(ul => auth.AuthenticationScheme != ul.LoginProvider)).ToList();
            ViewData["ShowRemoveButton"] = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action("LinkLoginCallback", "Manage");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return Challenge(properties, provider);
        }

        //
        // GET: /Manage/LinkLoginCallback
        [HttpGet]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var info = await _signInManager.GetExternalLoginInfoAsync(await _userManager.GetUserIdAsync(user));
            if (info == null)
            {
                return RedirectToAction(nameof(ManageLogins), new { Message = ManageMessageId.Error });
            }
            var result = await _userManager.AddLoginAsync(user, info);
            var message = result.Succeeded ? ManageMessageId.AddLoginSuccess : ManageMessageId.Error;
            return RedirectToAction(nameof(ManageLogins), new { Message = message });
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, _localizer[error.Description]);
            }
        }

        private void AddMessage(ManageMessageId message)
        {
            ViewData["Message"] = _localizer[Convert.ToString("message")];
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            AddLoginSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error,
            AddEmailSuccess,
            RemoveEmailSuccess
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private async Task<IdentityResult> UpdateUserClaims(ApplicationUser user, ICollection<Claim> claims)
        {
            var profileClaims = claims.Select(t => t.Type);
            var originalClaims = await _userManager.GetClaimsAsync(user);
            foreach (var claim in claims)
            {
                foreach (var originalClaim in originalClaims) {
                    if (profileClaims.Contains(originalClaim.Type)) {
                        await _userManager.RemoveClaimAsync(user, originalClaim);
                    }
                }
            }
            return await _userManager.AddClaimsAsync(user, claims);
        }

        private async Task<IEnumerable<string>> GetUserPhones(ApplicationUser user)
        {
            List<string> phones = new List<string>();
            string mainPhone = await _userManager.GetPhoneNumberAsync(user);
            if (!string.IsNullOrEmpty(mainPhone))
                phones.Add(mainPhone);
            var phoneClaims = (await _userManager.GetClaimsAsync(user)).Where(t => t.Type == JwtClaimTypes.PhoneNumber);
            phones.AddRange(phoneClaims.Select(t => t.Value));
            return phones;
        }
        private async Task<IEnumerable<string>> GetUserEmails(ApplicationUser user)
        {
            List<string> emails = new List<string>();
            string mainEmail = await _userManager.GetEmailAsync(user);
            if (!string.IsNullOrEmpty(mainEmail))
                emails.Add(mainEmail);
            var emailClaims = (await _userManager.GetClaimsAsync(user)).Where(t => t.Type == JwtClaimTypes.Email);
            emails.AddRange(emailClaims.Select(t => t.Value));
            return emails;
        }

        private async Task<IdentityResult> RemovePhoneNumberClaim(ApplicationUser user, string phoneNumber)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var phoneClaim = claims.FirstOrDefault(t => t.Type == JwtClaimTypes.PhoneNumber && t.Value == phoneNumber);
            return phoneClaim != null ?
                await _userManager.RemoveClaimAsync(user, phoneClaim) :
                IdentityResult.Failed(new IdentityError[] { new IdentityError() { Code = "not.found", Description = "Not found" } });
        }
        private async Task<IdentityResult> RemoveEmailClaim(ApplicationUser user, string email)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var emailClaim = claims.FirstOrDefault(t => t.Type == JwtClaimTypes.Email && t.Value == email);
            return emailClaim != null ?
                await _userManager.RemoveClaimAsync(user, emailClaim) :
                IdentityResult.Failed(new IdentityError[] { new IdentityError() { Code = "not.found", Description = "Not found" } });
        }
        #endregion
    }
}

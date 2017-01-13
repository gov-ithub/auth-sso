using GovITHub.Auth.Identity.Helpers;
using GovITHub.Auth.Common.Models;
using GovITHub.Auth.Identity.Models.AccountViewModels;
using GovITHub.Auth.Common.Services;
using IdentityModel;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GovITHub.Auth.Common.Services.DeviceDetection;
using GovITHub.Auth.Common.Infrastructure.Extensions;
using IdentityServer4.ResponseHandling;
using System.Net;

namespace GovITHub.Auth.Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEmailService _emailService;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly ILoginDeviceManagementService _deviceManagementService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IEmailService emailService,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            IDeviceDetector deviceDetector,
            ILoginDeviceManagementService deviceManagementService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _emailService = emailService;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _deviceManagementService = deviceManagementService;
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        [ActionName("Login")]
        public async Task<IActionResult> LoginAsync(string returnUrl = null)
        {
            // clear Identity.External cookie
            if (Request.Cookies["Identity.External"] != null)
            {
                Response.Cookies.Delete("Identity.External");
            }

            await SetViewDataAsync(returnUrl);

            return View();
        }



        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ActionName("Login")]
        public async Task<IActionResult> LoginAsync(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            await SetViewDataAsync(returnUrl);

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    await AuditDeviceInfoAsync(model.Email);
                    return Request.IsAjaxRequest() ? Json(new { res = true, returnUrl = returnUrl }) : RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return Request.IsAjaxRequest() ?
                        Json(new { res = true, returnUrl = Url.Action("SendAction", new { returnUrl = returnUrl, rememberMe = model.RememberMe }) }) :
                        (IActionResult)RedirectToAction(nameof(SendCodeAsync), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    return Request.IsAjaxRequest() ?
                        Json(new { result = false, returnUrl = Url.Action("Lockout") }) :
                        (IActionResult)View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Request.IsAjaxRequest() ? Json(new { res = false, msg = "Utilizator sau parola invalida" }) : (IActionResult)View(model);
                }
            }
            // If we got this far, something failed, redisplay form
            return Request.IsAjaxRequest() ? Json(new { res = false }) : (IActionResult)View(model);
        }

        private async Task AuditDeviceInfoAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            await _deviceManagementService.RegisterDeviceLoginAsync(user.Id, Request.GetUserAgent());
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        [ActionName("Register")]
        public async Task<IActionResult> RegisterAsync(string returnUrl = null)
        {
            await SetViewDataAsync(returnUrl);

            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ActionName("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            await SetViewDataAsync(returnUrl);
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    await SendEmailConfirmationMessageAsync(model, user);
                    _logger.LogInformation(3, "User created a new account with password.");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    await AuditDeviceInfoAsync(model.Email);
                    return Request.IsAjaxRequest() ?
                        Json(new { res = true, returnUrl = returnUrl }) :
                        RedirectToLocal(returnUrl);
                }
                else
                {
                    AddErrors(result);
                    var msg = result.Errors.Select(t => string.Format("{0} : {1}", t.Code, t.Description)).
                        Aggregate((current, next) => current + " , " + next);
                    return Request.IsAjaxRequest() ?
                        Json(new { res = false, msg = msg }) : (IActionResult)View(model);
                }
            }
            // If we got this far, something failed, redisplay form
            return Request.IsAjaxRequest() ?
                Json(new { res = false, msg = "Eroare - date invalide." }) : (IActionResult)View(model);
        }


        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("LogOff")]
        public async Task<IActionResult> LogOffAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        /// <summary>

        /// Show logout page

        /// </summary>

        [HttpGet]
        [ActionName("Logout")]
        public async Task<IActionResult> LogoutAsync(string logoutId)
        {
            if (User.Identity.IsAuthenticated == false)
            {
                // if the user is not authenticated, then just show logged out page
                return await LogoutAsync(new LogoutViewModel { LogoutId = logoutId });
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // if the logout request is authenticated, it's safe to automatically sign-out
                return await LogoutAsync(new LogoutViewModel { LogoutId = logoutId });
            }

            var vm = new LogoutViewModel
            {
                LogoutId = logoutId
            };
            return View(vm);
        }



        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Logout")]
        public async Task<IActionResult> LogoutAsync(LogoutViewModel model)
        {
            var idp = User?.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
            if (idp != null && idp != "local")
            {
                string url = "/Account/Logout?logoutId=" + model.LogoutId;
                try
                {
                    await HttpContext.Authentication.SignOutAsync(idp, new AuthenticationProperties { RedirectUri = url });
                }
                catch (Exception ex)
                {
                    _logger.LogError("Cannot sign out! IDP : {0}. Reason : {1}", idp, ex);
                }
            }
            // delete authentication cookie

            await HttpContext.Authentication.SignOutAsync();
            // await HttpContext.Authentication.CustomHandleSignOutAsync(Request, Response, null);
            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(model.LogoutId);
            var vm = new LoggedOutViewModel
            {

                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = logout?.ClientId,
                SignOutIframeUrl = logout?.SignOutIFrameUrl
            };
            return View("LoggedOut", vm);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            if (returnUrl != null)
            {
                returnUrl = UrlEncoder.Default.Encode(returnUrl);
            }
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        [ActionName("ExternalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallbackAsync(string returnUrl)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction("Login");
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                await AuditDeviceInfoAsync(info.Principal.FindFirstValue(ClaimTypes.Email));
                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCodeAsync), new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            var registeredUser = await _userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));
            // If the user does not have an account, then ask the user to create an account.
            if (registeredUser == null)
            {
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = info.Principal.FindFirstValue(ClaimTypes.Email) });
            }
            // Otherwise, add the new login to the existing user.
            var addLoginResult = await _userManager.AddLoginAsync(registeredUser, info);
            if (addLoginResult.Succeeded)
            {
                await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
                await AuditDeviceInfoAsync(info.Principal.FindFirstValue(ClaimTypes.Email));
                return RedirectToLocal(returnUrl);
            }
            // At this point fallback to Login form.
            return RedirectToAction(nameof(LoginAsync));
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        [ActionName("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ActionName("ForgotPassword")]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                await _emailService.SendEmailAsync(model.Email, "Resetare parolă",
                   $"Resetați parola apăsând pe această legătură: <a href='{callbackUrl}'>reset</a>");
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ActionName("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        [ActionName("SendCode")]
        public async Task<ActionResult> SendCodeAsync(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ActionName("SendCode")]
        public async Task<IActionResult> SendCodeAsync(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;
            if (model.SelectedProvider == "Email")
            {
                await _emailService.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction(nameof(VerifyCodeAsync), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }


        //
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        [ActionName("VerifyCode")]
        public async Task<IActionResult> VerifyCodeAsync(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ActionName("VerifyCode")]
        public async Task<IActionResult> VerifyCodeAsync(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid code.");
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ActionName("ExternalLoginConfirmation")]
        public async Task<IActionResult> ExternalLoginConfirmationAsync(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        await AuditDeviceInfoAsync(info.Principal.FindFirstValue(ClaimTypes.Email));
                        _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        #region Helpers

        /// <summary>
        /// Set view data required for return url
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private async Task SetViewDataAsync(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["ShowOrigin"] = false;

            // set returnUrlQueryString
            if (!string.IsNullOrEmpty(returnUrl))
            {
                ViewData["ReturnUrlQ"] = "?returnUrl=" + WebUtility.UrlEncode(returnUrl);
                var auth = await _interaction.GetAuthorizationContextAsync(returnUrl);
                if (auth != null)
                {
                    var uri = new Uri(auth.RedirectUri);
                    ViewData["OriginUrl"] = string.Format("{0}://{1}", uri.Scheme, uri.Authority);
                    ViewData["ShowOrigin"] = true;
                }
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private async Task SendEmailConfirmationMessageAsync(RegisterViewModel model, ApplicationUser user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
            try
            {
                await _emailService.SendEmailAsync(model.Email, "Confirm your account",
                    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Email could not be sent! Reason : {0}", ex);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                if (!_interaction.IsValidReturnUrl(returnUrl))
                {
                    returnUrl = Uri.UnescapeDataString(returnUrl);
                }
                if (_interaction.IsValidReturnUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
            }
            return Redirect("~/");
        }
        #endregion

        
    }
}

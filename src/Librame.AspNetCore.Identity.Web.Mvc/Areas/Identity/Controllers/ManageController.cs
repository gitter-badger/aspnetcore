#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Librame.AspNetCore.Identity.Web.Controllers
{
    using AspNetCore.Identity.Builders;
    using AspNetCore.Identity.Web.Models;
    using AspNetCore.Identity.Web.Resources;
    using AspNetCore.Web;
    using AspNetCore.Web.Applications;
    using Extensions;
    using Extensions.Core.Services;
    using Extensions.Network.Services;

    /// <summary>
    /// �����������
    /// </summary>
    [Authorize]
    [GenericApplicationModel]
    [Area(IdentityRouteBuilderExtensions.AreaName)]
    [Route(IdentityRouteBuilderExtensions.Template)]
    public class ManageController<TUser> : ApplicationController<TUser>
        where TUser : class
    {
        [InjectionService]
        private ILogger<ManageController<TUser>> _logger = null;

        [InjectionService]
        private ISmsService _smsService = null;

        [InjectionService]
        private IOptions<IdentityBuilderOptions> _builderOptions = null;

        [InjectionService]
        private IOptions<IdentityOptions> _options = null;

        [InjectionService]
        private IHtmlLocalizer<RegisterViewResource> _registerLocalizer = null;

        [InjectionService]
        private IHtmlLocalizer<IndexViewResource> _indexLocalizer = null;

        [InjectionService]
        private IHtmlLocalizer<AddPhoneNumberViewResource> _addPhoneNumberLocalizer = null;

        [InjectionService]
        private IHtmlLocalizer<VerifyPhoneNumberViewResource> _verifyPhoneNumberLocalizer = null;

        [InjectionService]
        private IHtmlLocalizer<ExternalLoginsViewResource> _manageLoginsLocalizer = null;


        /// <summary>
        /// ����һ�� <see cref="ManageController{TUser}"/>��
        /// </summary>
        /// <param name="injection">������ <see cref="IInjectionService"/>��</param>
        public ManageController(IInjectionService injection)
            : base(injection)
        {
        }


        /// <summary>
        /// GET: /Manage/Index
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] = GetManageMessage(message);
            ViewBag.Localizer = _indexLocalizer;

            return await VerifyLoginUserActionResult(async user =>
            {
                ViewBag.IsEmailConfirmed = await UserManager.IsEmailConfirmedAsync(user).ConfigureAndResultAsync();

                ViewBag.Profile = new ProfileViewModel
                {
                    HasPassword = await UserManager.HasPasswordAsync(user).ConfigureAndResultAsync(),
                    PhoneNumber = await UserManager.GetPhoneNumberAsync(user).ConfigureAndResultAsync(),
                    TwoFactor = await UserManager.GetTwoFactorEnabledAsync(user).ConfigureAndResultAsync(),
                    Logins = await UserManager.GetLoginsAsync(user).ConfigureAndResultAsync(),
                    BrowserRemembered = await SignInManager.IsTwoFactorClientRememberedAsync(user).ConfigureAndResultAsync(),
                    AuthenticatorKey = await UserManager.GetAuthenticatorKeyAsync(user).ConfigureAndResultAsync()
                };

                var userName = await UserManager.GetUserNameAsync(user).ConfigureAndResultAsync();
                var email = await UserManager.GetEmailAsync(user).ConfigureAndResultAsync();

                return View(new UserViewModel
                {
                    Name = userName,
                    Email = email
                });
            })
            .ConfigureAndResultAsync();
        }

        /// <summary>
        /// POST: /Manage/Index
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UserViewModel model)
        {
            model.NotNull(nameof(model));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return await VerifyLoginUserActionResult(async user =>
            {
                ManageMessageId? message = ManageMessageId.Error;

                var email = await UserManager.GetEmailAsync(user).ConfigureAndResultAsync();
                if (model.Email != email)
                {
                    var setEmailResult = await UserManager.SetEmailAsync(user, model.Email).ConfigureAndResultAsync();
                    if (!setEmailResult.Succeeded)
                    {
                        var userId = await UserManager.GetUserIdAsync(user).ConfigureAndResultAsync();
                        throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                    }

                    // In our UI email and user name are one and the same, so when we update the email
                    // we need to update the user name.
                    var setUserNameResult = await UserManager.SetUserNameAsync(user, model.Email).ConfigureAndResultAsync();
                    if (!setUserNameResult.Succeeded)
                    {
                        var userId = await UserManager.GetUserIdAsync(user).ConfigureAndResultAsync();
                        throw new InvalidOperationException($"Unexpected error occurred setting name for user with ID '{userId}'.");
                    }
                }

                await SignInManager.RefreshSignInAsync(user).ConfigureAndWaitAsync();

                message = ManageMessageId.SetEmailSuccess;
                return RedirectToAction(nameof(Index), new { Message = message });
            })
            .ConfigureAndResultAsync();
        }


        /// <summary>
        /// POST: /Manage/RemoveLogin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
        {
            model.NotNull(nameof(model));

            return await VerifyLoginUserActionResult(async user =>
            {
                ManageMessageId? message = ManageMessageId.Error;

                var result = await UserManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey).ConfigureAndResultAsync();
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false).ConfigureAndWaitAsync();
                    message = ManageMessageId.RemoveLoginSuccess;
                }

                return RedirectToAction(nameof(ManageLogins), new { Message = message });
            })
            .ConfigureAndResultAsync();
        }


        /// <summary>
        /// GET: /Manage/AddPhoneNumber
        /// </summary>
        /// <returns></returns>
        public IActionResult AddPhoneNumber()
        {
            ViewBag.Localizer = _addPhoneNumberLocalizer;

            return View();
        }

        /// <summary>
        /// POST: /Manage/AddPhoneNumber
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public async Task<IActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            model.NotNull(nameof(model));

            ViewBag.Localizer = _addPhoneNumberLocalizer;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return await VerifyLoginUserActionResult(async user =>
            {
                // Generate the token and send it
                var code = await UserManager.GenerateChangePhoneNumberTokenAsync(user, model.Phone).ConfigureAndResultAsync();

                await _smsService.SendAsync(model.Phone,
                    _addPhoneNumberLocalizer.GetString(r => r.YourSecurityCodeIs).Value + code).ConfigureAndWaitAsync();

                return RedirectToAction(nameof(VerifyPhoneNumber), new { phoneNumber = model.Phone });
            })
            .ConfigureAndResultAsync();
        }


        /// <summary>
        /// POST: /Manage/ResetAuthenticatorKey
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticatorKey()
        {
            return await VerifyLoginUserActionResult(async user =>
            {
                await UserManager.ResetAuthenticatorKeyAsync(user).ConfigureAndResultAsync();
                _logger.LogInformation(1, "User reset authenticator key.");

                return RedirectToAction(nameof(Index));
            })
            .ConfigureAndResultAsync();
        }


        /// <summary>
        /// POST: /Manage/GenerateRecoveryCode
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateRecoveryCode()
        {
            return await VerifyLoginUserActionResult(async user =>
            {
                var dependency = Application.ServiceFactory.GetRequiredService<IdentityWebBuilderDependency>();

                var codes = await UserManager.GenerateNewTwoFactorRecoveryCodesAsync(user,
                    dependency.TwoFactorRecoveryCodeLength).ConfigureAndResultAsync();
                _logger.LogInformation(1, "User generated new recovery code.");

                return View("DisplayRecoveryCodes", new DisplayRecoveryCodesViewModel { Codes = codes });
            })
            .ConfigureAndResultAsync();
        }


        /// <summary>
        /// POST: /Manage/EnableTwoFactorAuthentication
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            return await VerifyLoginUserActionResult(async user =>
            {
                await UserManager.SetTwoFactorEnabledAsync(user, true).ConfigureAndWaitAsync();
                await SignInManager.SignInAsync(user, isPersistent: false).ConfigureAndWaitAsync();
                _logger.LogInformation(2, "User enabled two-factor authentication.");

                return RedirectToAction(nameof(Index));
            })
            .ConfigureAndResultAsync();
        }


        /// <summary>
        /// POST: /Manage/DisableTwoFactorAuthentication
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            return await VerifyLoginUserActionResult(async user =>
            {
                await UserManager.SetTwoFactorEnabledAsync(user, false).ConfigureAndWaitAsync();
                await SignInManager.SignInAsync(user, isPersistent: false).ConfigureAndWaitAsync();
                _logger.LogInformation(2, "User disabled two-factor authentication.");

                return RedirectToAction(nameof(Index));
            })
            .ConfigureAndResultAsync();
        }


        /// <summary>
        /// GET: /Manage/VerifyPhoneNumber
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            ViewBag.Localizer = _verifyPhoneNumberLocalizer;

            return await VerifyLoginUserActionResult(async user =>
            {
                await UserManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber).ConfigureAndWaitAsync();

                // Send an SMS to verify the phone number
                return phoneNumber == null
                    ? View("Error")
                    : View(new VerifyPhoneNumberViewModel { Phone = phoneNumber });
            })
            .ConfigureAndResultAsync();
        }

        /// <summary>
        /// POST: /Manage/VerifyPhoneNumber
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            model.NotNull(nameof(model));

            ViewBag.Localizer = _verifyPhoneNumberLocalizer;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return await VerifyLoginUserActionResult(async user =>
            {
                var result = await UserManager.ChangePhoneNumberAsync(user, model.Phone, model.Code).ConfigureAndResultAsync();
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false).ConfigureAndWaitAsync();
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.AddPhoneSuccess });
                }

                // If we got this far, something failed, redisplay the form
                ModelState.AddModelError(string.Empty, _verifyPhoneNumberLocalizer.GetString(r => r.Failed).Value);
                return View();
            })
            .ConfigureAndResultAsync();
        }


        /// <summary>
        /// GET: /Manage/RemovePhoneNumber
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePhoneNumber()
        {
            return await VerifyLoginUserActionResult(async user =>
            {
                var result = await UserManager.SetPhoneNumberAsync(user, null).ConfigureAndResultAsync();
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false).ConfigureAndWaitAsync();
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.RemovePhoneSuccess });
                }

                AddModelErrors(result);
                return View();
            })
            .ConfigureAndResultAsync();
        }


        /// <summary>
        /// GET: /Manage/ChangePassword
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ChangePassword()
        {
            ViewBag.BuilderOptions = _builderOptions.Value;
            ViewBag.Options = _options.Value;
            ViewBag.RegisterLocalizer = _registerLocalizer;

            return View();
        }

        /// <summary>
        /// POST: /Manage/ChangePassword
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            model.NotNull(nameof(model));

            ViewBag.BuilderOptions = _builderOptions.Value;
            ViewBag.Options = _options.Value;
            ViewBag.RegisterLocalizer = _registerLocalizer;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return await VerifyLoginUserActionResult(async user =>
            {
                var result = await UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword).ConfigureAndResultAsync();
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false).ConfigureAndWaitAsync();
                    _logger.LogInformation(3, "User changed their password successfully.");
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
                }

                AddModelErrors(result);
                return View(model);
            })
            .ConfigureAndResultAsync();
        }


        /// <summary>
        /// GET: /Manage/SetPassword
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SetPassword()
        {
            ViewBag.BuilderOptions = _builderOptions.Value;
            ViewBag.Options = _options.Value;
            ViewBag.RegisterLocalizer = _registerLocalizer;

            return View();
        }

        /// <summary>
        /// POST: /Manage/SetPassword
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            model.NotNull(nameof(model));

            ViewBag.BuilderOptions = _builderOptions.Value;
            ViewBag.Options = _options.Value;
            ViewBag.RegisterLocalizer = _registerLocalizer;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return await VerifyLoginUserActionResult(async user =>
            {
                var result = await UserManager.AddPasswordAsync(user, model.NewPassword).ConfigureAndResultAsync();
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false).ConfigureAndWaitAsync();
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.SetPasswordSuccess });
                }

                AddModelErrors(result);
                return View(model);
            })
            .ConfigureAndResultAsync();
        }


        /// <summary>
        /// GET: /Manage/ManageLogins
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ManageLogins(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] = GetManageMessage(message);
            ViewBag.Localizer = _manageLoginsLocalizer;

            return await VerifyLoginUserActionResult(async user =>
            {
                var userLogins = await UserManager.GetLoginsAsync(user).ConfigureAndResultAsync();
                var schemes = await SignInManager.GetExternalAuthenticationSchemesAsync().ConfigureAndResultAsync();
                var otherLogins = schemes.Where(auth => userLogins.All(ul => auth.Name != ul.LoginProvider)).ToList();

                ViewData["ShowRemoveButton"] = UserManager.HasPasswordAsync(user).Result || userLogins.Count > 1;

                return View(new ManageLoginsViewModel
                {
                    CurrentLogins = userLogins,
                    OtherLogins = otherLogins
                });
            })
            .ConfigureAndResultAsync();
        }


        /// <summary>
        /// POST: /Manage/LinkLogin
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> LinkLogin(string provider)
        {
            return VerifyLoginUserActionResult(user =>
            {
                // Request a redirect to the external login provider to link a login for the current user
                var redirectUrl = Url.Action("LinkLoginCallback", "Manage");
                var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider,
                    redirectUrl, UserManager.GetUserId(User));

                return Challenge(properties, provider);
            });
        }

        /// <summary>
        /// GET: /Manage/LinkLoginCallback
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback()
        {
            return await VerifyLoginUserActionResult(async user =>
            {
                var userId = await UserManager.GetUserIdAsync(user).ConfigureAndResultAsync();
                var info = await SignInManager.GetExternalLoginInfoAsync(userId).ConfigureAndResultAsync();
                if (info.IsNull())
                {
                    return RedirectToAction(nameof(ManageLogins), new { Message = ManageMessageId.Error });
                }

                var result = await UserManager.AddLoginAsync(user, info).ConfigureAndResultAsync();
                var message = result.Succeeded ? ManageMessageId.AddLoginSuccess : ManageMessageId.Error;
                return RedirectToAction(nameof(ManageLogins), new { Message = message });
            })
            .ConfigureAndResultAsync();
        }


        #region Helpers

        /// <summary>
        /// ������Ϣ��ʶ��
        /// </summary>
        public enum ManageMessageId
        {
            /// <summary>
            /// �����ֻ��ųɹ���
            /// </summary>
            AddPhoneSuccess,

            /// <summary>
            /// ���ӵ���ɹ���
            /// </summary>
            AddLoginSuccess,

            /// <summary>
            /// �޸�����ɹ���
            /// </summary>
            ChangePasswordSuccess,

            /// <summary>
            /// ��������ɹ���
            /// </summary>
            SetEmailSuccess,

            /// <summary>
            /// ����˫���ӳɹ���
            /// </summary>
            SetTwoFactorSuccess,

            /// <summary>
            /// ��������ɹ���
            /// </summary>
            SetPasswordSuccess,

            /// <summary>
            /// �Ƴ�����ɹ���
            /// </summary>
            RemoveLoginSuccess,

            /// <summary>
            /// �Ƴ��ֻ��ųɹ���
            /// </summary>
            RemovePhoneSuccess,

            /// <summary>
            /// ����
            /// </summary>
            Error
        }


        private string GetManageMessage(ManageMessageId? message = null)
        {
            if (message.IsNotNull())
                return _indexLocalizer.GetString(message.Value.AsEnumName());

            return string.Empty;
        }

        #endregion

    }
}

#region License

/* **************************************************************************************
 * Copyright (c) Librame Pang All rights reserved.
 * 
 * http://librame.net
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Librame.AspNetCore.Identity.UI.Pages.Account
{
    using AspNetCore.UI;
    using Extensions;
    using Extensions.Core;
    using Extensions.Data;

    /// <summary>
    /// �ⲿ����ȷ��ҳ��ģ�͡�
    /// </summary>
    [AllowAnonymous]
    [UiTemplateWithUser(typeof(ExternalLoginPageModel<>))]
    public class ExternalLoginConfirmationPageModel : PageModel
    {
        /// <summary>
        /// ����ģ�͡�
        /// </summary>
        [BindProperty]
        public ExternalLoginConfirmationViewModel Input { get; set; }

        /// <summary>
        /// �����ṩ����
        /// </summary>
        public string LoginProvider { get; set; }

        /// <summary>
        /// �������ӡ�
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// ������Ϣ��
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }


        /// <summary>
        /// ��ȡ������
        /// </summary>
        /// <returns>���� <see cref="IActionResult"/>��</returns>
        public virtual IActionResult OnGet()
            => throw new NotImplementedException();

        /// <summary>
        /// �ύ������
        /// </summary>
        /// <param name="provider">�������ṩ����</param>
        /// <param name="returnUrl">�����ķ������ӡ�</param>
        /// <returns>���� <see cref="IActionResult"/>��</returns>
        public virtual IActionResult OnPost(string provider, string returnUrl = null)
            => throw new NotImplementedException();

        /// <summary>
        /// �첽��ȡ�ص�������
        /// </summary>
        /// <param name="returnUrl">�����ķ������ӡ�</param>
        /// <param name="remoteError">������Զ�̴���</param>
        /// <returns>����һ������ <see cref="IActionResult"/> ���첽������</returns>
        public virtual Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
            => throw new NotImplementedException();

        /// <summary>
        /// �첽�ύȷ�Ϸ�����
        /// </summary>
        /// <param name="returnUrl">�����ķ������ӡ�</param>
        /// <returns>����һ������ <see cref="IActionResult"/> ���첽������</returns>
        public virtual Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
            => throw new NotImplementedException();
    }


    internal class ExternalLoginConfirmationPageModel<TUser> : ExternalLoginConfirmationPageModel
        where TUser : class, IGenId
    {
        private readonly SignInManager<TUser> _signInManager;
        private readonly UserManager<TUser> _userManager;
        private readonly IUserStore<TUser> _userStore;
        private readonly ILogger<ExternalLoginConfirmationPageModel> _logger;
        private readonly IExpressionStringLocalizer<ErrorMessageResource> _errorLocalizer;
        private readonly IdentityStoreIdentifier _storeIdentifier;


        public ExternalLoginConfirmationPageModel(
            SignInManager<TUser> signInManager,
            IUserStore<TUser> userStore,
            ILogger<ExternalLoginConfirmationPageModel> logger,
            IExpressionStringLocalizer<ErrorMessageResource> errorLocalizer,
            IdentityStoreIdentifier storeIdentifier)
        {
            _signInManager = signInManager;
            _userManager = signInManager.UserManager;
            _userStore = userStore;
            _logger = logger;
            _errorLocalizer = errorLocalizer;
            _storeIdentifier = storeIdentifier;
        }


        public override IActionResult OnGet()
        {
            return RedirectToPage("./Login");
        }

        public override IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public override async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = _errorLocalizer[r => r.FromExternalProvider, remoteError]?.ToString();
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = _errorLocalizer[r => r.LoadingExternalLogin]?.ToString();
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                LoginProvider = info.LoginProvider;

                Input = new ExternalLoginConfirmationViewModel();

                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input.Email = info.Principal.FindFirstValue(ClaimTypes.Email);
                }

                return Page();
            }
        }

        public override async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = _errorLocalizer[r => r.LoadingExternalLoginWhenConfirmation]?.ToString();
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.Id = await _storeIdentifier.GetUserIdAsync();

                var result = await _userManager.CreateUserByEmail(_userStore, Input.Email, password: null, user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            LoginProvider = info.LoginProvider;
            ReturnUrl = returnUrl;
            return Page();
        }

        private TUser CreateUser()
        {
            try
            {
                return typeof(TUser).EnsureCreate<TUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(TUser)}'. " +
                    $"Ensure that '{nameof(TUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the external login page in ~/Areas/Identity/Pages/Account/ExternalLogin.cshtml");
            }
        }

    }

}

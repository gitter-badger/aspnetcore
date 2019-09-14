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
using System.Threading.Tasks;

namespace Librame.AspNetCore.Identity.UI.Pages.Account
{
    using AspNetCore.UI;
    using Extensions.Core;

    /// <summary>
    /// �ָ������ҳ��ģ�͡�
    /// </summary>
    [AllowAnonymous]
    [ApplicationSiteTemplateWithUser(typeof(LoginWithRecoveryCodePageModel<>))]
    public class LoginWithRecoveryCodePageModel : PageModel
    {
        /// <summary>
        /// ����ģ�͡�
        /// </summary>
        [BindProperty]
        public LoginWithRecoveryCodeViewModel Input { get; set; }

        /// <summary>
        /// �������ӡ�
        /// </summary>
        public string ReturnUrl { get; set; }


        /// <summary>
        /// �첽��ȡ������
        /// </summary>
        /// <param name="returnUrl">�����ķ��� URL��</param>
        /// <returns>����һ������ <see cref="IActionResult"/> ���첽������</returns>
        public virtual Task<IActionResult> OnGetAsync(string returnUrl = null)
            => throw new NotImplementedException();

        /// <summary>
        /// �첽�ύ������
        /// </summary>
        /// <param name="returnUrl">�����ķ��� URL��</param>
        /// <returns>����һ������ <see cref="IActionResult"/> ���첽������</returns>
        public virtual Task<IActionResult> OnPostAsync(string returnUrl = null)
            => throw new NotImplementedException();
    }


    internal class LoginWithRecoveryCodePageModel<TUser> : LoginWithRecoveryCodePageModel
        where TUser: class
    {
        private readonly SignInManager<TUser> _signInManager;
        private readonly UserManager<TUser> _userManager;
        private readonly ILogger<LoginWithRecoveryCodePageModel> _logger;
        private readonly IExpressionStringLocalizer<ErrorMessageResource> _errorLocalizer;


        public LoginWithRecoveryCodePageModel(
            SignInManager<TUser> signInManager,
            UserManager<TUser> userManager,
            ILogger<LoginWithRecoveryCodePageModel> logger,
            IExpressionStringLocalizer<ErrorMessageResource> errorLocalizer)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _errorLocalizer = errorLocalizer;
        }


        public override async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            ReturnUrl = returnUrl;

            return Page();
        }

        public override async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);
            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", userId);
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID '{UserId}' account locked out.", userId);
                return RedirectToPage("./Lockout");
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", userId);
                ModelState.AddModelError(string.Empty, _errorLocalizer[r => r.InvalidRecoveryCodeEntered]?.ToString());
                return Page();
            }
        }

    }
}

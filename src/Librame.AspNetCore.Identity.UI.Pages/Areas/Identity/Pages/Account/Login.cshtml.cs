#region License

/* **************************************************************************************
 * Copyright (c) Librame Pang All rights reserved.
 * 
 * http://librame.net
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Librame.AspNetCore.Identity.UI.Pages.Account
{
    using AspNetCore.UI;
    using Extensions;
    using Extensions.Core;

    /// <summary>
    /// ����ҳ��ģ�͡�
    /// </summary>
    [AllowAnonymous]
    [UiTemplateWithUser(typeof(LoginPageModel<>))]
    public class LoginPageModel : PageModel
    {
        /// <summary>
        /// ������ͼģ�͡�
        /// </summary>
        [BindProperty]
        public LoginViewModel Input { get; set; }

        /// <summary>
        /// �ⲿ���뷽���б�
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        /// ���� URL��
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// ������Ϣ��
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }


        /// <summary>
        /// �첽��ȡ������
        /// </summary>
        /// <param name="returnUrl">�����ķ��� URL��</param>
        /// <returns>����һ���첽������</returns>
        public virtual Task OnGetAsync(string returnUrl = null)
            => throw new NotImplementedException();

        /// <summary>
        /// �첽�ύ������
        /// </summary>
        /// <param name="returnUrl">�����ķ��� URL��</param>
        /// <returns>����һ������ <see cref="IActionResult"/> ���첽������</returns>
        public virtual Task<IActionResult> OnPostAsync(string returnUrl = null)
            => throw new NotImplementedException();
    }


    internal class LoginPageModel<TUser> : LoginPageModel where TUser : class
    {
        private readonly SignInManager<TUser> _signInManager;
        private readonly ILogger<LoginPageModel> _logger;
        private readonly IExpressionStringLocalizer<ErrorMessageResource> _errorLocalizer;


        public LoginPageModel(SignInManager<TUser> signInManager,
            ILogger<LoginPageModel> logger,
            IExpressionStringLocalizer<ErrorMessageResource> errorLocalizer)
        {
            _signInManager = signInManager;
            _logger = logger;
            _errorLocalizer = errorLocalizer;
        }


        public override async Task OnGetAsync(string returnUrl = null)
        {
            if (ErrorMessage.IsNotNullOrEmpty())
                ModelState.AddModelError(string.Empty, ErrorMessage);

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public override async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, _errorLocalizer[r => r.InvalidLoginAttempt]?.ToString());
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

    }
}

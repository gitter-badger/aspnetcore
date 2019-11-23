#region License

/* **************************************************************************************
 * Copyright (c) Librame Pang All rights reserved.
 * 
 * http://librame.net
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Librame.AspNetCore.Identity.UI.Pages.Account.Manage
{
    using AspNetCore.UI;
    using Extensions;
    using Extensions.Core;

    /// <summary>
    /// �޸�����ҳ��ģ�͡�
    /// </summary>
    [GenericApplicationModel(typeof(ChangePasswordPageModel<>))]
    public class ChangePasswordPageModel : PageModel
    {
        /// <summary>
        /// ����һ�� <see cref="ChangePasswordPageModel"/>��
        /// </summary>
        /// <param name="registerLocalizer">������ <see cref="IHtmlLocalizer{RegisterViewResource}"/>��</param>
        /// <param name="builderOptions">������ <see cref="IOptions{IdentityBuilderOptions}"/>��</param>
        /// <param name="options">������ <see cref="IOptions{IdentityOptions}"/>��</param>
        public ChangePasswordPageModel(IHtmlLocalizer<RegisterViewResource> registerLocalizer,
            IOptions<IdentityBuilderOptions> builderOptions, IOptions<IdentityOptions> options)
        {
            RegisterLocalizer = registerLocalizer;
            BuilderOptions = builderOptions.Value;
            Options = options.Value;
        }


        /// <summary>
        /// ���ػ���Դ��
        /// </summary>
        public IHtmlLocalizer<RegisterViewResource> RegisterLocalizer { get; }

        /// <summary>
        /// ������ѡ�
        /// </summary>
        public IdentityBuilderOptions BuilderOptions { get; }

        /// <summary>
        /// ѡ�
        /// </summary>
        public IdentityOptions Options { get; }

        /// <summary>
        /// ����ģ�͡�
        /// </summary>
        [BindProperty]
        public ChangePasswordViewModel Input { get; set; }

        /// <summary>
        /// ״̬��Ϣ��
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }


        /// <summary>
        /// ��ȡ������
        /// </summary>
        /// <returns>����һ�� <see cref="Task{IActionResult}"/>��</returns>
        public virtual Task<IActionResult> OnGetAsync()
            => throw new NotImplementedException();

        /// <summary>
        /// �ύ������
        /// </summary>
        /// <returns>����һ�� <see cref="Task{IActionResult}"/>��</returns>
        public virtual Task<IActionResult> OnPostAsync()
            => throw new NotImplementedException();
    }


    internal class ChangePasswordPageModel<TUser> : ChangePasswordPageModel
        where TUser : class
    {
        private readonly SignInManager<TUser> _signInManager;
        private readonly UserManager<TUser> _userManager;
        private readonly ILogger<ChangePasswordPageModel> _logger;
        private readonly IStringLocalizer<StatusMessageResource> _statusLocalizer;


        public ChangePasswordPageModel(
            SignInManager<TUser> signInManager,
            ILogger<ChangePasswordPageModel> logger,
            IStringLocalizer<StatusMessageResource> statusLocalizer,
            IHtmlLocalizer<RegisterViewResource> registerLocalizer,
            IOptions<IdentityBuilderOptions> builderOptions,
            IOptions<IdentityOptions> options)
            : base(registerLocalizer, builderOptions, options)
        {
            _signInManager = signInManager;
            _userManager = signInManager.UserManager;
            _logger = logger;
            _statusLocalizer = statusLocalizer;
        }


        public override async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAndResultAsync();
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user).ConfigureAndResultAsync();
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        public override async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User).ConfigureAndResultAsync();
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword).ConfigureAndResultAsync();
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user).ConfigureAndWaitAsync();
            _logger.LogInformation("User changed their password successfully.");
            
            StatusMessage = _statusLocalizer.GetString(r => r.ChangePassword);

            return RedirectToPage();
        }
    }
}

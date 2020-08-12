#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Librame.AspNetCore.Identity.Web.Pages.Account.Manage
{
    using AspNetCore.Identity.Builders;
    using AspNetCore.Identity.Web.Models;
    using AspNetCore.Identity.Web.Resources;
    using AspNetCore.Web.Applications;
    using Extensions;
    using Extensions.Core.Services;

    /// <summary>
    /// �޸�����ҳ��ģ�͡�
    /// </summary>
    [GenericApplicationModel(typeof(IdentityGenericTypeDefinitionMapper),
        typeof(ChangePasswordPageModel<>))]
    public class ChangePasswordPageModel : ApplicationPageModel
    {
        /// <summary>
        /// ����һ�� <see cref="ChangePasswordPageModel"/>��
        /// </summary>
        /// <param name="injection">������ <see cref="IInjectionService"/>��</param>
        public ChangePasswordPageModel(IInjectionService injection)
            : base(injection)
        {
        }


        /// <summary>
        /// ���ػ���Դ��
        /// </summary>
        [InjectionService]
        public IHtmlLocalizer<RegisterViewResource> RegisterLocalizer { get; set; }

        /// <summary>
        /// ������ѡ�
        /// </summary>
        [InjectionService]
        public IOptions<IdentityBuilderOptions> BuilderOptions { get; set; }

        /// <summary>
        /// ѡ�
        /// </summary>
        [InjectionService]
        public IOptions<IdentityOptions> Options { get; set; }

        /// <summary>
        /// ����ģ�͡�
        /// </summary>
        [BindProperty]
        public ChangePasswordViewModel Input { get; set; }


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


    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class ChangePasswordPageModel<TUser> : ChangePasswordPageModel
        where TUser : class
    {
        [InjectionService]
        private SignInManager<TUser> _signInManager = null;

        [InjectionService]
        private ILogger<ChangePasswordPageModel> _logger = null;

        [InjectionService]
        private IStringLocalizer<StatusMessageResource> _statusLocalizer = null;


        private readonly UserManager<TUser> _userManager;


        public ChangePasswordPageModel(IInjectionService injection)
            : base(injection)
        {
            _userManager = _signInManager.UserManager;
        }


        public override async Task<IActionResult> OnGetAsync()
        {
            return await VerifyLoginUserActionResult(_userManager, async user =>
            {
                var hasPassword = await _userManager.HasPasswordAsync(user).ConfigureAwait();
                if (!hasPassword)
                {
                    return RedirectToPage("./SetPassword");
                }

                return Page();
            })
            .ConfigureAwait();
        }

        public override async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            return await VerifyLoginUserActionResult(_userManager, async user =>
            {
                var result = await _userManager.ChangePasswordAsync(user,
                    Input.OldPassword, Input.NewPassword).ConfigureAwait();

                if (!result.Succeeded)
                {
                    AddModelErrors(result);
                    return Page();
                }

                await _signInManager.RefreshSignInAsync(user).ConfigureAwait();
                _logger.LogInformation("User changed their password successfully.");

                StatusMessage = _statusLocalizer.GetString(r => r.ChangePassword);
                return RedirectToPage();
            })
            .ConfigureAwait();
        }

    }
}

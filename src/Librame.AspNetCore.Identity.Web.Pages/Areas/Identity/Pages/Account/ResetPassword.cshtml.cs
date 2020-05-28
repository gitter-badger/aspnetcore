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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Librame.AspNetCore.Identity.Web.Pages.Account
{
    using AspNetCore.Web;
    using Builders;
    using Extensions;
    using Models;
    using Resources;

    /// <summary>
    /// ��������ҳ��ģ�͡�
    /// </summary>
    [AllowAnonymous]
    [GenericApplicationModel(typeof(ResetPasswordPageModel<>))]
    public class ResetPasswordPageModel : PageModel
    {
        /// <summary>
        /// ����һ�� <see cref="RegisterPageModel"/> ʵ����
        /// </summary>
        /// <param name="registerLocalizer">������ <see cref="IHtmlLocalizer{RegisterViewResource}"/>��</param>
        /// <param name="builderOptions">������ <see cref="IOptions{IdentityBuilderOptions}"/>��</param>
        /// <param name="options">������ <see cref="IOptions{IdentityOptions}"/>��</param>
        protected ResetPasswordPageModel(IHtmlLocalizer<RegisterViewResource> registerLocalizer,
            IOptions<IdentityBuilderOptions> builderOptions, IOptions<IdentityOptions> options)
        {
            RegisterLocalizer = registerLocalizer;
            BuilderOptions = builderOptions.NotNull(nameof(builderOptions)).Value;
            Options = options.NotNull(nameof(options)).Value;
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
        public ResetPasswordViewModel Input { get; set; }


        /// <summary>
        /// ��ȡ������
        /// </summary>
        /// <param name="token">���������ơ�</param>
        /// <returns>���� <see cref="IActionResult"/>��</returns>
        public virtual IActionResult OnGet(string token = null)
            => throw new NotImplementedException();

        /// <summary>
        /// �첽�ύ������
        /// </summary>
        /// <returns>����һ������ <see cref="IActionResult"/> ���첽������</returns>
        public virtual Task<IActionResult> OnPostAsync()
            => throw new NotImplementedException();
    }


    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class ResetPasswordPageModel<TUser> : ResetPasswordPageModel where TUser : class
    {
        private readonly UserManager<TUser> _userManager;


        public ResetPasswordPageModel(
            UserManager<TUser> userManager,
            IHtmlLocalizer<RegisterViewResource> registerLocalizer,
            IOptions<IdentityBuilderOptions> builderOptions,
            IOptions<IdentityOptions> options)
            : base(registerLocalizer, builderOptions, options)
        {
            _userManager = userManager;
        }


        public override IActionResult OnGet(string token = null)
        {
            if (token == null)
            {
                return BadRequest("A token must be supplied for password reset.");
            }
            else
            {
                Input = new ResetPasswordViewModel
                {
                    Code = token
                };

                return Page();
            }
        }

        public override async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByNameAsync(Input.Email).ConfigureAndResultAsync();
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password).ConfigureAndResultAsync();
            if (result.Succeeded)
            {
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

    }
}

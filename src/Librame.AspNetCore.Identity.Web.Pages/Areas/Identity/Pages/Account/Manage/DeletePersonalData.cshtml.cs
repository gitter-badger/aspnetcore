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
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Librame.AspNetCore.Identity.Web.Pages.Account.Manage
{
    using AspNetCore.Identity.Web.Models;
    using AspNetCore.Identity.Web.Resources;
    using AspNetCore.Web.Applications;
    using Extensions;

    /// <summary>
    /// ɾ����������ҳ��ģ�͡�
    /// </summary>
    [GenericApplicationModel(typeof(IdentityGenericTypeDefinitionMapper),
        typeof(DeletePersonalDataPageModel<>))]
    public class DeletePersonalDataPageModel : PageModel
    {
        /// <summary>
        /// ����ģ�͡�
        /// </summary>
        [BindProperty]
        public DeletePersonalDataViewModel Input { get; set; }

        /// <summary>
        /// ǿ�����롣
        /// </summary>
        public bool RequirePassword { get; set; }


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
    internal class DeletePersonalDataPageModel<TUser> : DeletePersonalDataPageModel
        where TUser: class
    {
        private readonly UserManager<TUser> _userManager;
        private readonly SignInManager<TUser> _signInManager;
        private readonly ILogger<DeletePersonalDataPageModel> _logger;
        private readonly IStringLocalizer<ErrorMessageResource> _errorLocalizer;


        public DeletePersonalDataPageModel(
            SignInManager<TUser> signInManager,
            ILogger<DeletePersonalDataPageModel> logger,
            IStringLocalizer<ErrorMessageResource> errorLocalizer)
        {
            _signInManager = signInManager;
            _userManager = signInManager.UserManager;
            _logger = logger;
            _errorLocalizer = errorLocalizer;
        }


        public override async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait();
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user).ConfigureAwait();
            return Page();
        }

        [SuppressMessage("Globalization", "CA1303:�벻Ҫ���ı���Ϊ���ػ���������", Justification = "<����>")]
        public override async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait();
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user).ConfigureAwait();
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password).ConfigureAwait())
                {
                    ModelState.AddModelError(string.Empty, _errorLocalizer.GetString(r => r.PasswordNotCorrect));
                    return Page();
                }
            }

            var result = await _userManager.DeleteAsync(user).ConfigureAwait();
            var userId = await _userManager.GetUserIdAsync(user).ConfigureAwait();
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleteing user with ID '{userId}'.");
            }

            await _signInManager.SignOutAsync().ConfigureAwait();

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return Redirect("~/");
        }
    }
}

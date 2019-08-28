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
using System;
using System.Threading.Tasks;

namespace Librame.AspNetCore.Identity.UI.Pages.Account
{
    using AspNetCore.UI;

    /// <summary>
    /// ��������ҳ��ģ�͡�
    /// </summary>
    [AllowAnonymous]
    [UiTemplateWithUser(typeof(ResetPasswordPageModel<>))]
    public class ResetPasswordPageModel : PageModel
    {
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


    internal class ResetPasswordPageModel<TUser> : ResetPasswordPageModel where TUser : class
    {
        private readonly UserManager<TUser> _userManager;


        public ResetPasswordPageModel(UserManager<TUser> userManager)
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

            var user = await _userManager.FindByNameAsync(Input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
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

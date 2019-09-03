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
    /// ȷ�ϵ���ҳ��ģ�͡�
    /// </summary>
    [AllowAnonymous]
    [UiTemplateWithUser(typeof(ConfirmEmailPageModel<>))]
    public class ConfirmEmailPageModel : PageModel
    {
        /// <summary>
        /// �첽��ȡ������
        /// </summary>
        /// <param name="userId">�������û���ʶ��</param>
        /// <param name="token">���������ơ�</param>
        /// <returns>����һ������ <see cref="IActionResult"/> ���첽������</returns>
        public virtual Task<IActionResult> OnGetAsync(string userId, string token)
            => throw new NotImplementedException();
    }


    internal class ConfirmEmailPageModel<TUser> : ConfirmEmailPageModel
        where TUser : class
    {
        private readonly UserManager<TUser> _userManager;


        public ConfirmEmailPageModel(UserManager<TUser> userManager)
        {
            _userManager = userManager;
        }


        public override async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Error confirming email for user with ID '{userId}':");
            }

            return Page();
        }
    }
}

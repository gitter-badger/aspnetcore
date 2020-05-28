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
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;

namespace Librame.AspNetCore.Identity.Web.Pages.Account
{
    /// <summary>
    /// ��������ȷ��ģ�͡�
    /// </summary>
    [AllowAnonymous]
    public class ResetPasswordConfirmationPageModel : PageModel
    {
        /// <summary>
        /// ��ȡ������
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void OnGet()
        {
        }

    }
}

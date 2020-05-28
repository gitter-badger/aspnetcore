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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Librame.AspNetCore.Identity.Web.Pages.Examples
{
    using AspNetCore.Web.Models;

    /// <summary>
    /// ����ҳ��ģ�͡�
    /// </summary>
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorPageModel : PageModel
    {
        /// <summary>
        /// ��ͼģ�͡�
        /// </summary>
        public ErrorViewModel ViewModel
            => new ErrorViewModel();

        /// <summary>
        /// ��ȡ������
        /// </summary>
        public void OnGet()
        {
            ViewModel.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }

    }
}

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
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Librame.AspNetCore.Identity.Web.Pages.Account
{
    using AspNetCore.Web.Applications;
    using Extensions;

    /// <summary>
    /// �ǳ�ҳ��ģ�͡�
    /// </summary>
    [AllowAnonymous]
    [GenericApplicationModel(typeof(IdentityGenericTypeDefinitionMapper),
        typeof(LogoutPageModel<>))]
    public class LogoutPageModel : PageModel
    {
        /// <summary>
        /// ��ȡ������
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void OnGet()
        {
        }


        /// <summary>
        /// �첽�ύ������
        /// </summary>
        /// <param name="returnUrl">�����ķ��� URL��</param>
        /// <returns>����һ������ <see cref="IActionResult"/> ���첽������</returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings")]
        public virtual Task<IActionResult> OnPost(string returnUrl = null)
            => throw new NotImplementedException();
    }


    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class LogoutPageModel<TUser> : LogoutPageModel where TUser : class
    {
        private readonly SignInManager<TUser> _signInManager;
        private readonly ILogger<LogoutPageModel> _logger;


        public LogoutPageModel(SignInManager<TUser> signInManager,
            ILogger<LogoutPageModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [SuppressMessage("Globalization", "CA1303:�벻Ҫ���ı���Ϊ���ػ���������", Justification = "<����>")]
        public override async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync().ConfigureAwait();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage();
            }
        }
    }
}

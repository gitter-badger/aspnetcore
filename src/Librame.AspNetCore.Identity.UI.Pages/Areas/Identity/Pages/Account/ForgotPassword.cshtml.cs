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
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Librame.AspNetCore.Identity.UI.Pages.Account
{
    using AspNetCore.UI;
    using Extensions.Network;
    using Models;

    /// <summary>
    /// ������������ҳ��ģ�͡�
    /// </summary>
    [AllowAnonymous]
    [PageApplicationModelWithUser(typeof(ForgotPasswordPageModel<>))]
    public abstract class AbstractForgotPasswordPageModel : PageModel
    {
        /// <summary>
        /// ����һ�� <see cref="AbstractForgotPasswordPageModel"/> ʵ����
        /// </summary>
        /// <param name="localizer">������ <see cref="IExpressionHtmlLocalizer{ForgotPasswordViewResource}"/>��</param>
        protected AbstractForgotPasswordPageModel(IExpressionHtmlLocalizer<ForgotPasswordViewResource> localizer)
        {
            Localizer = localizer;
        }


        /// <summary>
        /// ���ػ���Դ��
        /// </summary>
        public IExpressionHtmlLocalizer<ForgotPasswordViewResource> Localizer { get; set; }

        /// <summary>
        /// ����ģ�͡�
        /// </summary>
        [BindProperty]
        public ForgotPasswordViewModel Input { get; set; }

        /// <summary>
        /// ֧���û����䡣
        /// </summary>
        public bool SupportsUserEmail { get; set; }


        /// <summary>
        /// ��ȡ������
        /// </summary>
        public virtual void OnGet()
            => throw new NotImplementedException();

        /// <summary>
        /// �첽�ύ������
        /// </summary>
        /// <returns>����һ������ <see cref="IActionResult"/> ���첽������</returns>
        public virtual Task<IActionResult> OnPostAsync()
            => throw new NotImplementedException();
    }


    internal class ForgotPasswordPageModel<TUser> : AbstractForgotPasswordPageModel where TUser : class
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IEmailService _emailService;

        public ForgotPasswordPageModel(
            UserManager<TUser> userManager,
            IEmailService emailSender,
            IExpressionHtmlLocalizer<ForgotPasswordViewResource> localizer)
            : base(localizer)
        {
            _userManager = userManager;
            _emailService = emailSender;
        }


        public override void OnGet()
        {
            SupportsUserEmail = _userManager.SupportsUserEmail;
        }

        public override async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { token },
                    protocol: Request.Scheme);

                await _emailService.SendAsync(
                    Input.Email,
                    Localizer[r => r.ResetPassword]?.Value,
                    Localizer[r => r.ResetPasswordFormat, HtmlEncoder.Default.Encode(callbackUrl)]?.Value);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }

}

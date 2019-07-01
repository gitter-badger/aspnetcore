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
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Librame.AspNetCore.Identity.UI.Pages.Account.Manage
{
    using AspNetCore.UI;
    using Models;
    using Extensions.Core;
    using Extensions.Network;

    /// <summary>
    /// ������ҳҳ��ģ�͡�
    /// </summary>
    [ThemepackTemplate(typeof(IndexPageModel<>))]
    public abstract class AbstractIndexPageModel : PageModel
    {
        /// <summary>
        /// �Ƿ���ȷ�����䡣
        /// </summary>
        public bool IsEmailConfirmed { get; set; }

        /// <summary>
        /// �Ƿ���ȷ�ϵ绰��
        /// </summary>
        public bool IsPhoneConfirmed { get; set; }

        /// <summary>
        /// ״̬��Ϣ��
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        /// ����ģ�͡�
        /// </summary>
        [BindProperty]
        public IndexViewModel Input { get; set; }


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

        /// <summary>
        /// �ύ����������֤�뷽����
        /// </summary>
        /// <returns></returns>
        public virtual Task<IActionResult> OnPostSendVerificationEmailAsync()
            => throw new NotImplementedException();

        /// <summary>
        /// �ύ�����ֻ���֤�뷽����
        /// </summary>
        /// <returns></returns>
        public virtual Task<IActionResult> OnPostSendVerificationPhoneAsync()
            => throw new NotImplementedException();
    }

    internal class IndexPageModel<TUser> : AbstractIndexPageModel where TUser : class
    {
        private readonly SignInManager<TUser> _signInManager;
        private readonly UserManager<TUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly IExpressionStringLocalizer<RegisterViewResource> _registerLocalizer;
        private readonly IExpressionStringLocalizer<StatusMessageResource> _statusLocalizer;

        public IndexPageModel(SignInManager<TUser> signInManager,
            IEmailService emailService,
            ISmsService smsService,
            IExpressionStringLocalizer<RegisterViewResource> registerLocalizer,
            IExpressionStringLocalizer<StatusMessageResource> statusLocalizer)
        {
            _signInManager = signInManager;
            _userManager = signInManager.UserManager;
            _emailService = emailService;
            _smsService = smsService;
            _registerLocalizer = registerLocalizer;
            _statusLocalizer = statusLocalizer;
        }

        public override async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            
            Input = new IndexViewModel
            {
                Name = userName,
                Email = email,
                Phone = phoneNumber
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            IsPhoneConfirmed = await _userManager.IsPhoneNumberConfirmedAsync(user);

            return Page();
        }

        public override async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                }

                // In our UI email and user name are one and the same, so when we update the email
                // we need to update the user name.
                var setUserNameResult = await _userManager.SetUserNameAsync(user, Input.Email);
                if (!setUserNameResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting name for user with ID '{userId}'.");
                }
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.Phone != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.Phone);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = _statusLocalizer[r => r.ProfileUpdated]?.ToString();

            return RedirectToPage();
        }

        public override async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            
            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId, token },
                protocol: Request.Scheme);

            await _emailService.SendAsync(
                email,
                _registerLocalizer[r => r.ConfirmYourEmail]?.ToString(),
                _registerLocalizer[r => r.ConfirmYourEmailFormat, HtmlEncoder.Default.Encode(callbackUrl)]?.ToString());

            StatusMessage = _statusLocalizer[r => r.VerificationEmailSent]?.ToString();

            return RedirectToPage();
        }

        public override async Task<IActionResult> OnPostSendVerificationPhoneAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            //var callbackUrl = Url.Page(
            //    "/Account/ConfirmPhone",
            //    pageHandler: null,
            //    values: new { userId, token },
            //    protocol: Request.Scheme);

            //await _smsSender.SendAsync(token);

            //await _emailSender.SendAsync(
            //    email,
            //    _registerLocalizer[r => r.ConfirmYourEmail]?.ToString(),
            //    _registerLocalizer[r => r.ConfirmYourEmailFormat, HtmlEncoder.Default.Encode(callbackUrl)]?.ToString());

            StatusMessage = _statusLocalizer[r => r.VerificationSmsSent];

            return RedirectToPage();
        }

    }
}

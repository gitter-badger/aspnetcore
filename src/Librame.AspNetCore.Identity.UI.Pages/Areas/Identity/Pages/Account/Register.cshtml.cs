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
using Microsoft.Extensions.Logging;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Librame.AspNetCore.Identity.UI.Pages.Account
{
    using AspNetCore.UI;
    using Extensions;
    using Extensions.Data;
    using Extensions.Network;

    /// <summary>
    /// ע��ҳ��ģ�͡�
    /// </summary>
    [AllowAnonymous]
    [UiTemplateWithUser(typeof(RegisterPageModel<>))]
    public class RegisterPageModel : PageModel
    {
        /// <summary>
        /// ����һ�� <see cref="RegisterPageModel"/> ʵ����
        /// </summary>
        /// <param name="localizer">������ <see cref="IExpressionHtmlLocalizer{RegisterViewResource}"/>��</param>
        protected RegisterPageModel(IExpressionHtmlLocalizer<RegisterViewResource> localizer)
        {
            Localizer = localizer;
        }


        /// <summary>
        /// ���ػ���Դ��
        /// </summary>
        public IExpressionHtmlLocalizer<RegisterViewResource> Localizer { get; set; }

        /// <summary>
        /// ע����ͼģ�͡�
        /// </summary>
        [BindProperty]
        public RegisterViewModel Input { get; set; }

        /// <summary>
        /// ���� URL��
        /// </summary>
        public string ReturnUrl { get; set; }


        /// <summary>
        /// ��ȡ������
        /// </summary>
        /// <param name="returnUrl">�����ķ��� URL��</param>
        public virtual void OnGet(string returnUrl = null)
            => throw new NotImplementedException();

        /// <summary>
        /// �첽�ύ������
        /// </summary>
        /// <param name="returnUrl">�����ķ��� URL��</param>
        /// <returns>����һ������ <see cref="IActionResult"/> ���첽������</returns>
        public virtual Task<IActionResult> OnPostAsync(string returnUrl = null)
            => throw new NotImplementedException();
    }


    internal class RegisterPageModel<TUser> : RegisterPageModel
        where TUser : class, IGenId
    {
        private readonly SignInManager<TUser> _signInManager;
        private readonly UserManager<TUser> _userManager;
        private readonly IUserStore<TUser> _userStore;
        private readonly ILogger<LoginPageModel> _logger;
        private readonly IEmailService _emailService;
        private readonly IdentityStoreIdentifier _storeIdentifier;


        public RegisterPageModel(
            SignInManager<TUser> signInManager,
            IUserStore<TUser> userStore,
            ILogger<LoginPageModel> logger,
            IEmailService emailService,
            IExpressionHtmlLocalizer<RegisterViewResource> localizer,
            IdentityStoreIdentifier storeIdentifier)
            : base(localizer)
        {
            _signInManager = signInManager;
            _userManager = signInManager.UserManager;
            _userStore = userStore;
            _logger = logger;
            _emailService = emailService;
            _storeIdentifier = storeIdentifier;
        }


        public override void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public override async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.Id = await _storeIdentifier.GetUserIdAsync();

                var result = await _userManager.CreateUserByEmail(_userStore, Input.Email, Input.Password, user);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code },
                        protocol: Request.Scheme);

                    await _emailService.SendAsync(Input.Email,
                        Localizer[r => r.ConfirmYourEmail]?.Value,
                        Localizer[r => r.ConfirmYourEmailFormat, HtmlEncoder.Default.Encode(callbackUrl)]?.Value);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User created a new account with password.");

                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private TUser CreateUser()
        {
            try
            {
                return typeof(TUser).EnsureCreate<TUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(TUser)}'. " +
                    $"Ensure that '{nameof(TUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in ~/Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

    }
}

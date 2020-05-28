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
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Librame.AspNetCore.Identity.Web.Pages.Account.Manage
{
    using AspNetCore.Web;
    using Builders;
    using Extensions;
    using Models;
    using Resources;

    /// <summary>
    /// ������֤��ҳ��ģ�͡�
    /// </summary>
    [GenericApplicationModel(typeof(EnableAuthenticatorPageModel<>))]
    public class EnableAuthenticatorPageModel : PageModel
    {
        /// <summary>
        /// ������Կ��
        /// </summary>
        public string SharedKey { get; set; }

        /// <summary>
        /// ��֤�� URI��
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string AuthenticatorUri { get; set; }

        /// <summary>
        /// �ָ��뼯�ϡ�
        /// </summary>
        [TempData]
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] RecoveryCodes { get; set; }

        /// <summary>
        /// ״̬��Ϣ��
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        /// ����ģ�͡�
        /// </summary>
        [BindProperty]
        public EnableAuthenticatorViewModel Input { get; set; }


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
    internal class EnableAuthenticatorPageModel<TUser> : EnableAuthenticatorPageModel
        where TUser : class
    {
        private readonly UserManager<TUser> _userManager;
        private readonly UrlEncoder _urlEncoder;
        private readonly ILogger<EnableAuthenticatorPageModel> _logger;
        private readonly IStringLocalizer<StatusMessageResource> _statusLocalizer;
        private readonly IStringLocalizer<ErrorMessageResource> _errorLocalizer;
        private readonly IdentityBuilderOptions _options;


        public EnableAuthenticatorPageModel(
            UserManager<TUser> userManager,
            UrlEncoder urlEncoder,
            ILogger<EnableAuthenticatorPageModel> logger,
            IStringLocalizer<StatusMessageResource> statusLocalizer,
            IStringLocalizer<ErrorMessageResource> errorLocalizer,
            IOptions<IdentityBuilderOptions> options)
        {
            _userManager = userManager;
            _urlEncoder = urlEncoder;
            _logger = logger;
            _statusLocalizer = statusLocalizer;
            _errorLocalizer = errorLocalizer;
            _options = options.Value;
        }


        public override async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAndResultAsync();
            if (user == null)
            {
                NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadSharedKeyAndQrCodeUriAsync(user).ConfigureAndWaitAsync();

            return Page();
        }

        public override async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAndResultAsync();
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user).ConfigureAndWaitAsync();
                return Page();
            }

            // Strip spaces and hypens
            var verificationCode = Input.TwoFactorCode
                .Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode).ConfigureAndResultAsync();

            if (!is2faTokenValid)
            {
                ModelState.AddModelError(nameof(Input.TwoFactorCode), _errorLocalizer.GetString(r => r.InvalidVerificationCode));
                await LoadSharedKeyAndQrCodeUriAsync(user).ConfigureAndWaitAsync();
                return Page();
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true).ConfigureAndResultAsync();
            var userId = await _userManager.GetUserIdAsync(user).ConfigureAndResultAsync();
            _logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", userId);

            StatusMessage = _statusLocalizer.GetString(r => r.EnableAuthenticator);

            if (await _userManager.CountRecoveryCodesAsync(user).ConfigureAndResultAsync() == 0)
            {
                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10).ConfigureAndResultAsync();
                RecoveryCodes = recoveryCodes.ToArray();
                return RedirectToPage("./ShowRecoveryCodes");
            }
            else
            {
                return RedirectToPage("./TwoFactorAuthentication");
            }
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(TUser user)
        {
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user).ConfigureAndResultAsync();
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user).ConfigureAndResultAsync();
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user).ConfigureAndResultAsync();
            }

            SharedKey = FormatKey(unformattedKey);

            var email = await _userManager.GetEmailAsync(user).ConfigureAndResultAsync();
            AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey);
        }

        private static string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            // "Microsoft.AspNetCore.Identity.UI"
            var schemeName = _urlEncoder.Encode("Librame.AspNetCore.Identity.Web");
            email = _urlEncoder.Encode(email);

            var descriptor = new IdentityAuthenticatorDescriptor(schemeName, email, unformattedKey);
            var qrCodeUri = _options.AuthenticatorUriFactory?.Invoke(descriptor);
            if (qrCodeUri.IsEmpty())
                qrCodeUri = descriptor.BuildOtpAuthUriString();

            return qrCodeUri;
        }
    }
}

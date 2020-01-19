using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Librame.AspNetCore.Identity.Web.Pages.Examples
{
    using Extensions;
    using Extensions.Core.Services;
    using Extensions.Network.Services;
    using Resources;
    using Stores;

    /// <summary>
    /// ��ҳģ�͡�
    /// </summary>
    public class IndexPageModel : PageModel
    {
        [InjectionService]
        private IEmailService _emailService = null;

        //[InjectionService]
        //private ISmsService _smsService = null;

        [InjectionService]
        private IStringLocalizer<RegisterViewResource> _registerLocalizer = null;

        [InjectionService]
        private IStringLocalizer<StatusMessageResource> _statusLocalizer = null;

        [InjectionService]
        private SignInManager<DefaultIdentityUser<string>> _signInManager = null;

        private readonly UserManager<DefaultIdentityUser<string>> _userManager = null;


        /// <summary>
        /// ����һ�� <see cref="IndexPageModel"/>��
        /// </summary>
        /// <param name="injectionService">������ <see cref="IInjectionService"/>��</param>
        public IndexPageModel(IInjectionService injectionService)
        {
            injectionService.Inject(this);

            _userManager = _signInManager.UserManager;
        }


        /// <summary>
        /// �û�����
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// �Ƿ����ȷ�ϡ�
        /// </summary>
        public bool IsEmailConfirmed { get; set; }

        /// <summary>
        /// ״̬��Ϣ��
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        /// ����ģ�͡�
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }
        
        /// <summary>
        /// ����ģ�͡�
        /// </summary>
        //[ViewResourceMapping("Index")]
        public class InputModel : Account.Manage.IndexPageModel.InputModel
        {
            [Required(ErrorMessageResourceName = nameof(RequiredAttribute), ErrorMessageResourceType = typeof(ErrorMessageResource))]
            [Range(0, 199, ErrorMessageResourceName = nameof(RangeAttribute), ErrorMessageResourceType = typeof(ErrorMessageResource))]
            [Display(Name = nameof(Age))]
            public int Age { get; set; } = 18;
        }


        /// <summary>
        /// �첽��ȡ������
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAndResultAsync();
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            UserName = user.UserName;

            Input = new InputModel
            {
                Email = user.Email,
                Phone = user.PhoneNumber
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user).ConfigureAndResultAsync();

            return Page();
        }

        /// <summary>
        /// �첽�ύ������
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User).ConfigureAndResultAsync();
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var updateProfileResult = await _userManager.UpdateAsync(user).ConfigureAndResultAsync();
            if (!updateProfileResult.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error ocurred updating the profile for user with ID '{user.Id}'");
            }

            if (Input.Email != user.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email).ConfigureAndResultAsync();
                if (!setEmailResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            if (Input.Phone != user.PhoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.Phone).ConfigureAndResultAsync();
                if (!setPhoneResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }

            await _signInManager.RefreshSignInAsync(user).ConfigureAndWaitAsync();

            StatusMessage = _statusLocalizer.GetString(r => r.ProfileUpdated);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User).ConfigureAndResultAsync();
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userId = await _userManager.GetUserIdAsync(user).ConfigureAndResultAsync();
            var email = await _userManager.GetEmailAsync(user).ConfigureAndResultAsync();
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAndResultAsync();

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { user.Id, token },
                protocol: Request.Scheme);

            await _emailService.SendAsync(
                email,
                _registerLocalizer.GetString(r => r.ConfirmYourEmail),
                _registerLocalizer.GetString(r => r.ConfirmYourEmailFormat, HtmlEncoder.Default.Encode(callbackUrl))).ConfigureAndWaitAsync();

            StatusMessage = _statusLocalizer.GetString(r => r.VerificationEmailSent);

            return RedirectToPage();
        }

    }
}

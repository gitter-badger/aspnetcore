using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Librame.AspNetCore.Identity.UI.Pages.Examples
{
    using Extensions.Core;
    using Extensions.Network;
    using UI;

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
        private IExpressionStringLocalizer<RegisterViewResource> _registerLocalizer = null;

        [InjectionService]
        private IExpressionStringLocalizer<StatusMessageResource> _statusLocalizer = null;

        [InjectionService]
        private IIdentityBuilderWrapper _builderWrapper = null;

        [InjectionService]
        private IServiceProvider _serviceProvider = null;

        private readonly dynamic _signInManager = null;
        private readonly dynamic _userManager = null;
        private readonly dynamic _userStore = null;


        /// <summary>
        /// ����һ�� <see cref="IndexPageModel"/>��
        /// </summary>
        /// <param name="injectionService">������ <see cref="IInjectionService"/>��</param>
        public IndexPageModel(IInjectionService injectionService)
        {
            injectionService.Inject(this);

            _signInManager = _serviceProvider.GetService(typeof(SignInManager<>)
                .MakeGenericType(_builderWrapper.RawBuilder.UserType));
            _userManager = _signInManager.UserManager;
            _userStore = _serviceProvider.GetService(typeof(IUserStore<>)
                .MakeGenericType(_builderWrapper.RawBuilder.UserType));
        }


        /// <summary>
        /// �û�����
        /// </summary>
        public string Username { get; set; }

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
        [ViewResourceMapping("Index")]
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
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Username = user.UserName;

            Input = new InputModel
            {
                Email = user.Email,
                Phone = user.PhoneNumber
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

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

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var updateProfileResult = await _userManager.UpdateAsync(user);
            if (!updateProfileResult.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error ocurred updating the profile for user with ID '{user.Id}'");
            }

            if (Input.Email != user.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            if (Input.Phone != user.PhoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.Phone);
                if (!setPhoneResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }

            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = _statusLocalizer[r => r.ProfileUpdated];

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
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
                values: new { user.Id, token },
                protocol: Request.Scheme);

            await _emailService.SendAsync(
                email,
                _registerLocalizer[r => r.ConfirmYourEmail],
                _registerLocalizer[r => r.ConfirmYourEmailFormat, HtmlEncoder.Default.Encode(callbackUrl)]);

            StatusMessage = _statusLocalizer[r => r.VerificationEmailSent];

            return RedirectToPage();
        }

    }
}

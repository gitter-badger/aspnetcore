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
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Librame.AspNetCore.Identity.Web.Pages.Account
{
    using AspNetCore.Identity.Builders;
    using AspNetCore.Identity.Stores;
    using AspNetCore.Identity.Web.Models;
    using AspNetCore.Identity.Web.Resources;
    using AspNetCore.Web.Applications;
    using Extensions;
    using Extensions.Core.Identifiers;
    using Extensions.Core.Services;
    using Extensions.Data.ValueGenerators;
    using Extensions.Network.Services;

    /// <summary>
    /// ע��ҳ��ģ�͡�
    /// </summary>
    [AllowAnonymous]
    [GenericApplicationModel(typeof(IdentityGenericTypeDefinitionMapper),
        typeof(RegisterPageModel<,,>))]
    public class RegisterPageModel : PageModel
    {
        /// <summary>
        /// ����һ�� <see cref="RegisterPageModel"/> ʵ����
        /// </summary>
        /// <param name="localizer">������ <see cref="IHtmlLocalizer{RegisterViewResource}"/>��</param>
        /// <param name="builderOptions">������ <see cref="IOptions{IdentityBuilderOptions}"/>��</param>
        /// <param name="options">������ <see cref="IOptions{IdentityOptions}"/>��</param>
        protected RegisterPageModel(IHtmlLocalizer<RegisterViewResource> localizer,
            IOptions<IdentityBuilderOptions> builderOptions, IOptions<IdentityOptions> options)
        {
            Localizer = localizer.NotNull(nameof(localizer));
            BuilderOptions = builderOptions.NotNull(nameof(builderOptions)).Value;
            Options = options.NotNull(nameof(options)).Value;
        }


        /// <summary>
        /// ���ػ���Դ��
        /// </summary>
        public IHtmlLocalizer<RegisterViewResource> Localizer { get; }

        /// <summary>
        /// ������ѡ�
        /// </summary>
        public IdentityBuilderOptions BuilderOptions { get; }

        /// <summary>
        /// ѡ�
        /// </summary>
        public IdentityOptions Options { get; }

        /// <summary>
        /// ע����ͼģ�͡�
        /// </summary>
        [BindProperty]
        public RegisterViewModel Input { get; set; }

        /// <summary>
        /// ���� URL��
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string ReturnUrl { get; set; }


        /// <summary>
        /// ��ȡ������
        /// </summary>
        /// <param name="returnUrl">�����ķ��� URL��</param>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings")]
        public virtual void OnGet(string returnUrl = null)
            => throw new NotImplementedException();

        /// <summary>
        /// �첽�ύ������
        /// </summary>
        /// <param name="returnUrl">�����ķ��� URL��</param>
        /// <returns>����һ������ <see cref="IActionResult"/> ���첽������</returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings")]
        public virtual Task<IActionResult> OnPostAsync(string returnUrl = null)
            => throw new NotImplementedException();
    }


    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class RegisterPageModel<TUser, TGenId, TCreateBy> : RegisterPageModel
        where TUser : class, IIdentifier<TGenId>
        where TGenId : IEquatable<TGenId>
        where TCreateBy : IEquatable<TCreateBy>
    {
        private readonly SignInManager<TUser> _signInManager;
        private readonly UserManager<TUser> _userManager;
        private readonly IUserStore<TUser> _userStore;
        private readonly ILogger<LoginPageModel> _logger;
        private readonly IClockService _clock;
        private readonly IEmailService _email;
        private readonly IIdentityStoreIdentifierGenerator<TGenId> _identifierGenerator;
        private readonly IDefaultValueGenerator<TCreateBy> _createdByGenerator;


        public RegisterPageModel(
            SignInManager<TUser> signInManager,
            IUserStore<TUser> userStore,
            ILogger<LoginPageModel> logger,
            IClockService clock,
            IEmailService email,
            IDefaultValueGenerator<TCreateBy> createdByGenerator,
            ServiceFactory serviceFactory,
            IHtmlLocalizer<RegisterViewResource> localizer,
            IOptions<IdentityBuilderOptions> builderOptions,
            IOptions<IdentityOptions> options)
            : base(localizer, builderOptions, options)
        {
            _signInManager = signInManager.NotNull(nameof(signInManager));
            _userStore = userStore.NotNull(nameof(userStore));
            _logger = logger.NotNull(nameof(logger));
            _clock = clock.NotNull(nameof(clock));
            _email = email.NotNull(nameof(email));
            _createdByGenerator = createdByGenerator.NotNull(nameof(createdByGenerator));

            _identifierGenerator = serviceFactory.GetIdentityStoreIdentifierGeneratorByUser<TUser, TGenId>();
            _userManager = signInManager.UserManager;
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

                var userId = await _identifierGenerator.GenerateUserIdAsync().ConfigureAndResultAsync();
                await user.SetIdAsync(userId).ConfigureAndResultAsync();

                var createdBy = await _createdByGenerator.GetValueAsync(GetType()).ConfigureAndResultAsync();

                var result = await _userManager.CreateUserByEmail(_userStore,
                    _clock, user, createdBy, Input.Email, Input.Password).ConfigureAndResultAsync();

                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAndResultAsync();

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = userId.ToString(), code },
                        protocol: Request.Scheme);

                    await _email.SendAsync(Input.Email,
                        Localizer.GetString(r => r.ConfirmYourEmail)?.Value,
                        Localizer.GetString(r => r.ConfirmYourEmailFormat, HtmlEncoder.Default.Encode(callbackUrl))?.Value).ConfigureAndWaitAsync();

                    await _signInManager.SignInAsync(user, isPersistent: false).ConfigureAndWaitAsync();
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

        //private async Task<IdentityResult> CreateUserByEmail(IClockService clock, TUser user, string email, string password = null,
        //    CancellationToken cancellationToken = default)
        //{
        //    await _userStore.SetUserNameAsync(user, email, cancellationToken).ConfigureAndWaitAsync();

        //    if (!_userManager.SupportsUserEmail)
        //        throw new NotSupportedException("The identity builder requires a user store with email support.");

        //    var emailStore = (IUserEmailStore<TUser>)_userStore;
        //    await emailStore.SetEmailAsync(user, email, cancellationToken).ConfigureAndWaitAsync();

        //    // Populate Creation
        //    await EntityPopulator.PopulateCreationAsync<RegisterPageModel>(clock, user, cancellationToken: cancellationToken)
        //        .ConfigureAndWaitAsync();

        //    if (password.IsNotEmpty())
        //        return await _userManager.CreateAsync(user, password).ConfigureAndResultAsync();

        //    return await _userManager.CreateAsync(user).ConfigureAndResultAsync();
        //}

        private static TUser CreateUser()
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

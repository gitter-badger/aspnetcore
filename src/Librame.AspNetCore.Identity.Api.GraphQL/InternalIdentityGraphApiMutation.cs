﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pang All rights reserved.
 * 
 * http://librame.net
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Librame.AspNetCore.Identity.Api
{
    using AspNetCore.Api;
    using Extensions;
    using Extensions.Core;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// 内部身份 Graph API 变化。
    /// </summary>
    internal class InternalIdentityGraphApiMutation : ObjectGraphType, IGraphApiMutation
    {
        [InjectionService]
        private ILogger<InternalIdentityGraphApiMutation> _logger;

        [InjectionService]
        private IIdentityIdentifierService _identifierService;

        [InjectionService]
        private SignInManager<DefaultIdentityUser> _signInManager;

        private UserManager<DefaultIdentityUser> _userManager;


        /// <summary>
        /// 构造一个 <see cref="InternalIdentityGraphApiMutation"/> 实例。
        /// </summary>
        /// <param name="injectionService">给定的 <see cref="IInjectionService"/></param>
        public InternalIdentityGraphApiMutation(IInjectionService injectionService)
        {
            injectionService.Inject(this);

            _userManager = _signInManager.UserManager;

            Name = nameof(GraphQL.Types.ISchema.Mutation);

            AddLoginTypeField();

            AddRegisterTypeField();
        }


        private void AddLoginTypeField()
        {
            FieldAsync<LoginType>
            (
                name: "login",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<LoginInputType>> { Name = "user" }
                ),
                resolve: async context =>
                {
                    var model = context.GetArgument<LoginApiModel>("user");

                    var result = await _signInManager.PasswordSignInAsync(model.Name,
                        model.Password, model.RememberMe, lockoutOnFailure: true);

                    if (result.Succeeded)
                    {
                        model.Message = "User logged in.";
                    }
                    if (result.RequiresTwoFactor)
                    {
                        model.Message = "Need requires two factor.";
                        model.RedirectUrl = "./LoginWith2fa";
                    }
                    if (result.IsLockedOut)
                    {
                        model.Message = "User account locked out.";
                        model.RedirectUrl = "./Lockout";
                    }
                    else
                    {
                        model.IsError = true;
                        model.Message = "Invalid login attempt.";
                    }

                    return model.Log(_logger);
                }
            );
        }

        private void AddRegisterTypeField()
        {
            FieldAsync<RegisterType>
            (
                name: "addUser",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<RegisterInputType>> { Name = "user" }
                ),
                resolve: async context =>
                {
                    var model = context.GetArgument<RegisterApiModel>("user");
                    var user = new DefaultIdentityUser(model.Name)
                    {
                        Id = await _identifierService.GetUserIdAsync(),
                        Email = model.Email
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        model.Message = "User created a new account with password.";
                        model.UserId = await _userManager.GetUserIdAsync(user);
                        model.Token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        // 确认邮件
                        var confirmEmail = model.ConfirmEmailUrl.TryGetPath(out Uri confirmEmailUri);
                        //if (confirmEmail.Value)

                        //confirmEmail.Add(QueryString.Create("userId", model.UserId));
                        //confirmEmail.Add(QueryString.Create("token", model.Token));

                        //var confirmEmailUri = _httpContextAccessor.HttpContext.Request.NewUri(confirmEmail);
                        //var confirmEmailExternalLink = HtmlEncoder.Default.Encode(confirmEmailUri.ToString());

                        //await _emailService.SendAsync(user.Email,
                        //    _localizer[r => r.ConfirmYourEmail]?.Value,
                        //    _localizer[r => r.ConfirmYourEmailFormat, confirmEmailExternalLink]?.Value);

                        //IUserStore.GetUserEmailStore(signInManager).SetEmailAsync(user, model.Email, default).Wait();

                        _signInManager.SignInAsync(user, isPersistent: false).Wait();
                    }
                    else
                    {
                        if (result.Errors.IsNotNullOrEmpty())
                        {
                            model.Errors.AddRange(result.Errors.Select(error =>
                            {
                                return new Exception($"Code: {error.Code}, Description: {error.Description}");
                            }));
                        }
                    }

                    return model.Log(_logger);
                }
            );
        }

    }
}

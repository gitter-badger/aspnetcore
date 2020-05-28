#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Librame.Extensions;
using Microsoft.AspNetCore.Authentication;
using Librame.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="LibrameOpenIdConnectHandler"/> ��̬��չ��
    /// </summary>
    public static class LibrameOpenIdConnectExtensions
    {
        /// <summary>
        /// ��� LibrameOpenIdConnect��
        /// </summary>
        /// <param name="builder">������ <see cref="AuthenticationBuilder"/>��</param>
        /// <returns>���� <see cref="AuthenticationBuilder"/>��</returns>
        public static AuthenticationBuilder AddLibrameOpenIdConnect(this AuthenticationBuilder builder)
            => builder.AddLibrameOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, _ => { });

        /// <summary>
        /// ��� LibrameOpenIdConnect��
        /// </summary>
        /// <param name="builder">������ <see cref="AuthenticationBuilder"/>��</param>
        /// <param name="configureOptions">����������ѡ�</param>
        /// <returns>���� <see cref="AuthenticationBuilder"/>��</returns>
        public static AuthenticationBuilder AddLibrameOpenIdConnect(this AuthenticationBuilder builder, Action<OpenIdConnectOptions> configureOptions)
            => builder.AddLibrameOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, configureOptions);

        /// <summary>
        /// ��� LibrameOpenIdConnect��
        /// </summary>
        /// <param name="builder">������ <see cref="AuthenticationBuilder"/>��</param>
        /// <param name="authenticationScheme">��������֤������</param>
        /// <param name="configureOptions">����������ѡ�</param>
        /// <returns>���� <see cref="AuthenticationBuilder"/>��</returns>
        public static AuthenticationBuilder AddLibrameOpenIdConnect(this AuthenticationBuilder builder, string authenticationScheme, Action<OpenIdConnectOptions> configureOptions)
            => builder.AddLibrameOpenIdConnect(authenticationScheme, OpenIdConnectDefaults.DisplayName, configureOptions);

        /// <summary>
        /// ��� LibrameOpenIdConnect��
        /// </summary>
        /// <param name="builder">������ <see cref="AuthenticationBuilder"/>��</param>
        /// <param name="authenticationScheme">��������֤������</param>
        /// <param name="displayName">�����ķ�����ʾ���ơ�</param>
        /// <param name="configureOptions">����������ѡ�</param>
        /// <returns>���� <see cref="AuthenticationBuilder"/>��</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static AuthenticationBuilder AddLibrameOpenIdConnect(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<OpenIdConnectOptions> configureOptions)
        {
            builder.NotNull(nameof(builder));

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<OpenIdConnectOptions>, OpenIdConnectPostConfigureOptions>());
            return builder.AddRemoteScheme<OpenIdConnectOptions, LibrameOpenIdConnectHandler>(authenticationScheme, displayName, configureOptions);
        }

    }
}

// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace Librame.AspNetCore.IdentityServer.UI
{
    /// <summary>
    /// �ض�����ͼģ�͡�
    /// </summary>
    public class RedirectViewModel
    {
        /// <summary>
        /// �ض��� URL��
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string RedirectUrl { get; set; }
    }
}
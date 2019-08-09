// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Librame.AspNetCore.IdentityServer.UI
{
    /// <summary>
    /// �豸��Ȩ����ģ�͡�
    /// </summary>
    public class DeviceAuthorizationInputModel : ConsentInputModel
    {
        /// <summary>
        /// �û��롣
        /// </summary>
        public string UserCode { get; set; }
    }
}
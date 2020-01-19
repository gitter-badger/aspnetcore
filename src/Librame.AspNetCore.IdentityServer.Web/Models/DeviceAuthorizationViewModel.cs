// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Librame.AspNetCore.IdentityServer.Web.Models
{
    /// <summary>
    /// �豸��Ȩ��ͼģ�͡�
    /// </summary>
    public class DeviceAuthorizationViewModel : ConsentViewModel
    {
        /// <summary>
        /// �û��롣
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// ȷ���û��롣
        /// </summary>
        public bool ConfirmUserCode { get; set; }
    }
}
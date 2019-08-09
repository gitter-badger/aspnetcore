// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Librame.AspNetCore.IdentityServer.UI
{
    /// <summary>
    /// �ǳ���ͼģ�͡�
    /// </summary>
    public class LogoutViewModel : LogoutInputModel
    {
        /// <summary>
        /// ��ʾ�ǳ���ʾ��
        /// </summary>
        public bool ShowLogoutPrompt { get; set; }
            = true;
    }
}

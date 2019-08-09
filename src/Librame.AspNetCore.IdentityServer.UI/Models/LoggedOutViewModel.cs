// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Librame.AspNetCore.IdentityServer.UI
{
    /// <summary>
    /// �ǳ���ͼģ�͡�
    /// </summary>
    public class LoggedOutViewModel
    {
        /// <summary>
        /// ���õǳ��ض��� URI��
        /// </summary>
        public string PostLogoutRedirectUri { get; set; }

        /// <summary>
        /// �ͻ������ơ�
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// ǩ�� Iframe URL��
        /// </summary>
        public string SignOutIframeUrl { get; set; }

        /// <summary>
        /// �Զ��ض����ǩ����
        /// </summary>
        public bool AutomaticRedirectAfterSignOut { get; set; } = false;

        /// <summary>
        /// �ǳ���ʶ��
        /// </summary>
        public string LogoutId { get; set; }

        /// <summary>
        /// �����ⲿ�ǳ���
        /// </summary>
        public bool TriggerExternalSignout
            => ExternalAuthenticationScheme != null;

        /// <summary>
        /// �ⲿ��֤������
        /// </summary>
        public string ExternalAuthenticationScheme { get; set; }
    }
}
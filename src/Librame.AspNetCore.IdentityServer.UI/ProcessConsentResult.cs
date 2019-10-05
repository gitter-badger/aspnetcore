// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Librame.AspNetCore.IdentityServer.UI
{
    using Extensions;

    /// <summary>
    /// ������׼�����
    /// </summary>
    public class ProcessConsentResult
    {
        /// <summary>
        /// ���ض���
        /// </summary>
        public bool IsRedirect
            => RedirectUri.IsNotEmpty();

        /// <summary>
        /// �ض��� URI��
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// �ͻ��˱�ʶ��
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// ��ʾ��ͼ��
        /// </summary>
        public bool ShowView
            => ViewModel.IsNotNull();

        /// <summary>
        /// ��׼��ͼģ�͡�
        /// </summary>
        public ConsentViewModel ViewModel { get; set; }

        /// <summary>
        /// ����֤����
        /// </summary>
        public bool HasValidationError
            => ValidationError.IsNotEmpty();

        /// <summary>
        /// ��֤����
        /// </summary>
        public string ValidationError { get; set; }
    }
}

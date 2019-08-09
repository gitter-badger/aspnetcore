// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Librame.AspNetCore.IdentityServer.UI
{
    /// <summary>
    /// ������ͼģ�͡�
    /// </summary>
    public class LoginViewModel : LoginInputModel
    {
        /// <summary>
        /// �����ס���롣
        /// </summary>
        public bool AllowRememberLogin { get; set; }
            = true;

        /// <summary>
        /// ���ñ��ص��롣
        /// </summary>
        public bool EnableLocalLogin { get; set; }
            = true;

        /// <summary>
        /// �ⲿ����ṩ����
        /// </summary>
        public IEnumerable<ExternalProvider> ExternalProviders { get; set; }

        /// <summary>
        /// �ɼ����ⲿ����ṩ����
        /// </summary>
        public IEnumerable<ExternalProvider> VisibleExternalProviders
            => ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        /// <summary>
        /// �������ⲿ���롣
        /// </summary>
        public bool IsExternalLoginOnly
            => EnableLocalLogin == false && ExternalProviders?.Count() == 1;

        /// <summary>
        /// �ⲿ���뷽����
        /// </summary>
        public string ExternalLoginScheme
            => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;
    }
}
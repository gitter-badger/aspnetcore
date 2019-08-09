// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Librame.AspNetCore.IdentityServer.UI
{
    /// <summary>
    /// ��������ģ�͡�
    /// </summary>
    public class LoginInputModel
    {
        /// <summary>
        /// �û�����
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// ���롣
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// ��ס���롣
        /// </summary>
        public bool RememberLogin { get; set; }

        /// <summary>
        /// ���� URL��
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
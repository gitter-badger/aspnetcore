// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Librame.AspNetCore.IdentityServer.Web.Models
{
    using Resources;

    /// <summary>
    /// ��������ģ�͡�
    /// </summary>
    public class LoginInputModel
    {
        /// <summary>
        /// ���ʡ�
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(RequiredAttribute), ErrorMessageResourceType = typeof(ErrorMessageResource))]
        [Display(Name = nameof(Email), ResourceType = typeof(UserViewModelResource))]
        [EmailAddress(ErrorMessageResourceName = nameof(EmailAddressAttribute), ErrorMessageResourceType = typeof(ErrorMessageResource))]
        public string Email { get; set; }

        /// <summary>
        /// ���롣
        /// </summary>
        [StringLength(20, MinimumLength = 6, ErrorMessageResourceName = nameof(Password), ErrorMessageResourceType = typeof(ErrorMessageResource))]
        [DataType(DataType.Password)]
        [Display(Name = nameof(Password), ResourceType = typeof(UserViewModelResource))]
        public string Password { get; set; }

        /// <summary>
        /// ��ס���롣
        /// </summary>
        [Display(Name = nameof(RememberLogin), ResourceType = typeof(UserViewModelResource))]
        public bool RememberLogin { get; set; }

        /// <summary>
        /// ���� URL��
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string ReturnUrl { get; set; }
    }
}
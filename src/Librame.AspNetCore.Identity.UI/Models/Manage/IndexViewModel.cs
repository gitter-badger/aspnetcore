#region License

/* **************************************************************************************
 * Copyright (c) Librame Pang All rights reserved.
 * 
 * http://librame.net
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using System.ComponentModel.DataAnnotations;

namespace Librame.AspNetCore.Identity.UI.Models
{
    /// <summary>
    /// ������ͼģ�͡�
    /// </summary>
    public class IndexViewModel
    {
        /// <summary>
        /// �ƺ���
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(RequiredAttribute), ErrorMessageResourceType = typeof(ErrorMessageResource))]
        [Display(Name = nameof(Name), ResourceType = typeof(UserViewModelResource))]
        public string Name { get; set; }

        /// <summary>
        /// ���䡣
        /// </summary>
        [EmailAddress(ErrorMessageResourceName = nameof(EmailAddressAttribute), ErrorMessageResourceType = typeof(ErrorMessageResource))]
        [Display(Name = nameof(Email), ResourceType = typeof(UserViewModelResource))]
        public string Email { get; set; }

        /// <summary>
        /// �绰��
        /// </summary>
        //[Required(ErrorMessageResourceName = nameof(RequiredAttribute), ErrorMessageResourceType = typeof(ErrorMessageResource))]
        [Display(Name = nameof(Phone), ResourceType = typeof(UserViewModelResource))]
        public string Phone { get; set; }
    }
}

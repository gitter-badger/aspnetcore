#region License

/* **************************************************************************************
 * Copyright (c) Librame Pang All rights reserved.
 * 
 * http://librame.net
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Librame.AspNetCore.Identity.UI
{
    /// <summary>
    /// ��ҳ��ͼģ�͡�
    /// </summary>
    public class IndexViewModel
    {
        /// <summary>
        /// �����롣
        /// </summary>
        public bool HasPassword { get; set; }

        /// <summary>
        /// �û�������Ϣ�б�
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList<UserLoginInfo> Logins { get; set; }

        /// <summary>
        /// �ֻ��š�
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// ˫���ӡ�
        /// </summary>
        public bool TwoFactor { get; set; }

        /// <summary>
        /// ��ס�������
        /// </summary>
        public bool BrowserRemembered { get; set; }

        /// <summary>
        /// ��֤����Կ��
        /// </summary>
        public string AuthenticatorKey { get; set; }
    }

    ///// <summary>
    ///// ������ͼģ�͡�
    ///// </summary>
    //public class IndexViewModel
    //{
    //    /// <summary>
    //    /// �ƺ���
    //    /// </summary>
    //    [Required(ErrorMessageResourceName = nameof(RequiredAttribute), ErrorMessageResourceType = typeof(ErrorMessageResource))]
    //    [Display(Name = nameof(Name), ResourceType = typeof(UserViewModelResource))]
    //    public string Name { get; set; }

    //    /// <summary>
    //    /// ���ʡ�
    //    /// </summary>
    //    [EmailAddress(ErrorMessageResourceName = nameof(EmailAddressAttribute), ErrorMessageResourceType = typeof(ErrorMessageResource))]
    //    [Display(Name = nameof(Email), ResourceType = typeof(UserViewModelResource))]
    //    public string Email { get; set; }

    //    /// <summary>
    //    /// �绰��
    //    /// </summary>
    //    //[Required(ErrorMessageResourceName = nameof(RequiredAttribute), ErrorMessageResourceType = typeof(ErrorMessageResource))]
    //    [Display(Name = nameof(Phone), ResourceType = typeof(UserViewModelResource))]
    //    public string Phone { get; set; }
    //}
}

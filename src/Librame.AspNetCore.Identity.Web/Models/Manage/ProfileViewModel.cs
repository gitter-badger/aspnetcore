#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Librame.AspNetCore.Identity.Web.Models
{
    /// <summary>
    /// ������ͼģ�͡�
    /// </summary>
    public class ProfileViewModel
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
}

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

namespace Librame.AspNetCore.Identity.UI.Models.ManageViewModels
{
    /// <summary>
    /// ������ͼģ�͡�
    /// </summary>
    public class IndexViewModel
    {
        /// <summary>
        /// �������롣
        /// </summary>
        public bool HasPassword { get; set; }

        /// <summary>
        /// ��¼�����б�
        /// </summary>
        public IList<UserLoginInfo> Logins { get; set; }

        /// <summary>
        /// �绰���롣
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// ����˫���ӡ�
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

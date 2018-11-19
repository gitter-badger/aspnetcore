#region License

/* **************************************************************************************
 * Copyright (c) Librame Pang All rights reserved.
 * 
 * http://librame.net
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Librame.AspNetCore.Identity.UI.Models.ManageViewModels
{
    /// <summary>
    /// �����¼������ͼģ�͡�
    /// </summary>
    public class ManageLoginsViewModel
    {
        /// <summary>
        /// ��ǰ��¼��Ϣ�б�
        /// </summary>
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        /// <summary>
        /// ��֤�����б�
        /// </summary>
        public IList<AuthenticationScheme> OtherLogins { get; set; }
    }
}

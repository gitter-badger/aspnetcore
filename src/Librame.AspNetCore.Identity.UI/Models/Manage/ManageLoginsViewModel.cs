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
using System.Diagnostics.CodeAnalysis;

namespace Librame.AspNetCore.Identity.UI
{
    /// <summary>
    /// ���������ͼģ�͡�
    /// </summary>
    public class ManageLoginsViewModel
    {
        /// <summary>
        /// ��ǰ������Ϣ�б�
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        /// <summary>
        /// �������뷽���б�
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList<AuthenticationScheme> OtherLogins { get; set; }
    }
}

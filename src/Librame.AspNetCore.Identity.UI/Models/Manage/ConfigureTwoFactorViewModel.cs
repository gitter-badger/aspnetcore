#region License

/* **************************************************************************************
 * Copyright (c) Librame Pang All rights reserved.
 * 
 * http://librame.net
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Librame.AspNetCore.Identity.UI
{
    /// <summary>
    /// ����˫������ͼģ�͡�
    /// </summary>
    public class ConfigureTwoFactorViewModel
    {
        /// <summary>
        /// ��ѡ����ṩ����
        /// </summary>
        public string SelectedProvider { get; set; }

        /// <summary>
        /// �ṩ���򼯺ϡ�
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<SelectListItem> Providers { get; set; }
    }
}

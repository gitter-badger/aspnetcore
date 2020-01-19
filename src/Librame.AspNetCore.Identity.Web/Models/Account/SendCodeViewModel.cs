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

namespace Librame.AspNetCore.Identity.Web.Models
{
    /// <summary>
    /// ��������ͼģ�͡�
    /// </summary>
    public class SendCodeViewModel
    {
        /// <summary>
        /// ѡ����ṩ����
        /// </summary>
        public string SelectedProvider { get; set; }

        /// <summary>
        /// �ṩ���򼯺ϡ�
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<SelectListItem> Providers { get; set; }

        /// <summary>
        /// ���� URL��
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string ReturnUrl { get; set; }

        /// <summary>
        /// ��ס�ҡ�
        /// </summary>
        public bool RememberMe { get; set; }
    }
}

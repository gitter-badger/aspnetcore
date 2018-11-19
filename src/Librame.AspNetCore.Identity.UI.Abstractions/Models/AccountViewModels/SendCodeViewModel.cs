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

namespace Librame.AspNetCore.Identity.UI.Models.AccountViewModels
{
    /// <summary>
    /// ���ʹ�����ͼģ�͡�
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
        public ICollection<SelectListItem> Providers { get; set; }

        /// <summary>
        /// �ص� URL��
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// ��ס�ҡ�
        /// </summary>
        public bool RememberMe { get; set; }
    }
}

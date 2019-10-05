﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pang All rights reserved.
 * 
 * http://librame.net
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

namespace Librame.AspNetCore.UI.Themepack.Simple
{
    using Extensions.Core;

    class ThemepackInfoResource_zh_TW : ResourceDictionary
    {
        public ThemepackInfoResource_zh_TW()
            : base()
        {
            AddOrUpdate("Name", "簡約", (key, value) => "簡約");
            AddOrUpdate("PrivacyAndCookiePolicy", PrivacyAndCookiePolicy, (key, value) => PrivacyAndCookiePolicy);
            AddOrUpdate("PrivacyAndCookiePolicyButton", "授權", (key, value) => "授權");
        }


        private static string PrivacyAndCookiePolicy
            => "我們致力於保護您的私隱。閲讀我們的客戶私隱政策，瞭解我們如何收集、利用、披露、傳輸和存儲您的個人資料。";
    }
}

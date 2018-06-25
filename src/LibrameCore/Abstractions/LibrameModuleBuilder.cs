﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pang All rights reserved.
 * 
 * http://librame.net
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using LibrameStandard.Utilities;
using Microsoft.AspNetCore.Builder;

namespace LibrameCore
{
    /// <summary>
    /// Librame 模块构建器。
    /// </summary>
    public class LibrameModuleBuilder : ILibrameModuleBuilder
    {
        /// <summary>
        /// 构造一个 <see cref="LibrameModuleBuilder"/> 实例。
        /// </summary>
        /// <param name="builder">给定的 <see cref="IApplicationBuilder"/>。</param>
        public LibrameModuleBuilder(IApplicationBuilder builder)
        {
            Builder = builder.NotNull(nameof(builder));
        }


        /// <summary>
        /// 应用构建器。
        /// </summary>
        /// <value>
        /// 返回 <see cref="IApplicationBuilder"/>。
        /// </value>
        public IApplicationBuilder Builder { get; }
    }
}

﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pang All rights reserved.
 * 
 * http://librame.net
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using LibrameCore;
using LibrameCore.Authentication;
using LibrameCore.Utilities;

namespace Microsoft.AspNetCore.Builder
{
    using Http;

    /// <summary>
    /// Librame MVC 应用构建器静态扩展。
    /// </summary>
    public static class LibrameMvcApplicationBuilderExtensions
    {

        /// <summary>
        /// 使用 Librame 应用。
        /// </summary>
        /// <param name="app">给定的应用构建器接口。</param>
        /// <param name="tokenGenerate">给定的令牌生成选项（可选）。</param>
        /// <returns>返回应用构建器接口。</returns>
        public static IApplicationBuilder UseLibrameMvc(this IApplicationBuilder app,
            TokenOptions tokenGenerate = null)
        {
            app.NotNull(nameof(app));

            // 取得 Librame 构建器
            var builder = app.GetLibrameBuilder();

            // 使用 HTTP 上下文
            app.UseHttpContext(builder);

            // 运行认证令牌
            app.UseLibrameAuthenticationToken(tokenGenerate, builder);

            return app;
        }


        /// <summary>
        /// 使用 HTTP 上下文。
        /// </summary>
        /// <param name="app">给定的应用构建器接口。</param>
        /// <param name="builder">给定的 Librame 构建器（可选）。</param>
        /// <returns>返回应用构建器接口。</returns>
        public static IApplicationBuilder UseHttpContext(this IApplicationBuilder app,
            ILibrameBuilder builder = null)
        {
            app.NotNull(nameof(app));

            if (builder == null)
                builder = app.GetLibrameBuilder();

            // 注入 HttpContext 访问器
            builder.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return app;
        }


        /// <summary>
        /// 获取 Librame 构建器接口。
        /// </summary>
        /// <param name="app">给定的应用构建器接口。</param>
        /// <returns>返回 Librame 构建器接口。</returns>
        public static ILibrameBuilder GetLibrameBuilder(this IApplicationBuilder app)
        {
            app.NotNull(nameof(app));

            var builder = app.ApplicationServices.GetService(typeof(ILibrameBuilder));

            return (builder.NotNull(nameof(builder)) as ILibrameBuilder);
        }


        /// <summary>
        /// 获取应用构建器的 HTTP 上下文。
        /// </summary>
        /// <param name="app">给定的应用构建器接口。</param>
        /// <returns>返回 HTTP 上下文。</returns>
        public static HttpContext GetHttpContext(this IApplicationBuilder app)
        {
            app.NotNull(nameof(app));

            var accessor = app.GetLibrameBuilder().GetService<IHttpContextAccessor>();
            return accessor.HttpContext;
        }

    }
}

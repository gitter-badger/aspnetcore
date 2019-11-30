﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pang All rights reserved.
 * 
 * http://librame.net
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// 身份区域路由构建器静态扩展。
    /// </summary>
    public static class IdentityAreaRouteBuilderExtensions
    {
        internal const string AreaName = nameof(Identity);
        internal const string Template = "Identity/{controller}/{action}/{id?}";


        /// <summary>
        /// 映射身份区域路由。
        /// </summary>
        /// <param name="builder">给定的 <see cref="IRouteBuilder"/>。</param>
        /// <returns>返回 <see cref="IRouteBuilder"/>。</returns>
        public static IRouteBuilder MapIdentityAreaRoute(this IRouteBuilder builder)
        {
            return builder.MapAreaRoute(
                name: AreaName,
                areaName: AreaName,
                template: Template,
                defaults: new { controller = "Manage", action = "Index" }
            );
        }


        /// <summary>
        /// 映射身份区域路由。
        /// </summary>
        /// <param name="builder">给定的 <see cref="IEndpointRouteBuilder"/>。</param>
        /// <returns>返回 <see cref="ControllerActionEndpointConventionBuilder"/>。</returns>
        public static ControllerActionEndpointConventionBuilder MapIdentityAreaRoute(this IEndpointRouteBuilder builder)
        {
            return builder.MapAreaControllerRoute(
                name: AreaName,
                areaName: AreaName,
                pattern: Template,
                defaults: new { controller = "Manage", action = "Index" }
            );
        }

    }
}

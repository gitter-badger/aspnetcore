﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pang All rights reserved.
 * 
 * http://librame.net
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Librame.AspNetCore.UI
{
    using Extensions;

    /// <summary>
    /// 外部认证方案集合页过滤器。
    /// </summary>
    /// <typeparam name="TUser">给定的用户类型。</typeparam>
    public class ExternalAuthenticationSchemesPageFilter<TUser> : IAsyncPageFilter
        where TUser : class
    {
        private readonly UiBuilderOptions _options;


        /// <summary>
        /// 构造一个 <see cref="ExternalAuthenticationSchemesPageFilter{TUser}"/>。
        /// </summary>
        /// <param name="options">给定的 <see cref="UiBuilderOptions"/>。</param>
        public ExternalAuthenticationSchemesPageFilter(UiBuilderOptions options)
        {
            _options = options.NotNull(nameof(options));
        }


        /// <summary>
        /// 异步执行页处理程序。
        /// </summary>
        /// <param name="context">给定的 <see cref="PageHandlerExecutingContext"/>。</param>
        /// <param name="next">给定的 <see cref="PageHandlerExecutionDelegate"/>。</param>
        /// <returns>返回一个异步操作。</returns>
        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context,
            PageHandlerExecutionDelegate next)
        {
            if (next.IsNull())
                return;

            var executedContext = await next.Invoke().ConfigureAndResultAsync();
            if (executedContext.Result is PageResult pageResult)
            {
                var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<TUser>>();
                var schemes = await signInManager.GetExternalAuthenticationSchemesAsync().ConfigureAndResultAsync();

                pageResult.ViewData[_options.HasExternalAuthenticationSchemesKey] = schemes.IsNotEmpty();
            }
        }


        /// <summary>
        /// 异步选择页处理程序。
        /// </summary>
        /// <param name="context">给定的 <see cref="PageHandlerSelectedContext"/>。</param>
        /// <returns>返回一个异步操作。</returns>
        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
            => Task.CompletedTask;
    }
}

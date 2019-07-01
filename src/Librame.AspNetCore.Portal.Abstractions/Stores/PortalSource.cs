﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pang All rights reserved.
 * 
 * http://librame.net
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using System;

namespace Librame.AspNetCore.Portal
{
    using Extensions.Data;

    /// <summary>
    /// 门户来源。
    /// </summary>
    public class PortalSource : PortalSource<int, int>
    {
    }


    /// <summary>
    /// 门户来源。
    /// </summary>
    /// <typeparam name="TId">指定的标识类型。</typeparam>
    /// <typeparam name="TCategoryId">指定的分类标识类型。</typeparam>
    public class PortalSource<TId, TCategoryId> : AbstractEntity<TId>
        where TId : IEquatable<TId>
        where TCategoryId : IEquatable<TCategoryId>
    {
        /// <summary>
        /// 分类标识。
        /// </summary>
        public virtual TCategoryId CategoryId { get; set; }

        /// <summary>
        /// 名称。
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 标志。
        /// </summary>
        public virtual string Logo { get; set; }

        /// <summary>
        /// 链接。
        /// </summary>
        public virtual string Link { get; set; }

        /// <summary>
        /// 备注。
        /// </summary>
        public virtual string Descr { get; set; }
    }
}

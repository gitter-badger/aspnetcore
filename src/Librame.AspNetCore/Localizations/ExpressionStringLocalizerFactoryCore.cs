#region License

/* **************************************************************************************
 * Copyright (c) Librame Pang All rights reserved.
 * 
 * http://librame.net
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Librame.AspNetCore
{
    using Extensions.Core;

    /// <summary>
    /// ���ʽ�ַ�����λ���������ġ�
    /// </summary>
    public class ExpressionStringLocalizerFactoryCore : ExpressionStringLocalizerFactory
    {
        /// <summary>
        /// ����һ�� <see cref="ExpressionStringLocalizerFactoryCore"/>��
        /// </summary>
        /// <param name="localizationOptions">������ <see cref="IOptions{LocalizationOptions}"/>��</param>
        /// <param name="loggerFactory">������ <see cref="ILoggerFactory"/>��</param>
        public ExpressionStringLocalizerFactoryCore(IOptions<LocalizationOptions> localizationOptions,
            ILoggerFactory loggerFactory)
            : base(localizationOptions, loggerFactory)
        {
        }


        /// <summary>
        /// ��Դ���Ƽ��ϻ��档
        /// </summary>
        protected IResourceNamesCache ResourceNamesCache { get; private set; }
            = new ResourceNamesCache();


        /// <summary>
        /// ������Դ�������ַ�����λ����
        /// </summary>
        /// <param name="assembly">�����ĳ��򼯡�</param>
        /// <param name="baseName">�����Ļ������ơ�</param>
        /// <returns>���� <see cref="ResourceManagerStringLocalizer"/>��</returns>
        protected override ResourceManagerStringLocalizer CreateResourceManagerStringLocalizer(Assembly assembly, string baseName)
        {
            var resourceManager = new ResourceManagerCore(baseName, assembly, LoggerFactory.CreateLogger<ResourceManagerCore>());
            
            return new ResourceManagerStringLocalizer(resourceManager, assembly, baseName, ResourceNamesCache, Logger);
        }

    }
}
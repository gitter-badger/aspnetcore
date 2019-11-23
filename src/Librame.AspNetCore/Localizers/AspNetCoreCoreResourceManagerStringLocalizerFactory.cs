//#region License

/////* **************************************************************************************
// * Copyright (c) Librame Pang All rights reserved.
// * 
// * http://librame.net
// * 
// * You must not remove this notice, or any other, from this software.
// * **************************************************************************************/

//#endregion

//using Microsoft.Extensions.Localization;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System.Reflection;

//namespace Librame.AspNetCore
//{
//    using Extensions.Core;

//    /// <summary>
//    /// <see cref="CoreResourceManagerStringLocalizerFactory"/> for ASP.NET Core��
//    /// </summary>
//    public class AspNetCoreCoreResourceManagerStringLocalizerFactory : CoreResourceManagerStringLocalizerFactory
//    {
//        /// <summary>
//        /// ����һ�� <see cref="AspNetCoreCoreResourceManagerStringLocalizerFactory"/>��
//        /// </summary>
//        /// <param name="localizationOptions">������ <see cref="IOptions{LocalizationOptions}"/>��</param>
//        /// <param name="loggerFactory">������ <see cref="ILoggerFactory"/>��</param>
//        /// <param name="builderOptions">������ <see cref="IOptions{CoreBuilderOptions}"/>��</param>
//        public AspNetCoreCoreResourceManagerStringLocalizerFactory(IOptions<LocalizationOptions> localizationOptions,
//            ILoggerFactory loggerFactory, IOptions<CoreBuilderOptions> builderOptions)
//            : base(localizationOptions, loggerFactory, builderOptions)
//        {
//        }


//        /// <summary>
//        /// ��Դ���Ƽ��ϻ��档
//        /// </summary>
//        protected IResourceNamesCache ResourceNamesCache { get; private set; }
//            = new ResourceNamesCache();


//        /// <summary>
//        /// ������Դ�������ַ�����λ����
//        /// </summary>
//        /// <param name="assembly">�����ĳ��򼯡�</param>
//        /// <param name="baseName">�����Ļ������ơ�</param>
//        /// <returns>���� <see cref="ResourceManagerStringLocalizer"/>��</returns>
//        protected override ResourceManagerStringLocalizer CreateResourceManagerStringLocalizer(Assembly assembly, string baseName)
//        {
//            var resourceManager = new AspNetCoreResourceManager(baseName, assembly, LoggerFactory.CreateLogger<AspNetCoreResourceManager>());
//            return new ResourceManagerStringLocalizer(resourceManager, assembly, baseName, ResourceNamesCache, Logger);
//        }

//    }
//}
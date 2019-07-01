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
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;

namespace Librame.AspNetCore
{
    using Extensions;

    /// <summary>
    /// ASP.NET Core ��Դ�������ַ�����λ��������
    /// </summary>
    public class CoreResourceManagerStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly string _resourcesRelativePath;


        /// <summary>
        /// ����һ�� <see cref="CoreResourceManagerStringLocalizerFactory"/> ʵ����
        /// </summary>
        /// <param name="localizationOptions">������ <see cref="IOptions{LocalizationOptions}"/>��</param>
        /// <param name="loggerFactory">������ <see cref="ILoggerFactory"/>��</param>
        public CoreResourceManagerStringLocalizerFactory(
            IOptions<LocalizationOptions> localizationOptions,
            ILoggerFactory loggerFactory)
        {
            localizationOptions.NotNull(nameof(localizationOptions));

            LoggerFactory = loggerFactory.NotNull(nameof(loggerFactory));

            _resourcesRelativePath = localizationOptions.Value.ResourcesPath ?? string.Empty;
            
            if (_resourcesRelativePath.IsNotNullOrEmpty())
            {
                _resourcesRelativePath = _resourcesRelativePath.Replace(Path.AltDirectorySeparatorChar, '.')
                    .Replace(Path.DirectorySeparatorChar, '.') + ".";
            }
        }


        /// <summary>
        /// ��¼��������
        /// </summary>
        protected ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// ��¼����
        /// </summary>
        protected ILogger Logger => LoggerFactory.CreateLogger<CoreResourceManagerStringLocalizerFactory>();

        /// <summary>
        /// ��Դ���Ƽ��ϻ��档
        /// </summary>
        protected IResourceNamesCache ResourceNamesCache { get; private set; }
            = new ResourceNamesCache();

        /// <summary>
        /// ��λ�����档
        /// </summary>
        protected ConcurrentDictionary<string, ResourceManagerStringLocalizer> LocalizerCache { get; private set; }
            = new ConcurrentDictionary<string, ResourceManagerStringLocalizer>();


        /// <summary>
        /// ��ȡ��Դǰ׺��
        /// </summary>
        /// <param name="typeInfo">������ <see cref="TypeInfo"/>��</param>
        /// <returns>�����ַ�����</returns>
        protected virtual string GetResourcePrefix(TypeInfo typeInfo)
        {
            typeInfo.NotNull(nameof(typeInfo));

            return GetResourcePrefix(typeInfo, GetRootNamespace(typeInfo.Assembly), GetResourcePath(typeInfo.Assembly));
        }

        /// <summary>
        /// ��ȡ��Դǰ׺��
        /// </summary>
        /// <param name="typeInfo">������ <see cref="TypeInfo"/>��</param>
        /// <param name="baseNamespace">�����Ļ��������ռ䡣</param>
        /// <param name="resourcesRelativePath">����������Դ�����ļ��е����·����</param>
        /// <returns>�����ַ�����</returns>
        /// <remarks>
        /// For the type "Sample.Controllers.Home" if there's a resourceRelativePath return
        /// "Sample.Resourcepath.Controllers.Home" if there isn't one then it would return "Sample.Controllers.Home".
        /// </remarks>
        protected virtual string GetResourcePrefix(TypeInfo typeInfo, string baseNamespace, string resourcesRelativePath)
        {
            typeInfo.NotNull(nameof(typeInfo));
            baseNamespace.NotNullOrEmpty(nameof(baseNamespace));

            var prefix = string.Empty;

            var reusableAttribute = GetResourceMappingAttribute(typeInfo);
            if (reusableAttribute.IsNotNull() && reusableAttribute.Enabled)
            {
                if (reusableAttribute.PrefixFactory.IsNull())
                {
                    reusableAttribute.PrefixFactory = (_baseNamespace, _resourcesRelativePath, _typeInfo) =>
                    {
                        if (resourcesRelativePath.IsNullOrEmpty())
                            return $"{_baseNamespace}.{_typeInfo.Name}";
                        else
                            return $"{_baseNamespace}.{_resourcesRelativePath}{_typeInfo.Name}"; // �Ѹ�ʽ��Ϊ��ָ������磺Resources.��
                    };
                }

                prefix = reusableAttribute.PrefixFactory.Invoke(baseNamespace, resourcesRelativePath, typeInfo);
            }
            else
            {
                if (resourcesRelativePath.IsNullOrEmpty())
                {
                    prefix = typeInfo.FullName;
                }
                else
                {
                    // This expectation is defined by dotnet's automatic resource storage.
                    // We have to conform to "{RootNamespace}.{ResourceLocation}.{FullTypeName - AssemblyName}".
                    var assemblyName = new AssemblyName(typeInfo.Assembly.FullName).Name;
                    prefix = baseNamespace + "." + resourcesRelativePath + TrimPrefix(typeInfo.FullName, assemblyName + ".");
                }
            }

            Logger.LogInformation($"{typeInfo.FullName} resource prefix: {prefix}");

            return prefix;
        }

        /// <summary>
        /// ��ȡ��Դǰ׺��
        /// </summary>
        /// <param name="baseResourceName">�����Ļ�����Դ���ơ�</param>
        /// <param name="baseNamespace">�����Ļ��������ռ䡣</param>
        /// <returns>�����ַ�����</returns>
        protected virtual string GetResourcePrefix(string baseResourceName, string baseNamespace)
        {
            baseResourceName.NotNullOrEmpty(nameof(baseResourceName));
            baseNamespace.NotNullOrEmpty(nameof(baseNamespace));

            var assemblyName = new AssemblyName(baseNamespace);
            var assembly = Assembly.Load(assemblyName);

            var rootNamespace = GetRootNamespace(assembly);
            var resourceLocation = GetResourcePath(assembly);
            var locationPath = rootNamespace + "." + resourceLocation;

            baseResourceName = locationPath + TrimPrefix(baseResourceName, baseNamespace + ".");

            return baseResourceName;
        }

        /// <summary>
        /// ��ȡ��Դǰ׺��
        /// </summary>
        /// <param name="location">��������ԴĿ¼��λ��</param>
        /// <param name="baseName">�����Ļ������ơ�</param>
        /// <param name="resourceLocation">��Դ�� <paramref name="location"/> �е�λ�á�</param>
        /// <returns>�����ַ�����</returns>
        protected virtual string GetResourcePrefix(string location, string baseName, string resourceLocation)
        {
            // Re-root the base name if a resources path is set
            return location + "." + resourceLocation + TrimPrefix(baseName, location + ".");
        }


        /// <summary>
        /// ��ȡ��Դӳ�����ԡ�
        /// </summary>
        /// <param name="typeInfo">������ <see cref="TypeInfo"/>��</param>
        /// <returns>���� <see cref="ResourceMappingAttribute"/>��</returns>
        protected virtual ResourceMappingAttribute GetResourceMappingAttribute(TypeInfo typeInfo)
        {
            if (typeInfo.TryGetCustomAttribute(out ResourceMappingAttribute typeAttribute))
                return typeAttribute;

            if (typeInfo.Assembly.TryGetCustomAttribute(out ResourceMappingAttribute assemblyAttribute))
                return assemblyAttribute;

            return null;
        }


        /// <summary>
        /// �����ַ�����λ����
        /// </summary>
        /// <param name="resourceSource">��������Դ��Դ���͡�</param>
        /// <returns>���� <see cref="ResourceManagerStringLocalizer"/>��</returns>
        public virtual IStringLocalizer Create(Type resourceSource)
        {
            var typeInfo = resourceSource.NotNull(nameof(resourceSource)).GetTypeInfo();

            var baseName = GetResourcePrefix(typeInfo);

            var assembly = typeInfo.Assembly;

            return LocalizerCache.GetOrAdd(baseName, _ =>
            {
                return CreateResourceManagerStringLocalizer(assembly, baseName);
            });
        }

        /// <summary>
        /// �����ַ�����λ����
        /// </summary>
        /// <param name="baseName">�����Ļ������ơ�</param>
        /// <param name="location">��������ԴĿ¼��λ��</param>
        /// <returns>���� <see cref="ResourceManagerStringLocalizer"/>��</returns>
        public virtual IStringLocalizer Create(string baseName, string location)
        {
            baseName.NotNullOrEmpty(nameof(baseName));
            location.NotNullOrEmpty(nameof(location));

            return LocalizerCache.GetOrAdd($"B={baseName},L={location}", _ =>
            {
                var assemblyName = new AssemblyName(location);
                var assembly = Assembly.Load(assemblyName);
                baseName = GetResourcePrefix(baseName, location);

                return CreateResourceManagerStringLocalizer(assembly, baseName);
            });
        }

        /// <summary>
        /// ������Դ�������ַ�����λ����
        /// </summary>
        /// <param name="assembly">�����ĳ��򼯡�</param>
        /// <param name="baseName">�����Ļ������ơ�</param>
        /// <returns>���� <see cref="ResourceManagerStringLocalizer"/>��</returns>
        protected virtual ResourceManagerStringLocalizer CreateResourceManagerStringLocalizer(Assembly assembly, string baseName)
        {
            // ʹ�ú�����Դ������
            var resourceManager = new CoreResourceManager(baseName, assembly, LoggerFactory.CreateLogger<CoreResourceManager>());
            
            return new ResourceManagerStringLocalizer(
                resourceManager,
                assembly,
                baseName,
                ResourceNamesCache,
                Logger);
        }


        /// <summary>
        /// ��ȡ�������ռ����ԡ�
        /// </summary>
        /// <param name="assembly">�����ĳ��򼯡�</param>
        /// <returns>���� <see cref="ResourceLocationAttribute"/>��</returns>
        protected virtual ResourceLocationAttribute GetResourceLocationAttribute(Assembly assembly)
        {
            return assembly.GetCustomAttribute<ResourceLocationAttribute>();
        }

        /// <summary>
        /// ��ȡ�������ռ����ԡ�
        /// </summary>
        /// <param name="assembly">�����ĳ��򼯡�</param>
        /// <returns>���� <see cref="RootNamespaceAttribute"/>��</returns>
        protected virtual RootNamespaceAttribute GetRootNamespaceAttribute(Assembly assembly)
        {
            return assembly.GetCustomAttribute<RootNamespaceAttribute>();
        }


        private string GetRootNamespace(Assembly assembly)
        {
            var rootNamespaceAttribute = GetRootNamespaceAttribute(assembly);

            return rootNamespaceAttribute?.RootNamespace ??
                new AssemblyName(assembly.FullName).Name;
        }

        private string GetResourcePath(Assembly assembly)
        {
            var resourceLocationAttribute = GetResourceLocationAttribute(assembly);

            // If we don't have an attribute assume all assemblies use the same resource location.
            var resourceLocation = resourceLocationAttribute == null
                ? _resourcesRelativePath
                : resourceLocationAttribute.ResourceLocation + ".";

            resourceLocation = resourceLocation
                .Replace(Path.DirectorySeparatorChar, '.')
                .Replace(Path.AltDirectorySeparatorChar, '.');

            return resourceLocation;
        }

        private static string TrimPrefix(string name, string prefix)
        {
            if (name.StartsWith(prefix, StringComparison.Ordinal))
                return name.Substring(prefix.Length);

            return name;
        }

    }
}
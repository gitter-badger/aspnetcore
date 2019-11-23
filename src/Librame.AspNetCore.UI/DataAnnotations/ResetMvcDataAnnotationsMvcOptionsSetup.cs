#region License

/* **************************************************************************************
 * Copyright (c) Librame Pang All rights reserved.
 * 
 * http://librame.net
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace Librame.AspNetCore.UI
{
    using Extensions;

    /// <summary>
    /// ���� MVC ����ע�ͼ���ѡ�װ��
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class ResetMvcDataAnnotationsMvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;
        private readonly IValidationAttributeAdapterProvider _validationAttributeAdapterProvider;
        private readonly IOptions<MvcDataAnnotationsLocalizationOptions> _dataAnnotationLocalizationOptions;


        /// <summary>
        /// ���� MVC ����ע�͡�
        /// </summary>
        /// <param name="validationAttributeAdapterProvider">������ <see cref="IValidationAttributeAdapterProvider"/>��</param>
        /// <param name="dataAnnotationLocalizationOptions">������ <see cref="IOptions{MvcDataAnnotationsLocalizationOptions}"/>��</param>
        /// <param name="stringLocalizerFactory">������ <see cref="IStringLocalizerFactory"/>��</param>
        public ResetMvcDataAnnotationsMvcOptionsSetup(
            IValidationAttributeAdapterProvider validationAttributeAdapterProvider,
            IOptions<MvcDataAnnotationsLocalizationOptions> dataAnnotationLocalizationOptions,
            IStringLocalizerFactory stringLocalizerFactory)
            : this(validationAttributeAdapterProvider, dataAnnotationLocalizationOptions)
        {
            _stringLocalizerFactory = stringLocalizerFactory.NotNull(nameof(stringLocalizerFactory));
        }

        /// <summary>
        /// ���� MVC ����ע�͡�
        /// </summary>
        /// <param name="validationAttributeAdapterProvider">������ <see cref="IValidationAttributeAdapterProvider"/>��</param>
        /// <param name="dataAnnotationLocalizationOptions">������ <see cref="IOptions{MvcDataAnnotationsLocalizationOptions}"/>��</param>
        protected ResetMvcDataAnnotationsMvcOptionsSetup(
            IValidationAttributeAdapterProvider validationAttributeAdapterProvider,
            IOptions<MvcDataAnnotationsLocalizationOptions> dataAnnotationLocalizationOptions)
        {
            _validationAttributeAdapterProvider = validationAttributeAdapterProvider
                .NotNull(nameof(validationAttributeAdapterProvider));

            _dataAnnotationLocalizationOptions = dataAnnotationLocalizationOptions
                .NotNull(nameof(dataAnnotationLocalizationOptions));
        }


        /// <summary>
        /// ���� MVC ѡ�
        /// </summary>
        /// <param name="options">������ <see cref="MvcOptions"/>��</param>
        public void Configure(MvcOptions options)
        {
            options.NotNull(nameof(options));

            options.ModelMetadataDetailsProviders.Add(new ResetDataAnnotationsMetadataProvider(
                _dataAnnotationLocalizationOptions,
                _stringLocalizerFactory));

            options.ModelValidatorProviders.Add(new ResetDataAnnotationsModelValidatorProvider(
                _validationAttributeAdapterProvider,
                _dataAnnotationLocalizationOptions,
                _stringLocalizerFactory));
        }

    }
}
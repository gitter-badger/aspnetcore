#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Librame.AspNetCore.Web.DataAnnotations
{
    using Extensions;

    /// <summary>
    /// �����ļ���չ����������������
    /// </summary>
    internal class ResetFileExtensionsAttributeAdapter : AttributeAdapterBase<FileExtensionsAttribute>
    {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;
        private readonly string _extensions;
        private readonly string _formattedExtensions;


        /// <summary>
        /// ����һ�� <see cref="ResetFileExtensionsAttributeAdapter"/> ʵ����
        /// </summary>
        /// <param name="attribute">������ <see cref="FileExtensionsAttribute"/>��</param>
        /// <param name="stringLocalizer">������ <see cref="IStringLocalizer"/>��</param>
        /// <param name="stringLocalizerFactory">������ <see cref="IStringLocalizerFactory"/>��</param>
        [SuppressMessage("Globalization", "CA1308:���ַ����淶��Ϊ��д")]
        public ResetFileExtensionsAttributeAdapter(FileExtensionsAttribute attribute, IStringLocalizer stringLocalizer,
            IStringLocalizerFactory stringLocalizerFactory)
            : base(attribute, stringLocalizer)
        {
            _stringLocalizerFactory = stringLocalizerFactory.NotNull(nameof(stringLocalizerFactory));
            // Build the extension list based on how the JQuery Validation's 'extension' method expects it
            // https://jqueryvalidation.org/extension-method/

            // These lines follow the same approach as the FileExtensionsAttribute.
            var normalizedExtensions = Attribute.Extensions
                .Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace(".", string.Empty, StringComparison.OrdinalIgnoreCase)
                .ToLowerInvariant();

            var parsedExtensions = normalizedExtensions.Split(',').Select(e => "." + e);

            _formattedExtensions = string.Join(", ", parsedExtensions);
            _extensions = string.Join(",", parsedExtensions);
        }


        /// <summary>
        /// �����֤��
        /// </summary>
        /// <param name="context">������ <see cref="ClientModelValidationContext"/>��</param>
        public override void AddValidation(ClientModelValidationContext context)
        {
            context.NotNull(nameof(context));

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-fileextensions", GetErrorMessage(context));
            MergeAttribute(context.Attributes, "data-val-fileextensions-extensions", _extensions);
        }


        /// <summary>
        /// ��ȡ������Ϣ��
        /// </summary>
        /// <param name="validationContext">������ <see cref="ModelValidationContextBase"/>��</param>
        /// <returns>�����ַ�����</returns>
        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            validationContext.NotNull(nameof(validationContext));

            return GetErrorMessage(validationContext.ModelMetadata,
                validationContext.ModelMetadata.DisplayName, _formattedExtensions);
        }

        /// <summary>
        /// ��ȡ������Ϣ��
        /// </summary>
        /// <param name="modelMetadata">������ <see cref="ModelMetadata"/>��</param>
        /// <param name="arguments">�����Ĳ����������顣</param>
        /// <returns>�����ַ�����</returns>
        protected override string GetErrorMessage(ModelMetadata modelMetadata, params object[] arguments)
        {
            if (Attribute.ErrorMessageResourceType.IsNotNull())
                return Attribute.FormatErrorMessage(_stringLocalizerFactory, modelMetadata, arguments);

            return Attribute.FormatErrorMessage(modelMetadata.DisplayName);
        }

    }
}

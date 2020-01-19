#region License

/* **************************************************************************************
 * Copyright (c) Librame Pang All rights reserved.
 * 
 * http://librame.net
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Librame.AspNetCore.Web.DataAnnotations
{
    using Extensions;

    /// <summary>
    /// �����ַ�������������������
    /// </summary>
    internal class ResetStringLengthAttributeAdapter : AttributeAdapterBase<StringLengthAttribute>
    {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;
        private readonly string _max;
        private readonly string _min;


        /// <summary>
        /// ����һ�� <see cref="ResetStringLengthAttributeAdapter"/> ʵ����
        /// </summary>
        /// <param name="attribute">������ <see cref="StringLengthAttribute"/>��</param>
        /// <param name="stringLocalizer">������ <see cref="IStringLocalizer"/>��</param>
        /// <param name="stringLocalizerFactory">������ <see cref="IStringLocalizerFactory"/>��</param>
        public ResetStringLengthAttributeAdapter(StringLengthAttribute attribute, IStringLocalizer stringLocalizer,
            IStringLocalizerFactory stringLocalizerFactory)
            : base(attribute, stringLocalizer)
        {
            _stringLocalizerFactory = stringLocalizerFactory.NotNull(nameof(stringLocalizerFactory));
            _max = Attribute.MaximumLength.ToString(CultureInfo.InvariantCulture);
            _min = Attribute.MinimumLength.ToString(CultureInfo.InvariantCulture);
        }


        /// <summary>
        /// �����֤��
        /// </summary>
        /// <param name="context">������ <see cref="ClientModelValidationContext"/>��</param>
        public override void AddValidation(ClientModelValidationContext context)
        {
            context.NotNull(nameof(context));

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-length", GetErrorMessage(context));

            if (Attribute.MaximumLength != int.MaxValue)
            {
                MergeAttribute(context.Attributes, "data-val-length-max", _max);
            }

            if (Attribute.MinimumLength != 0)
            {
                MergeAttribute(context.Attributes, "data-val-length-min", _min);
            }
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
                validationContext.ModelMetadata.DisplayName, Attribute.MaximumLength, Attribute.MinimumLength);
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

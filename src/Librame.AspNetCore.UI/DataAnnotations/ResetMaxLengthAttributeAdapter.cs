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

namespace Librame.AspNetCore.UI
{
    using Extensions;

    /// <summary>
    /// ������󳤶�������������
    /// </summary>
    public class ResetMaxLengthAttributeAdapter : AttributeAdapterBase<MaxLengthAttribute>
    {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;
        private readonly string _max;


        /// <summary>
        /// ����һ�� <see cref="ResetMaxLengthAttributeAdapter"/> ʵ����
        /// </summary>
        /// <param name="attribute">������ <see cref="MaxLengthAttribute"/>��</param>
        /// <param name="stringLocalizer">������ <see cref="IStringLocalizer"/>��</param>
        /// <param name="stringLocalizerFactory">������ <see cref="IStringLocalizerFactory"/>��</param>
        public ResetMaxLengthAttributeAdapter(MaxLengthAttribute attribute, IStringLocalizer stringLocalizer,
            IStringLocalizerFactory stringLocalizerFactory)
            : base(attribute, stringLocalizer)
        {
            _stringLocalizerFactory = stringLocalizerFactory.NotNull(nameof(stringLocalizerFactory));
            _max = Attribute.Length.ToString(CultureInfo.InvariantCulture);
        }


        /// <summary>
        /// �����֤��
        /// </summary>
        /// <param name="context">������ <see cref="ClientModelValidationContext"/>��</param>
        public override void AddValidation(ClientModelValidationContext context)
        {
            context.NotNull(nameof(context));

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-maxlength", GetErrorMessage(context));
            MergeAttribute(context.Attributes, "data-val-maxlength-max", _max);
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
                validationContext.ModelMetadata.DisplayName, Attribute.Length);
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

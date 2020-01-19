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
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace Librame.AspNetCore.Web.DataAnnotations
{
    using Extensions;

    /// <summary>
    /// ������֤�����������ṩ����
    /// </summary>
    public class ResetValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;


        /// <summary>
        /// ����һ�� <see cref="ResetValidationAttributeAdapterProvider"/> ʵ����
        /// </summary>
        /// <param name="stringLocalizerFactory">������ <see cref="IStringLocalizerFactory"/>��</param>
        public ResetValidationAttributeAdapterProvider(IStringLocalizerFactory stringLocalizerFactory)
        {
            _stringLocalizerFactory = stringLocalizerFactory.NotNull(nameof(stringLocalizerFactory));
        }


        /// <summary>
        /// Ϊ�������Դ���������������
        /// </summary>
        /// <param name="attribute">������ <see cref="ValidationAttribute"/>��</param>
        /// <param name="stringLocalizer">������ <see cref="IStringLocalizer"/>��</param>
        /// <returns>���� <see cref="IAttributeAdapter"/>��</returns>
        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            IAttributeAdapter adapter;

            if (attribute is RegularExpressionAttribute regularExpressionAttribute)
            {
                adapter = new ResetRegularExpressionAttributeAdapter(regularExpressionAttribute, stringLocalizer, _stringLocalizerFactory);
            }
            else if (attribute is MaxLengthAttribute maxLengthAttribute)
            {
                adapter = new ResetMaxLengthAttributeAdapter(maxLengthAttribute, stringLocalizer, _stringLocalizerFactory);
            }
            else if (attribute is RequiredAttribute requiredAttribute)
            {
                adapter = new ResetRequiredAttributeAdapter(requiredAttribute, stringLocalizer, _stringLocalizerFactory);
            }
            else if (attribute is CompareAttribute compareAttribute)
            {
                adapter = new ResetCompareAttributeAdapter(compareAttribute, stringLocalizer, _stringLocalizerFactory);
            }
            else if (attribute is MinLengthAttribute minLengthAttribute)
            {
                adapter = new ResetMinLengthAttributeAdapter(minLengthAttribute, stringLocalizer, _stringLocalizerFactory);
            }
            else if (attribute is CreditCardAttribute creditCardAttribute)
            {
                adapter = new ResetDataTypeAttributeAdapter(creditCardAttribute, "data-val-creditcard", stringLocalizer, _stringLocalizerFactory);
            }
            else if (attribute is StringLengthAttribute stringLengthAttribute)
            {
                adapter = new ResetStringLengthAttributeAdapter(stringLengthAttribute, stringLocalizer, _stringLocalizerFactory);
            }
            else if (attribute is RangeAttribute rangeAttribute)
            {
                adapter = new ResetRangeAttributeAdapter(rangeAttribute, stringLocalizer, _stringLocalizerFactory);
            }
            else if (attribute is EmailAddressAttribute emailAddressAttribute)
            {
                adapter = new ResetDataTypeAttributeAdapter(emailAddressAttribute, "data-val-email", stringLocalizer, _stringLocalizerFactory);
            }
            else if (attribute is PhoneAttribute phoneAttribute)
            {
                adapter = new ResetDataTypeAttributeAdapter(phoneAttribute, "data-val-phone", stringLocalizer, _stringLocalizerFactory);
            }
            else if (attribute is UrlAttribute urlAttribute)
            {
                adapter = new ResetDataTypeAttributeAdapter(urlAttribute, "data-val-url", stringLocalizer, _stringLocalizerFactory);
            }
            else if (attribute is FileExtensionsAttribute fileExtensionsAttribute)
            {
                adapter = new ResetFileExtensionsAttributeAdapter(fileExtensionsAttribute, stringLocalizer, _stringLocalizerFactory);
            }
            else
            {
                adapter = null;
            }

            return adapter;
        }

    }
}

﻿// Copyright (c) Librame.NET All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Librame.Security
{
    /// <summary>
    /// Web 编码器。
    /// </summary>
    /// <author>Librame Pang</author>
    internal static class WebEncoders
    {
        /// <summary>
        /// 解码 Base64 URL 格式字符串。
        /// </summary>
        /// <param name="input">给定的 Base64 URL 格式字符串。</param>
        /// <returns>返回解码后的字节数组。</returns>
        public static byte[] Base64UrlDecode(string input)
        {
            // Assumption: input is base64url encoded without padding and contains no whitespace.

            // First, we need to add the padding characters back.
            int numPaddingCharsToAdd = GetNumBase64PaddingCharsToAddForDecode(input.Length);
            char[] completeBase64Array = new char[checked(input.Length + numPaddingCharsToAdd)];
            Debug.Assert(completeBase64Array.Length % 4 == 0, "Invariant: Array length must be a multiple of 4.");
            input.CopyTo(0, completeBase64Array, 0, input.Length);
            for (int i = 1; i <= numPaddingCharsToAdd; i++)
            {
                completeBase64Array[completeBase64Array.Length - i] = '=';
            }

            // Next, fix up '-' -> '+' and '_' -> '/'
            for (int i = 0; i < completeBase64Array.Length; i++)
            {
                char c = completeBase64Array[i];
                if (c == '-')
                {
                    completeBase64Array[i] = '+';
                }
                else if (c == '_')
                {
                    completeBase64Array[i] = '/';
                }
            }

            // Finally, decode.
            // If the caller provided invalid base64 chars, they'll be caught here.
            return Convert.FromBase64CharArray(completeBase64Array, 0, completeBase64Array.Length);
        }

        /// <summary>
        /// 以 Base64 URL 格式字符串编码。
        /// </summary>
        /// <param name="input">给定要编码的字节数组。</param>
        /// <returns>返回编码后的字符串。</returns>
        public static string Base64UrlEncode(byte[] input)
        {
            // Special-case empty input
            if (input.Length == 0)
            {
                return String.Empty;
            }

            // We're going to use base64url encoding with no padding characters.
            // See RFC 4648, Sec. 5.
            char[] buffer = new char[GetNumBase64CharsRequiredForInput(input.Length)];
            int numBase64Chars = Convert.ToBase64CharArray(input, 0, input.Length, buffer, 0);

            // Fix up '+' -> '-' and '/' -> '_'
            for (int i = 0; i < numBase64Chars; i++)
            {
                char ch = buffer[i];
                if (ch == '+')
                {
                    buffer[i] = '-';
                }
                else if (ch == '/')
                {
                    buffer[i] = '_';
                }
                else if (ch == '=')
                {
                    // We've reached a padding character: truncate the string from this point
                    return new String(buffer, 0, i);
                }
            }

            // If we got this far, the buffer didn't contain any padding chars, so turn
            // it directly into a string.
            return new String(buffer, 0, numBase64Chars);
        }

        private static int GetNumBase64CharsRequiredForInput(int inputLength)
        {
            int numWholeOrPartialInputBlocks = checked(inputLength + 2) / 3;
            return checked(numWholeOrPartialInputBlocks * 4);
        }

        private static int GetNumBase64PaddingCharsInString(string str)
        {
            // Assumption: input contains a well-formed base64 string with no whitespace.

            // base64 guaranteed have 0 - 2 padding characters.
            if (str[str.Length - 1] == '=')
            {
                if (str[str.Length - 2] == '=')
                {
                    return 2;
                }
                return 1;
            }
            return 0;
        }

        private static int GetNumBase64PaddingCharsToAddForDecode(int inputLength)
        {
            switch (inputLength % 4)
            {
                case 0:
                    return 0;
                case 2:
                    return 2;
                case 3:
                    return 1;
                default:
                    throw new FormatException("TODO: Malformed input.");
            }
        }
    }
}

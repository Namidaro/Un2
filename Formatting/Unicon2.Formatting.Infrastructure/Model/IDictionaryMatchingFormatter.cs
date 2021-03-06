﻿using System.Collections.Generic;
using Unicon2.Infrastructure.Interfaces;

namespace Unicon2.Formatting.Infrastructure.Model
{
    public interface IDictionaryMatchingFormatter : IUshortsFormatter
    {
        Dictionary<ushort, string> StringDictionary { get; set; }
        bool IsKeysAreNumbersOfBits { get; set; }
        bool UseDefaultMessage { get; set; }
        string DefaultMessage { get; set; }
    }
}
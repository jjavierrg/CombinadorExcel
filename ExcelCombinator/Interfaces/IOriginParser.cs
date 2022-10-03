﻿using System.Collections.Generic;

namespace ExcelCombinator.Interfaces
{
    public interface IOriginParser: IParser
    {
        bool Parse();
        IDictionary<IKey, IDictionary<string, object>> Values { get; }
    }
}
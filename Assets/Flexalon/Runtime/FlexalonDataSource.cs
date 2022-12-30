using System.Collections.Generic;
using System;

namespace Flexalon
{
    public interface DataSource
    {
        IReadOnlyList<object> Data { get; }
        event Action DataChanged;
    }
}
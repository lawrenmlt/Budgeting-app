using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budgeting.Common
{
    public interface ILookup
    {
        /// <summary>
        /// The value part of the name/value pair.
        /// </summary>
        string DataValue { get; }

        /// <summary>
        /// The name part of the name/value pair.
        /// </summary>
        string DataText { get; }
    }
}

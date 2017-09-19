using Budgeting.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budgeting.Dto
{
    public class LookupDto : ILookup
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string DataValue { get { return Id.ToString(); } }

        public string DataText { get { return Name; } }

    }
}

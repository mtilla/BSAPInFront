using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BS.API.Models
{
    public class Column
    {
        public string ColumnName { get; set; }
        public IList<object> Values { get; set; } = new List<object>();

    }
}
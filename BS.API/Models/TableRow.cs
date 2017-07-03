using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BS.API.Models
{
    public class TableRow
    {
        public Dictionary<String, String> RowValues { get; set; } = new Dictionary<string, string>();
    }
}
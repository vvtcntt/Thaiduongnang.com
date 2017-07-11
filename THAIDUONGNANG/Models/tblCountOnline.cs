using System;
using System.Collections.Generic;

namespace THAIDUONGNANG.Models
{
    public partial class tblCountOnline
    {
        public int id { get; set; }
        public Nullable<int> Online { get; set; }
        public Nullable<int> Sum { get; set; }
        public Nullable<System.DateTime> DateCreate { get; set; }
    }
}

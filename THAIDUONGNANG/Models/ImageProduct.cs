using System;
using System.Collections.Generic;

namespace THAIDUONGNANG.Models
{
    public partial class ImageProduct
    {
        public int id { get; set; }
        public Nullable<int> idProduct { get; set; }
        public string Name { get; set; }
        public Nullable<int> Type { get; set; }
        public string Images { get; set; }
    }
}

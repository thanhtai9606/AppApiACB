using System;
using System.Collections.Generic;

namespace App.Models
{
    public partial class Product
    {
        public Product()
        {
            SaleDetail = new HashSet<SaleDetail>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int OrderPrice { get; set; }
        public int SalePrice { get; set; }
        public string Model { get; set; }
        public int Inventory { get; set; }
        public int Warranty { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int IsActive { get; set; }

        public virtual ICollection<SaleDetail> SaleDetail { get; set; }
    }
}

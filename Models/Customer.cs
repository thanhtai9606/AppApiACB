using System;
using System.Collections.Generic;

namespace App.Models
{
    public partial class Customer
    {
        public Customer()
        {
            SaleHeader = new HashSet<SaleHeader>();
        }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<SaleHeader> SaleHeader { get; set; }
    }
}

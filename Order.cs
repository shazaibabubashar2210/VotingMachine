
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueenLocalDataHandling
{
    class Order
    {
        private string OrderId;
        private string CNIC;
        private string customerName;
        private string Phone;
        private string address;
        private string ProductId;
        private string Price;
        private string SizeOfProduct;
     
        // Declaring Properties
        public string OrderID { get; set; }

        public string GetCNIC { get; set; }
        public string CustomerName { get; set; }
        public string GetPhone { get; set; }

        public string GetAddress { get; set; }

        public string GetProductId { get; set; }

        public string GetPrice { get; set; }

        public string GetSizeOfProduct { get; set; }
    }
}

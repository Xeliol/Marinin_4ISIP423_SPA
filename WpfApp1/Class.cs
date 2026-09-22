using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    internal class Class
    {
    }
    class CurData
    {
        public int ID { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int Type { get; set; }
    }

    public partial class Products
    { 
        public double FinalPrice
        {
            get
            {
                double pr = Price * (100 - Sale) / 100;
                return pr;
            }
        }
    }
}

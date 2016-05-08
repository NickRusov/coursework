using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadDelivery
{
    struct Customer
    {
        public int Id;
        public string Address;
        public bool hasEuroLot;
        public float Demand;
        public float Latitude;
        public float Longitude;
        public string CoordString { get { return string.Format("{0} {1}", Latitude, Longitude); } }
    }
}

using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Objects
{
    public class Tweet
    {
        public PointLatLng Point { get; set; }
        //public DateTime date { get; set; }
        public string Message { get; set; }
        public double Weight { get; set; }
        //public int Counter { get; set; }
        //public List<string> wordsIn = new List<string>();
    }
}

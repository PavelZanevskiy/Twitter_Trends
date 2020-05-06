using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Objects.Parsers
{
    public class SentimentsParser
    {
        public SentimentsParser()
        {
            if (sents == null)
            {
                ParseSentiments();
            }
        }

        public Dictionary<string, double> sents;

        public void ParseSentiments()
        {
            sents = new Dictionary<string, double>();
            string sentiments = new StreamReader("D:/TwitTrends/Data/sentiments.csv").ReadToEnd();
            string[] dict = sentiments.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);            
            foreach (var str in dict)
            {
                string[] values = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                sents.Add(values[0], Convert.ToDouble(values[1].Replace('.', ',')));
            }            
        }
    }
}

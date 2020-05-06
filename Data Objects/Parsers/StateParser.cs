using Business_Objects;
using GMap.NET;
using GMap.NET.WindowsForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Objects.Parsers
{
    public class StateParser
    {
        public StateParser()
        {
            if (states==null)
            {
                Parse();
            }
        }

        public Dictionary<string, State> states;

        private void Parse()
        {
            string jsonString = new StreamReader("D:/TwitTrends/Data/states.json").ReadToEnd();            
            states = JsonConvert.DeserializeObject<Dictionary<string, State>>(jsonString);
        }

        public void FillStates(List<Tweet> tweets)
        {
            List<string> keys = new List<string>(states.Keys);
            List<PointLatLng> points = new List<PointLatLng>();


            for (int i = 0; i < keys.Count; i++)
            {
                double avgState = 0;
                int counterState = 0;
                double valueState = 0;
                List<Tweet> stateTweets = new List<Tweet>();

                for (int j = 0; j < states[keys[i]].Polygons.Count; j++)
                {
                    foreach (List<double> item in states[keys[i]].Polygons[j])
                    {
                        points.Add(new PointLatLng(item[1], item[0]));
                    }
                    for (int k = 0; k < tweets.Count; k++)
                    {
                        GMapPolygon gMapPolygon = new GMapPolygon(points, keys[i]);
                        if (gMapPolygon.IsInside(tweets[k].Point))
                        {
                            valueState += tweets[k].Weight;
                            counterState++;
                            stateTweets.Add(tweets[k]);
                        }
                    }
                    points.Clear();
                }
                if (counterState != 0)
                {
                    avgState = valueState / counterState;
                }
                states[keys[i]].StateTweets = stateTweets;
                states[keys[i]].Weight = avgState;
            }
        }
    }
}

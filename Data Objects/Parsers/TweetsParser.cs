using Business_Objects;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Data_Objects.Parsers
{
    public class TweetsParser
    {
        List<Tweet> tweets;

        public List<Tweet> ParseTweets(string fileName, Dictionary<string, double> sents)
        {
            tweets = new List<Tweet>();
            string allTweets = new StreamReader(fileName).ReadToEnd();
            string[] strTweets = allTweets.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            tweets.Clear();
            var pointsPattern = new Regex(@"[+-]?([0-9]*[.])?[0-9]+,\s[+-]?([0-9]*[.])?[0-9]+");
            var messagePattern = new Regex(@"\d\t.+");
            var regex = new Regex(@"\d\t");

            foreach (var item in strTweets)
            {
                Match point = pointsPattern.Match(item); 
                Match message = messagePattern.Match(item);
                tweets.Add(new Tweet
                {
                    Point = GetPoint(point),
                    Message = GetMessage(message,regex),
                    Weight = CountWeight(GetMessage(message, regex), sents)

                }); 
            }
            return tweets;
        }

        private double CountWeight(string message, Dictionary<string, double> sents)
        {
            string[] words = message.Split(new char[] { ',', '!', '?', ' ', '.', '\'', '\"', '#', '*', ':', ';', '(', ')', '&' }, StringSplitOptions.RemoveEmptyEntries);
            double sentsValue = 0;
            int counter = 0;
            double avgSentsValue = 0;

            foreach (var word in words)
            {
                if (sents.ContainsKey(word.ToLower()))
                {
                    sentsValue += sents[word.ToLower()];
                    counter++;
                }
            }

            if (counter != 0)
            {
                avgSentsValue = sentsValue / counter;
            }

            return avgSentsValue;
        }

        private PointLatLng GetPoint(Match point)
        {
            string[] LatLng = point.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return new PointLatLng(Convert.ToDouble(LatLng[0].Replace('.', ',')), Convert.ToDouble(LatLng[1].Replace('.', ',')));
        }

        private string GetMessage(Match message, Regex regex)
        {
            return regex.Replace(message.Value, "");
        }

    }
}

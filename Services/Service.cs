using Business_Objects;
using Data_Objects.Parsers;
using Data_Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class Service
    {
        readonly TweetsParser tweetsParser = new TweetsParser();
        readonly SentimentsParser sentimentsParser = new SentimentsParser();
        readonly Database database = new Database();
        readonly StateParser stateParser = new StateParser();
        

        public List<Tweet> ParseTweets(string fileName, Dictionary<string, double> sents)
        {
            return tweetsParser.ParseTweets(fileName,sents);
        }

        public Dictionary<string, double> GetSentiments()
        {
            return sentimentsParser.sents;
        }

        public Dictionary<string, State> GetStates()
        {
            return stateParser.states;
        }

        public List<string> GetFileNames()
        {
            return database.fileNames;
        }

        public void FillStates(List<Tweet> tweets)
        {
            stateParser.FillStates(tweets);
        }
    }
}

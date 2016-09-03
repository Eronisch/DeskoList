using System;
using System.Web.Mvc;
using Core.Business.Websites;
using Core.Models.Vote;
using Localization.Languages.Business.Vote;

namespace Web.Website
{
    public class WebsiteVoteService
    {
        private readonly WebsiteService _websiteService;
        private readonly Random _random;
        private const string VoteAnswerKey = "websiteVoteAnswer";

        public WebsiteVoteService()
        {
            _websiteService = new WebsiteService();
            _random = new Random();
        }

        /// <summary>
        /// Set vote answer, Returns true if the website exists
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="tempData"></param>
        /// <returns>True if the website exists</returns>
        public bool SetAnswer(int websiteId, TempDataDictionary tempData)
        {
            var website = _websiteService.GetWebsite(websiteId);

            if (website != null)
            {
                string answer;

                switch ((AnswerType)_random.Next(1, 5))
                {
                    case AnswerType.Dog:
                        answer = Vote.Dog;
                        break;
                    case AnswerType.Cat:
                        answer = Vote.Cat;
                        break;
                    case AnswerType.Bunny:
                        answer = Vote.Bunny;
                        break;
                    default:
                        answer = Vote.Duck;
                        break;
                }

                tempData[VoteAnswerKey] = answer;

                return true;
            }

            return false;
        }

        public string GetAnswer(TempDataDictionary tempData)
        {
            return tempData.ContainsKey(VoteAnswerKey) ? tempData[VoteAnswerKey].ToString() : String.Empty;
        }

        public bool ValidateAnswer(string answer, TempDataDictionary tempData)
        {
            return tempData.ContainsKey(VoteAnswerKey) && tempData[VoteAnswerKey].ToString() == answer;
        }
    }
}

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using Tweetinvi;
using Tweetinvi.Models;
using WebApplication1.Models;

namespace WebApplication1
{
    [HubName("tweet")]
    public class TweetHub : Hub
    {

        private Tweetinvi.Streaming.IFilteredStream stream;
        private static string searchTermHolder = "";

        public TweetHub()
        {
            var cred = Auth.SetUserCredentials("", "", "", "");
            stream = Stream.CreateFilteredStream();
        }

        public void sendMessage(tweet tweet)
        {
            //Main entry point to push back data to the clients
            Clients.Caller.SendMessage(tweet);
        }

        public void newTweet(string searchTerm)
        {
            callTweetApi(searchTerm);
        }

        public void callTweetApi(string searchTerm)
        {

            if (!string.IsNullOrEmpty(searchTermHolder))
            {
                stream.RemoveTrack(searchTermHolder);
                stream.StopStream();
            }

            searchTermHolder = searchTerm;
            stream.AddTrack(searchTerm);
            stream.AddTweetLanguageFilter(LanguageFilter.English);

            stream.MatchingTweetReceived += (sender, arguments) =>
            {
                if (!(arguments.Tweet.Media == null || arguments.Tweet.Media.Count <= 0))
                {
                    foreach (var item in arguments.Tweet.Media)
                    {
                        if (item.MediaType == "photo" && arguments.MatchingTracks[0] == searchTermHolder)
                        {
                            try
                            {
                                List<FaceModel> faceList = WebApplication1.FaceApi.FaceApi.MakeAnalysisRequest(arguments.Tweet.Media[0].MediaURL).Result;
                                tweet twt = new tweet();
                                twt.Text = arguments.Tweet.FullText;
                                twt.MediaUrl = arguments.Tweet.Media[0].MediaURL;
                                twt.CreatedAt = arguments.Tweet.CreatedAt;

                                foreach (var gender in faceList)
                                {
                                    if (gender.faceAttributes.gender == "male")
                                    {
                                        twt.MaleFace += 1;
                                    }
                                    else if (gender.faceAttributes.gender == "femail")
                                    {
                                        twt.FemailFace += 1;
                                    }
                                }

                                twt.MatchingTrack = arguments.MatchingTracks[0] + " - " + faceList.Count + " Face(s) Detected, " + twt.FemailFace + " female and " + twt.MaleFace + " male";
                                sendMessage(twt);
                            }
                            catch (System.Exception)
                            {
                                stream.ResumeStream();

                            }

                        }
                    }
                }
            };

            stream.StartStreamMatchingAllConditions();
        }
    }

}

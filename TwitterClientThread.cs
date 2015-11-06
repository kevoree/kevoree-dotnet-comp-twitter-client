using LinqToTwitter;
using Newtonsoft.Json.Linq;
using Org.Kevoree.Core.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Org.Kevoree.Library.Comp.Twitter.Client
{
    class TwitterClientThread
    {
        private TwitterClient twitterClient;

        private bool _stop = false;

        public TwitterClientThread(TwitterClient twitterClient)
        {
            this.twitterClient = twitterClient;
        }
        internal void Start()
        {
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = twitterClient.getConsumerKey(),
                    ConsumerSecret = twitterClient.getConsumerSecret(),
                    AccessToken = twitterClient.getAccessToken(),
                    AccessTokenSecret = twitterClient.getAccessTokenSecret()
                }
            };

            var twitterCtx = new TwitterContext(auth);

            callAsync(twitterCtx, twitterClient.getKeyword(), twitterClient.getTwit());

            while (!_stop)
            {
                Thread.Sleep(1000);
            }
        }

        private async static void callAsync(TwitterContext twitterCtx, string keyword, Port port)
        {
            var cancelTokenSrc = new CancellationTokenSource();

            await
                    (from strm in twitterCtx.Streaming.WithCancellation(cancelTokenSrc.Token)
                     where strm.Type == StreamingType.Filter && strm.Track == keyword
                     select strm).StartAsync(async strm => { await HandleStreamResponse(strm, port); });
        }

        static async Task<int> HandleStreamResponse(StreamContent strm, Port port)
        {
            switch (strm.EntityType)
            {
                case StreamEntityType.Status:
                    var status = strm.Entity as Status;
                    JObject o = new JObject();
                    o["Account"] = status.User.ScreenNameResponse;
                    o["Message"] = status.Text;
                    port.send(o.ToString(), null);
                    break;
                default:
                    // we only handle Status messages, other are just skipped.
                    break;
            }

            return await Task.FromResult(0);
        }

        internal void Stop()
        {
            this._stop = true;
        }
    }
}

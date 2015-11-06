using Org.Kevoree.Annotation;
using Org.Kevoree.Core.Api;
using Org.Kevoree.Log.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Org.Kevoree.Library.Comp.Twitter.Client
{
    [ComponentType]
    [Serializable]
    public class TwitterClient
    {
        [KevoreeInject]
        private ILogger _logger;

        [Output]
        private Port twit;

        [Param(Optional = false)]
        private string consumerKey;

        [Param(Optional = false)]
        private string consumerSecret;

        [Param(Optional = false)]
        private string accessToken;

        [Param(Optional = false)]
        private string accessTokenSecret;

        [Param(Optional = false)]
        private string keyword;

        private TwitterClientThread tcInstance;

        [Start]
        public void Start()
        {
            tcInstance = new TwitterClientThread(this);
            new Thread(new ThreadStart(tcInstance.Start)).Start();
        }

        [Stop]
        public void Stop()
        {
            this.tcInstance.Stop();
        }

        internal string getConsumerKey()
        {
            return this.consumerKey;
        }

        internal string getConsumerSecret()
        {
            return this.consumerSecret;
        }

        internal string getKeyword()
        {
            return this.keyword;
        }

        internal Port getTwit()
        {
            return this.twit;
        }

        internal string getAccessToken()
        {
            return this.accessToken;
        }

        internal string getAccessTokenSecret()
        {
            return this.accessTokenSecret;
        }
    }
}

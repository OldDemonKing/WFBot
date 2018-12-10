﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TRKS.WF.QQBot
{
    public static class WebHelper
    {
        static WebHelper()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        private static ThreadLocal<WebClient> webClient = new ThreadLocal<WebClient>(() => new WebClientEx2() { Encoding = Encoding.UTF8 });
        public static T DownloadJson<T>(string url)
        {
            a:
            try
            {
                return webClient.Value.DownloadString(url).JsonDeserialize<T>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                goto a;
            }
        }


        public static T DownloadJson<T>(string url, string body)
        {
            return webClient.Value.UploadString(url, body).JsonDeserialize<T>();
        }
    }

    public class WebClientEx2 : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            var rq = (HttpWebRequest)base.GetWebRequest(address);
            if (rq != null)
            {
                rq.KeepAlive = false;
            }
            return rq;
        }
    }
}

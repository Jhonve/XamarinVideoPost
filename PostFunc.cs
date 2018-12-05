using System.Text;
using System;
using System.Net;
using System.IO;
using System.Collections.Generic;

namespace PostFunc
{
    public class MyPost
    {
        public static string PostFunc(string image)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "image", image },
            };
            StringBuilder json_f = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                {
                    json_f.Append("&");
                }
                json_f.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }

            string url = "http://10.76.5.74:8000";
            // string json_f = "{\"name\":\"shen\"}";

            var httpReq = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            httpReq.Method = "POST";
            httpReq.ContentType = "application/x-www-form-urlencoded";

            byte[] data = Encoding.ASCII.GetBytes(json_f.ToString());

            httpReq.ContentLength = data.Length;

            using (Stream reqStream = httpReq.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }

            HttpWebResponse httpRes = (HttpWebResponse)httpReq.GetResponse();
            Stream stream = httpRes.GetResponseStream();

            string myResult = "connection failed!";

            if (httpRes.StatusCode == HttpStatusCode.OK)
            {
                myResult = "connection succeed";
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    myResult = reader.ReadToEnd();
                }
            }
            return myResult;
        }
    }
}
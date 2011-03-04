using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace RevitTicTacToe
{
    public class MultiplayerServer
    {
        private const string BASE_URL = "http://tictactoe.punchyn.com/";
        public string StartNewGame()
        {
            string result = NewPostRequest("Game/NewGame/", new Dictionary<string, string>());
            return result;
        }

        /// <summary>
        /// Sends a post request to the sever
        /// </summary>
        /// <param name="method"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        private string NewPostRequest(string method, IDictionary<string, string> keyValues)
        {
            Uri address = new Uri(BASE_URL + method);
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            StringBuilder data = new StringBuilder();

            bool first = true;
            foreach (string key in keyValues.Keys)
            {
                if (!first)
                    data.Append("&");
                data.Append(key + "=" + HttpUtility.UrlEncode(keyValues[key]));
                first = false;
            }

            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());
            request.ContentLength = byteData.Length;

            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Get the response stream  
                StreamReader reader = new StreamReader(response.GetResponseStream());

                // Console application output  
               return reader.ReadToEnd();
            }
        }


        /// <summary>
        /// Sends a get request to the server
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private string NewGetRequest(string method)
        {
            HttpWebRequest request = WebRequest.Create(BASE_URL + method) as HttpWebRequest;

            // Get response  
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Get the response stream  
                StreamReader reader = new StreamReader(response.GetResponseStream());

                // Console application output  
                return reader.ReadToEnd();
            }  
        }
    }
}

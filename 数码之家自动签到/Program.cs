using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Configuration;
using System.IO;

namespace autocheck.mydigit.cn
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isCheck = false;
            while (true)
            {
                if (DateTime.Now.Hour > 8)
                {
                    if (!isCheck)
                    {
                        if (check())
                        {
                            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "已签到成功！");
                            isCheck = true;
                        }
                    }
                }
                if (isCheck && DateTime.Now.Hour == 0)
                {
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "复位签到标记");
                    isCheck = false;
                }
                Thread.Sleep(1000 * 60 * 30);
            }
        }
        static string GetAppSettingsValue(string key)
        {
              ConfigurationManager.RefreshSection("appSettings");
            return ConfigurationManager.AppSettings[key];
        }
        static bool check()
        {
            string VhUn_2132_auth = GetAppSettingsValue("VhUn_2132_auth");
            string VhUn_2132_saltkey = GetAppSettingsValue("VhUn_2132_saltkey");
            string formhash = GetAppSettingsValue("formhash");
            string Http_UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.71 Safari/537.36 Core/1.94.238.400 QQBrowser/12.4.5622.400";
            string url = "https://www.mydigit.cn/plugin.php?id=k_misign:sign&operation=qiandao&formhash=" + formhash + "&format=empty&inajax=1&ajaxtarget=";
            var cookie = new CookieContainer();
            cookie.Add(new Cookie() { Domain = "www.mydigit.cn", Path = "/", Name = "VhUn_2132_auth", Value = VhUn_2132_auth });
            cookie.Add(new Cookie() { Domain = "www.mydigit.cn", Path = "/", Name = "VhUn_2132_saltkey", Value = VhUn_2132_saltkey });
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF8";
                request.CookieContainer = cookie;
                request.KeepAlive = true;
                request.UserAgent = Http_UserAgent;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Cookies = cookie.GetCookies(response.ResponseUri);
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                if (retString.Contains("今日已签"))
                {
                    return true;
                } 
            }
            catch (Exception ex)
            {

            }
            return false;
        }
    }
}

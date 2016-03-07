using System;
using System.Net;
using System.IO;
using System.Text;
using HtmlAgilityPack;

namespace EasySpider
{
	public class HTMLDownloader
	{
		int timeOut = 15000;

		public int TimeOut{ get { return timeOut; } set { timeOut = value; } }

		string userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.97 Safari/537.11";

		public string UserAgent { get { return userAgent; } set { userAgent = value; } }

		public bool KeepCookie { get; set; }

		public bool LockHost { get; set; }

		public bool LimitSpeed { get; set; }

		public event HTMLDownLoadedHandler downloadedEvent;

		public string Download (string url)
		{
			HttpWebRequest httpRequest = null;
			//httpRequest.UserAgent = UserAgent;
			HttpWebResponse httpResponse = null;
			Stream dataStream;
			string HtmlContent = "";
			try {
				httpRequest = WebRequest.CreateHttp (url);
				httpResponse = httpRequest.GetResponse () as HttpWebResponse;
				dataStream = httpResponse.GetResponseStream ();
				StreamReader reader = new StreamReader (dataStream, Encoding.UTF8);
				HtmlContent = reader.ReadToEnd ();
				reader.Close ();
				dataStream.Close ();
				httpResponse.Close ();
				Console.WriteLine (url + " Downloaded");
				//downloadedEvent (url, HtmlContent);
			} catch (Exception e) {
				Console.WriteLine (url + "Failed");
			} finally {
				if (httpRequest != null)
					httpRequest.Abort ();
				if (httpResponse != null)
					httpResponse.Close ();
			}
			return HtmlContent;
		}
	}
}


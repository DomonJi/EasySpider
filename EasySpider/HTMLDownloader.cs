using System;
using System.Net;
using System.IO;
using System.Text;

namespace EasySpider
{
	public class HTMLDownloader
	{
		int timeOut = 10000;

		public int TimeOut{ get { return timeOut; } set { timeOut = value; } }

		string userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.97 Safari/537.11";

		public string UserAgent { get { return userAgent; } set { userAgent = value; } }

		public bool KeepCookie { get; set; }

		public bool LockHost { get; set; }

		public bool LimitSpeed { get; set; }

		public string Download (string url)
		{
			HttpWebRequest request = null;
			HttpWebResponse response = null;
			Stream dataStream;
			string HtmlContent = "";
			try {
				request = WebRequest.CreateHttp (url);
				response = request.GetResponse () as HttpWebResponse;
				dataStream = response.GetResponseStream ();
				StreamReader reader = new StreamReader (dataStream, Encoding.UTF8);
				HtmlContent = reader.ReadToEnd ();
				reader.Close ();
				dataStream.Close ();
				response.Close ();
				Console.WriteLine (url + " Downloaded");
			} catch {
				Console.WriteLine (url + "Failed");
			} finally {
				if (request != null)
					request.Abort ();
				if (response != null)
					response.Close ();
			}
			return HtmlContent;
		}
	}
}


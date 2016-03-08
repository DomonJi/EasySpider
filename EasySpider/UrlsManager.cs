using System;
using System.Collections.Generic;
using System.Collections;
using System.Security.Policy;
using System.Linq;
using System.Globalization;

namespace EasySpider
{

	public class UrlsManager
	{

		int crawDepth = 10;

		public int CrawDepth { get { return crawDepth; } set { crawDepth = value; } }

		Queue<KeyValuePair<string,int>> newUrlQueue = new Queue<KeyValuePair<string,int>> ();
		readonly List<string> visitedURLs = new List<string> ();

		public bool HasNewUrl{ get { return newUrlQueue.Count > 0; } }

		public void AddUrl (KeyValuePair<string,int> url)
		{
			if (string.IsNullOrEmpty (url.Key) || newUrlQueue.Any (uinfo => uinfo.Key == url.Key) || visitedURLs.Contains (url.Key) || url.Value > CrawDepth)
				return;
			newUrlQueue.Enqueue (url);
		}

		public void AddUrls (IEnumerable<KeyValuePair<string,int>> urls)
		{
			if (urls == null)
				return;
			foreach (var url in urls) {
				AddUrl (url);
			}
		}

		public KeyValuePair<string,int> GetUrl ()
		{
			var newUrl = newUrlQueue.Dequeue ();
			visitedURLs.Add (newUrl.Key);
			return newUrl;
		}

		public int GetOldNum ()
		{
			return visitedURLs.Count;
		}
	}

}


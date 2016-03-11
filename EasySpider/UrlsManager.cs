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

		readonly Queue<KeyValuePair<string,int>> newURLQueue = new Queue<KeyValuePair<string,int>> ();

		BloomFilter bloomFilter = new BloomFilter (100000, 3);

		public BloomFilter Bloom { get { return bloomFilter; } set { bloomFilter = value; } }

		public bool HasNewUrl{ get { return newURLQueue.Count > 0; } }

		public void AddUrl (KeyValuePair<string,int> url)
		{
			if (string.IsNullOrEmpty (url.Key) || newURLQueue.Any (uinfo => uinfo.Key == url.Key) || Bloom.Test (url.Key) || url.Value > CrawDepth)
				return;
			newURLQueue.Enqueue (url);
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
			var newURL = newURLQueue.Dequeue ();
			Bloom.Add (newURL.Key);
			return newURL;
		}

		public int GetNewNum ()
		{
			return newURLQueue.Count;
		}
	}
}


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

	public class BloomFilter
	{
		readonly BitArray hashBits;
		int numKeys;
		readonly int[] hashKeys;

		public BloomFilter (int tableSize, int nKeys)
		{
			numKeys = nKeys;
			hashKeys = new int[numKeys];
			hashBits = new BitArray (tableSize);
		}

		int HashString (string s)
		{
			int hash = 0;
			for (int i = 0; i < s.Length; i++) {
				hash += s [i];
				hash += (hash << 10);
				hash ^= (hash >> 6);
			}
			hash += (hash << 3);
			hash ^= (hash >> 11);
			hash += (hash << 15);
			return hash;
		}

		void CreateHashes (string str)
		{
			int hash1 = str.GetHashCode ();
			int hash2 = HashString (str);

			hashKeys [0] = Math.Abs (hash1 % hashBits.Count);
			for (int i = 1; i < numKeys; i++) {
				hashKeys [i] = Math.Abs ((hash1 + (i * hash2)) % hashBits.Count);
			}
		}

		public bool Test (string str)
		{
			CreateHashes (str);
			foreach (int hash in hashKeys) {
				if (!hashBits [hash])
					return false;
			}
			return true;
		}

		public bool Add (string str)
		{
			bool rslt = true;
			CreateHashes (str);
			foreach (int hash in hashKeys) {
				if (!hashBits [hash]) {
					rslt = false;
					hashBits [hash] = true;
				}
			}
			return rslt;
		}
	}

}


using System;
using System.Collections;

namespace EasySpider
{
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


using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using System.Text.RegularExpressions;

namespace EasySpider
{
	public class DataHandler
	{
		readonly DataBase dataBase = new DataBase ("mongodb://127.0.0.1:27017", "Spider");

		public string[] URLRegexFilters{ get; set; }

		public KeyValuePair<int,Func<string,bool>>[] ContentFilters{ get; set; }

		public Func<List<string>[],KeyValuePair<string,string>> OutPuter{ get; set; }

		public int[] Matches{ get; set; }

		public Action<List<string>[]> UnionFilter{ get; set; }

		public async void Output (KeyValuePair<string,string> kvp)
		{
			StreamWriter sw = new StreamWriter (kvp.Key, true, System.Text.Encoding.Unicode);
			await sw.WriteAsync (kvp.Value);
			sw.Close ();
		}

		public void CollectData (string url, int depth, string html, List<string>[] content)
		{
			if (content.Any (c => c == null))
				return;
			if (URLRegexFilters != null && URLRegexFilters.All (f => !Regex.IsMatch (url, f, RegexOptions.IgnoreCase)))
				return;
			if (ContentFilters != null) {
				for (int i = 0; i < ContentFilters.Length; i++) {
					var toRemove = new List<string> ();
					content [ContentFilters [i].Key].ForEach (c => {
						if (!ContentFilters [i].Value (c))
							toRemove.Add (c);
					});
					toRemove.ForEach (a => content [i].Remove (a));
				}
			}
			if (UnionFilter != null) {
				UnionFilter (content);
			}
			if (content.Any (c => c.Count < 1))
				return;
			//dataBase.Add (url, new URLInfo{ Depth = depth,  SlelectedContent = content });
			dataBase.Add (new URLInfo{ URL = url, Depth = depth, SlelectedContent = content });
			Output (OutPuter (content));
		}
	}

	//	public struct URLInfo
	//	{
	//		public int Depth{ get; set; }
	//
	//		public string HTMLContent{ get; set; }
	//
	//		public List<string>[] SlelectedContent{ get; set; }
	//	}
}


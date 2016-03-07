using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;

namespace EasySpider
{
	public class DataHandler
	{
		readonly Dictionary<string,URLInfo> dataBase = new Dictionary<string, URLInfo> ();

		public string[] KeywordsFilters { get; set; }

		public void Output (Object file)
		{
			KeyValuePair<string,string> kvp = (KeyValuePair<string,string>)file;
			StreamWriter sw = new StreamWriter (kvp.Key, true, System.Text.Encoding.Unicode);
			sw.Write (kvp.Value);
			sw.Close ();
		}

		public void CollectData (string url, int depth, string html, string[] content)
		{
			if (content.All (c => c == null))
				return;
			if (KeywordsFilters.Any (k => !string.IsNullOrEmpty (k))) {
				if (content.All (c => c != null && !KeywordsFilters.Any (c.Contains))) {
					return;
				}
			}
			dataBase.Add (url, new URLInfo{ Depth = depth, HTMLContent = html, SlelectedContent = content });
			new Thread (Output).Start (new KeyValuePair<string,string> (content [0].Replace ("\n", "") + ".txt", content [0] + "\n" + content [1]));
		}

	}

	public class URLInfo
	{
		public int Depth{ get; set; }

		public string HTMLContent{ get; set; }

		public string[] SlelectedContent{ get; set; }
	}
}


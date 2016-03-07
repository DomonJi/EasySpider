using System;
using System.Collections.Generic;
using System.IO;

namespace EasySpider
{
	public class DataHandler
	{
		readonly Dictionary<string,URLInfo> dataBase = new Dictionary<string, URLInfo> ();

		public void Output ()
		{
		}

		public void CollectData (string url, int depth, string html, string[] content)
		{
			dataBase.Add (url, new URLInfo{ Depth = depth, HTMLContent = html, FilteredContent = content });
			var res = "";
			res = url + "\n" + depth;
			for (int i = 0; i < content.Length; i++) {
				res += content [i] + "\n";
			}
			//Console.WriteLine (res);
			if (content [0] != null) {
				StreamWriter sw = new StreamWriter (content [0].Replace ("\n", "") + ".txt", true, System.Text.Encoding.Unicode);
				sw.Write (content [0] + "\n" + content [1]);
				sw.Close ();
			}
		}

	}

	public class URLInfo
	{
		public int Depth{ get; set; }

		public string HTMLContent{ get; set; }

		public string[] FilteredContent{ get; set; }
	}
}


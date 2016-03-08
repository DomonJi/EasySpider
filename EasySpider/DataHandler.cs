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
		readonly Dictionary<string,URLInfo> dataBase = new Dictionary<string, URLInfo> ();

		public string[] URLRegexFilters{ get; set; }

		public KeyValuePair<int,Func<string,bool>>[] ContentFilters{ get; set; }

		public Func<string[],KeyValuePair<string,string>> OutPuterNameRule{ get; set; }

		public void Output (Object file)
		{
			KeyValuePair<string,string> kvp = (KeyValuePair<string,string>)file;
			StreamWriter sw = new StreamWriter (kvp.Key, true, System.Text.Encoding.Unicode);
			sw.Write (kvp.Value);
			sw.Close ();
		}

		public void CollectData (string url, int depth, string html, string[] content)
		{
			if (content.Any (c => c == null))
				return;
			if (URLRegexFilters != null && URLRegexFilters.All (f => !Regex.IsMatch (url, f, RegexOptions.IgnoreCase)))
				return;
			if (ContentFilters.Any (s => s.Value != null && content [s.Key] != null && !s.Value (content [s.Key])))
				return;
			dataBase.Add (url, new URLInfo{ Depth = depth, HTMLContent = html, SlelectedContent = content });
			new Thread (Output).Start (OutPuterNameRule (content));
		}

	}

	public struct URLInfo
	{
		public int Depth{ get; set; }

		public string HTMLContent{ get; set; }

		public string[] SlelectedContent{ get; set; }
	}
}


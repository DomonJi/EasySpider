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

		public void CollectData (string url, int depth, string html, object content)
		{
			if (URLRegexFilters != null && URLRegexFilters.All (f => !Regex.IsMatch (url, f, RegexOptions.IgnoreCase)) || content == null)
				return;
			dataBase.Add (new URLInfo{ URL = url, Depth = depth, SlelectedContent = content });
		}
	}
}


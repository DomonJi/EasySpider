using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace EasySpider
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			SpiderPrefabs.zhihuSpider.Crawl ();
		}
	}
}

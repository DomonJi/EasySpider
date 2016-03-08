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
			new Spider ("https://www.zhihu.com/topics") {
				ThreadsNum = 8,
				Downloader = new HTMLDownloader {
					TimeOut = 10000,
				},
				UrlsMng = new UrlsManager {
					CrawDepth = 5,
				},
				Parser = new HTMLParser {
					URLRegexFilter = new []{ @".*?question.*?", @".*?topic.*?" },
					EscapeWords = new []{ "编辑于", "发布于", "按时间排序", "什么是答案总结", "查看全部" },
					URLSdantarlize = u => {
						if (!u.StartsWith ("https://www.zhihu.com"))
							u = "https://www.zhihu.com" + u;
						if (u.Contains ("answer"))
							u = Regex.Split (u, "answer", RegexOptions.IgnoreCase) [0];
						return u;
					},
					XPathSelectors = new [] {
						"//*[@class=\"zm-item-title zm-editable-content\"]",
						"//*[@class=\"zm-editable-content clearfix\"]",
					},
				},
				DataHdler = new DataHandler {
					URLRegexFilters = new []{ @".*?question.*?" },
					ContentFilters = new KeyValuePair<int, Func<string, bool>>[] {
						new KeyValuePair<int, Func<string, bool>> (0, s => new []{ "考研", "工作", "恋爱", "爱情" }.Any (s.Contains)),
						new KeyValuePair<int, Func<string, bool>> (1, s => new []{ "考研", "工作" }.Any (s.Contains)),
					},
					OutPuterNameRule = new Func<string[], KeyValuePair<string, string>> (
						s => new KeyValuePair<string,string> (
							s [0].Replace ("\n", "").Replace ("/", "") + ".txt", 
							s [0] + "\n" + s [1]
						)
					)
				}
			}.Crawl ();
		}
	}
}

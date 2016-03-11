using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace EasySpider
{
	public static class SpiderPrefabs
	{
		public static Spider zhihuSpider = new Spider ("https://www.zhihu.com/topics") {
			ThreadsNum = 16,
			Downloader = new HTMLDownloader {
				TimeOut = 10000,
			},
			UrlsMng = new UrlsManager {
				CrawDepth = 8,
				Bloom = new BloomFilter (100000, 4),
			},
			Parser = new HTMLParser {
				URLRegexFilter = new []{ @".*?question.*?", @".*?topic.*?" },
				EscapeWords = new []{ "编辑于", "发布于", "按时间排序", "什么是答案总结", "查看全部" },
				URLSdantarlize = u => {
					if (!u.Contains ("zhihu.com"))
						u = "https://www.zhihu.com" + u;
					if (u.Contains ("answer"))
						u = Regex.Split (u, "answer", RegexOptions.IgnoreCase) [0];
					if (u.EndsWith ("/un"))
						u = Regex.Split (u, "/un", RegexOptions.IgnoreCase) [0];
					return u;
				},
				XPathSelectors = new [] {
					"//*[@class=\"zm-item-title zm-editable-content\"]",
					"//*[@class=\"zm-editable-content clearfix\"]",
					"//*[@id=\"zh-question-answer-wrap\"]//*[@class=\"count\"]",
				},
			},
			DataHdler = new DataHandler {
				URLRegexFilters = new []{ @".*?question.*?" },
				ContentFilters = new KeyValuePair<int, Func<string, bool>>[] {
					//new KeyValuePair<int, Func<string, bool>> (0, s => new []{ "考研", "工作", "恋爱", "爱情", "互联网", "应用", "旅行" }.Any (s.Contains)),
					new KeyValuePair<int, Func<string, bool>> (1, s => new []{ "考研", "工作", "恋爱", "爱情", "互联网", "应用", "旅行" }.Any (s.Contains)),
					new KeyValuePair<int, Func<string, bool>> (2, s => Int32.Parse (s.Replace ("K", "000")) > 1000),
				},
				UnionFilter = cla => {
					var n1 = new List<string> ();
					var n2 = new List<string> ();
					for (int i = 0; i < (cla [1].Count > cla [2].Count ? cla [2].Count : cla [1].Count); i++) {
						if (!string.IsNullOrEmpty (cla [1] [i]) && !string.IsNullOrEmpty (cla [2] [i])) {
							n1.Add (cla [1] [i]);
							n2.Add (cla [2] [i]);
						}
					}
					cla [1] = n1;
					cla [2] = n2;
				},
				OutPuter = new Func<List<string>[], KeyValuePair<string, string>> (
					s => {
						string content = s [0] [0];
						for (int i = 0; i < (s [1].Count > s [2].Count ? s [2].Count : s [1].Count); i++) {
							content += s [2] [i] + "\n" + s [1] [i] + "\n";
						}
						return new KeyValuePair<string,string> (s [0] [0].Replace ("\n", "").Replace ("/", "") + ".txt", content);
					}
				)
			}
		};
	}
}


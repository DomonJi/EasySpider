using System;
using System.Net;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace EasySpider
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			new Spider ("https://www.zhihu.com/question/20899988") {
				ThreadsNum = 4,
				Downloader = new HTMLDownloader {
					TimeOut = 10000,
				},
				UrlsMng = new UrlsManager {
					CrawDepth = 4,
				},
				Parser = new HTMLParser {
					Keywords = new []{ "" },
					URLRegexFilter = new []{ @".*?question.*?" },
					EscapeWords = new []{ "编辑于", "发布于", "按时间排序", "什么是答案总结", "查看全部" },
					URLSdantarlize = u => {
						if (u.StartsWith ("/"))
							u = "https://www.zhihu.com" + u;
						if (u.Contains ("answer"))
							u = Regex.Split (u, "answer", RegexOptions.IgnoreCase) [0];
						return u;
					},
					XPathSelectors = new [] {
						"//*[@class=\"zm-item-title zm-editable-content\"]",
						"//*[@class=\"zm-editable-content clearfix\"]"
					},
				}
			}.Crawl ();
		}
	}
}

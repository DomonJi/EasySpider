using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

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
				ContentSelector = hd => {
					var quesitionNode = hd.SelectSingleNode ("//*[@class=\"zm-item-title zm-editable-content\"]");
					var questiondetailNode = hd.SelectSingleNode ("//*[@id=\"zh-question-detail\"]/div");
					if (quesitionNode == null || questiondetailNode == null)
						return null;
					var answer = hd.SelectNodes ("//*[@class=\"zm-item-answer  zm-item-expanded\"]");
					List<object> answerWrap = new List<object> ();
					if (answer != null)
						answer.ToList ().ForEach (a => {
							var a1 = a.SelectSingleNode (".//*[@class=\"count\"]");
							var a2 = a.SelectSingleNode (".//*[@class=\"zm-editable-content clearfix\"]");
							answerWrap.Add (new {
								Agree = a1 != null ? a1.InnerText : "",
								AnswerContent = a2 != null ? a2.InnerText : "",
							});
						});
					return new {Question = quesitionNode.InnerText,QuesDetail = questiondetailNode.InnerText,Answers = answerWrap};
				}
			},
			DataHdler = new DataHandler {
				URLRegexFilters = new []{ @".*?question.*?" },
			}
		};
	}
}


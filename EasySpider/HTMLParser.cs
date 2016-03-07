using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using HtmlAgilityPack;
using System.Xml;

namespace EasySpider
{

	public class HTMLParser
	{
		public string[] Keywords{ get; set; }

		public string[] EscapeWords{ get; set; }

		public string[] URLRegexFilter{ get; set; }

		public Func<string,string> URLSdantarlize{ get; set; }

		public string[] XPathSelectors{ get; set; }

		public Func<string,string> ContentHandler{ get; set; }

		public event LinksParsedHandler linksParsedEvent;

		public event ContentFilteredHandler contentFilteredEvent;

		public Dictionary<string,int> Parse (string htmlContent, int depth)
		{
			Dictionary<string,string> urls = new Dictionary<string, string> ();
			Dictionary<string,int> res = new Dictionary<string,int> ();
			var match = Regex.Match (htmlContent, @"(?i)<a .*?href=\""([^\""]+)\""[^>]*>(.*?)</a>");
			while (match.Success) {
				urls [match.Groups [1].Value] = Regex.Replace (match.Groups [2].Value, "(?i)<.*?>", "");
				match = match.NextMatch ();
			}
			foreach (var link in urls) {
				string href = link.Key;
				string linkDescription = link.Value;
				if (!string.IsNullOrEmpty (href)) {
					bool canBeAdded = true;
					if (EscapeWords != null) {
						canBeAdded = !EscapeWords.Any (linkDescription.Contains);
					}
					if (Keywords != null && !Keywords.Any (linkDescription.Contains)) {
						canBeAdded = false;
					}
					if (canBeAdded) {
						string url = contentStandarlize (href);
						string linkText = contentStandarlize (linkDescription);

						if (String.IsNullOrEmpty (url) ||
						    url.StartsWith ("#") ||
						    url.StartsWith ("mailto:", StringComparison.OrdinalIgnoreCase) ||
						    url.StartsWith ("javascript:", StringComparison.OrdinalIgnoreCase)) {
							continue;
						}
						url = URLSdantarlize (url);

						if (URLRegexFilter != null && !IsUrlMatchRegex (url, URLRegexFilter)) {
							continue;
						}
						if (!res.Keys.Contains (url)) {
							res.Add (url, depth + 1);
						}
					}

				}
			}
			return res;
		}

		public string[] ParseHTML (string html, string originURL)
		{
			HtmlDocument htmlDocument = new HtmlDocument ();
			htmlDocument.LoadHtml (html);
			int length = XPathSelectors.Length;
			string[] res = new string[length];
			List<HtmlNode>[] allNodes = new List<HtmlNode>[length];
			for (int i = 0; i < XPathSelectors.Length; i++) {
				allNodes [i] = new List<HtmlNode> ();
				var nodesCollection = htmlDocument.DocumentNode.SelectNodes (XPathSelectors [i]);
				if (nodesCollection != null) {
					nodesCollection.ToList ().ForEach (allNodes [i].Add);
					allNodes [i].ForEach (n => res [i] += ContentHandler != null ? (ContentHandler (n.InnerText) + "\n") : (n.InnerText + "\n"));
					contentStandarlize (res [i]);
				}
			}
			return res;
		}

		bool IsUrlMatchRegex (string url, string[] urlRegexFilter = null)
		{
			bool result;
			if (urlRegexFilter != null) {
				result = urlRegexFilter.Any (f => Regex.IsMatch (url, f, RegexOptions.IgnoreCase));
			} else {
				result = true;
			}
			return  result;
		}

		string contentStandarlize (string content)
		{
			return content
				.Replace ("%3f", "?")
				.Replace ("%3d", "=")
				.Replace ("%2f", "/")
				.Replace ("&amp;", "&")
				.Replace ("&lt;", "<")
				.Replace ("&gt;", ">")
				.Replace ("&raquo;", ">>");
		}
	}
}


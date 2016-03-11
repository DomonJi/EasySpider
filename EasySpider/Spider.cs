using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace EasySpider
{
	public class Spider
	{
		string rootUrl;
		int threadsNum = 4;
		Thread[] threads;
		bool[] idleThreads;

		public int ThreadsNum{ get { return threadsNum; } set { threadsNum = value; } }

		public UrlsManager UrlsMng{ get; set; }

		public HTMLDownloader Downloader { get; set; }

		public HTMLParser Parser{ get; set; }

		public DataHandler DataHdler;

		public Spider (string _rootUrl)
		{
			rootUrl = _rootUrl;
		}

		void Init ()
		{
			UrlsMng = UrlsMng ?? new UrlsManager ();
			Downloader = Downloader ?? new HTMLDownloader ();
			Parser = Parser ?? new HTMLParser ();
			DataHdler = DataHdler ?? new DataHandler ();
			threads = new Thread[threadsNum];
			idleThreads = new bool[threadsNum];
			for (int i = 0; i < threadsNum; i++) {
				threads [i] = new Thread (new ParameterizedThreadStart (CrawlProc));
			}
		}

		public void Crawl ()
		{
			Init ();
			Console.WriteLine ("Spider Initialized");
			UrlsMng.AddUrl (new KeyValuePair<string, int> (rootUrl, 0));

			for (int i = 0; i < threadsNum; i++) {
				threads [i].Start (i);
				idleThreads [i] = false;
				Console.WriteLine ("第" + (i + 1) + "条线程开启");
			}
		}

		public void Stop ()
		{
			for (int i = 0; i < ThreadsNum; i++) {
				threads [i].Abort ();
			}
		}

		void CrawlProc (object threadIndex)
		{
			var currentIndex = (int)threadIndex;
			while (true) {
				if (!UrlsMng.HasNewUrl) {
					idleThreads [currentIndex] = true;
					if (idleThreads.All (t => t)) {
						Console.WriteLine ("第" + currentIndex + "条线程退出");
						break;
					}
					Thread.Sleep (2000);
					continue;
				}
				idleThreads [currentIndex] = false;
			
				KeyValuePair<string,int> curntURL = new KeyValuePair<string, int> ();
				lock (UrlsMng) {
					if (UrlsMng.HasNewUrl)
						curntURL = UrlsMng.GetUrl ();
					else
						continue;
				}
				var html = Downloader.Download (curntURL.Key);
				var parseResult = Parser.ParseURLS (html, curntURL.Value);
				lock (UrlsMng)
					parseResult.Keys.ToList ().ForEach (url => UrlsMng.AddUrl (new KeyValuePair<string, int> (url, curntURL.Value + 1)));
				var filteredContent = Parser.ParseHTML (html);
				lock (DataHdler)
					DataHdler.CollectData (curntURL.Key, curntURL.Value, html, filteredContent);
			}
		}
	}
}


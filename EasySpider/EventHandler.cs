using System;

namespace EasySpider
{
	public delegate void NewURLAddedHandler (string url, int depth);
	public delegate void HTMLDownLoadedHandler (string url, string html);
	public delegate void LinksParsedHandler (string newURL, string decription);
	public delegate void ContentFilteredHandler (string url, string filteredContent);
}


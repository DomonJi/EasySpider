using System;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace EasySpider
{
	public class DataBase
	{
		MongoClient client;
		readonly IMongoDatabase db;

		public DataBase (string conctstr, string dbName)
		{
			try {
				client = new MongoClient (conctstr);
				db = client.GetDatabase (dbName);
			} catch (Exception ex) {
				throw new Exception ("DataBase Connect Failed");
			}
		}

		public async void Add<T> (T record)
		{
			IMongoCollection<T> collection = db.GetCollection<T> (typeof(T).Name);
			await collection.InsertOneAsync (record);
		}

		//		public T Get<T> (BsonDocument queryDocument)
		//		{
		//			IMongoCollection<T> collection = db.GetCollection<T> ("URLInfos");
		//			return collection.Find (queryDocument).FirstOrDefault ();
		//		}
	}

	public class URLInfo
	{
		public ObjectId Id{ get; set; }

		public string URL{ get; set; }

		public int Depth{ get; set; }

		//public string HTMLContent{ get; set; }

		public List<string>[] SlelectedContent{ get; set; }
	}
}


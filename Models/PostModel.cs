using System;
namespace DataMining_MVC.Models
{
	public class PostModel
    {
        public string id { get; set; }
        public string Title { get; set; }
        public int Ups { get; set; }
        public string Url { get; set; }
        public int Commnets { get; set; }
        public DateTime timestamp { get; set; }
    }
}


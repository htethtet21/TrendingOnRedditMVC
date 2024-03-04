using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DataMining_MVC.Models
{
	public class TrendingPostViewModel
	{
        public Guid Id { get; set; }
        [Required]
        [DisplayName("TrendingPost")]
        public String PostName { get; set; }
        [Required]
        [DisplayName("TotalVotes")]
        public int votes { get; set; }
        [DisplayName("TotalComments")]
        public int? comments { get; set; }
        public String URL { get; set; }
        [DisplayName("Time")]
        public DateTime TimeStamp { get; set; }
        public int SortIndex { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class PdfViewModel
    {
        public string Base64String { get; set; }
        public string FileName { get; set; }
    }


}


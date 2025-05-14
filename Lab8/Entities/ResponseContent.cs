using System.Net;

namespace Lab8.Entities
{
    public class ResponseContent
    {
        public int? id { get; set; }
        public ShopApiStatus status { get; set; }
        public string error { get; set; }
    }
}

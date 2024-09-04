using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firebase.CRUD.Models
{
    public class ApiResponse
    {
        public bool success {  get; set; }
        public string message { get; set; }
        public int code { get; set; }
        public Order order { get; set; }
        public List<Order> orders { get; set; }
    }
    public class ApiErrorResponse
    {
        public int code { get; set; }
        public string message { get; set; }
    }
}

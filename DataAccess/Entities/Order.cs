using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public ICollection<Product> Products { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.New;

        public override string ToString()
        {
            return $"{Id}";
        }
    }
    public enum OrderStatus
    {
        New,
        Preparing,
        Ready
    }
}

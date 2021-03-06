using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class UserMoney
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public double MoneyValue { get; set; }
        public DateTime LastUpdate { get; set; }
        public string Type { get; set; }
    }

    public class MoneyType
    {
        public string Code { get; set; }
        public string Text { get; set; }
    }

    public class MoneyHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public double Money { get; set; }
        public string Reason { get; set; }
        public DateTime UpdateTime { get; set; }
        public int Times { get; set; }
        public string Type { get; set; }
        public bool State { get; set; }
    }
}

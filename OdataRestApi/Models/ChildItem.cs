using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdataRestApi.Models
{
    public class ChildItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public long TodoItemId { get; set; }
    }
}
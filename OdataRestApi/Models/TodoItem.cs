using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace OdataRestApi.Models
{
    public class TodoItem
    {
        public TodoItem()
        {
            ChildItems = new Collection<ChildItem>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
        public virtual ICollection<ChildItem> ChildItems { get; set; }
    }
}
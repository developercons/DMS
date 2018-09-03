using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
   public class DocumentViewModel
    {
        public int DocumentId { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentName { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}

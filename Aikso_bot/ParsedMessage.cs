using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikso_bot
{
    public class ParsedMessage
    {
       

        public string UserEmail { get; set; }
        public string Executor { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime DateTimeNotification { get; set; }
    }

    

}

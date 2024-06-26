using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Message
    {
        public int? id { get; set; }
        public int? SenderId { get; set; }
        public string? SenderUsername { get; set; }
        public AppUsers? Sender { get; set; }

        public int? RecipientId { get; set; }
        public string? RecipientUsername { get; set; }
        public AppUsers? Recipient { get; set; }
        public string? Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.Now;
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }

    }
}
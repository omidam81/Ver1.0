﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class MessageRecord
    {
        public virtual int Id { get; set; }

        public virtual string Text { get; set; }

        public virtual string From { get; set; }

        public virtual DateTime SendDate { get; set; }

        public virtual int UserId { get; set; }
         
    }
}
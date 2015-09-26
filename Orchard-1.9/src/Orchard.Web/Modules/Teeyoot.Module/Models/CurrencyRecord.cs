﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class CurrencyRecord
    {
        public virtual int Id { get; set; }

        public virtual string Code { get; set; }

        public virtual string Name { get; set; }

        public virtual string ShortName { get; set; }

        //todo: drop this field 
        public virtual string CurrencyCulture { get; set; }

        public virtual string FlagFileName { get; set; }
    }
}
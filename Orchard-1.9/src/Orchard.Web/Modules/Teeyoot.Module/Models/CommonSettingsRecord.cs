﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class CommonSettingsRecord
    {
        public virtual int Id { get; set; }
        public virtual string Name{ get; set; }
        public virtual bool Value { get; set; }
    }
}
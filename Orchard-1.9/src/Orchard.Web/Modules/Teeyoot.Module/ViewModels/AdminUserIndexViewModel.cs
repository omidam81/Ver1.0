﻿using System.Collections.Generic;

namespace Teeyoot.Module.ViewModels
{
    public class AdminUserIndexViewModel
    {
        public IEnumerable<UserItemViewModel> Users { get; set; }
        public dynamic Pager { get; set; }
    }
}

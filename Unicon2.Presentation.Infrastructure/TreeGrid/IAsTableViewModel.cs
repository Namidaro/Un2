﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicon2.Presentation.Infrastructure.TreeGrid
{
   public interface IAsTableViewModel
    {
        bool IsTableView { get; set; }
        string AsossiatedDetailsViewName { get; }
    }
}
﻿using Unicon2.Infrastructure.Progress;

namespace Unicon2.Model.Progress
{
  public  class TaskProgressReport: ITaskProgressReport
    {
        #region Implementation of ITaskProgressReport

        public int CurrentProgressAmount { get; set; }
        public int TotalProgressAmount { get; set; }
        public string CurrentProgressMessage { get; set; }

        #endregion
    }
}

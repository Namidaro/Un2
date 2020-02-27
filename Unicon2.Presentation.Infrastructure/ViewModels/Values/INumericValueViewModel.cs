﻿using Unicon2.Infrastructure.Values;

namespace Unicon2.Presentation.Infrastructure.ViewModels.Values
{
    public interface INumericValueViewModel : IFormattedValueViewModel
    {
        string NumValue { get; set; }
    }
}
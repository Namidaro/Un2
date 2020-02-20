﻿using System;
using Unicon2.Infrastructure.Interfaces;
using Unicon2.Infrastructure.Values;

namespace Unicon2.Presentation.Infrastructure.ViewModels.Values
{
    public interface IFormattedValueViewModel : IStronglyNamed, IRangeable, IMeasurable
    {
        string Header { get; set; }
        void InitFromValue(IFormattedValue value);
    }
}
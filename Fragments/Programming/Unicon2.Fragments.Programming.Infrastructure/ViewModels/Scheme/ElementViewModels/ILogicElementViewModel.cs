﻿using System;
using System.Collections.ObjectModel;
using Unicon2.Infrastructure.ViewModel;

namespace Unicon2.Fragments.Programming.Infrastructure.ViewModels.Scheme.ElementViewModels
{
    public interface ILogicElementViewModel : ISelectable, IViewModel, ICanvasPosition, IDisposable
    {
        string ElementName { get; }
        string Caption { get; set; }
        string Description { get; }
        string Symbol { get; }
        bool ValidationError { get; set; }
        bool DebugMode { get; set; }
        ObservableCollection<IConnectorViewModel> Connectors { get; }
        object Clone();
        void OpenPropertyWindow();
    }
}
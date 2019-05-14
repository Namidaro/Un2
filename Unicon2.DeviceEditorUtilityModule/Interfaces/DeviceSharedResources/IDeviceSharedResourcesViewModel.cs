﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Unicon2.Infrastructure.ViewModel;
using Unicon2.Presentation.Infrastructure.ViewModels.Resources;

namespace Unicon2.DeviceEditorUtilityModule.Interfaces.DeviceSharedResources
{
    public interface IDeviceSharedResourcesViewModel : IViewModel
    {
        ObservableCollection<IResourceViewModel> ResourcesCollection { get; }

        IResourceViewModel SelectedResourceViewModel { get; set; }
        ICommand OpenResourceForEditingCommand { get; }
        ICommand SelectResourceCommand { get; }
        ICommand DeleteResourceCommand { get; }
        ICommand SubmitCommand { get; }
        ICommand CloseCommand { get; }
        ICommand RenameResourceCommand { get; }

        bool IsSelectingMode { get; set; }
        void Initialize(Type typeNeeded);
       
    }
}
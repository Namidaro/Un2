﻿using System.Windows.Input;
using Unicon2.DeviceEditorUtilityModule.ViewModels.Validation;
using Unicon2.Presentation.Infrastructure.ViewModels.Validation;

namespace Unicon2.DeviceEditorUtilityModule.Interfaces
{
    public interface IDeviceEditorViewModel
    {
        ICommand LoadExistingDevice { get; }
        ICommand CreateDeviceCommand { get; }
        ICommand SaveInFileCommand { get; }
        ICommand OpenSharedResourcesCommand { get; }
        ICommand DeleteFragmentCommand { get; }
        IDeviceEditorValidationViewModel DeviceEditorValidationViewModel { get; }
    }
}
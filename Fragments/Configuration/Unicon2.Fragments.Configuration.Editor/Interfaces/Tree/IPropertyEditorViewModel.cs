﻿using Unicon2.Fragments.Configuration.Editor.ViewModels;
using Unicon2.Fragments.Configuration.Infrastructure.ViewModel;
using Unicon2.Fragments.Configuration.Infrastructure.ViewModel.Properties;
using Unicon2.Infrastructure.Interfaces.EditOperations;
using Unicon2.Presentation.Infrastructure.ViewModels;

namespace Unicon2.Fragments.Configuration.Editor.Interfaces.Tree
{
    public interface IPropertyEditorViewModel :IEditorConfigurationItemViewModel, IPropertyViewModel, IEditable, IDeletable, IAddressIncreaseableDecreaseable,
        IUshortFormattableEditorViewModel
    {
        string Address { get; set; }
        string NumberOfPoints { get; set; }
    }
}
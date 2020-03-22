﻿using Unicon2.Fragments.Measuring.Editor.Interfaces.Factories;
using Unicon2.Fragments.Measuring.Editor.Interfaces.ViewModel;
using Unicon2.Fragments.Measuring.Infrastructure.Model;
using Unicon2.Unity.Interfaces;

namespace Unicon2.Fragments.Measuring.Editor.Factories
{
    public class MeasuringGroupEditorViewModelFactory : IMeasuringGroupEditorViewModelFactory
    {
        private readonly ITypesContainer _container;

        public MeasuringGroupEditorViewModelFactory(ITypesContainer container)
        {
            _container = container;
        }

        public IMeasuringGroupEditorViewModel CreateMeasuringGroupEditorViewModel(IMeasuringGroup measuringGroup)
        {
            IMeasuringGroupEditorViewModel measuringGroupEditorViewModel =
                _container.Resolve<IMeasuringGroupEditorViewModel>();
            measuringGroupEditorViewModel.Model = measuringGroup;
            return measuringGroupEditorViewModel;
        }

        public IMeasuringGroupEditorViewModel CreateMeasuringGroupEditorViewModel()
        {
            IMeasuringGroupEditorViewModel measuringGroupEditorViewModel =
                _container.Resolve<IMeasuringGroupEditorViewModel>();
            IMeasuringGroup measuringGroup = _container.Resolve<IMeasuringGroup>();
            measuringGroupEditorViewModel.Model = measuringGroup;
            measuringGroupEditorViewModel.Header = "Group";
            return measuringGroupEditorViewModel;
        }
    }
}

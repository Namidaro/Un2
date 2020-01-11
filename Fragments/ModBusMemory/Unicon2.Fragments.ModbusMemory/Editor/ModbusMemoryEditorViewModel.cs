﻿using Unicon2.Fragments.ModbusMemory.Infrastructure.Model;
using Unicon2.Infrastructure;
using Unicon2.Presentation.Infrastructure.ViewModels.FragmentInterfaces;
using Unicon2.Presentation.Infrastructure.ViewModels.FragmentInterfaces.FragmentOptions;

namespace Unicon2.Fragments.ModbusMemory.Editor
{
    public class ModbusMemoryEditorViewModel : IFragmentEditorViewModel, IFragmentViewModel
    {
        private IModbusMemory _model;

        public ModbusMemoryEditorViewModel(IModbusMemory modbusMemory)
        {
            this._model = modbusMemory;
        }


        public string StrongName => ApplicationGlobalNames.FragmentInjectcionStrings.MODBUSMEMORY + ApplicationGlobalNames.CommonInjectionStrings.EDITOR_VIEWMODEL;

        public object Model
        {
            get { return this._model; }
            set { this._model = value as IModbusMemory; }
        }

        public string NameForUiKey => ApplicationGlobalNames.FragmentInjectcionStrings.MODBUSMEMORY;

        public IFragmentOptionsViewModel FragmentOptionsViewModel { get; set; }
    }
}

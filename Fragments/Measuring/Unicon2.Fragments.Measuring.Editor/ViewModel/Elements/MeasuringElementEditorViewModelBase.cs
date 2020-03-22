﻿using System;
using Unicon2.Fragments.Measuring.Editor.Interfaces.ViewModel.Elements;
using Unicon2.Fragments.Measuring.Infrastructure.Model.Elements;
using Unicon2.Unity.ViewModels;

namespace Unicon2.Fragments.Measuring.Editor.ViewModel.Elements
{
    public abstract class MeasuringElementEditorViewModelBase : ViewModelBase, IMeasuringElementEditorViewModel
    {
        private string _header;

        public abstract string StrongName { get; }

        public string Header
        {
            get { return _header; }

            set
            {
                _header = value;
                RaisePropertyChanged();
            }
        }

        public abstract string NameForUiKey { get; }
        public Guid Id { get; private set; }
        public void SetId(Guid id)
        {
	        Id = id;
        }
    }
}

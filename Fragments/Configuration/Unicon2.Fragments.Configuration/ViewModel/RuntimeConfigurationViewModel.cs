﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Unicon2.Fragments.Configuration.Infrastructure.Factories;
using Unicon2.Fragments.Configuration.Infrastructure.StructItemsInterfaces;
using Unicon2.Fragments.Configuration.Infrastructure.ViewModel;
using Unicon2.Fragments.Configuration.ViewModel.Helpers;
using Unicon2.Infrastructure;
using Unicon2.Infrastructure.Extensions;
using Unicon2.Presentation.Infrastructure.TreeGrid;
using Unicon2.Presentation.Infrastructure.ViewModels.FragmentInterfaces;
using Unicon2.Presentation.Infrastructure.ViewModels.FragmentInterfaces.FragmentOptions;
using Unicon2.Unity.Commands;
using Unicon2.Unity.Common;
using Unicon2.Unity.Interfaces;
using Unicon2.Unity.ViewModels;

namespace Unicon2.Fragments.Configuration.ViewModel
{
    public class RuntimeConfigurationViewModel : ViewModelBase, IRuntimeConfigurationViewModel, IDeviceDataProvider
    {
        private IDeviceConfiguration _deviceConfiguration;
        private readonly ITypesContainer _container;
        private readonly IRuntimeConfigurationItemViewModelFactory _runtimeConfigurationItemViewModelFactory;

        public RuntimeConfigurationViewModel(ITypesContainer container,
            IRuntimeConfigurationItemViewModelFactory runtimeConfigurationItemViewModelFactory)
        {
            this._container = container;
            this._runtimeConfigurationItemViewModelFactory = runtimeConfigurationItemViewModelFactory;
            this.AllRows = new ObservableCollection<IRuntimeConfigurationItemViewModel>();
            this.MainRows = new ObservableCollection<MainConfigItemViewModel>();
            this.FragmentOptionsViewModel =
                (new ConfigurationOptionsHelper()).CreateConfigurationFragmentOptionsViewModel(this, this._container);
            this.RootConfigurationItemViewModels = new ObservableCollection<IRuntimeConfigurationItemViewModel>();
            MainItemSelectedCommand=new RelayCommand<object>(OnMainItemSelected);
        }

        private void OnMainItemSelected(object obj)
        {
            if(!(obj is MainConfigItemViewModel mainItem))return;
            var currentRows=new List<IConfigurationItemViewModel>();
            FillCurrentRows(currentRows, mainItem.RelatedConfigurationItemViewModel,0);
            CurrentRows = currentRows;
        }

        private void FillCurrentRows(List<IConfigurationItemViewModel> currentRows, IConfigurationItemViewModel row,int level)
        {
            foreach (var child in row.ChildStructItemViewModels)
            {
                child.Level=level;
                currentRows.Add(child);
                FillCurrentRows(currentRows,child,level+1);
            }
        }

        private ObservableCollection<IRuntimeConfigurationItemViewModel> _allRows;
        private IFragmentOptionsViewModel _fragmentOptionsViewModel;
        private ObservableCollection<IRuntimeConfigurationItemViewModel> _rootConfigurationItemViewModels;
        private string _deviceName;
        private ObservableCollection<MainConfigItemViewModel> _mainRows;
        private List<IConfigurationItemViewModel> _currentRows;

        public ICommand MainItemSelectedCommand { get; }

        public ObservableCollection<IRuntimeConfigurationItemViewModel> RootConfigurationItemViewModels
        {
            get { return this._rootConfigurationItemViewModels; }
            set
            {
                this._rootConfigurationItemViewModels = value;
                this.RaisePropertyChanged();

            }
        }

        public List<IConfigurationItemViewModel> CurrentRows
        {
            get => _currentRows;
            set
            {
                _currentRows = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<MainConfigItemViewModel> MainRows
        {
            get { return this._mainRows; }
            set
            {
                this._mainRows = value;
                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<IRuntimeConfigurationItemViewModel> AllRows
        {
            get { return this._allRows; }
            set
            {
                this._allRows = value;
                this.RaisePropertyChanged();
            }
        }

        public string StrongName => ApplicationGlobalNames.FragmentInjectcionStrings.RUNTIME_CONFIGURATION_VIEWMODEL;

        public string NameForUiKey => this._deviceConfiguration.StrongName;


        public IFragmentOptionsViewModel FragmentOptionsViewModel
        {
            get { return this._fragmentOptionsViewModel; }
            set
            {
                this._fragmentOptionsViewModel = value;
                this.RaisePropertyChanged();
            }
        }


        public object Model
        {
            get { return this._deviceConfiguration; }
            set
            {
                if (!(value is IDeviceConfiguration)) throw new ArgumentException();
                this.SetModel(value as IDeviceConfiguration);

            }
        }


        private void SetModel(IDeviceConfiguration deviceConfiguration)
        {
            if (this._deviceConfiguration == deviceConfiguration) return;
            this._deviceConfiguration?.Dispose();
            this.AllRows.Clear();
            this.RootConfigurationItemViewModels.Clear();
            this._deviceConfiguration = deviceConfiguration;
            if (!this._deviceConfiguration.IsInitialized)
            {
                this._deviceConfiguration.InitializeFromContainer(this._container);
            }

            if (this._deviceConfiguration.RootConfigurationItemList != null)
            {
                foreach (IConfigurationItem configurationItem in this._deviceConfiguration.RootConfigurationItemList)
                {
                    this.RootConfigurationItemViewModels.Add(this._runtimeConfigurationItemViewModelFactory
                        .CreateRuntimeConfigurationItemViewModel(configurationItem));

                }
            }

            this.AllRows.AddCollection(this.RootConfigurationItemViewModels);
            this.MainRows.AddCollection(FilterMainConfigItems(this.RootConfigurationItemViewModels));

        }

        private ObservableCollection<MainConfigItemViewModel> FilterMainConfigItems(
            IEnumerable<IConfigurationItemViewModel> rootItems)
        {
            var resultCollection = new ObservableCollection<MainConfigItemViewModel>();
            resultCollection.AddCollection(rootItems.Where(item =>
                item is IItemGroupViewModel itemGroupViewModel &&
                ((itemGroupViewModel.Model as IItemsGroup).IsMain ?? true)).Select(item =>
                new MainConfigItemViewModel(FilterMainConfigItems(item.ChildStructItemViewModels), item)));
            return resultCollection;
        }

        public void SetDeviceData(string deviceName)
        {
            _deviceName = deviceName;
        }

        public string GetDeviceName()
        {
            return _deviceName;
        }
    }
}
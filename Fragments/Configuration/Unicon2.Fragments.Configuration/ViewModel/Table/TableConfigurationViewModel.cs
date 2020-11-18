﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using Unicon2.Fragments.Configuration.Behaviors;
using Unicon2.Fragments.Configuration.Infrastructure.StructItemsInterfaces.Dependencies.Conditions;
using Unicon2.Fragments.Configuration.Infrastructure.ViewModel.Properties;
using Unicon2.Fragments.Configuration.Infrastructure.ViewModel.Runtime;
using Unicon2.Fragments.Configuration.ViewModel.Helpers;
using Unicon2.Infrastructure.Extensions;
using Unicon2.Infrastructure.Functional;
using Unicon2.Presentation.Infrastructure.DeviceContext;
using Unicon2.Presentation.Infrastructure.TreeGrid;
using Unicon2.Presentation.Infrastructure.ViewModels;
using Unicon2.Unity.ViewModels;

namespace Unicon2.Fragments.Configuration.ViewModel.Table
{

    public class ConfigItemWrapper
    {
        public ConfigItemWrapper(IEnumerable<ConfigItemWrapper> childConfigItemWrappers,
            IConfigurationItemViewModel relatedConfigurationItemViewModel, bool toInclude)
        {
            ChildConfigItemWrappers = childConfigItemWrappers;
            RelatedConfigurationItemViewModel = relatedConfigurationItemViewModel;
            ToInclude = toInclude;
        }

        public bool ToInclude { get; set; }
        public IConfigurationItemViewModel RelatedConfigurationItemViewModel { get; }
        public IEnumerable<ConfigItemWrapper> ChildConfigItemWrappers { get; }
    }
    public class TableConfigurationViewModel : ViewModelBase
    {
        private readonly List<IConfigurationItemViewModel> _itemGroupsToTransform;
        private List<ConfigItemWrapper> _filteredGroupsToTransform;

        private readonly List<RuntimeFilterViewModel> _runtimeFilterViewModels;
        private DynamicPropertiesTable _dynamicPropertiesTable;
        private DynamicPropertiesTable _filteredPropertiesTable;
        private bool _isFilterApplied;




        public TableConfigurationViewModel(List<IConfigurationItemViewModel> itemGroupsToTransform,
            List<RuntimeFilterViewModel> runtimeFilterViewModels)
        {
            _itemGroupsToTransform = itemGroupsToTransform;
            _runtimeFilterViewModels = runtimeFilterViewModels;
            Initialize();
        }

        public DynamicPropertiesTable DynamicPropertiesTable
        {
            get => _dynamicPropertiesTable;
            set => SetProperty(ref _dynamicPropertiesTable, value);
        }

        public DynamicPropertiesTable FilteredPropertiesTable
        {
            get => _filteredPropertiesTable;
            set => SetProperty(ref _filteredPropertiesTable, value);
        }

        public bool IsFilterApplied
        {
            get => _isFilterApplied;
            set
            {
                _isFilterApplied = value;
                RaisePropertyChanged();
            }
        }

        private List<ConfigItemWrapper> FillGroupCorrespondToFilters(
            IEnumerable<IConfigurationItemViewModel> viewModels, bool leftNotCorresponding, int? offset)
        {
            var result = new List<ConfigItemWrapper>();

            foreach (var viewModel in viewModels)
            {
                if (viewModel is RuntimeItemGroupViewModel runtimeItemGroupViewModel)
                {
                    result.Add(new ConfigItemWrapper(
                        FillGroupCorrespondToFilters(runtimeItemGroupViewModel.ChildStructItemViewModels, false, runtimeItemGroupViewModel.Offset),
                        viewModel, true));
                }

                if (viewModel is IRuntimePropertyViewModel propertyViewModel)
                {
                    result.Add(new ConfigItemWrapper(new List<ConfigItemWrapper>(), viewModel,
                        CheckConditions(GetValueToCompare(propertyViewModel,offset ?? 0))));
                }
            }

            return result;
        }


        private ushort GetValueToCompare(IRuntimePropertyViewModel runtimePropertyViewModel, int offset)
        {
            var propertyUshort =
                runtimePropertyViewModel.DeviceContext.DeviceMemory.LocalMemoryValues[(ushort)(runtimePropertyViewModel.Address+offset)];
            if (runtimePropertyViewModel is IRuntimeSubPropertyViewModel subPropertyViewModel)
            {
                var resultBitArray = new bool[16];
                var boolArray = propertyUshort.GetBoolArrayFromUshort();
                int counter = 0;
                for (int i = 0; i < 16; i++)
                {
                    if (subPropertyViewModel.BitNumbersInWord.Contains(i))
                    {
                        resultBitArray[counter] = boolArray[i];
                        counter++;
                    }

                }

                propertyUshort = resultBitArray.BoolArrayToUshort();
            }

            return propertyUshort;
        }

        private bool CheckConditions(ushort valueToCompare)
        {
            foreach (var filterViewModel in _runtimeFilterViewModels)
            {
                if (!filterViewModel.IsActivated) continue;

                foreach (var condition in filterViewModel.Conditions)
                {
                    var res = ConditionHelper.CheckCondition(condition as ICompareCondition,valueToCompare);
                    if (res.IsSuccess && !res.Item)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void Initialize()
        {
            IsFilterApplied = _runtimeFilterViewModels.Any(model => model.IsActivated);
            if (IsFilterApplied)
            {
                _filteredGroupsToTransform =
                    _itemGroupsToTransform.Select(model =>
                        new ConfigItemWrapper(FillGroupCorrespondToFilters(model.ChildStructItemViewModels, true,(model as RuntimeItemGroupViewModel)?.Offset),
                            model, true)).ToList();
          
                //filter rows
                _filteredGroupsToTransform = _filteredGroupsToTransform.Where(wrapper =>
                    wrapper.ChildConfigItemWrappers.Any(itemWrapper => itemWrapper.ToInclude)).ToList();

                if (_filteredGroupsToTransform.Any())
                {
                    var columnsNumber = _filteredGroupsToTransform.First().ChildConfigItemWrappers.Count();

                    for (int i = 0; i < columnsNumber; i++)
                    {
                        var column = _filteredGroupsToTransform
                            .Select(wrapper => wrapper.ChildConfigItemWrappers.ToList()[i])
                            .ToList();
                        bool toIncludeColumn = column.Any(wrapper => wrapper.ToInclude);
                        if (toIncludeColumn)
                        {
                            column.ForEach(wrapper => wrapper.ToInclude = true);
                        }
                    }
                }

                var columnNamesWithPropertiesFiltered = new List<Tuple<string, IConfigurationItemViewModel>>();
                FillFilteredColumnNames(_filteredGroupsToTransform, columnNamesWithPropertiesFiltered);
                var lookupFiltered =
                    columnNamesWithPropertiesFiltered.ToLookup(tuple => tuple.Item1, tuple => tuple.Item2);
                var columnNamesFiltered = lookupFiltered.Select(models => models.Key).ToList();
                FilteredPropertiesTable = new DynamicPropertiesTable(columnNamesFiltered,
                    _filteredGroupsToTransform.Select((model => model.RelatedConfigurationItemViewModel.Header))
                        .ToList(), false);
                _filteredGroupsToTransform.ForEach((group =>
                    FilteredPropertiesTable.AddPropertyViewModel(
                        GetRowFromItemGroupFiltered(group, lookupFiltered, columnNamesFiltered)
                            .Select((tuple => tuple.Value)).ToList())));
            }


            var columnNamesWithProperties = new List<Tuple<string, IConfigurationItemViewModel>>();
            FillColumnNames(_itemGroupsToTransform, columnNamesWithProperties);
            var lookup = columnNamesWithProperties.ToLookup(tuple => tuple.Item1, tuple => tuple.Item2);
            var columnNames = lookup.Select(models => models.Key).ToList();
            DynamicPropertiesTable = new DynamicPropertiesTable(columnNames,
                _itemGroupsToTransform.Select((model => model.Header)).ToList(), false);
            _itemGroupsToTransform.ForEach((group =>
                DynamicPropertiesTable.AddPropertyViewModel(GetRowFromItemGroup(group, lookup, columnNames)
                    .Select((tuple => tuple.Value)).ToList())));
        }




        private void FillFilteredColumnNames(IEnumerable<ConfigItemWrapper> items,
            List<Tuple<string, IConfigurationItemViewModel>> columnNamesWithProperties)
        {
            foreach (var item in items)
            {
                if(!item.ToInclude)continue;
                
                if (item.ChildConfigItemWrappers.Any())
                {
                    FillFilteredColumnNames(item.ChildConfigItemWrappers, columnNamesWithProperties);
                }
                else
                {
                    columnNamesWithProperties.Add(
                        new Tuple<string, IConfigurationItemViewModel>(item.RelatedConfigurationItemViewModel.Header, item.RelatedConfigurationItemViewModel));
                }
            }
        }



        private void FillColumnNames(IEnumerable<IConfigurationItemViewModel> items,
            List<Tuple<string, IConfigurationItemViewModel>> columnNamesWithProperties)
        {
            foreach (var item in items)
            {
                if (item.ChildStructItemViewModels.Any())
                {
                    FillColumnNames(item.ChildStructItemViewModels, columnNamesWithProperties);
                }
                else
                {
                    columnNamesWithProperties.Add(
                        new Tuple<string, IConfigurationItemViewModel>(item.Header, item));
                }
            }
        }
        private Dictionary<string, ILocalAndDeviceValueContainingViewModel> GetRowFromItemGroupFiltered(
            ConfigItemWrapper group, ILookup<string, IConfigurationItemViewModel> lookup,
            List<string> columnNames, Dictionary<string, ILocalAndDeviceValueContainingViewModel> initialList = null)
        {
            if (initialList == null)
            {
                initialList = new Dictionary<string, ILocalAndDeviceValueContainingViewModel>();
                foreach (var columnName in columnNames)
                {
                    initialList.Add(columnName, null);
                }
            }

            group.ChildConfigItemWrappers.ForEach((item =>
            {
                if(!item.ToInclude)return;
                if (item.ChildConfigItemWrappers.Any())
                {
                    GetRowFromItemGroupFiltered(item, lookup, columnNames, initialList);
                }
                else
                {
                    InsertProperty(initialList, lookup, item.RelatedConfigurationItemViewModel);
                }
            }));
            return initialList;
        }
        private Dictionary<string, ILocalAndDeviceValueContainingViewModel> GetRowFromItemGroup(
            IConfigurationItemViewModel group, ILookup<string, IConfigurationItemViewModel> lookup,
            List<string> columnNames, Dictionary<string, ILocalAndDeviceValueContainingViewModel> initialList = null)
        {
            if (initialList == null)
            {
                initialList = new Dictionary<string, ILocalAndDeviceValueContainingViewModel>();
                foreach (var columnName in columnNames)
                {
                    initialList.Add(columnName, null);
                }
            }

            group.ChildStructItemViewModels.ForEach((item =>
            {
                if (item.ChildStructItemViewModels.Any())
                {
                    GetRowFromItemGroup(item, lookup, columnNames, initialList);
                }
                else
                {
                    InsertProperty(initialList, lookup, item);
                }
            }));
            return initialList;
        }


        private void InsertProperty(Dictionary<string, ILocalAndDeviceValueContainingViewModel> resultList,
            ILookup<string, IConfigurationItemViewModel> lookup,
            IConfigurationItemViewModel itemToAdd)
        {

            var columnName = lookup.FirstOrDefault((models => models.Contains(itemToAdd)))?.Key;
            if (columnName != null)
                resultList[columnName] = itemToAdd as ILocalAndDeviceValueContainingViewModel;
        }

    }
}
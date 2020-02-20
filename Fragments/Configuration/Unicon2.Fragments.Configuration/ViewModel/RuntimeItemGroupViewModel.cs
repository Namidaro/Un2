﻿using System;
using System.Linq;
using System.Windows.Input;
using Unicon2.Fragments.Configuration.Infrastructure.Factories;
using Unicon2.Fragments.Configuration.Infrastructure.Keys;
using Unicon2.Fragments.Configuration.Infrastructure.StructItemsInterfaces;
using Unicon2.Fragments.Configuration.Infrastructure.ViewModel;
using Unicon2.Fragments.Configuration.Infrastructure.ViewModel.Runtime;
using Unicon2.Fragments.Configuration.ViewModel.Table;
using Unicon2.Infrastructure;
using Unicon2.Infrastructure.Extensions;
using Unicon2.Presentation.Infrastructure.Events;
using Unicon2.Presentation.Infrastructure.TreeGrid;
using Unicon2.Unity.Commands;

namespace Unicon2.Fragments.Configuration.ViewModel
{
    public class RuntimeItemGroupViewModel : RuntimeConfigurationItemViewModelBase, IRuntimeItemGroupViewModel, IAsTableViewModel
    {
        private readonly IRuntimeConfigurationItemViewModelFactory _runtimeConfigurationItemViewModelFactory;
        private readonly IGlobalEventsService _globalEventsService;
        private bool _isTableView;
        private TableConfigurationViewModel _tableConfigurationViewModel;
        private bool _isTableViewAllowed;

        public RuntimeItemGroupViewModel(IRuntimeConfigurationItemViewModelFactory runtimeConfigurationItemViewModelFactory)
        {
            this._runtimeConfigurationItemViewModelFactory = runtimeConfigurationItemViewModelFactory;
            this.IsCheckable = true;
        }

        private void OnTryTransformToTable()
        {
            if (!IsTableView) return;
            if (ChildStructItemViewModels.All((model => model is RuntimeItemGroupViewModel)) &&
                TableConfigurationViewModel == null) 
            {
                TableConfigurationViewModel = new TableConfigurationViewModel(ChildStructItemViewModels);
            }
        }

        public TableConfigurationViewModel TableConfigurationViewModel
        {
            get => _tableConfigurationViewModel;
            set
            {
                SetProperty(ref _tableConfigurationViewModel, value);
            }
        }

        public override string TypeName => ConfigurationKeys.DEFAULT_ITEM_GROUP;

        public override string StrongName => ConfigurationKeys.RUNTIME_DEFAULT_ITEM_GROUP +
                                             ApplicationGlobalNames.CommonInjectionStrings.VIEW_MODEL;


        protected override void SetModel(object model)
        {
            this.ChildStructItemViewModels.Clear();
            foreach (IConfigurationItem configurationItem in ((IItemsGroup) model).ConfigurationItemList)
            {
                this.ChildStructItemViewModels.Add(this._runtimeConfigurationItemViewModelFactory
                    .CreateRuntimeConfigurationItemViewModel(configurationItem));
            }
            IsTableViewAllowed = ((IItemsGroup) model).IsTableViewAllowed;
            base.SetModel(model);
        }

        public bool IsTableView
        {
            get => _isTableView;
            set
            {
                SetProperty(ref _isTableView, value);
                OnTryTransformToTable();
            }
        }

        public string AsossiatedDetailsViewName => "ConfigAsTableView";

        public bool IsTableViewAllowed
        {
            get => _isTableViewAllowed;
            set => SetProperty(ref _isTableViewAllowed, value);
        }

        public override T Accept<T>(IConfigurationItemVisitor<T> visitor)
        {
            return visitor.VisitItemsGroup(this);
        }
    }
}
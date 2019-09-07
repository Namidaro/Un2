﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unicon2.Fragments.Configuration.Matrix.Interfaces.Model;
using Unicon2.Fragments.Configuration.Matrix.Keys;
using Unicon2.Fragments.Configuration.Matrix.Model.OptionTemplates;
using Unicon2.Fragments.Configuration.Matrix.ViewModel.Helpers;
using Unicon2.Infrastructure;
using Unicon2.Infrastructure.Values;
using Unicon2.Presentation.Values;
using Unicon2.Presentation.Values.Base;
using Unicon2.Presentation.Values.Editable;
using Unicon2.SharedResources.Behaviors;
using Unicon2.Unity.Commands;

namespace Unicon2.Fragments.Configuration.Matrix.ViewModel
{
    public class EditableMatrixValueViewModel : EditableValueViewModelBase
    {
        private readonly MatrixViewModelTableFactory _matrixViewModelTableFactory;
        private ushort[] _initialUshortsToCompare;
        private DynamicDataTable _table;

        #region Overrides of EditableValueViewModelBase

        public EditableMatrixValueViewModel(MatrixViewModelTableFactory matrixViewModelTableFactory)
        {
            _matrixViewModelTableFactory = matrixViewModelTableFactory;
            MatrixUpdatedCommand = new RelayCommand(OnMatrixEdited);
        }

        private void OnMatrixEdited()
        {
            var newUshorts = (new MatrixViewModelTableParser()).GetUshortsFromTable(Table, Model as IMatrixValue);
            if (!newUshorts.SequenceEqual(_initialUshortsToCompare))
            {
                ValueChangedAction?.Invoke(newUshorts);
                SetIsChangedProperty(nameof(_initialUshortsToCompare), _initialUshortsToCompare != newUshorts);

            }
        }

        public override string StrongName => ApplicationGlobalNames.CommonInjectionStrings.EDITABLE + MatrixKeys.MATRIX_VALUE + ApplicationGlobalNames.CommonInjectionStrings.VIEW_MODEL;
        public override void InitFromValue(IFormattedValue value)
        {
            Model = value;
            FillTable();
        }
        public DynamicDataTable Table
        {
            get => _table;
            set
            {
                _table = value;
                this.RaisePropertyChanged();
            }
        }

        private void FillTable()
        {
            if (!(Model is IMatrixValue matrixValue)) return;
            try
            {
                Table = _matrixViewModelTableFactory.CreateMatrixDataTable(matrixValue, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public ICommand MatrixUpdatedCommand { get; }


        public override void SetBaseValueToCompare(ushort[] ushortsToCompare)
        {
            _initialUshortsToCompare = ushortsToCompare;
        }

        public override object Model { get; set; }

        protected override void OnDisposing()
        {
            if (Table != null) Table.TableUpdateAction = null;
            base.OnDisposing();
        }

        #endregion
    }
}
﻿using Unicon2.Fragments.Measuring.Infrastructure.Keys;
using Unicon2.Fragments.Measuring.Infrastructure.Model.Elements;
using Unicon2.Fragments.Measuring.Infrastructure.ViewModel.Elements;
using Unicon2.Infrastructure;
using Unicon2.Presentation.Infrastructure.Factories;

namespace Unicon2.Fragments.Measuring.ViewModel.Elements
{
    public class AnalogMeasuringElementViewModel : MeasuringElementViewModelBase, IAnalogMeasuringElementViewModel
    {
        private string _measureUnit;
        private bool _isMeasureUnitEnabled;

        public AnalogMeasuringElementViewModel()
        {
        }

        public override string StrongName => MeasuringKeys.ANALOG_MEASURING_ELEMENT +
                                             ApplicationGlobalNames.CommonInjectionStrings.VIEW_MODEL;

        public string MeasureUnit
        {
            get { return this._measureUnit; }
            set
            {
                this._measureUnit = value;
                RaisePropertyChanged();
            }
        }

        public bool IsMeasureUnitEnabled
        {
            get { return this._isMeasureUnitEnabled; }
            set
            {
                this._isMeasureUnitEnabled = value;
                RaisePropertyChanged();
            }
        }

        //protected override void SetModel(object model)
        //{
        //    base.SetModel(model);
        //    this.MeasureUnit = (this._measuringElement as IAnalogMeasuringElement).MeasureUnit;
        //    this.IsMeasureUnitEnabled = (this._measuringElement as IAnalogMeasuringElement).IsMeasureUnitEnabled;
        //    this._measuringElement.ElementChangedAction += () =>
        //    {
        //        this.FormattedValueViewModel =
        //            this._valueViewModelFactory.CreateFormattedValueViewModel(
        //                (this._measuringElement as IAnalogMeasuringElement).UshortsFormatter.Format(
        //                    (this._measuringElement as IAnalogMeasuringElement).DeviceUshortsValue));
        //    };
        //}
    }

}

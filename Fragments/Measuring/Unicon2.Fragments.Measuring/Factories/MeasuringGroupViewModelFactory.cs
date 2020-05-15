﻿using System.Linq;
using System.Runtime.Remoting.Messaging;
using Unicon2.Fragments.Measuring.Infrastructure.Factories;
using Unicon2.Fragments.Measuring.Infrastructure.Model;
using Unicon2.Fragments.Measuring.Infrastructure.Model.Elements;
using Unicon2.Fragments.Measuring.Infrastructure.ViewModel;
using Unicon2.Fragments.Measuring.ViewModel;
using Unicon2.Fragments.Measuring.ViewModel.Presentation;
using Unicon2.Unity.Interfaces;

namespace Unicon2.Fragments.Measuring.Factories
{
	public class MeasuringGroupViewModelFactory : IMeasuringGroupViewModelFactory
	{
		private readonly ITypesContainer _container;
		private readonly IMeasuringElementViewModelFactory _measuringElementViewModelFactory;

		public MeasuringGroupViewModelFactory(ITypesContainer container,
			IMeasuringElementViewModelFactory measuringElementViewModelFactory)
		{
			_container = container;
			_measuringElementViewModelFactory = measuringElementViewModelFactory;
		}


		public IMeasuringGroupViewModel CreateMeasuringGroupViewModel(IMeasuringGroup measuringGroup)
		{
			MeasuringGroupViewModel measuringGroupViewModel =
				_container.Resolve<IMeasuringGroupViewModel>() as MeasuringGroupViewModel;

			measuringGroupViewModel.Header = measuringGroup.Name;
			measuringGroupViewModel.MeasuringElementViewModels.Clear();

			foreach (var groupOfElements in measuringGroup.PresentationSettings.GroupsOfElements)
			{
				measuringGroupViewModel.PresentationMeasuringElements.Add(
					new PresentationMeasuringElementViewModel(groupOfElements.PositioningInfo,
						new PresentationGroupViewModel(groupOfElements.Name)));
			}

			foreach (IMeasuringElement measuringElement in measuringGroup.MeasuringElements)
			{
				var elementViewModel =
					_measuringElementViewModelFactory.CreateMeasuringElementViewModel(measuringElement,
						measuringGroup.Name);
				var positioningInfo =
					measuringGroup.PresentationSettings.Elements.FirstOrDefault(info =>
						info.RelatedMeasuringElementId == measuringElement.Id);
				if (positioningInfo != null)
				{
					measuringGroupViewModel.PresentationMeasuringElements.Add(
						new PresentationMeasuringElementViewModel(positioningInfo.PositioningInfo, elementViewModel));
				}

				measuringGroupViewModel.MeasuringElementViewModels.Add(elementViewModel);
			}

			return measuringGroupViewModel;
		}
	}
}
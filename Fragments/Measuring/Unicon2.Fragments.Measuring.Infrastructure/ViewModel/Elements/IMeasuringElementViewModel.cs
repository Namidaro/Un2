﻿using Unicon2.Infrastructure.Interfaces;
using Unicon2.Presentation.Infrastructure.ViewModels.Values;

namespace Unicon2.Fragments.Measuring.Infrastructure.ViewModel.Elements
{
	public interface IMeasuringElementViewModel : IStronglyNamed, IUniqueIdWithSet
	{
		string Header { get; }
		string GroupName { get; set; }
		IFormattedValueViewModel FormattedValueViewModel { get; set; }
	}
}
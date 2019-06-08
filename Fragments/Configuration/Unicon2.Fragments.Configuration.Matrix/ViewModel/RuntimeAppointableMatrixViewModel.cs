﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unicon2.Fragments.Configuration.Infrastructure.Keys;
using Unicon2.Fragments.Configuration.Infrastructure.ViewModel;
using Unicon2.Fragments.Configuration.Infrastructure.ViewModel.Properties;
using Unicon2.Fragments.Configuration.Matrix.Keys;
using Unicon2.Fragments.Configuration.ViewModel;
using Unicon2.Fragments.Configuration.ViewModel.Properties;
using Unicon2.Infrastructure;
using Unicon2.Presentation.Infrastructure.Factories;
using Unicon2.Presentation.Infrastructure.ViewModels;
using Unicon2.Presentation.Infrastructure.ViewModels.Values;
using Unicon2.Unity.Commands;
using Unicon2.Unity.Interfaces;

namespace Unicon2.Fragments.Configuration.Matrix.ViewModel
{
	public class RuntimeAppointableMatrixViewModel: RuntimePropertyViewModel
	{
		public RuntimeAppointableMatrixViewModel(ITypesContainer container, IValueViewModelFactory valueViewModelFactory) : base(container, valueViewModelFactory)
		{
			ShowDetails=new RelayCommand(OnShowDetails);
		}

		private void OnShowDetails()
		{
			
		}



		#region Overrides of ConfigurationItemViewModelBase

		public override string TypeName => ConfigurationKeys.APPOINTABLE_MATRIX;
		public override string StrongName=>ConfigurationKeys.RUNTIME+ConfigurationKeys.APPOINTABLE_MATRIX + ApplicationGlobalNames.CommonInjectionStrings.VIEW_MODEL;


		public RelayCommand ShowDetails { get; }

		#endregion



	}
}
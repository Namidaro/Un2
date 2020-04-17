﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unicon2.Fragments.Configuration.Infrastructure.MemoryViewModelMapping;
using Unicon2.Fragments.Configuration.Infrastructure.StructItemsInterfaces.Properties;
using Unicon2.Fragments.Configuration.Infrastructure.ViewModel.Runtime;
using Unicon2.Fragments.Configuration.ViewModelMemoryMapping;
using Unicon2.Infrastructure.Common;
using Unicon2.Infrastructure.DeviceInterfaces;
using Unicon2.Infrastructure.Extensions;
using Unicon2.Infrastructure.Services.Formatting;
using Unicon2.Presentation.Infrastructure.ViewModels;
using Unicon2.Presentation.Infrastructure.ViewModels.Values;
using Unicon2.Presentation.Infrastructure.Visitors;

namespace Unicon2.Fragments.Configuration.MemoryAccess.Subscriptions.ComplexProperty
{
	class LocalDataComplexPropertyEditedSubscription : ILocalDataMemorySubscription
	{
		private readonly IRuntimeComplexPropertyViewModel _complexPropertyViewModel;
		private readonly IDeviceMemory _deviceMemory;
		private readonly ISubProperty _subProperty;
		private readonly IComplexProperty _complexProperty;
		private readonly EditableValueSetUnchangedSubscription _dependancy;
		private readonly int _offset;

		public LocalDataComplexPropertyEditedSubscription(IRuntimeComplexPropertyViewModel complexPropertyViewModel,
			IDeviceMemory deviceMemory, ISubProperty subProperty, IComplexProperty complexProperty, EditableValueSetUnchangedSubscription dependancy,int offset)
		{
			_complexPropertyViewModel = complexPropertyViewModel;
			_deviceMemory = deviceMemory;
			_subProperty = subProperty;
			_complexProperty = complexProperty;
			_dependancy = dependancy;
			_offset = offset;
		}

		public void Execute()
		{
			IFormattingService formattingService = StaticContainer.Container.Resolve<IFormattingService>();
			IEditableValueFetchingFromViewModelVisitor fetchingFromViewModelVisitor = StaticContainer.Container.Resolve<IEditableValueFetchingFromViewModelVisitor>();


			var resultBitArray=new bool[16];


			foreach (var subProperty in _complexProperty.SubProperties)
			{
				var ushorts = formattingService.FormatBack(subProperty?.UshortsFormatter, (_complexPropertyViewModel.ChildStructItemViewModels[_complexProperty.SubProperties.IndexOf(subProperty)] as ILocalAndDeviceValueContainingViewModel).LocalValue.Accept(fetchingFromViewModelVisitor));
				var ushortOfSubProperty = ushorts.First();
				var boolArray = ushorts.GetBoolArrayFromUshortArray();
				int counterInner = 0;
				foreach (var bitNumber in subProperty.BitNumbersInWord)
				{
					resultBitArray[bitNumber] = boolArray[counterInner];
					counterInner++;
				}
			}

		


			MemoryAccessor.GetUshortsInMemory(_deviceMemory, (ushort)(_complexProperty.Address+_offset), new ushort[]{ resultBitArray.BoolArrayToUshort() }, true);
			_dependancy?.Execute();
		}

	}
}
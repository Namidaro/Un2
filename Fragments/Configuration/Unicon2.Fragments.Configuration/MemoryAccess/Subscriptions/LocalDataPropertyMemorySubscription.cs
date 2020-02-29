using Unicon2.Fragments.Configuration.Infrastructure.MemoryViewModelMapping;
using Unicon2.Fragments.Configuration.Infrastructure.StructItemsInterfaces;
using Unicon2.Fragments.Configuration.Infrastructure.StructItemsInterfaces.Properties;
using Unicon2.Fragments.Configuration.ViewModelMemoryMapping;
using Unicon2.Infrastructure.Common;
using Unicon2.Infrastructure.Interfaces.Visitors;
using Unicon2.Infrastructure.Services.Formatting;
using Unicon2.Infrastructure.Values;
using Unicon2.Presentation.Infrastructure.ViewModels.Values;
using Unicon2.Presentation.Infrastructure.Visitors;

namespace Unicon2.Fragments.Configuration.MemoryAccess.Subscriptions
{
    public class LocalDataPropertyMemorySubscription : ILocalDataMemorySubscription
    {
        private readonly IConfigurationMemory _configurationMemory;
        private readonly IProperty _property;

        public LocalDataPropertyMemorySubscription(IEditableValueViewModel editableValueViewModel,
            IConfigurationMemory configurationMemory, IProperty property)
        {
            _configurationMemory = configurationMemory;
            _property = property;
            EditableValueViewModel = editableValueViewModel;
        }

        public void Execute()
        {
            IFormattingService formattingService = StaticContainer.Container.Resolve<IFormattingService>();
            IEditableValueFetchingFromViewModelVisitor fetchingFromViewModelVisitor = StaticContainer.Container.Resolve<IEditableValueFetchingFromViewModelVisitor>();

            var ushorts = formattingService.FormatBack(_property?.UshortsFormatter, EditableValueViewModel.Accept(fetchingFromViewModelVisitor));
            
            MemoryAccessor.GetUshortsInMemory(_configurationMemory, _property.Address, ushorts, true);
        }

        public IEditableValueViewModel EditableValueViewModel { get; }
    }
}
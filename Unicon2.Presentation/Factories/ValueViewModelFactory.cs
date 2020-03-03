using Unicon2.Infrastructure.Interfaces;
using Unicon2.Infrastructure.Values;
using Unicon2.Presentation.Infrastructure.Factories;
using Unicon2.Presentation.Infrastructure.ViewModels.Values;

namespace Unicon2.Presentation.Factories
{
    public class ValueViewModelFactory : IValueViewModelFactory
    {
        public IFormattedValueViewModel CreateFormattedValueViewModel(IFormattedValue formattedValue)
        {
            return formattedValue.Accept(new FormattedValueViewModelFactory(formattedValue as IRangeable,
                formattedValue as IMeasurable));
        }

        public IEditableValueViewModel CreateEditableValueViewModel(IFormattedValue formattedValue)
        {
            return formattedValue.Accept(new EditableValueViewModelFactory(formattedValue as IRangeable,
                formattedValue as IMeasurable));
        }
    }
}
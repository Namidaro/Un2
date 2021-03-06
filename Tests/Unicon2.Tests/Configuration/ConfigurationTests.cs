using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Unicon2.Connections.MockConnection.Model;
using Unicon2.Formatting.Infrastructure.Model;
using Unicon2.Fragments.Configuration.Infrastructure.StructItemsInterfaces;
using Unicon2.Fragments.Configuration.Infrastructure.StructItemsInterfaces.Dependencies;
using Unicon2.Fragments.Configuration.Infrastructure.StructItemsInterfaces.Properties;
using Unicon2.Fragments.Configuration.Infrastructure.ViewModel.Runtime;
using Unicon2.Fragments.Configuration.MemoryAccess;
using Unicon2.Fragments.Configuration.Model.Dependencies.Conditions;
using Unicon2.Fragments.Configuration.ViewModel;
using Unicon2.Fragments.Configuration.ViewModel.Helpers;
using Unicon2.Infrastructure;
using Unicon2.Infrastructure.Dependencies;
using Unicon2.Infrastructure.DeviceInterfaces;
using Unicon2.Infrastructure.Extensions;
using Unicon2.Infrastructure.Functional;
using Unicon2.Infrastructure.Interfaces.Dependancy;
using Unicon2.Infrastructure.Services;
using Unicon2.Model.Memory;
using Unicon2.Presentation.Infrastructure.Factories;
using Unicon2.Presentation.Infrastructure.TreeGrid;
using Unicon2.Presentation.Infrastructure.ViewModels.Values;
using Unicon2.Presentation.Values;
using Unicon2.Presentation.Values.Editable;
using Unicon2.Presentation.ViewModels.Fragment;
using Unicon2.Shell.ViewModels;
using Unicon2.Tests.Utils;
using Unicon2.Unity.Commands;
using Unicon2.Unity.Common;
using Unicon2.Unity.Interfaces;
using Unity;

namespace Unicon2.Tests.Configuration
{
    [TestFixture]
    public class ConfigurationTests
    {
        private ITypesContainer _typesContainer;
        private IDevice _device;
        private IDeviceConfiguration _configuration;
        private RuntimeConfigurationViewModel _configurationFragmentViewModel;
        private RelayCommand _readCommand;
        private ShellViewModel _shell;

        [SetUp]
        public async Task OnSetup()
        {
            Program.CleanProject();
            _typesContainer =
                new TypesContainer(Program.GetApp().Container.Resolve(typeof(IUnityContainer)) as IUnityContainer);

            var appInfo = Program.RefreshProject();

            _device = appInfo.device;
            _configuration = appInfo.configuration;



            _shell = appInfo.shellViewModel;

            _device.DeviceMemory = appInfo.device.DeviceMemory;
            _configurationFragmentViewModel = appInfo.configurationViewModel;
          

            _readCommand = _configurationFragmentViewModel.FragmentOptionsViewModel.FragmentOptionGroupViewModels
                .First(model => model.NameKey == "Device").FragmentOptionCommandViewModels
                .First(model => model.TitleKey == ApplicationGlobalNames.UiCommandStrings.READ_STRING_KEY)
                .OptionCommand as RelayCommand;
            await _typesContainer.Resolve<IDevicesContainerService>()
                .ConnectDeviceAsync(_device, new MockConnection());
            _shell.ActiveFragmentViewModel = new FragmentPaneViewModel()
            {
                FragmentViewModel = _configurationFragmentViewModel
            };
            await _configurationFragmentViewModel.SetFragmentOpened(true);
   
        }



        [Test]
        public async Task BoolDefaultPropertyTest()
        {

            var boolTestDefaultProperty =
                _configuration.RootConfigurationItemList.FindItemByName(item => item.Name == "boolTestDefaultProperty")
                    .Item as IProperty;

            var defaultPropertyWithBoolFormatting = _configurationFragmentViewModel.RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "boolTestDefaultProperty")
                .Item as IRuntimePropertyViewModel;

            await Read();

            Func<IBoolValueViewModel> deviceValue = () =>
                defaultPropertyWithBoolFormatting.DeviceValue as IBoolValueViewModel;
            Func<EditableBoolValueViewModel> localValue = () =>
                defaultPropertyWithBoolFormatting.LocalValue as EditableBoolValueViewModel;

            Assert.False(deviceValue().BoolValueProperty);
            Assert.False(localValue().BoolValueProperty);

            Assert.True(localValue().IsEditEnabled);
            localValue().BoolValueProperty = true;
            Assert.True(localValue().IsFormattedValueChanged);

            await Write();


            Assert.True(_configurationFragmentViewModel.DeviceContext.DeviceMemory
                .DeviceMemoryValues[boolTestDefaultProperty.Address].Equals(1));

            Assert.False(localValue().IsFormattedValueChanged);

            Assert.True(deviceValue().BoolValueProperty);
            Assert.True(localValue().BoolValueProperty);

            Assert.True(localValue().IsEditEnabled);
            localValue().BoolValueProperty = false;
            Assert.True(localValue().IsFormattedValueChanged);

            await Write();


            Assert.True(_configurationFragmentViewModel.DeviceContext.DeviceMemory
                .DeviceMemoryValues[boolTestDefaultProperty.Address].Equals(0));

            Assert.False(localValue().IsFormattedValueChanged);



        }


        [Test]
        public async Task DefaultPropertyAllFormattersTest()
        {

            var defaultPropertyStringFormatter1251 =
                _configuration.RootConfigurationItemList.FindItemByName(item => item.Name == "defaultPropertyStringFormatter1251")
                    .Item as IProperty;
            var defaultPropertyStringFormatter =
                _configuration.RootConfigurationItemList.FindItemByName(item => item.Name == "defaultPropertyAsciiStringFormatter")
                    .Item as IProperty;
            var defaultPropertyNumericFormatter =
                _configuration.RootConfigurationItemList.FindItemByName(item => item.Name == "defaultPropertyNumericFormatter")
                    .Item as IProperty;
            var defaultPropertySelectFormatter =
                _configuration.RootConfigurationItemList.FindItemByName(item => item.Name == "defaultPropertySelectFormatter")
                    .Item as IProperty;
            var defaultPropertyFormulaFormatter =
                _configuration.RootConfigurationItemList.FindItemByName(item => item.Name == "defaultPropertyFormulaFormatter")
                    .Item as IProperty;
            
            
            var defaultPropertyStringFormatter1251ViewModel = _configurationFragmentViewModel.RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "defaultPropertyStringFormatter1251")
                .Item as IRuntimePropertyViewModel;
            var defaultPropertyStringFormatterViewModel = _configurationFragmentViewModel.RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "defaultPropertyAsciiStringFormatter")
                .Item as IRuntimePropertyViewModel;
            var defaultPropertyNumericFormatterViewModel = _configurationFragmentViewModel.RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "defaultPropertyNumericFormatter")
                .Item as IRuntimePropertyViewModel;
            var defaultPropertySelectFormatterViewModel = _configurationFragmentViewModel.RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "defaultPropertySelectFormatter")
                .Item as IRuntimePropertyViewModel;
            var defaultPropertyFormulaFormatterViewModel = _configurationFragmentViewModel.RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "defaultPropertyFormulaFormatter")
                .Item as IRuntimePropertyViewModel;
            
            await Read();
            
            var defaultPropertyStringFormatter1251ViewModelDeviceValue = defaultPropertyStringFormatter1251ViewModel.DeviceValue as IStringValueViewModel;
            var defaultPropertyStringFormatter1251ViewModelLocalValue = defaultPropertyStringFormatter1251ViewModel.LocalValue as StringValueViewModel;
            
            var defaultPropertyStringFormatterViewModelDeviceValue = defaultPropertyStringFormatterViewModel.DeviceValue as IStringValueViewModel;
            var defaultPropertyStringFormatterViewModelLocalValue = defaultPropertyStringFormatterViewModel.LocalValue as StringValueViewModel;

            var defaultPropertyNumericFormatterViewModelDeviceValue = defaultPropertyNumericFormatterViewModel.DeviceValue as INumericValueViewModel;
            var defaultPropertyNumericFormatterViewModelLocalValue = defaultPropertyNumericFormatterViewModel.LocalValue as EditableNumericValueViewModel;
            
            var defaultPropertySelectFormatterViewModelDeviceValue = defaultPropertySelectFormatterViewModel.DeviceValue as IChosenFromListValueViewModel;
            var defaultPropertySelectFormatterViewModelLocalValue = defaultPropertySelectFormatterViewModel.LocalValue as EditableChosenFromListValueViewModel;
            
            var defaultPropertyFormulaFormatterViewModelDeviceValue = defaultPropertyFormulaFormatterViewModel.DeviceValue as INumericValueViewModel;
            var defaultPropertyFormulaFormatterViewModelLocalValue = defaultPropertyFormulaFormatterViewModel.LocalValue as EditableNumericValueViewModel;

            Assert.True(defaultPropertyStringFormatter1251ViewModelDeviceValue.StringValue.Replace("\0", "") == "");
            Assert.True(defaultPropertyStringFormatter1251ViewModelLocalValue.StringValue.Replace("\0", "") == "");

            Assert.True(defaultPropertyStringFormatterViewModelDeviceValue.StringValue.Replace("\0", "") == "");
            Assert.True(defaultPropertyStringFormatterViewModelLocalValue.StringValue.Replace("\0", "") == "");
            
            Assert.True(defaultPropertyNumericFormatterViewModelDeviceValue.NumValue=="0");
            Assert.True(defaultPropertyNumericFormatterViewModelLocalValue.NumValue=="0");
            
            Assert.True(defaultPropertySelectFormatterViewModelDeviceValue.SelectedItem=="1");
            Assert.True(defaultPropertySelectFormatterViewModelLocalValue.SelectedItem=="1");


            Assert.True((defaultPropertyFormulaFormatter.UshortsFormatter as IFormulaFormatter).FormulaString == "3*x+1");

            Assert.True(defaultPropertyFormulaFormatterViewModelDeviceValue.NumValue=="1");
            Assert.True(defaultPropertyFormulaFormatterViewModelLocalValue.NumValue=="1");
            

            Assert.False(defaultPropertyStringFormatter1251ViewModelLocalValue.IsEditEnabled);
            Assert.False(defaultPropertyStringFormatterViewModelLocalValue.IsEditEnabled);
            Assert.True(defaultPropertyNumericFormatterViewModelLocalValue.IsEditEnabled);
            Assert.True(defaultPropertySelectFormatterViewModelLocalValue.IsEditEnabled);
            Assert.True(defaultPropertyFormulaFormatterViewModelLocalValue.IsEditEnabled);

            defaultPropertyNumericFormatterViewModelLocalValue.NumValue = "225";
            defaultPropertySelectFormatterViewModelLocalValue.SelectedItem =
                defaultPropertySelectFormatterViewModelLocalValue.AvailableItemsList[1];
            defaultPropertyFormulaFormatterViewModelLocalValue.NumValue = "20";
            
            Assert.False(defaultPropertyStringFormatter1251ViewModelLocalValue.IsFormattedValueChanged);
            Assert.False(defaultPropertyStringFormatterViewModelLocalValue.IsFormattedValueChanged);
            Assert.True(defaultPropertyNumericFormatterViewModelLocalValue.IsFormattedValueChanged);
            Assert.True(defaultPropertySelectFormatterViewModelLocalValue.IsFormattedValueChanged);
            Assert.True(defaultPropertyFormulaFormatterViewModelLocalValue.IsFormattedValueChanged);

            await Write();
            
            Assert.False(defaultPropertyStringFormatter1251ViewModelLocalValue.IsFormattedValueChanged);
            Assert.False(defaultPropertyStringFormatterViewModelLocalValue.IsFormattedValueChanged);
            Assert.False(defaultPropertyNumericFormatterViewModelLocalValue.IsFormattedValueChanged);
            Assert.False(defaultPropertySelectFormatterViewModelLocalValue.IsFormattedValueChanged);
            Assert.False(defaultPropertyFormulaFormatterViewModelLocalValue.IsFormattedValueChanged);

             

            Assert.True(_configurationFragmentViewModel.DeviceContext.DeviceMemory
                .DeviceMemoryValues[defaultPropertyStringFormatter1251.Address].Equals(0));
            Assert.True(_configurationFragmentViewModel.DeviceContext.DeviceMemory
                .DeviceMemoryValues[defaultPropertyStringFormatter.Address].Equals(0));
            Assert.True(_configurationFragmentViewModel.DeviceContext.DeviceMemory
                .DeviceMemoryValues[defaultPropertyNumericFormatter.Address].Equals(225));
            Assert.True(_configurationFragmentViewModel.DeviceContext.DeviceMemory
                .DeviceMemoryValues[defaultPropertySelectFormatter.Address].Equals(1));
            Assert.True(_configurationFragmentViewModel.DeviceContext.DeviceMemory
                .DeviceMemoryValues[defaultPropertyFormulaFormatter.Address].Equals(6));

        }

        [Test]
        public async Task SubPropertyInComplexPropertiesWithSameAddressTest()
        {

            var boolTestSubProperty1 =
                _configuration.RootConfigurationItemList
                    .FindItemByName(item => item.Name == "subPropInComplexPropWithSameAddr1")
                    .Item as ISubProperty;

            var boolTestSubProperty2 =
                _configuration.RootConfigurationItemList
                    .FindItemByName(item => item.Name == "subPropInComplexPropWithSameAddr2")
                    .Item as ISubProperty;


            var defaultPropertyWithBoolFormatting1 = _configurationFragmentViewModel.RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "subPropInComplexPropWithSameAddr1")
                .Item as IRuntimePropertyViewModel;

            var defaultPropertyWithBoolFormatting2 = _configurationFragmentViewModel.RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "subPropInComplexPropWithSameAddr2")
                .Item as IRuntimePropertyViewModel;

            await Read();

            var deviceValue1 = defaultPropertyWithBoolFormatting1.DeviceValue as IBoolValueViewModel;
            var localValue1 = defaultPropertyWithBoolFormatting1.LocalValue as EditableBoolValueViewModel;

            var deviceValue2 = defaultPropertyWithBoolFormatting2.DeviceValue as IBoolValueViewModel;
            var localValue2 = defaultPropertyWithBoolFormatting2.LocalValue as EditableBoolValueViewModel;


            Assert.False(deviceValue1.BoolValueProperty);
            Assert.False(localValue1.BoolValueProperty);

            Assert.True(localValue1.IsEditEnabled);
            localValue1.BoolValueProperty = true;
            localValue2.BoolValueProperty = true;

            Assert.True(localValue1.IsFormattedValueChanged);
            Assert.True(localValue2.IsFormattedValueChanged);

        }


        [Test]
        public async Task BoolSubPropertyTest()
        {

            var boolTestSubProperty =
                _configuration.RootConfigurationItemList.FindItemByName(item => item.Name == "boolTestSubProperty")
                    .Item as IProperty;

            var defaultPropertyWithBoolFormatting = _configurationFragmentViewModel.RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "boolTestSubProperty")
                .Item as IRuntimePropertyViewModel;

            await Read();

            var deviceValue = defaultPropertyWithBoolFormatting.DeviceValue as IBoolValueViewModel;
            var localValue = defaultPropertyWithBoolFormatting.LocalValue as EditableBoolValueViewModel;

            Assert.False(deviceValue.BoolValueProperty);
            Assert.False(localValue.BoolValueProperty);

            Assert.True(localValue.IsEditEnabled);
            localValue.BoolValueProperty = true;
            Assert.True(localValue.IsFormattedValueChanged);
            
            await Write();

            var boolsInDevice = _configurationFragmentViewModel.DeviceContext.DeviceMemory
                .DeviceMemoryValues[boolTestSubProperty.Address].GetBoolArrayFromUshort();
            Assert.True(boolsInDevice[boolTestSubProperty.BitNumbers.First()]);

            Assert.False(localValue.IsFormattedValueChanged);

            
            
            localValue.BoolValueProperty = false;
            Assert.True(localValue.IsFormattedValueChanged);
            
            await Write();

            boolsInDevice = _configurationFragmentViewModel.DeviceContext.DeviceMemory
                .DeviceMemoryValues[boolTestSubProperty.Address].GetBoolArrayFromUshort();
            Assert.False(boolsInDevice[boolTestSubProperty.BitNumbers.First()]);
            Assert.False(localValue.IsFormattedValueChanged);
            
        }


        [Test]
        public async Task MultipleBoolSubPropertyTest()
        {

            var boolTestSubProperty =
                _configuration.RootConfigurationItemList.FindItemByName(item => item.Name == "boolTestSubProperty")
                    .Item as IProperty;

            var boolTestSubProperty1 =
                _configuration.RootConfigurationItemList.FindItemByName(item => item.Name == "boolTestSubProperty1")
                    .Item as IProperty;

            var boolTestSubProperty2 =
                _configuration.RootConfigurationItemList.FindItemByName(item => item.Name == "boolTestSubProperty2")
                    .Item as IProperty;

            var boolTestSubProperty3 =
                _configuration.RootConfigurationItemList.FindItemByName(item => item.Name == "boolTestSubProperty3")
                    .Item as IProperty;

            var boolTestSubProperty4 =
                _configuration.RootConfigurationItemList.FindItemByName(item => item.Name == "boolTestSubProperty4")
                    .Item as IProperty;

            var boolTestSubProperty5 =
                _configuration.RootConfigurationItemList.FindItemByName(item => item.Name == "boolTestSubProperty5")
                    .Item as IProperty;

            var boolTestSubProperty6 =
                _configuration.RootConfigurationItemList.FindItemByName(item => item.Name == "boolTestSubProperty6")
                    .Item as IProperty;


            var properties = new List<(IProperty, bool, IRuntimePropertyViewModel)>()
            {
                (boolTestSubProperty, false, _configurationFragmentViewModel.RootConfigurationItemViewModels
                    .Cast<IConfigurationItemViewModel>().ToList()
                    .FindItemViewModelByName(model => model.Header == boolTestSubProperty.Name)
                    .Item as IRuntimePropertyViewModel),
                (boolTestSubProperty1, true, _configurationFragmentViewModel.RootConfigurationItemViewModels
                    .Cast<IConfigurationItemViewModel>().ToList()
                    .FindItemViewModelByName(model => model.Header == boolTestSubProperty1.Name)
                    .Item as IRuntimePropertyViewModel),
                (boolTestSubProperty2, false, _configurationFragmentViewModel.RootConfigurationItemViewModels
                    .Cast<IConfigurationItemViewModel>().ToList()
                    .FindItemViewModelByName(model => model.Header == boolTestSubProperty2.Name)
                    .Item as IRuntimePropertyViewModel),
                (boolTestSubProperty3, true, _configurationFragmentViewModel.RootConfigurationItemViewModels
                    .Cast<IConfigurationItemViewModel>().ToList()
                    .FindItemViewModelByName(model => model.Header == boolTestSubProperty3.Name)
                    .Item as IRuntimePropertyViewModel),
                (boolTestSubProperty4, true, _configurationFragmentViewModel.RootConfigurationItemViewModels
                    .Cast<IConfigurationItemViewModel>().ToList()
                    .FindItemViewModelByName(model => model.Header == boolTestSubProperty4.Name)
                    .Item as IRuntimePropertyViewModel),
                (boolTestSubProperty5, false, _configurationFragmentViewModel.RootConfigurationItemViewModels
                    .Cast<IConfigurationItemViewModel>().ToList()
                    .FindItemViewModelByName(model => model.Header == boolTestSubProperty5.Name)
                    .Item as IRuntimePropertyViewModel),
                (boolTestSubProperty6, true, _configurationFragmentViewModel.RootConfigurationItemViewModels
                    .Cast<IConfigurationItemViewModel>().ToList()
                    .FindItemViewModelByName(model => model.Header == boolTestSubProperty6.Name)
                    .Item as IRuntimePropertyViewModel),
            };

            await Read();

            foreach (var propertyWithValueTuple in properties)
            {
                var deviceValue = propertyWithValueTuple.Item3.DeviceValue as IBoolValueViewModel;
                var localValue = propertyWithValueTuple.Item3.LocalValue as EditableBoolValueViewModel;

                Assert.False(deviceValue.BoolValueProperty);
                Assert.False(localValue.BoolValueProperty);

                Assert.True(localValue.IsEditEnabled);
                localValue.BoolValueProperty = propertyWithValueTuple.Item2;
                Assert.AreEqual(localValue.IsFormattedValueChanged, propertyWithValueTuple.Item2);

            }


            await Write();

            foreach (var propertyWithValueTuple in properties)
            {
                var localValue = propertyWithValueTuple.Item3.LocalValue as EditableBoolValueViewModel;

                var boolsInDevice = _configurationFragmentViewModel.DeviceContext.DeviceMemory
                    .DeviceMemoryValues[propertyWithValueTuple.Item1.Address].GetBoolArrayFromUshort();
                Assert.AreEqual(boolsInDevice[propertyWithValueTuple.Item1.BitNumbers.First()],
                    propertyWithValueTuple.Item2);

                Assert.False(localValue.IsFormattedValueChanged);
            }


        }

        [Test]
        public async Task SubPropertyTransferFromDevice()
        {
            var boolTestSubProperty =
                _configuration.RootConfigurationItemList.FindItemByName(item => item.Name == "boolTestSubProperty")
                    .Item as IProperty;

            var boolTestSubPropertyViewModel = _configurationFragmentViewModel
                .RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "boolTestSubProperty")
                .Item as IRuntimePropertyViewModel;

            Func<IBoolValueViewModel> deviceValue = () =>
                boolTestSubPropertyViewModel.DeviceValue as IBoolValueViewModel;
            Func<EditableBoolValueViewModel> localValue = () =>
                boolTestSubPropertyViewModel.LocalValue as EditableBoolValueViewModel;


            await Read();

            Assert.True(localValue().IsEditEnabled);
            localValue().BoolValueProperty = true;

           await Write();

           Assert.True(deviceValue().BoolValueProperty);
           Assert.True(localValue().BoolValueProperty);
           localValue().BoolValueProperty = false;
           Assert.True(localValue().IsFormattedValueChanged);

            await TransferFromDeviceToLocal();
            Assert.True(localValue().BoolValueProperty);
            Assert.False(localValue().IsFormattedValueChanged);

        }


        [Test]
        public async Task DependencyDefaultToDefaultPropertyCheckTest()
        {
            var boolTestPropertyDependencySource =
                _configuration.RootConfigurationItemList
                    .FindItemByName(item => item.Name == "boolTestDefaultPropertyOutputDependency")
                    .Item as IProperty;

            var boolTestPropertyDependencyConsumer =
                _configuration.RootConfigurationItemList
                    .FindItemByName(item => item.Name == "boolTestDefaultPropertyDependencyConsumer")
                    .Item as IProperty;

            var boolTestPropertyDependencySourceViewModel = _configurationFragmentViewModel
                .RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "boolTestDefaultPropertyOutputDependency")
                .Item as IRuntimePropertyViewModel;

            var boolTestPropertyDependencyConsumerViewModel = _configurationFragmentViewModel
                .RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "boolTestDefaultPropertyDependencyConsumer")
                .Item as IRuntimePropertyViewModel;


            await Read();

            for (int i = 0; i < 2; i++)
            {
                Assert.True(boolTestPropertyDependencyConsumer.Dependencies.Any(delegate(IDependency dependency)
                {
                    if (dependency is IConditionResultDependency conditionResultDependency)
                    {
                        if (conditionResultDependency.Condition is CompareResourceCondition compareResourceCondition)
                        {
                            return compareResourceCondition.ConditionsEnum == ConditionsEnum.Equal &&
                                   compareResourceCondition.UshortValueToCompare == 0;
                        }
                    }

                    return false;
                }));

                Func<EditableNumericValueViewModel> localValueOfDependencySource = () =>
                    (boolTestPropertyDependencySourceViewModel.LocalValue as EditableNumericValueViewModel);
                Func<EditableBoolValueViewModel> localValueOfDependencyConsumer = () =>
                    (boolTestPropertyDependencyConsumerViewModel.LocalValue as EditableBoolValueViewModel);

                Assert.True(localValueOfDependencySource().NumValue == "0");
                Assert.False(localValueOfDependencyConsumer().IsEditEnabled);

                localValueOfDependencySource().NumValue = "15";

                Assert.True(localValueOfDependencySource().IsFormattedValueChanged);
                Assert.True(localValueOfDependencyConsumer().IsEditEnabled);
                localValueOfDependencyConsumer().BoolValueProperty = true;
                Assert.True(localValueOfDependencyConsumer().BoolValueProperty);
                Assert.True(localValueOfDependencyConsumer().IsFormattedValueChanged);


                await Write();


                Assert.AreEqual(_configurationFragmentViewModel.DeviceContext.DeviceMemory.DeviceMemoryValues[boolTestPropertyDependencySource.Address],
                    15);
                Assert.AreEqual(_configurationFragmentViewModel.DeviceContext.DeviceMemory.DeviceMemoryValues[boolTestPropertyDependencyConsumer.Address],
                    1);
                Assert.False(localValueOfDependencySource().IsFormattedValueChanged);
                Assert.False(localValueOfDependencyConsumer().IsFormattedValueChanged);




                localValueOfDependencySource().NumValue = "0";

                Assert.True(localValueOfDependencySource().IsFormattedValueChanged);
                Assert.False(localValueOfDependencyConsumer().IsEditEnabled);

                await Write();
                Assert.AreEqual(_configurationFragmentViewModel.DeviceContext.DeviceMemory.DeviceMemoryValues[boolTestPropertyDependencySource.Address],
                    0);
                Assert.AreEqual(_configurationFragmentViewModel.DeviceContext.DeviceMemory.DeviceMemoryValues[boolTestPropertyDependencyConsumer.Address],
                    1);
                Assert.False(localValueOfDependencySource().IsFormattedValueChanged);
                Assert.False(localValueOfDependencyConsumer().IsFormattedValueChanged);




                localValueOfDependencySource().NumValue = "15";

                Assert.True(localValueOfDependencySource().IsFormattedValueChanged);
                Assert.True(localValueOfDependencyConsumer().IsEditEnabled);
                localValueOfDependencyConsumer().BoolValueProperty = false;
                Assert.False(localValueOfDependencyConsumer().BoolValueProperty);
                Assert.True(localValueOfDependencyConsumer().IsFormattedValueChanged);


                await Write();


                Assert.AreEqual(_configurationFragmentViewModel.DeviceContext.DeviceMemory.DeviceMemoryValues[boolTestPropertyDependencySource.Address],
                    15);
                Assert.AreEqual(_configurationFragmentViewModel.DeviceContext.DeviceMemory.DeviceMemoryValues[boolTestPropertyDependencyConsumer.Address],
                    0);
                Assert.False(localValueOfDependencySource().IsFormattedValueChanged);
                Assert.False(localValueOfDependencyConsumer().IsFormattedValueChanged);
                
                
                localValueOfDependencySource().NumValue = "0";
                await Write();

            }

        }


        [Test]
        public async Task DependencyInRepetitionGroups()
        {

            var depSourceInsideRepetitionGroup =
                _configuration.RootConfigurationItemList
                    .FindItemByName(item => item.Name == "depSourceInsideRepetitionGroup")
                    .Item as IProperty;

            var depConsumerInsideRepetitionGroup =
                _configuration.RootConfigurationItemList
                    .FindItemByName(item => item.Name == "depConsumerInsideRepetitionGroup")
                    .Item as IProperty;

            var depSourceInsideRepetitionGroupViewModels = _configurationFragmentViewModel
                .RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindAllItemViewModelsByName(model => model.Header == "depSourceInsideRepetitionGroup")
                .Cast<IRuntimePropertyViewModel> ()
                .ToList();

            var depConsumerInsideRepetitionGroupViewModels = _configurationFragmentViewModel
                .RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindAllItemViewModelsByName(model => model.Header == "depConsumerInsideRepetitionGroup")
                .Cast<IRuntimePropertyViewModel>()
                .ToList();
            await Read();


            Func<EditableBoolValueViewModel> depSourceInsideRepetitionGroupLocalValue1 = () =>
                depSourceInsideRepetitionGroupViewModels[0].LocalValue as EditableBoolValueViewModel;
            Func<EditableBoolValueViewModel> depSourceInsideRepetitionGroupLocalValue2 = () =>
                depSourceInsideRepetitionGroupViewModels[1].LocalValue as EditableBoolValueViewModel;

            Func<EditableBoolValueViewModel> depConsumerInsideRepetitionGroupLocalValue1 = () =>
                depConsumerInsideRepetitionGroupViewModels[0].LocalValue as EditableBoolValueViewModel;
            Func<EditableBoolValueViewModel> depConsumerInsideRepetitionGroupLocalValue2 = () =>
                depConsumerInsideRepetitionGroupViewModels[1].LocalValue as EditableBoolValueViewModel;


            Assert.False(depSourceInsideRepetitionGroupLocalValue1().BoolValueProperty);
            Assert.False(depSourceInsideRepetitionGroupLocalValue2().BoolValueProperty);
            Assert.False(depConsumerInsideRepetitionGroupLocalValue1().BoolValueProperty);
            Assert.False(depConsumerInsideRepetitionGroupLocalValue2().BoolValueProperty);

            Assert.False(depConsumerInsideRepetitionGroupLocalValue1().IsEditEnabled);
            Assert.False(depConsumerInsideRepetitionGroupLocalValue2().IsEditEnabled);


            depSourceInsideRepetitionGroupLocalValue1().BoolValueProperty=true;

            Assert.True(depConsumerInsideRepetitionGroupLocalValue1().IsEditEnabled);
            Assert.False(depConsumerInsideRepetitionGroupLocalValue2().IsEditEnabled);


        }


        [Test]
        public async Task ComplexSameAddressInRepetitionGroups()
        {
    

            var subSameAddrPropInRepetitionGroup1ViewModels = _configurationFragmentViewModel
                .RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindAllItemViewModelsByName(model => model.Header == "subSameAddrPropInRepetitionGroup1")
                .Cast<IRuntimePropertyViewModel>()
                .ToList();

            var subSameAddrPropInRepetitionGroup2ViewModels = _configurationFragmentViewModel
                .RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindAllItemViewModelsByName(model => model.Header == "subSameAddrPropInRepetitionGroup2")
                .Cast<IRuntimePropertyViewModel>()
                .ToList();
            await Read();


            Func<EditableBoolValueViewModel> subSameAddrPropInRepetitionGroup1LocalValue1 = () =>
                subSameAddrPropInRepetitionGroup1ViewModels[0].LocalValue as EditableBoolValueViewModel;
            Func<EditableBoolValueViewModel> subSameAddrPropInRepetitionGroup1LocalValue2 = () =>
                subSameAddrPropInRepetitionGroup1ViewModels[1].LocalValue as EditableBoolValueViewModel;

            Func<EditableBoolValueViewModel> subSameAddrPropInRepetitionGroup2LocalValue1 = () =>
                subSameAddrPropInRepetitionGroup2ViewModels[0].LocalValue as EditableBoolValueViewModel;
            Func<EditableBoolValueViewModel> subSameAddrPropInRepetitionGroup2LocalValue2 = () =>
                subSameAddrPropInRepetitionGroup2ViewModels[1].LocalValue as EditableBoolValueViewModel;


            Assert.False(subSameAddrPropInRepetitionGroup1LocalValue1().BoolValueProperty);
            Assert.False(subSameAddrPropInRepetitionGroup1LocalValue2().BoolValueProperty);
            Assert.False(subSameAddrPropInRepetitionGroup2LocalValue1().BoolValueProperty);
            Assert.False(subSameAddrPropInRepetitionGroup2LocalValue2().BoolValueProperty);



            subSameAddrPropInRepetitionGroup1LocalValue1().BoolValueProperty = true;

            Assert.False(subSameAddrPropInRepetitionGroup2LocalValue1().BoolValueProperty);

            subSameAddrPropInRepetitionGroup2LocalValue2().BoolValueProperty = true;

            Assert.False(subSameAddrPropInRepetitionGroup1LocalValue2().BoolValueProperty);

            var res = subSameAddrPropInRepetitionGroup1LocalValue1().BoolValueProperty;
            var res1 = subSameAddrPropInRepetitionGroup1LocalValue2().BoolValueProperty;
            var res2 = subSameAddrPropInRepetitionGroup2LocalValue1().BoolValueProperty;
            var res3 = subSameAddrPropInRepetitionGroup2LocalValue2().BoolValueProperty;

        }

        [Test]
        public async Task DependencyDefaultPropertyToSubproperty()
        {

            var defaultPropertyFromSubPropertyDependencySource =
                _configuration.RootConfigurationItemList
                    .FindItemByName(item => item.Name == "defaultPropertyFromSubPropertyDependencySource")
                    .Item as IProperty;

            var defaultPropertyFromSubPropertyDependencyConsumer =
                _configuration.RootConfigurationItemList
                    .FindItemByName(item => item.Name == "defaultPropertyFromSubPropertyDependencyConsumer")
                    .Item as IProperty;

            var defaultPropertyFromSubPropertyDependencySourceViewModel = _configurationFragmentViewModel
                .RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "defaultPropertyFromSubPropertyDependencySource")
                .Item as IRuntimePropertyViewModel;

            var defaultPropertyFromSubPropertyDependencyConsumerViewModel = _configurationFragmentViewModel
                .RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "defaultPropertyFromSubPropertyDependencyConsumer")
                .Item as IRuntimePropertyViewModel;
            await Read();


            Func<EditableBoolValueViewModel> defaultPropertyFromSubPropertyDependencySourceViewModelLocalValue = () =>
                defaultPropertyFromSubPropertyDependencySourceViewModel.LocalValue as EditableBoolValueViewModel;
            Func<EditableBoolValueViewModel> defaultPropertyFromSubPropertyDependencyConsumerViewModelViewModelLocalValue = () =>
                defaultPropertyFromSubPropertyDependencyConsumerViewModel.LocalValue as EditableBoolValueViewModel;
            
            Assert.False(defaultPropertyFromSubPropertyDependencySourceViewModelLocalValue().BoolValueProperty);
            Assert.False(defaultPropertyFromSubPropertyDependencyConsumerViewModelViewModelLocalValue().BoolValueProperty);
            Assert.False(defaultPropertyFromSubPropertyDependencyConsumerViewModelViewModelLocalValue().IsEditEnabled);

            defaultPropertyFromSubPropertyDependencySourceViewModelLocalValue().BoolValueProperty = true;
            Assert.True(defaultPropertyFromSubPropertyDependencySourceViewModelLocalValue().IsFormattedValueChanged);
            Assert.True(defaultPropertyFromSubPropertyDependencyConsumerViewModelViewModelLocalValue().IsEditEnabled);

            defaultPropertyFromSubPropertyDependencySourceViewModelLocalValue().BoolValueProperty = false;
            Assert.False(defaultPropertyFromSubPropertyDependencySourceViewModelLocalValue().IsFormattedValueChanged);
            Assert.False(defaultPropertyFromSubPropertyDependencyConsumerViewModelViewModelLocalValue().IsEditEnabled);
            
       
        }


        [Test]
        public async Task DependencySubpropertyToSubpropertyConsumerValue()
        {

            var boolTestSubPropertyDependencySource =
                _configuration.RootConfigurationItemList
                    .FindItemByName(item => item.Name == "boolTestSubPropertyDependencySource")
                    .Item as IProperty;

            var boolTestSubPropertyDependencyConsumer =
                _configuration.RootConfigurationItemList
                    .FindItemByName(item => item.Name == "boolTestSubPropertyDependencyConsumer")
                    .Item as IProperty;

            var boolTestSubPropertyDependencyNotRelated =
                _configuration.RootConfigurationItemList
                    .FindItemByName(item => item.Name == "boolTestSubProperty6")
                    .Item as IProperty;


            var boolTestSubPropertyDependencySourceViewModel = _configurationFragmentViewModel
                .RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "boolTestSubPropertyDependencySource")
                .Item as IRuntimePropertyViewModel;

            var boolTestSubPropertyDependencyConsumerViewModel = _configurationFragmentViewModel
                .RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "boolTestSubPropertyDependencyConsumer")
                .Item as IRuntimePropertyViewModel;

            var boolTestSubPropertyDependencyNotRelatedViewModel = _configurationFragmentViewModel
                .RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "boolTestSubProperty6")
                .Item as IRuntimePropertyViewModel;

            await Read();


            Func<EditableBoolValueViewModel> boolTestSubPropertyDependencySourceViewModelLocalValue = () =>
                boolTestSubPropertyDependencySourceViewModel.LocalValue as EditableBoolValueViewModel;
            Func<EditableBoolValueViewModel> boolTestSubPropertyDependencyConsumerViewModelLocalValue = () =>
                boolTestSubPropertyDependencyConsumerViewModel.LocalValue as EditableBoolValueViewModel;

            Func<EditableBoolValueViewModel> boolTestSubPropertyDependencyNotRelatedViewModelLocalValue = () =>
                boolTestSubPropertyDependencyNotRelatedViewModel.LocalValue as EditableBoolValueViewModel;


            Assert.False(boolTestSubPropertyDependencySourceViewModelLocalValue().BoolValueProperty);
            Assert.False(boolTestSubPropertyDependencyConsumerViewModelLocalValue().BoolValueProperty);
            Assert.False(boolTestSubPropertyDependencyConsumerViewModelLocalValue().IsEditEnabled);


            boolTestSubPropertyDependencySourceViewModelLocalValue().BoolValueProperty = true;
            Assert.True(boolTestSubPropertyDependencySourceViewModelLocalValue().IsFormattedValueChanged);

            Assert.False(boolTestSubPropertyDependencyConsumerViewModelLocalValue().BoolValueProperty);
            Assert.True(boolTestSubPropertyDependencyConsumerViewModelLocalValue().IsEditEnabled);


            boolTestSubPropertyDependencyConsumerViewModelLocalValue().BoolValueProperty = true;
            Assert.True(boolTestSubPropertyDependencyConsumerViewModelLocalValue().IsFormattedValueChanged);
            Assert.True(boolTestSubPropertyDependencyConsumerViewModelLocalValue().IsEditEnabled);

            Assert.True(boolTestSubPropertyDependencySourceViewModelLocalValue().IsFormattedValueChanged);
            Assert.True(boolTestSubPropertyDependencySourceViewModelLocalValue().IsEditEnabled);


            boolTestSubPropertyDependencySourceViewModelLocalValue().BoolValueProperty = false;


            Assert.False(boolTestSubPropertyDependencySourceViewModelLocalValue().BoolValueProperty);
            Assert.False(boolTestSubPropertyDependencyConsumerViewModelLocalValue().IsEditEnabled);


            Assert.False(boolTestSubPropertyDependencyNotRelatedViewModelLocalValue().BoolValueProperty);
            Assert.True(boolTestSubPropertyDependencyNotRelatedViewModelLocalValue().IsEditEnabled);

            boolTestSubPropertyDependencyNotRelatedViewModelLocalValue().BoolValueProperty = true;


            Assert.True(boolTestSubPropertyDependencyNotRelatedViewModelLocalValue().BoolValueProperty);
            Assert.True(boolTestSubPropertyDependencyNotRelatedViewModelLocalValue().IsEditEnabled);

            Assert.False(boolTestSubPropertyDependencySourceViewModelLocalValue().BoolValueProperty);
            Assert.False(boolTestSubPropertyDependencyConsumerViewModelLocalValue().IsEditEnabled);
            boolTestSubPropertyDependencyNotRelatedViewModelLocalValue().BoolValueProperty = false;

        }


        [Test]
        public async Task DependencySubpropertyToSubpropertyWithNotRelatedButSameComplexPropConsumerValue()
        {

            var boolTestSubPropertyDependencySource =
                _configuration.RootConfigurationItemList
                    .FindItemByName(item => item.Name == "subToSubPropertySameComplexPropSource")
                    .Item as IProperty;

            var boolTestSubPropertyDependencyConsumer =
                _configuration.RootConfigurationItemList
                    .FindItemByName(item => item.Name == "subToSubPropertySameComplexPropConsumer")
                    .Item as IProperty;

            var boolTestSubPropertyDependencyNotRelated =
                _configuration.RootConfigurationItemList
                    .FindItemByName(item => item.Name == "subToSubPropertySameComplexPropNotRelated")
                    .Item as IProperty;


            var boolTestSubPropertyDependencySourceViewModel = _configurationFragmentViewModel
                .RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "subToSubPropertySameComplexPropSource")
                .Item as IRuntimePropertyViewModel;

            var boolTestSubPropertyDependencyConsumerViewModel = _configurationFragmentViewModel
                .RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "subToSubPropertySameComplexPropConsumer")
                .Item as IRuntimePropertyViewModel;

            var boolTestSubPropertyDependencyNotRelatedViewModel = _configurationFragmentViewModel
                .RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "subToSubPropertySameComplexPropNotRelated")
                .Item as IRuntimePropertyViewModel;

            await Read();


            Func<EditableBoolValueViewModel> boolTestSubPropertyDependencySourceViewModelLocalValue = () =>
                boolTestSubPropertyDependencySourceViewModel.LocalValue as EditableBoolValueViewModel;
            Func<EditableBoolValueViewModel> boolTestSubPropertyDependencyConsumerViewModelLocalValue = () =>
                boolTestSubPropertyDependencyConsumerViewModel.LocalValue as EditableBoolValueViewModel;

            Func<EditableBoolValueViewModel> boolTestSubPropertyDependencyNotRelatedViewModelLocalValue = () =>
                boolTestSubPropertyDependencyNotRelatedViewModel.LocalValue as EditableBoolValueViewModel;


            Assert.False(boolTestSubPropertyDependencySourceViewModelLocalValue().BoolValueProperty);
            Assert.False(boolTestSubPropertyDependencyConsumerViewModelLocalValue().BoolValueProperty);
            Assert.False(boolTestSubPropertyDependencyConsumerViewModelLocalValue().IsEditEnabled);


            boolTestSubPropertyDependencySourceViewModelLocalValue().BoolValueProperty = true;
            Assert.True(boolTestSubPropertyDependencySourceViewModelLocalValue().IsFormattedValueChanged);

            Assert.False(boolTestSubPropertyDependencyConsumerViewModelLocalValue().BoolValueProperty);
            Assert.True(boolTestSubPropertyDependencyConsumerViewModelLocalValue().IsEditEnabled);


            boolTestSubPropertyDependencyConsumerViewModelLocalValue().BoolValueProperty = true;
            Assert.True(boolTestSubPropertyDependencyConsumerViewModelLocalValue().IsFormattedValueChanged);
            Assert.True(boolTestSubPropertyDependencyConsumerViewModelLocalValue().IsEditEnabled);

            Assert.True(boolTestSubPropertyDependencySourceViewModelLocalValue().IsFormattedValueChanged);
            Assert.True(boolTestSubPropertyDependencySourceViewModelLocalValue().IsEditEnabled);


            boolTestSubPropertyDependencySourceViewModelLocalValue().BoolValueProperty = false;


            Assert.False(boolTestSubPropertyDependencySourceViewModelLocalValue().BoolValueProperty);
            Assert.False(boolTestSubPropertyDependencyConsumerViewModelLocalValue().IsEditEnabled);


            Assert.False(boolTestSubPropertyDependencyNotRelatedViewModelLocalValue().BoolValueProperty);
            Assert.True(boolTestSubPropertyDependencyNotRelatedViewModelLocalValue().IsEditEnabled);

            boolTestSubPropertyDependencyNotRelatedViewModelLocalValue().BoolValueProperty = true;


            Assert.True(boolTestSubPropertyDependencyNotRelatedViewModelLocalValue().BoolValueProperty);
            Assert.True(boolTestSubPropertyDependencyNotRelatedViewModelLocalValue().IsEditEnabled);

            Assert.False(boolTestSubPropertyDependencySourceViewModelLocalValue().BoolValueProperty);
            Assert.False(boolTestSubPropertyDependencyConsumerViewModelLocalValue().IsEditEnabled);
        }

        
        [Test]
        public async Task DependencySubpropertyToSubproperty()
        {

            var boolTestSubPropertyDependencySource =
                _configuration.RootConfigurationItemList
                    .FindItemByName(item => item.Name == "boolTestSubPropertyDependencySource")
                    .Item as IProperty;

            var boolTestSubPropertyDependencyConsumer =
                _configuration.RootConfigurationItemList
                    .FindItemByName(item => item.Name == "boolTestSubPropertyDependencyConsumer")
                    .Item as IProperty;

            var boolTestSubPropertyDependencySourceViewModel = _configurationFragmentViewModel
                .RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "boolTestSubPropertyDependencySource")
                .Item as IRuntimePropertyViewModel;

            var boolTestSubPropertyDependencyConsumerViewModel = _configurationFragmentViewModel
                .RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "boolTestSubPropertyDependencyConsumer")
                .Item as IRuntimePropertyViewModel;
            await Read();


            Func<EditableBoolValueViewModel> boolTestSubPropertyDependencySourceViewModelLocalValue = () =>
                boolTestSubPropertyDependencySourceViewModel.LocalValue as EditableBoolValueViewModel;
            Func<EditableBoolValueViewModel> boolTestSubPropertyDependencyConsumerViewModelLocalValue = () =>
                boolTestSubPropertyDependencyConsumerViewModel.LocalValue as EditableBoolValueViewModel;
            
            Assert.False(boolTestSubPropertyDependencySourceViewModelLocalValue().BoolValueProperty);
            Assert.False(boolTestSubPropertyDependencyConsumerViewModelLocalValue().BoolValueProperty);
            Assert.False(boolTestSubPropertyDependencyConsumerViewModelLocalValue().IsEditEnabled);

            
            
            boolTestSubPropertyDependencySourceViewModelLocalValue().BoolValueProperty = true;
            Assert.True(boolTestSubPropertyDependencySourceViewModelLocalValue().IsFormattedValueChanged);
            Assert.True(boolTestSubPropertyDependencyConsumerViewModelLocalValue().IsEditEnabled);


            boolTestSubPropertyDependencySourceViewModelLocalValue().BoolValueProperty = false;
            Assert.False(boolTestSubPropertyDependencySourceViewModelLocalValue().IsFormattedValueChanged);
            Assert.False(boolTestSubPropertyDependencyConsumerViewModelLocalValue().IsEditEnabled);
        }

        [Test]
        public async Task EditedLocalValueMustNotBeOverwritten()
        {
            _device.DeviceMemory.DeviceMemoryValues.Clear();

            var boolTestDefaultProperty =
                _configuration.RootConfigurationItemList.FindItemByName(item => item.Name == "boolTestDefaultProperty")
                    .Item as IProperty;

            var defaultPropertyWithBoolFormatting = _configurationFragmentViewModel.RootConfigurationItemViewModels
                .Cast<IConfigurationItemViewModel>().ToList()
                .FindItemViewModelByName(model => model.Header == "boolTestDefaultProperty")
                .Item as IRuntimePropertyViewModel;
            await Read();

            _configurationFragmentViewModel.DeviceContext.DeviceMemory.LocalMemoryValues[boolTestDefaultProperty.Address] = 1;
            _configurationFragmentViewModel.DeviceContext.DeviceEventsDispatcher.TriggerLocalAddressSubscription(
                boolTestDefaultProperty.Address, boolTestDefaultProperty.NumberOfPoints);

            Assert.True(defaultPropertyWithBoolFormatting.LocalValue.IsFormattedValueChanged);
            Assert.True((defaultPropertyWithBoolFormatting.LocalValue as EditableBoolValueViewModel).BoolValueProperty);

            await Read();

            Assert.True(_configurationFragmentViewModel.DeviceContext.DeviceMemory.LocalMemoryValues[boolTestDefaultProperty.Address]== 1);

            Assert.True(_configurationFragmentViewModel.DeviceContext.DeviceMemory.DeviceMemoryValues[boolTestDefaultProperty.Address] == 0);
            Assert.True(defaultPropertyWithBoolFormatting.LocalValue.IsFormattedValueChanged);

        }

        
        [Test]
        public async Task GroupFilterCompareResourceCheck()
        {
            var groupViewModel =
                _configurationFragmentViewModel.RootConfigurationItemViewModels.First(model => model.Header == "Входные логические сигналы") as RuntimeItemGroupViewModel;
            groupViewModel.IsTableView = true;
            groupViewModel.FilterViewModels[0].IsActivated = true;
            Assert.AreEqual(groupViewModel.TableConfigurationViewModel.FilteredPropertiesTable.Values.Count, 0);
            Assert.AreEqual(groupViewModel.TableConfigurationViewModel.FilteredPropertiesTable.RowHeadersStrings.Count, 0);
            Assert.AreEqual(groupViewModel.TableConfigurationViewModel.FilteredPropertiesTable.ColumnNamesStrings.Count, 0);


            ((groupViewModel.ChildStructItemViewModels[5].ChildStructItemViewModels[5] as IRuntimePropertyViewModel).LocalValue as
                EditableBoolValueViewModel).BoolValueProperty = true;
            groupViewModel.FilterViewModels[0].IsActivated = false;  
            groupViewModel.FilterViewModels[0].IsActivated = true;

            
            Assert.AreEqual(groupViewModel.TableConfigurationViewModel.FilteredPropertiesTable.Values[0].Count, 1);
            Assert.AreEqual(groupViewModel.TableConfigurationViewModel.FilteredPropertiesTable.RowHeadersStrings.Count, 1);
            Assert.AreEqual(groupViewModel.TableConfigurationViewModel.FilteredPropertiesTable.ColumnNamesStrings.Count, 1);

        }
        
     
        
        private async Task ReadAndTransfer()
        {
            await Read();
            await TransferFromDeviceToLocal();
        }
        
        private async Task TransferFromDeviceToLocal()
        {
            var memoryAccessor = new ConfigurationMemoryAccessor(_configuration,
                _configurationFragmentViewModel.DeviceContext, MemoryAccessEnum.TransferFromDeviceToLocal,true);
            await memoryAccessor.Process();
        }

        private async Task Read()
        {
            if (_readCommand.CanExecute(null))
            {
                _readCommand.Execute(null);
            }

            Assert.True(await TestsUtils.WaitUntil(() => _readCommand.CanExecute(null), 30000));

        }

        private async Task Write()
        {
            if (LocalValuesWriteValidator.ValidateLocalValuesToWrite(_configurationFragmentViewModel
                .RootConfigurationItemViewModels))
            {
                await new MemoryWriterVisitor(_configurationFragmentViewModel.DeviceContext, new List<ushort>(),
                    _configuration, 0).ExecuteWrite();
            }
        }
        
        
    }
}
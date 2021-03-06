﻿using Unicon2.Fragments.Programming.Infrastructure.Factories;
using Unicon2.Fragments.Programming.Infrastructure.Keys;
using Unicon2.Fragments.Programming.Infrastructure.Model;
using Unicon2.Fragments.Programming.Infrastructure.Model.Elements;
using Unicon2.Fragments.Programming.Infrastructure.ViewModels.Scheme;
using Unicon2.Fragments.Programming.Infrastructure.ViewModels.Scheme.ElementViewModels;
using Unicon2.Fragments.Programming.Model;
using Unicon2.Fragments.Programming.Model.Elements;
using Unicon2.Fragments.Programming.Other;
using Unicon2.Fragments.Programming.ViewModels;
using Unicon2.Fragments.Programming.ViewModels.ElementViewModels;
using Unicon2.Infrastructure;
using Unicon2.Infrastructure.Services;
using Unicon2.Presentation.Infrastructure.ViewModels.FragmentInterfaces;
using Unicon2.Unity.Interfaces;

namespace Unicon2.Fragments.Programming.Module
{
    public class ProgrammingModule : IUnityModule
    {
        public void Initialize(ITypesContainer container)
        {
            container.Register<IProgramModel, ProgramModel>();
            container.Register<LogicDeviceProvider>();

            container.Register<IFragmentViewModel, ProgrammingViewModel>(ProgrammingKeys.PROGRAMMING + ApplicationGlobalNames.CommonInjectionStrings.VIEW_MODEL);
            container.Register<ISchemeTabViewModel, SchemeTabViewModel>(ProgrammingKeys.SCHEME_TAB + ApplicationGlobalNames.CommonInjectionStrings.VIEW_MODEL);

            container.Register<ILogicElementFactory, LogicElementFactory>();

            container.Register<ILogicElement, Input>(ProgrammingKeys.INPUT);
            container.Register<ILogicElementViewModel, InputViewModel>(ProgrammingKeys.INPUT + ApplicationGlobalNames.CommonInjectionStrings.VIEW_MODEL);
            container.Register<ILogicElement, Output>(ProgrammingKeys.OUTPUT);
            container.Register<ILogicElementViewModel, OutputViewModel>(ProgrammingKeys.OUTPUT + ApplicationGlobalNames.CommonInjectionStrings.VIEW_MODEL);
            container.Register<ILogicElement, Inversion>(ProgrammingKeys.INVERSION);
            container.Register<ILogicElementViewModel, InversionViewModel>(ProgrammingKeys.INVERSION + ApplicationGlobalNames.CommonInjectionStrings.VIEW_MODEL);
            container.Register<ILogicElement, And>(ProgrammingKeys.AND);
            container.Register<ILogicElementViewModel, AndViewModel>(ProgrammingKeys.AND + ApplicationGlobalNames.CommonInjectionStrings.VIEW_MODEL);
            container.Register<ILogicElement, Or>(ProgrammingKeys.OR);
            container.Register<ILogicElementViewModel, OrViewModel>(ProgrammingKeys.OR + ApplicationGlobalNames.CommonInjectionStrings.VIEW_MODEL);
            container.Register<ILogicElement, Xor>(ProgrammingKeys.XOR);
            container.Register<ILogicElementViewModel, XorViewModel>(ProgrammingKeys.XOR + ApplicationGlobalNames.CommonInjectionStrings.VIEW_MODEL);
            container.Register<ILogicElement, AlarmJournal>(ProgrammingKeys.ALARM_JOURNAL);
            container.Register<ILogicElementViewModel, AlarmJournalViewModel>(ProgrammingKeys.ALARM_JOURNAL + ApplicationGlobalNames.CommonInjectionStrings.VIEW_MODEL);
            container.Register<ILogicElement, SystemJournal>(ProgrammingKeys.SYSTEM_JOURNAL);
            container.Register<ILogicElementViewModel, SystemJournalViewModel>(ProgrammingKeys.SYSTEM_JOURNAL + ApplicationGlobalNames.CommonInjectionStrings.VIEW_MODEL);
            container.Register<ILogicElement, Timer>(ProgrammingKeys.TIMER);
            container.Register<ILogicElementViewModel, TimerViewModel>(ProgrammingKeys.TIMER + ApplicationGlobalNames.CommonInjectionStrings.VIEW_MODEL);
            container.Register<ILogicElement, RsTrigger>(ProgrammingKeys.RS_TRIGGER);
            container.Register<ILogicElementViewModel, RsTriggerViewModel>(ProgrammingKeys.RS_TRIGGER + ApplicationGlobalNames.CommonInjectionStrings.VIEW_MODEL);
            container.Register<ILogicElement, SrTrigger>(ProgrammingKeys.SR_TRIGGER);
            container.Register<ILogicElementViewModel, SrTriggerViewModel>(ProgrammingKeys.SR_TRIGGER + ApplicationGlobalNames.CommonInjectionStrings.VIEW_MODEL);

            container.Resolve<IXamlResourcesService>().AddResourceAsGlobal("UI/ProgrammingViewTemplate.xaml", GetType().Assembly);
        }
    }
}

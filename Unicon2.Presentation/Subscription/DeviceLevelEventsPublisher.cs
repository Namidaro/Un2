using System;
using System.Collections.Generic;
using Unicon2.Infrastructure.Extensions;
using Unicon2.Infrastructure.Functional;
using Unicon2.Presentation.Infrastructure.Subscription;
using Unicon2.Presentation.Infrastructure.ViewModels.FragmentInterfaces;

namespace Unicon2.Presentation.Subscription
{
    public class DeviceLevelEventsPublisher : IDeviceEventsPublisher
    {
        private readonly Func<IFragmentViewModel> _getActiveFragment;
        private Dictionary<IFragmentViewModel, IDeviceEventsDispatcher> _fragmentAwareDispatchers;

        public DeviceLevelEventsPublisher(Func<IFragmentViewModel> getActiveFragment)
        {
            _getActiveFragment = getActiveFragment;
            _fragmentAwareDispatchers = new Dictionary<IFragmentViewModel, IDeviceEventsDispatcher>();
        }

        public void AddFragmentDispatcher(IFragmentViewModel fragmentViewModel,
            IDeviceEventsDispatcher deviceEventsDispatcher)
        {
            _fragmentAwareDispatchers.Add(fragmentViewModel, deviceEventsDispatcher);
        }

        public void Dispose()
        {
            _fragmentAwareDispatchers.ForEach(pair => pair.Value.Dispose());
        }

        public Result TriggerLocalAddressSubscription(ushort triggeredAddress, ushort numberOfPoints,
            MemoryKind memoryKind = MemoryKind.UshortMemory)
        {
            return _fragmentAwareDispatchers[_getActiveFragment()]
                .TriggerLocalAddressSubscription(triggeredAddress, numberOfPoints, memoryKind);
        }



        public Result TriggerDeviceAddressSubscription(ushort triggeredAddress, ushort numberOfPoints,
            MemoryKind memoryKind = MemoryKind.UshortMemory)
        {

            return _fragmentAwareDispatchers[_getActiveFragment()]
                .TriggerDeviceAddressSubscription(triggeredAddress, numberOfPoints, memoryKind);
        }

        public Result TriggerSubscriptionById(Guid id)
        {
            return _fragmentAwareDispatchers[_getActiveFragment()]
                .TriggerSubscriptionById(id);
        }
    }
}
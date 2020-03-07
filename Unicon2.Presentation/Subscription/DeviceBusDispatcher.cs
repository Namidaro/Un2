using System;
using System.Collections.Generic;
using System.Linq;
using Unicon2.Infrastructure.Functional;
using Unicon2.Presentation.Infrastructure.Subscription;

namespace Unicon2.Presentation.Subscription
{
    public class DeviceEventsDispatcher : IDeviceEventsDispatcher
    {
        private readonly Dictionary<ushort, MemorySubscriptionCollection<IMemorySubscription>>
            _deviceMemoryObservers;

        private readonly Dictionary<ushort, MemorySubscriptionCollection<IMemorySubscription>>
            _localMemoryDataObservers;

        private readonly Dictionary<Guid, MemorySubscriptionCollection<IMemorySubscription>>
            _idObservers;

        public DeviceEventsDispatcher()
        {
            _deviceMemoryObservers =
                new Dictionary<ushort, MemorySubscriptionCollection<IMemorySubscription>>();
            _localMemoryDataObservers =
                new Dictionary<ushort, MemorySubscriptionCollection<IMemorySubscription>>();
            _idObservers =
                new Dictionary<Guid, MemorySubscriptionCollection<IMemorySubscription>>();
        }

        private static ushort[] GetAddressesRelated(ushort start, ushort length)
        {
            var res = new ushort[length];
            var count = 0;
            for (var i = start; i < start + length; i++)
            {
                res[count++] = i;
            }

            return res;
        }


        private void AddDeviceSubscriptionToCollection(ushort address,
            IMemorySubscription deviceDataMemorySubscription,
            Dictionary<ushort, MemorySubscriptionCollection<IMemorySubscription>>
                memoryDataObservers)
        {
            if (!memoryDataObservers.ContainsKey(address))
            {
                memoryDataObservers.Add(address,
                    new MemorySubscriptionCollection<IMemorySubscription>(deviceDataMemorySubscription));
            }
            else
            {
                if (_deviceMemoryObservers[address].Collection
                    .All(subscription => subscription != deviceDataMemorySubscription))
                    memoryDataObservers[address].Collection.Add(deviceDataMemorySubscription);
            }
        }


        public void Dispose()
        {
            _deviceMemoryObservers.Clear();
            _idObservers.Clear();

        }


        public Result AddDeviceAddressSubscription(ushort start, ushort length,
            IMemorySubscription memorySubscription)
        {
            return AddAddressSubscription(start, length,
                memorySubscription, _deviceMemoryObservers);
        }

        private Result AddAddressSubscription(ushort start, ushort length,
            IMemorySubscription memorySubscription,
            Dictionary<ushort, MemorySubscriptionCollection<IMemorySubscription>>
                memoryObservers)
        {
            var addresses = GetAddressesRelated(start,
                length);
            if (addresses.All(address =>
                memoryObservers.ContainsKey(address) && memoryObservers[address].Collection.Any(
                    subscription =>
                        subscription == memorySubscription)))
            {
                return Result.Create(true);
            }

            foreach (var address in addresses)
            {
                AddDeviceSubscriptionToCollection(address, memorySubscription, memoryObservers);
            }

            return Result.Create(true);
        }

        public Result AddLocalAddressSubscription(ushort start, ushort length,
            IMemorySubscription memorySubscription)
        {
            return AddAddressSubscription(start, length,
                memorySubscription, _localMemoryDataObservers);
        }

        public Result AddSubscriptionById(IMemorySubscription subscription, Guid id)
        {
            if (_idObservers.ContainsKey(id))
            {
                if (_idObservers[id].Collection
                    .Any(subscriptionExisting => subscriptionExisting == subscription))
                {
                    return Result.Create(true);
                }
                else
                {
                    _idObservers[id].Collection
                        .Add(subscription);
                    return Result.Create(true);

                }
            }

            _idObservers.Add(id,
                new MemorySubscriptionCollection<IMemorySubscription>(subscription));
            return Result.Create(true);
        }

        public Result TriggerLocalAddressSubscription(ushort triggeredAddress, ushort numberOfPoints)
        {
            return TriggerAddressSubscription(triggeredAddress, numberOfPoints, _localMemoryDataObservers);
        }

        private Result TriggerAddressSubscription(ushort triggeredAddress, ushort numberOfPoints,
            Dictionary<ushort, MemorySubscriptionCollection<IMemorySubscription>>
                memoryDataObservers)
        {
            List<IMemorySubscription> deviceDataMemorySubscriptions =
                new List<IMemorySubscription>();
            for (var i = triggeredAddress; i < triggeredAddress + numberOfPoints; i++)
            {
                deviceDataMemorySubscriptions.AddRange(memoryDataObservers[i].Collection);
            }

            deviceDataMemorySubscriptions.Distinct().ToList().ForEach(subscription => subscription.Execute());
            return Result.Create(true);
        }

        public Result TriggerDeviceAddressSubscription(ushort triggeredAddress, ushort numberOfPoints)
        {
            return TriggerAddressSubscription(triggeredAddress, numberOfPoints, _deviceMemoryObservers);
        }

        public Result TriggerSubscriptionById(Guid id)
        {
            _idObservers[id].Collection.ForEach(subscription => subscription.Execute());
            return Result.Create(true);
        }
    }

    public class MemorySubscriptionCollection<T>
    {
        public MemorySubscriptionCollection()
        {
            Collection = new List<T>();
        }

        public MemorySubscriptionCollection(T initial)
        {
            Collection = new List<T>() {initial};
        }

        public List<T> Collection { get; }

    }


}
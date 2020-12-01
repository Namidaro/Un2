﻿using Unicon2.Connections.ModBusRtuConnection.Interfaces.Factories;
using Unicon2.Infrastructure.DeviceInterfaces;
using Unicon2.Unity.Interfaces;

namespace Unicon2.Connections.ModBusRtuConnection.Factories
{
    public class ComPortConfigurationFactory : IComPortConfigurationFactory
    {
        private readonly ITypesContainer _container;

        public ComPortConfigurationFactory(ITypesContainer container)
        {
            _container = container;
        }
        
        public IComPortConfiguration CreateComPortConfiguration()
        {
            return _container.Resolve<IComPortConfiguration>();
        }
    }
}

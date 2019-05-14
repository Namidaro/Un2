﻿using System;
using NModbus4.Data;
using NModbus4.Message;
using Xunit;

namespace NModbus4.UnitTests.Message
{
    public class WriteMultipleCoilsRequestFixture
    {
        [Fact]
        public void CreateWriteMultipleCoilsRequest()
        {
            DiscreteCollection col = new DiscreteCollection(true, false, true, false, true, true, true, false, false);
            WriteMultipleCoilsRequest request = new WriteMultipleCoilsRequest(34, 45, col);
            Assert.Equal(Modbus.WriteMultipleCoils, request.FunctionCode);
            Assert.Equal(34, request.SlaveAddress);
            Assert.Equal(45, request.StartAddress);
            Assert.Equal(9, request.NumberOfPoints);
            Assert.Equal(2, request.ByteCount);
            Assert.Equal(col.NetworkBytes, request.Data.NetworkBytes);
        }

        [Fact]
        public void CreateWriteMultipleCoilsRequestTooMuchData()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new WriteMultipleCoilsRequest(1, 2, MessageUtility.CreateDefaultCollection<DiscreteCollection, bool>(true, Modbus.MaximumDiscreteRequestResponseSize + 1)));
        }

        [Fact]
        public void CreateWriteMultipleCoilsRequestMaxSize()
        {
            WriteMultipleCoilsRequest request = new WriteMultipleCoilsRequest(1, 2,
                MessageUtility.CreateDefaultCollection<DiscreteCollection, bool>(true, Modbus.MaximumDiscreteRequestResponseSize));
            Assert.Equal(Modbus.MaximumDiscreteRequestResponseSize, request.Data.Count);
        }

        [Fact]
        public void ToString_WriteMultipleCoilsRequest()
        {
            DiscreteCollection col = new DiscreteCollection(true, false, true, false, true, true, true, false, false);
            WriteMultipleCoilsRequest request = new WriteMultipleCoilsRequest(34, 45, col);

            Assert.Equal((string) "Write 9 coils starting at address 45.", (string) request.ToString());
        }
    }
}
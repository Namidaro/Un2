﻿using System;
using NModbus4.Message;

namespace NModbus4.IO
{
    public class EmptyTransport : ModbusTransport
    {
        internal override byte[] ReadRequest()
        {
            throw new NotImplementedException();
        }

        internal override IModbusMessage ReadResponse<T>()
        {
            throw new NotImplementedException();
        }

        internal override byte[] BuildMessageFrame(IModbusMessage message)
        {
            throw new NotImplementedException();
        }

        internal override void Write(IModbusMessage message)
        {
            throw new NotImplementedException();
        }

        internal override void OnValidateResponse(IModbusMessage request, IModbusMessage response)
        {
            throw new NotImplementedException();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using Unicon2.Fragments.Programming.Infrastructure.Keys;
using Unicon2.Fragments.Programming.Infrastructure.Model.Elements;

namespace Unicon2.Fragments.Programming.Model.Elements
{
    public class Input : IInput
    {
        private const int BIN_SIZE = 3;

        private ushort _inputSignal;
        private ushort _base;
        private int _connectionNumber;

        public Dictionary<int, Dictionary<int, string>> AllInputSignals { get; private set; }
        public List<string> Bases { get; private set; }

        public Input()
        {
            this.Functional = Functional.BOOLEAN;
            this.Group = Group.INPUT_OUTPUT;

            this.Bases = new List<string> {"Base1"};

            this.AllInputSignals =
                new Dictionary<int, Dictionary<int, string>>
                {
                    {this.Bases.Count - 1, new Dictionary<int, string> {{0, string.Empty}}}
                };
        }

        private Input(Input cloneable)
        {
            this.CopyValues(cloneable);
        }

        public Functional Functional { get; private set; }
        public Group Group { get; private set; }
        public int BinSize => BIN_SIZE;

        public void CopyValues(ILogicElement source)
        {
            if (!(source is Input inputSource))
            {
                throw new ArgumentException("Copied source is not " + typeof(Input));
            }

            this.Functional = inputSource.Functional;
            this.Group = inputSource.Group;
            this._inputSignal = inputSource._inputSignal;
            this._base = inputSource._base;
            this._connectionNumber = inputSource._connectionNumber;
            this.Bases = new List<string>(inputSource.Bases);

            this.AllInputSignals = new Dictionary<int, Dictionary<int, string>>(inputSource.AllInputSignals);
            for (int i = 0; i < this.Bases.Count; i++)
            {
                var copiedDictionary = inputSource.AllInputSignals[i];
                this.AllInputSignals[i] = new Dictionary<int, string>(copiedDictionary);
            }
        }

        public ushort[] GetProgrammBin()
        {
            ushort[] bindata = new ushort[this.BinSize];
            switch (this._base)
            {
                case 0:
                {
                    bindata[0] = 2;
                    break;
                }
                case 1:
                {
                    bindata[0] = 3;
                    break;
                }
                case 2:
                {
                    bindata[0] = 4;
                    break;
                }
                case 3:
                {
                    bindata[0] = 33;
                    break;
                }
                case 4:
                {
                    bindata[0] = 34;
                    break;
                }
            }
            bindata[1] = this._inputSignal;
            bindata[2] = (ushort)this._connectionNumber;
            return bindata;
        }

        public void BinProgrammToProperty(ushort[] bin)
        {
            this._base = bin[0];
            this._inputSignal = bin[1];
            this._connectionNumber = bin[2];
        }

        public int ConnectionNumber
        {
            get { return this._connectionNumber; }
            set { this._connectionNumber = value; }
        }

        #region IStronglyName
        public string StrongName => ProgrammingKeys.INPUT;
        #endregion

        public object Clone()
        {
            return new Input(this);
        }
    }
}

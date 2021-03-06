﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OSCForPCL.Values
{
    public class OSCNull : IOSCValue
    {
        public OSCNull()
        {
            Bytes = new byte[0];
        }

        public byte[] Bytes { get; }
        public char TypeTag { get { return 'N'; } }

        public object GetValue()
        {
            return null;
        }

        public override string ToString()
        {
            return "Null";
        }
        
    }
}

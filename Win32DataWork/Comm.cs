using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Win32DataWork
{
    public enum Security_Mode : byte
    {
        None = 0,
        MD5 = 1,
        DES = 2,
        TripeDes = 3,
    }

    public enum CheckSum_Mode : byte
    {
        None = 0,
        CRC8 = 1,
        CRC16 = 2,
        CRC32 = 3,
        XOR = 4
    }

}

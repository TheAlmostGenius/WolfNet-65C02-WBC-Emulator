using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulator
{
    public class ExitCodes
    {
        public static readonly int NO_ERROR = 0x00;

        public static readonly int LOAD_BIOS_FILE_ERROR = 0x01;
        public static readonly int BIOS_LOADPROGRAM_ERROR = 0x02;
        public static readonly int LOAD_ROM_FILE_ERROR = 0x03;
        public static readonly int ROM_LOADPROGRAM_ERROR = 0x04;
        public static readonly int LOAD_STATE_ERROR = 0x05;

        public static readonly int USER_ERROR = 0x06;
    }
}

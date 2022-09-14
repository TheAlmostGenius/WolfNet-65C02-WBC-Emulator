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
        public static readonly int BIOS_LOAD2MEM_ERROR = 0x01;
        public static readonly int BIOS_FILE_LOAD_ERROR = 0x02;
    }
}

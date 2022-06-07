using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram
{
    public class DEH : DevelopingExceptionHandler { }
    public class DevelopingExceptionHandler
    {
        public static void ThrowException(string message)
        {
            throw new Exception(message);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace U4Actividad1Roberto.Helpers
{
    public class CodeHelper
    {
        public static int GetCodigo()
        {
            Random random = new Random();
            int codigo1 = random.Next(1000, 9999);
            int codigo2 = random.Next(1000, 9999);
            return (codigo1 + codigo2);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class MathUtils
{
    public static int UniqueID
    {
        get
        {
            return BitConverter.ToInt32(Encoding.UTF8.GetBytes(System.Guid.NewGuid().ToString()), 0);
        }
    }
}


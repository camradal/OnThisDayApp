using System;
using Microsoft.Phone.Info;

namespace Utilities
{
    public static class LowMemoryHelper
    {
        public static bool IsLowMemDevice { get; set; }

        static LowMemoryHelper()
        {
            try
            {
                Int64 result = (Int64)DeviceExtendedProperties.GetValue("ApplicationWorkingSetLimit");
                if (result < 94371840L)
                    IsLowMemDevice = true;
                else
                    IsLowMemDevice = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                // Windows Phone OS update not installed, which indicates a 512-MB device. 
                IsLowMemDevice = false;
            }
        }
    }
}

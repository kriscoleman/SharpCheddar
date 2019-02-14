using System;

namespace SharpCheddar.EntityFramework
{
    public static class GuidComb
    {
        public static Guid Generate()
        {
            var guidBinary = new byte[16];
            Array.Copy(Guid.NewGuid().ToByteArray(), 0, guidBinary, 0, 8);
            Array.Copy(BitConverter.GetBytes(DateTime.Now.Ticks), 0, guidBinary, 8, 8);
            return new Guid(guidBinary);
        }
    }
}
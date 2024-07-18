namespace PixelFarm.Drawing.Internal;

[System.Security.SuppressUnmanagedCodeSecurity] //apply this to all native methods in this class
internal static class NativeMemMx
{
    //check this ....
    //for cross platform code

    //TODO: review here again***
    //this is platform specific ***

#if NETCORE3
//this need System.Runtime.CompilerServices.Unsafe
    public static unsafe void memset(byte* dest, byte c, int byteCount)
    {
        System.Runtime.CompilerServices.Unsafe.InitBlock((void*)(System.IntPtr)dest, c, (uint)byteCount);
    }
    public static unsafe void memcpy(byte* dest, byte* src, int byteCount)
    {
        System.Runtime.CompilerServices.Unsafe.CopyBlock((void*)(System.IntPtr)dest, (void*)(System.IntPtr)src, (uint)byteCount);
    }
#else

    [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe void memset(byte* dest, byte c, int byteCount);

    [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe void memcpy(byte* dest, byte* src, int byteCount);

#endif
}
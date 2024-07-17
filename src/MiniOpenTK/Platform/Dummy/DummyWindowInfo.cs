#if Level1

namespace OpenTK.Platform.Dummy;

public class DummyWindowInfo : IWindowInfo
{
    public void Dispose()
    {
    }

    public IntPtr Handle => IntPtr.Zero;
}

#endif
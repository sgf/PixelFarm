#if Level1

namespace OpenTK.Platform;

public interface IDisplayDeviceDriver
{
    bool TryChangeResolution(DisplayDevice device, DisplayResolution resolution);
    bool TryRestoreResolution(DisplayDevice device);
    DisplayDevice GetDisplay(DisplayIndex displayIndex);
}

#endif
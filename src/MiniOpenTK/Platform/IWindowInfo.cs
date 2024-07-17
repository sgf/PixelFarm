#if Level1


using System;

namespace OpenTK.Platform;

/// <summary>Describes an OS window.</summary>
public interface IWindowInfo : IDisposable
{
    /// <summary>
    /// Retrieves a platform-specific handle to this window.
    /// </summary>
    IntPtr Handle { get; }
}

#endif
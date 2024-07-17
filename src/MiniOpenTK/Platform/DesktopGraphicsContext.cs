#if Level1

using System.Diagnostics;
using OpenTK.Graphics;

namespace OpenTK.Platform;

// Provides the foundation for all desktop IGraphicsContext implementations.
public abstract class DesktopGraphicsContext : GraphicsContextBase
{
    public override void LoadAll()
    {
        Stopwatch time = Stopwatch.StartNew();

        //#if OPENGL
        //            new OpenTK.Graphics.OpenGL.GL().LoadEntryPoints();
        //            new OpenTK.Graphics.OpenGL4.GL().LoadEntryPoints();
        //#endif
        //#if GLES
        //new OpenTK.Graphics.ES11.GL().LoadEntryPoints();
        new OpenTK.Graphics.ES20.GL().LoadEntryPoints();
        new OpenTK.Graphics.ES30.GL().LoadEntryPoints();
        //#endif

        Debug.Print("Bindings loaded in {0} ms.", time.Elapsed.TotalMilliseconds);
    }
}


#endif
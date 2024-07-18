//BSD, 2014-present, WinterDev

//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007-2011
//
// Permission to copy, use, modify, sell and distribute this software
// is granted provided this copyright notice appears in all copies.
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------

using PixelFarm.CpuBlit;

namespace PixelFarm.Drawing
{
    public enum TargetBuffer
    {
        ColorBuffer,
        MaskBuffer
    }

    /// <summary>
    /// this class provides drawing method on specific drawboard,
    /// (0,0) is on left-lower corner for every implementaion
    /// </summary>
    public interface IPainter
    {
        //who implement this class
        //1. AggPainter
        //2. GdiPlusPainter
        //3. GLPainter

        public float OriginX { get; }
        public float OriginY { get; }

        public void SetOrigin(float ox, float oy);

        public PixelFarm.CpuBlit.VertexProcessing.ICoordTransformer CoordTransformer { get; set; }

        public RenderQuality RenderQuality { get; set; }

        public int Width { get; }
        public int Height { get; }
        public Rectangle ClipBox { get; set; }

        public void SetClipBox(int x1, int y1, int x2, int y2);

        /// <summary>
        /// we DO NOT store vxs
        /// </summary>
        /// <param name="vxs"></param>
        public void SetClipRgn(VertexStore vxs);

        public TargetBuffer TargetBuffer { get; set; }
        public FillingRule FillingRule { get; set; }

        public bool EnableMask { get; set; }

        //
        public double StrokeWidth { get; set; }

        public SmoothingMode SmoothingMode { get; set; }
        public bool UseLcdEffectSubPixelRendering { get; set; }
        public float FillOpacity { get; set; }
        public Color FillColor { get; set; }
        public Color StrokeColor { get; set; }

        public LineJoin LineJoin { get; set; }
        public LineCap LineCap { get; set; }
        public IDashGenerator LineDashGen { get; set; }
        //

        public Brush CurrentBrush { get; set; }
        public Pen CurrentPen { get; set; }

        //
        public void Clear(Color color);

        public RenderSurfaceOriginKind Orientation { get; set; }

        public void DrawLine(double x0, double y0, double x1, double y1);

        //
        public void DrawRect(double left, double top, double width, double height);

        public void FillRect(double left, double top, double width, double height);

        //
        public void FillEllipse(double left, double top, double width, double height);

        public void DrawEllipse(double left, double top, double width, double height);

        //

        /// <summary>
        /// draw image, not scale
        /// </summary>
        /// <param name="img"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public void DrawImage(Image img, double left, double top);

        public void DrawImage(Image img, double left, double top, int srcLeft, int srcTop, int srcW, int srcH);

        public void DrawImage(Image img);

        public void DrawImage(Image img, in AffineMat mat);

        public void DrawImage(Image img, double left, double top, CpuBlit.VertexProcessing.ICoordTransformer coordTx);

        public void ApplyFilter(IImageFilter imgFilter);

        ////////////////////////////////////////////////////////////////////////////
        //vertext store/snap/rendervx
        public void Fill(VertexStore vxs);

        public void Draw(VertexStore vxs);

        //---------------------------------------
        public void FillRegion(Region rgn);

        public void FillRegion(VertexStore vxs);

        public void DrawRegion(Region rgn);

        public void DrawRegion(VertexStore vxs);

        //---------------------------------------

        public RenderVx CreateRenderVx(VertexStore vxs);

        public void FillRenderVx(Brush brush, RenderVx renderVx);

        public void FillRenderVx(RenderVx renderVx);

        public void DrawRenderVx(RenderVx renderVx);

        public void Render(RenderVx renderVx);

        //////////////////////////////////////////////////////////////////////////////

        public RenderVxFormattedString CreateRenderVx(IFormattedGlyphPlanList formattedGlyphPlans);

        public RenderVxFormattedString CreateRenderVx(string textspan);

        public RenderVxFormattedString CreateRenderVx(char[] textspanBuff, int startAt, int len);

        //text,string
        //TODO: review text drawing funcs

        public RequestFont CurrentFont { get; set; }

        public void DrawString(
           string text,
           double x,
           double y);

        public void DrawString(RenderVxFormattedString renderVx, double x, double y);
    }

    public interface IDashGenerator
    {
    }

    public enum LineCap
    {
        Butt,
        Square,
        Round
    }

    public enum LineJoin
    {
        Miter,
        MiterRevert,
        Round,
        Bevel,
        MiterRound

        //TODO: implement svg arg join
    }

    public enum InnerJoin
    {
        Bevel,
        Miter,
        Jag,
        Round
    }
}
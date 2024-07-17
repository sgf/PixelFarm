﻿//MIT, 2016-present, WinterDev

using System;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.CpuBlit.FragmentProcessing;
using PixelFarm.CpuBlit.PixelProcessing;

namespace PixelFarm.CpuBlit
{

    partial class AggPainter
    {
        public enum LineRenderingTechnique
        {
            StrokeVxsGenerator,
            OutlineAARenderer,
        }
        //--------------------
        //pen 
        Stroke _stroke;
        Color _strokeColor;
        Pen _curPen;
        LineDashGenerator _lineDashGen;
        double _strokeW;
        int _ellipseGenNSteps = 20;
        //--------------------

        //low-level outline renderer
        LineRenderingTechnique _lineRenderingTech;
        Rasterization.Lines.LineProfileAnitAlias _lineProfileAA;
        Rasterization.Lines.OutlineAARasterizer _outlineRas; //low-level outline aa
        Rasterization.Lines.PreBuiltLineAAGammaTable _lineAAGamma = Rasterization.Lines.PreBuiltLineAAGammaTable.None;//default
        public Pen CurrentPen
        {
            get => _curPen;
            set => _curPen = value;
        }
        public Rasterization.Lines.PreBuiltLineAAGammaTable LineAAGammaTable
        {
            get => _lineAAGamma;
            set
            {
                _lineAAGamma = value;
                if (_lineProfileAA != null)
                {
                    _lineProfileAA.SetGamma(value);
                }
            }
        }
        public LineRenderingTechnique LineRenderingTech
        {
            get => _lineRenderingTech;
            set
            {
                if (value == LineRenderingTechnique.OutlineAARenderer)
                {
                    if (_lineProfileAA == null)
                    {
                        _lineProfileAA = new Rasterization.Lines.LineProfileAnitAlias(this.StrokeWidth, _lineAAGamma);

                        var blender = new PixelBlenderBGRA();

                        var outlineRenderer = new Rasterization.Lines.OutlineRenderer(
                            new ClipProxyImage(new SubBitmapBlender(_aggsx.DestBitmap, blender)), //Need ClipProxyImage
                            blender,
                            _lineProfileAA);
                        outlineRenderer.SetClipBox(0, 0, this.Width, this.Height);

                        //TODO: impl 'Pen'

                        _outlineRas = new Rasterization.Lines.OutlineAARasterizer(outlineRenderer);
                        _outlineRas.LineJoin = Rasterization.Lines.OutlineAARasterizer.OutlineJoin.Round;
                        _outlineRas.RoundCap = true;
                    }
                    else
                    {
                        if (_lineProfileAA.SubPixelWidth != StrokeWidth)
                        {
                            //update 
                            _lineProfileAA.SubPixelWidth = (float)StrokeWidth;
                        }
                    }
                }
                _lineRenderingTech = value;
            }
        }

        public Color StrokeColor
        {
            get => _strokeColor;
            set => _strokeColor = value;
        }
        public LineJoin LineJoin
        {
            get => _stroke.LineJoin;
            set => _stroke.LineJoin = value;
        }
        public LineCap LineCap
        {
            get => _stroke.LineCap;
            set => _stroke.LineCap = value;
        }

        public IDashGenerator LineDashGen
        {
            get => _lineDashGen;
            set => _lineDashGen = (LineDashGenerator)value;
        }


        /// <summary>
        /// draw line
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="color"></param>
        public void DrawLine(double x0, double y0, double x1, double y1)
        {
            //BitmapExt
            if (this.RenderQuality == RenderQuality.Fast)
            {
                //_bxt.DrawLine(
                //   (int)Math.Round(x1),
                //   (int)Math.Round(y1),
                //   (int)Math.Round(x2),
                //   (int)Math.Round(y2),
                //    _strokeColor.ToARGB()
                //   );

                //return;
            }

            //----------------------------------------------------------
            //Agg
            using (VxsTemp.Borrow(out var v1, out var v2))
            using (VectorToolBox.Borrow(v1, out PathWriter pw))
            {
                pw.Clear();
                if (_orientation == RenderSurfaceOriginKind.LeftBottom)
                {
                    //as original 
                    pw.MoveTo(x0, y0);
                    pw.LineTo(x1, y1);
                }
                else
                {
                    //left-top
                    int h = this.Height;
                    pw.MoveTo(x0, h - y0);
                    pw.LineTo(x1, h - y1);
                }
                //----------------------------------------------------------
                if (_lineRenderingTech == LineRenderingTechnique.StrokeVxsGenerator)
                {
                    _aggsx.Render(_stroke.MakeVxs(v1, v2), _strokeColor);

                }
                else
                {
                    _outlineRas.RenderVertexSnap(v1, _strokeColor);
                }
            }
        }

        public void DrawRenderVx(RenderVx renderVx)
        {
            AggRenderVx aggRenderVx = (AggRenderVx)renderVx;
            Draw(aggRenderVx._vxs);
        }
        public double StrokeWidth
        {
            get => _strokeW;
            set
            {
                if (value != _strokeW)
                {
                    //strokeW change
                    _strokeW = value;
                    if (_lineRenderingTech == LineRenderingTechnique.StrokeVxsGenerator)
                    {
                        _stroke.Width = value;
                    }
                    else
                    {
                        _lineProfileAA.SubPixelWidth = (float)value;
                    }
                }
            }
        }

        public void Draw(VertexStore vxs)
        {
            if (RenderQuality == RenderQuality.Fast)
            {

                //VertexCmd cmd;
                //int index = 0;
                //double lastMoveX = 0, lastMoveY = 0;
                //double lastX = 0, lastY = 0;
                //int stroke_color = _strokeColor.ToARGB();
                //while ((cmd = vxs.GetVertex(index++, out double x, out double y)) != VertexCmd.NoMore)
                //{
                //    switch (cmd)
                //    {
                //        case VertexCmd.MoveTo:
                //            lastX = lastMoveX = x;
                //            lastY = lastMoveY = y;

                //            break;
                //        case VertexCmd.LineTo:

                //            //need rounding
                //            _bxt.DrawLine((int)Math.Round(lastX), (int)Math.Round(lastY), (int)Math.Round(x), (int)Math.Round(y), stroke_color);

                //            lastX = x;
                //            lastY = y;

                //            break;
                //        case VertexCmd.Close:

                //            _bxt.DrawLine((int)Math.Round(lastX), (int)Math.Round(lastY), (int)Math.Round(lastMoveX), (int)Math.Round(lastMoveY), stroke_color);

                //            lastX = x;
                //            lastY = y;

                //            break;
                //    }
                //}

                //return;
            }
            if (_lineDashGen == null)
            {
                //no line dash

                if (LineRenderingTech == LineRenderingTechnique.StrokeVxsGenerator)
                {
                    using (Tools.BorrowVxs(out var v1))
                    {
                        _aggsx.Render(_stroke.MakeVxs(vxs, v1), _strokeColor);
                    }
                }
                else
                {
                    _outlineRas.RenderVertexSnap(vxs, _strokeColor);
                }
            }
            else
            {
                if (LineRenderingTech == LineRenderingTechnique.StrokeVxsGenerator)
                {

                    using (Tools.BorrowVxs(out var v1))
                    {

                        _lineDashGen.CreateDash(vxs, v1);

                        int n = v1.Count;
                        double px = 0, py = 0;

                        LineDashGenerator tmp = _lineDashGen;
                        _lineDashGen = null; //tmp turn dash gen off

                        for (int i = 0; i < n; ++i)
                        {
                            VertexCmd cmd = v1.GetVertex(i, out double x, out double y);
                            switch (cmd)
                            {
                                case VertexCmd.MoveTo:
                                    px = x;
                                    py = y;
                                    break;
                                case VertexCmd.LineTo:
                                    this.DrawLine(px, py, x, y);
                                    break;
                            }
                            px = x;
                            py = y;
                        }
                        _lineDashGen = tmp; //restore prev dash gen
                    }
                }
                else
                {
                    using (Tools.BorrowVxs(out var v1))
                    {

                        //TODO: check lineDash
                        //_lineDashGen.CreateDash(vxs, v1);
                        _outlineRas.RenderVertexSnap(v1, _strokeColor);
                    }
                }

            }
        }

        public void DrawRect(double left, double top, double width, double height)
        {

            //BitmapExt
            if (this.RenderQuality == RenderQuality.Fast)
            {

                //if (_orientation == RenderSurfaceOriginKind.LeftBottom)
                //{

                //    _bxt.DrawRectangle(
                //       (int)Math.Round(left),
                //       (int)Math.Round(top),
                //       (int)Math.Round(left + width),
                //       (int)Math.Round(top + height),

                //   ColorInt.FromArgb(_strokeColor.ToARGB()));
                //}
                //else
                //{
                //    //TODO: review here
                //    throw new NotSupportedException();
                //    //int canvasH = this.Height; 
                //    ////_simpleRectVxsGen.SetRect(left + 0.5, canvasH - (bottom + 0.5 + height), right - 0.5, canvasH - (top - 0.5 + height));
                //    //_bmpBuffer.DrawRectangle(
                //    //(int)Math.Round(left),
                //    //(int)Math.Round(top),
                //    //(int)Math.Round(left + width),
                //    //(int)Math.Round(top + height),
                //    //ColorInt.FromArgb(this.strokeColor.ToARGB()));
                //}
                //return; //exit
            }

            //----------------------------------------------------------
            //Agg


            using (Tools.BorrowRect(out var rectTool))
            {
                if (_orientation == RenderSurfaceOriginKind.LeftBottom)
                {
                    double right = left + width;
                    double bottom = top + height;
                    rectTool.SetRect(left + 0.5, bottom + 0.5, right - 0.5, top - 0.5);
                }
                else
                {
                    double right = left + width;
                    double bottom = top - height;
                    int canvasH = this.Height;
                    //_simpleRectVxsGen.SetRect(left + 0.5, canvasH - (bottom + 0.5), right - 0.5, canvasH - (top - 0.5));
                    rectTool.SetRect(left + 0.5, canvasH - (bottom + 0.5 + height), right - 0.5, canvasH - (top - 0.5 + height));
                }

                if (LineRenderingTech == LineRenderingTechnique.StrokeVxsGenerator)
                {
                    using (Tools.BorrowVxs(out var v1, out var v2))
                    {
                        _aggsx.Render(_stroke.MakeVxs(rectTool.MakeVxs(v1), v2), _strokeColor);
                    }
                }
                else
                {
                    using (Tools.BorrowVxs(out var v1))
                    {
                        _outlineRas.RenderVertexSnap(rectTool.MakeVxs(v1), _strokeColor);
                    }
                }
            }
        }

        public void DrawEllipse(double left, double top, double width, double height)
        {

            double ox = (left + width / 2);
            double oy = (top + height / 2);
            if (_orientation == RenderSurfaceOriginKind.LeftTop)
            {
                //modified
                oy = this.Height - oy;
            }


            using (Tools.BorrowEllipse(out var ellipseTool))
            {
                ellipseTool.Set(ox,
                       oy,
                       width / 2,
                       height / 2,
                       _ellipseGenNSteps);
                if (LineRenderingTech == LineRenderingTechnique.StrokeVxsGenerator)
                {
                    using (Tools.BorrowVxs(out var v1, out var v2))
                    {
                        _aggsx.Render(_stroke.MakeVxs(ellipseTool.MakeVxs(v1), v2), _strokeColor);
                    }
                }
                else
                {
                    using (Tools.BorrowVxs(out var v1))
                    {
                        _outlineRas.RenderVertexSnap(ellipseTool.MakeVxs(v1), _strokeColor);
                    }
                }
            }

        }


    }
}

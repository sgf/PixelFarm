﻿//BSD, 2014-present, WinterDev
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------


namespace PixelFarm.Drawing
{ /// <summary>
  /// vertex command and flags
  /// </summary>
    public enum VertexCmd : byte
    {
        //---------------------------------
        //the order of these fields are significant!
        //---------------------------------
        //first lower 4 bits compact flags
        /// <summary>
        /// no more command
        /// </summary>
        NoMore = 0x00,
        //----------------------- 
        /// <summary>
        /// close current polygon
        /// </summary>
        Close = 0x02,
        //----------------------- 
        //start from move to 
        MoveTo = 0x04,
        LineTo = 0x05,
        /// <summary>
        /// control point for curve3
        /// </summary>
        C3 = 0x06, // 
        /// <summary>
        /// control point for curve4
        /// </summary>
        C4 = 0x07,
    }
    public enum EndVertexOrientation
    {
        Unknown, //0
        CCW,//1
        CW//2
    }

    public sealed class VertexStore
    {

        int _vertices_count;
        int _allocated_vertices_count;
        double[] _coord_xy;
        byte[] _cmds;

#if DEBUG
        public readonly bool dbugIsTrim;
        static int dbugTotal = 0;
        public readonly int dbugId = dbugGetNewId();
        public int dbugNote;
        static int dbugGetNewId()
        {
            return dbugTotal++;
        }
#endif
        public VertexStore()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("vxs_1_dbugId=" + dbugId);
#endif
            AllocIfRequired(2);
        }
        public static void SetSharedState(VertexStore vxs, bool isShared)
        {
            vxs.IsShared = isShared;
        }
        public bool IsShared { get; private set; }
        /// <summary>
        /// num of vertex
        /// </summary>
        public int Count => _vertices_count;
        //
        public VertexCmd GetLastCommand()
        {
            if (_vertices_count != 0)
            {
                return GetCommand(_vertices_count - 1);
            }

            return VertexCmd.NoMore;
        }
        public VertexCmd GetLastVertex(out double x, out double y)
        {
            if (_vertices_count != 0)
            {
                return GetVertex((int)(_vertices_count - 1), out x, out y);
            }

            x = 0;
            y = 0;
            return VertexCmd.NoMore;
        }

        public VertexCmd GetVertex(int index, out double x, out double y)
        {
            x = _coord_xy[index << 1];
            y = _coord_xy[(index << 1) + 1];
            return (VertexCmd)_cmds[index];
        }


        public void GetVertexXY(int index, out double x, out double y)
        {

            x = _coord_xy[index << 1];
            y = _coord_xy[(index << 1) + 1];
        }
        public VertexCmd GetCommand(int index)
        {
            return (VertexCmd)_cmds[index];
        }

        public void Clear()
        {
            //we clear only command part!
            //clear only latest
            //System.Array.Clear(m_cmds, 0, m_cmds.Length);
            System.Array.Clear(_cmds, 0, _vertices_count); //only latest 
            _vertices_count = 0;
        }
        public void ConfirmNoMore()
        {
            AddVertex(0, 0, VertexCmd.NoMore);
            _vertices_count--;//not count
        }
        public void AddVertex(double x, double y, VertexCmd cmd)
        {
#if DEBUG
            if (VertexStore.dbugCheckNANs(x, y))
            {

            }
#endif

            if (_vertices_count + 1 >= _allocated_vertices_count)
            {
                AllocIfRequired(_vertices_count + 1);
            }
            _coord_xy[_vertices_count << 1] = x;
            _coord_xy[(_vertices_count << 1) + 1] = y;
            _cmds[_vertices_count] = (byte)cmd;
            _vertices_count++;
        }

        internal void ReplaceVertex(int index, double x, double y)
        {
#if DEBUG
            _dbugIsChanged = true;
#endif
            _coord_xy[index << 1] = x;
            _coord_xy[(index << 1) + 1] = y;
        }
        internal void ReplaceCommand(int index, VertexCmd cmd)
        {
            _cmds[index] = (byte)cmd;
        }
        internal void SwapVertices(int v1, int v2)
        {
            double x_tmp = _coord_xy[v1 << 1];
            double y_tmp = _coord_xy[(v1 << 1) + 1];
            _coord_xy[v1 << 1] = _coord_xy[v2 << 1];//x
            _coord_xy[(v1 << 1) + 1] = _coord_xy[(v2 << 1) + 1];//y
            _coord_xy[v2 << 1] = x_tmp;
            _coord_xy[(v2 << 1) + 1] = y_tmp;
            byte cmd = _cmds[v1];
            _cmds[v1] = _cmds[v2];
            _cmds[v2] = cmd;
        }


#if DEBUG
        public override string ToString()
        {
            return _vertices_count.ToString();
        }

        public bool _dbugIsChanged;

        public static bool dbugCheckNANs(double x, double y) => double.IsNaN(x) || double.IsNaN(y);
#endif
        void AllocIfRequired(int indexToAdd)
        {
            if (indexToAdd < _allocated_vertices_count)
            {
                return;
            }

#if DEBUG
            int nrounds = 0;
#endif
            while (indexToAdd >= _allocated_vertices_count)
            {
#if DEBUG

                if (nrounds > 0)
                {
                }
                nrounds++;
#endif

                //newsize is LARGER than original  ****
                int newSize = ((indexToAdd + 257) / 256) * 256; //calculate new size in single round
                //int newSize = m_allocated_vertices + 256; //original
                //-------------------------------------- 
                double[] new_xy = new double[newSize << 1];
                byte[] newCmd = new byte[newSize];

                if (_coord_xy != null)
                {
                    //copy old buffer to new buffer 
                    int actualLen = _vertices_count << 1;
                    //-----------------------------
                    //TODO: review faster copy
                    //----------------------------- 
                    unsafe
                    {
#if COSMOS
                        System.Array.Copy(m_coord_xy, new_xy, actualLen);
                        System.Array.Copy(m_cmds, newCmd, m_num_vertices);
#else
                        //unsafed version?
                        fixed (double* srcH = &_coord_xy[0])
                        {
                            System.Runtime.InteropServices.Marshal.Copy(
                                (System.IntPtr)srcH,
                                new_xy, //dest
                                0,
                                actualLen);
                        }
                        fixed (byte* srcH = &_cmds[0])
                        {
                            System.Runtime.InteropServices.Marshal.Copy(
                                (System.IntPtr)srcH,
                                newCmd, //dest
                                0,
                                _vertices_count);
                        }
#endif

                    }
                }
                _coord_xy = new_xy;
                _cmds = newCmd;
                _allocated_vertices_count = newSize;
            }
        }

        /// <summary>
        /// copy data from 'another' append to this vxs,we DO NOT store 'another' vxs inside this
        /// </summary>
        /// <param name="another"></param>
        public void AppendVertexStore(VertexStore another)
        {

            //append data from another
            if (_allocated_vertices_count < _vertices_count + another._vertices_count)
            {
                //alloc a new one
                int new_alloc = _vertices_count + another._vertices_count;


                //_vertices_count = new_alloc;//new 

                var new_coord_xy = new double[(new_alloc + 1) << 1];//*2
                var new_cmds = new byte[(new_alloc + 1)];
                //copy org

                //A.1
                System.Array.Copy(
                     _coord_xy,//src_arr
                     0, //src_index
                     new_coord_xy,//dst
                     0, //dst index
                     _vertices_count << 1); //len


                //A.2
                System.Array.Copy(
                    _cmds,//src_arr
                    0,//src_index
                    new_cmds,//dst
                    0, //dst index
                    _vertices_count);//len


                //B.1
                System.Array.Copy(
                   another._coord_xy,//src
                   0, //srcIndex
                   new_coord_xy, //dst
                   _vertices_count << 1,//dst index
                   another._vertices_count << 1);
                //**             

                //B.2 
                System.Array.Copy(
                        another._cmds,//src
                        0,//srcIndex
                        new_cmds, //dst
                        _vertices_count, //dst index
                        another._vertices_count);

                _coord_xy = new_coord_xy;
                _cmds = new_cmds;
                _vertices_count += another._vertices_count;
                _allocated_vertices_count = new_alloc;
            }
            else
            {
                System.Array.Copy(
                  another._coord_xy,//src
                  0,//src index
                  _coord_xy,//dst
                  _vertices_count << 1,//*2 //
                  another._vertices_count << 1);

                //B.2 
                System.Array.Copy(
                        another._cmds,//src
                        0,//src index
                       _cmds,//dst
                       _vertices_count, //dst index
                      another._vertices_count);

                _vertices_count += another._vertices_count;

            }
        }
        private VertexStore(VertexStore src, bool trim)
        {
            //for copy from src to this instance
#if DEBUG
            System.Diagnostics.Debug.WriteLine("vxs_3_dbugId=" + dbugId);
#endif

            _vertices_count = src._vertices_count;

            if (trim)
            {
#if DEBUG
                dbugIsTrim = true;
#endif
                int coord_len = _vertices_count; //+1 for no more cmd
                int cmds_len = _vertices_count; //+1 for no more cmd

                _coord_xy = new double[(coord_len + 1) << 1];//*2
                _cmds = new byte[(cmds_len + 1)];

                System.Array.Copy(
                     src._coord_xy,
                     0,
                     _coord_xy,
                     0,
                     coord_len << 1); //*2

                System.Array.Copy(
                     src._cmds,
                     0,
                     _cmds,
                     0,
                     cmds_len);

                _allocated_vertices_count = _cmds.Length;
            }
            else
            {
                int coord_len = src._coord_xy.Length;
                int cmds_len = src._cmds.Length;

                _coord_xy = new double[(coord_len + 1) << 1];
                _cmds = new byte[(cmds_len + 1)]; //TODO: review here again***

                System.Array.Copy(
                     src._coord_xy,
                     0,
                     _coord_xy,
                     0,
                     coord_len);

                System.Array.Copy(
                     src._cmds,
                     0,
                     _cmds,
                     0,
                     cmds_len);
                _allocated_vertices_count = _cmds.Length;
            }

        }
        private VertexStore(VertexStore src, PixelFarm.CpuBlit.VertexProcessing.ICoordTransformer tx)
        {
            //for copy from src to this instance
#if DEBUG
            System.Diagnostics.Debug.WriteLine("vxs_4_dbugId=" + dbugId);
#endif
            _allocated_vertices_count = src._allocated_vertices_count;
            _vertices_count = src._vertices_count;
            //
            //
#if DEBUG
            dbugIsTrim = true;
#endif
            //
            int coord_len = _vertices_count; //+1 for no more cmd
            int cmds_len = _vertices_count; //+1 for no more cmd

            _coord_xy = new double[(coord_len + 1) << 1];//*2
            _cmds = new byte[(cmds_len + 1)];


            System.Array.Copy(
                 src._coord_xy,
                 0,
                 _coord_xy,
                 0,
                 coord_len << 1); //*2

            System.Array.Copy(
                 src._cmds,
                 0,
                 _cmds,
                 0,
                 cmds_len);

            //-------------------------
            int coord_count = coord_len;
            int a = 0;
            for (int n = 0; n < coord_count; ++n)
            {
                tx.Transform(ref _coord_xy[a++], ref _coord_xy[a++]);
            }
        }
        private VertexStore(VertexStore src, in PixelFarm.CpuBlit.AffineMat tx)
        {
            //for copy from src to this instance

            _allocated_vertices_count = src._allocated_vertices_count;
            _vertices_count = src._vertices_count;
            //
            //
#if DEBUG
            dbugIsTrim = true;
#endif
            //
            int coord_len = _vertices_count; //+1 for no more cmd
            int cmds_len = _vertices_count; //+1 for no more cmd

            _coord_xy = new double[(coord_len + 1) << 1];//*2
            _cmds = new byte[(cmds_len + 1)];


            System.Array.Copy(
                 src._coord_xy,
                 0,
                 _coord_xy,
                 0,
                 coord_len << 1); //*2

            System.Array.Copy(
                 src._cmds,
                 0,
                 _cmds,
                 0,
                 cmds_len);

            //-------------------------
            int coord_count = coord_len;
            int a = 0;
            for (int n = 0; n < coord_count; ++n)
            {
                tx.Transform(ref _coord_xy[a++], ref _coord_xy[a++]);
            }
        }

        /// <summary>
        /// copy from src to the new one
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static VertexStore CreateCopy(VertexStore src)
        {
            return new VertexStore(src, false);
        }
        /// <summary>
        /// trim to new vertex store
        /// </summary>
        /// <returns></returns>
        public VertexStore CreateTrim() => new VertexStore(this, true);

        public VertexStore CreateTrim(PixelFarm.CpuBlit.VertexProcessing.ICoordTransformer tx) => new VertexStore(this, tx);

        public VertexStore CreateTrim(in PixelFarm.CpuBlit.AffineMat tx) => new VertexStore(this, tx);

    }


    public static class VertexStoreExtensions
    {


        public static void AddC3To(this VertexStore vxs, double x1, double y1, double x2, double y2)
        {
            vxs.AddVertex(x1, y1, VertexCmd.C3);
            vxs.AddVertex(x2, y2, VertexCmd.LineTo);
        }
        public static void AddC4To(this VertexStore vxs, double x1, double y1, double x2, double y2, double x3, double y3)
        {
            vxs.AddVertex(x1, y1, VertexCmd.C4);
            vxs.AddVertex(x2, y2, VertexCmd.C4);
            vxs.AddVertex(x3, y3, VertexCmd.LineTo);
        }

        public static void AddMoveTo(this VertexStore vxs, double x0, double y0)
        {
            vxs.AddVertex(x0, y0, VertexCmd.MoveTo);
        }
        public static void AddLineTo(this VertexStore vxs, double x1, double y1)
        {
            vxs.AddVertex(x1, y1, VertexCmd.LineTo);
        }
        public static void AddCurve4To(this VertexStore vxs,
            double x1, double y1,
            double x2, double y2,
            double x3, double y3)
        {
            vxs.AddVertex(x1, y1, VertexCmd.C4);
            vxs.AddVertex(x2, y2, VertexCmd.C4);
            vxs.AddVertex(x3, y3, VertexCmd.LineTo);
        }
        public static void AddCurve3To(this VertexStore vxs,
           double x1, double y1,
           double x2, double y2)
        {
            vxs.AddVertex(x1, y1, VertexCmd.C3);
            vxs.AddVertex(x2, y2, VertexCmd.LineTo);
        }
        public static void AddCloseFigure(this VertexStore vxs)
        {
            vxs.AddVertex(0, 0, VertexCmd.Close);
        }
        public static void AddCloseFigure(this VertexStore vxs, double x, double y)
        {
            vxs.AddVertex(x, y, VertexCmd.Close);
        }
        public static void AddNoMore(this VertexStore vxs)
        {
            vxs.AddVertex(0, 0, VertexCmd.NoMore);
        }


    }

    public static class VertexHelper
    {
        public static bool IsVertextCommand(VertexCmd c)
        {
            // return c >= VertexCmd.MoveTo;
            return c > VertexCmd.NoMore;
        }
        public static bool IsEmpty(VertexCmd c)
        {
            return c == VertexCmd.NoMore;
        }
        public static bool IsMoveTo(VertexCmd c)
        {
            return c == VertexCmd.MoveTo;
        }
        public static bool IsCloseOrEnd(VertexCmd c)
        {
            //check only 2 lower bit
            //TODO: review here
            return ((int)c & 0x3) >= (int)VertexCmd.Close;
        }

        public static bool IsNextPoly(VertexCmd c)
        {
            //?
            return c <= VertexCmd.MoveTo;
        }

        //internal static void ShortenPath(VertexDistanceList vertexDistanceList, double s, bool closed)
        //{
        //    if (s > 0.0 && vertexDistanceList.Count > 1)
        //    {
        //        double d;
        //        int n = (int)(vertexDistanceList.Count - 2);
        //        while (n != 0)
        //        {
        //            d = vertexDistanceList[n].dist;
        //            if (d > s) break;
        //            vertexDistanceList.RemoveLast();
        //            s -= d;
        //            --n;
        //        }
        //        if (vertexDistanceList.Count < 2)
        //        {
        //            vertexDistanceList.Clear();
        //        }
        //        else
        //        {
        //            n = (int)vertexDistanceList.Count - 1;
        //            VertexDistance prev = vertexDistanceList[n - 1];
        //            VertexDistance last = vertexDistanceList[n];
        //            d = (prev.dist - s) / prev.dist;
        //            double x = prev.x + (last.x - prev.x) * d;
        //            double y = prev.y + (last.y - prev.y) * d;
        //            last = new VertexDistance(x, y);
        //            if (prev.IsEqual(last))
        //            {
        //                vertexDistanceList.RemoveLast();
        //            }
        //            vertexDistanceList.Close(closed);
        //        }
        //    }
        //}
        public static void ArrangeOrientationsAll(VertexStore myvxs, bool closewise)
        {
            int start = 0;
            while (start < myvxs.Count)
            {
                start = ArrangeOrientations(myvxs, start, closewise);
            }
        }
        //---------------------------------------------------------------- 
        // Arrange the orientation of a polygon, all polygons in a path, 
        // or in all paths. After calling arrange_orientations() or 
        // arrange_orientations_all_paths(), all the polygons will have 
        // the same orientation, i.e. path_flags_cw or path_flags_ccw
        //--------------------------------------------------------------------
        static int ArrangePolygonOrientation(VertexStore myvxs, int start, bool clockwise)
        {
            //if (orientation == ShapePath.FlagsAndCommand.FlagNone) return start;

            // Skip all non-vertices at the beginning
            //ShapePath.FlagsAndCommand orientFlags = clockwise ? ShapePath.FlagsAndCommand.FlagCW : ShapePath.FlagsAndCommand.FlagCCW;

            int vcount = myvxs.Count;
            while (start < vcount &&
                  !VertexHelper.IsVertextCommand(myvxs.GetCommand(start)))
            {
                ++start;
            }

            // Skip all insignificant move_to
            while (start + 1 < vcount &&
                  VertexHelper.IsMoveTo(myvxs.GetCommand(start)) &&
                  VertexHelper.IsMoveTo(myvxs.GetCommand(start + 1)))
            {
                ++start;
            }

            // Find the last vertex
            int end = start + 1;
            while (end < vcount && !VertexHelper.IsNextPoly(myvxs.GetCommand(end)))
            {
                ++end;
            }
            if (end - start > 2)
            {
                bool isCW;
                if ((isCW = IsCW(myvxs, start, end)) != clockwise)
                {
                    // Invert polygon, set orientation flag, and skip all end_poly
                    InvertPolygon(myvxs, start, end);
                    VertexCmd flags;
                    int myvxs_count = myvxs.Count;
                    var orientFlags = isCW ? (int)EndVertexOrientation.CW : (int)EndVertexOrientation.CCW;
                    while (end < myvxs_count &&
                          VertexHelper.IsCloseOrEnd(flags = myvxs.GetCommand(end)))
                    {
                        //TODO: review hhere
                        myvxs.ReplaceVertex(end++, orientFlags, 0);
                        //myvxs.ReplaceCommand(end++, flags | orientFlags);// Path.set_orientation(cmd, orientation));
                    }
                }
            }
            return end;
        }

        static int ArrangeOrientations(VertexStore myvxs, int start, bool closewise)
        {
            while (start < myvxs.Count)
            {
                start = ArrangePolygonOrientation(myvxs, start, closewise);
                if (VertexHelper.IsEmpty(myvxs.GetCommand(start)))
                {
                    ++start;
                    break;
                }
            }

            return start;
        }
        static bool IsCW(VertexStore myvxs, int start, int end)
        {
            // Calculate signed area (double area to be exact)
            //---------------------
            int np = end - start;
            double area = 0.0;
            int i;
            for (i = 0; i < np; i++)
            {
                double x1, y1, x2, y2;
                myvxs.GetVertexXY(start + i, out x1, out y1);
                myvxs.GetVertexXY(start + (i + 1) % np, out x2, out y2);
                area += x1 * y2 - y1 * x2;
            }
            return (area < 0.0);
            //return (area < 0.0) ? ShapePath.FlagsAndCommand.FlagCW : ShapePath.FlagsAndCommand.FlagCCW;
        }
        //--------------------------------------------------------------------
        public static void InvertPolygon(VertexStore myvxs, int start)
        {
            // Skip all non-vertices at the beginning
            int vcount = myvxs.Count;
            while (start < vcount &&
                  !VertexHelper.IsVertextCommand(myvxs.GetCommand(start)))
            { ++start; }

            // Skip all insignificant move_to
            while (start + 1 < vcount &&
                  VertexHelper.IsMoveTo(myvxs.GetCommand(start)) &&
                  VertexHelper.IsMoveTo(myvxs.GetCommand(start + 1)))
            { ++start; }

            // Find the last vertex
            int end = start + 1;
            while (end < vcount && !VertexHelper.IsNextPoly(myvxs.GetCommand(end))) { ++end; }

            InvertPolygon(myvxs, start, end);
        }



        static void InvertPolygon(VertexStore myvxs, int start, int end)
        {
            int i;
            VertexCmd tmp_PathAndFlags = myvxs.GetCommand(start);
            --end; // Make "end" inclusive 
            // Shift all commands to one position
            for (i = start; i < end; i++)
            {
                myvxs.ReplaceCommand(i, myvxs.GetCommand(i + 1));
            }
            // Assign starting command to the ending command
            myvxs.ReplaceCommand(end, tmp_PathAndFlags);

            // Reverse the polygon
            while (end > start)
            {
                myvxs.SwapVertices(start++, end--);
            }
        }
        public static void FlipX(VertexStore vxs, double x1, double x2)
        {
            int i;
            double x, y;
            int count = vxs.Count;
            for (i = 0; i < count; ++i)
            {
                VertexCmd flags = vxs.GetVertex(i, out x, out y);
                if (VertexHelper.IsVertextCommand(flags))
                {
                    vxs.ReplaceVertex(i, x2 - x + x1, y);
                }
            }
        }

        public static void FlipY(VertexStore vxs, double y1, double y2)
        {
            int i;
            double x, y;
            int count = vxs.Count;
            for (i = 0; i < count; ++i)
            {
                VertexCmd flags = vxs.GetVertex(i, out x, out y);
                if (VertexHelper.IsVertextCommand(flags))
                {
                    vxs.ReplaceVertex(i, x, y2 - y + y1);
                }
            }
        }
    }




}

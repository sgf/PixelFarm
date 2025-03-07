﻿//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2009 the Open Toolkit library, except where noted.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//

using System;

//using OpenTK.Graphics;
//using OpenTK.Platform;

namespace OpenTK
{
    internal class WinGLControl : IGLControl
    {
        public WinGLControl()
        {

        }
        public bool IsIdle => throw new NotImplementedException();
    }
}

/*
namespace OpenTK
{
    internal class WinGLControl : IGLControl
    {
        struct MSG
        {
            public IntPtr HWnd;
            public uint Message;
            public IntPtr WParam;
            public IntPtr LParam;
            public uint Time;
            public POINT Point;
            //internal object RefObject;

            public override string ToString()
            {
                return String.Format("msg=0x{0:x} ({1}) hwnd=0x{2:x} wparam=0x{3:x} lparam=0x{4:x} pt=0x{5:x}", (int)Message, Message.ToString(), HWnd.ToInt32(), WParam.ToInt32(), LParam.ToInt32(), Point);
            }
        }

        struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public System.Drawing.Point ToPoint()
            {
                return new System.Drawing.Point(X, Y);
            }

            public override string ToString()
            {
                return "Point {" + X.ToString() + ", " + Y.ToString() + ")";
            }
        }

        [System.Security.SuppressUnmanagedCodeSecurity]
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool PeekMessage(ref MSG msg, IntPtr hWnd, int messageFilterMin, int messageFilterMax, int flags);

        MSG _msg = new MSG();
        GraphicsMode _mode;

        public WinGLControl(GraphicsMode mode, IntPtr controlHandle)
        {
            _mode = mode;
            WindowInfo = Utilities.CreateWindowsWindowInfo(controlHandle);
        }

        public IGraphicsContext CreateContext(int major, int minor, GraphicsContextFlags flags)
        {
            return new GraphicsContext(_mode, WindowInfo, major, minor, flags);
        }

        public bool IsIdle => !PeekMessage(ref _msg, IntPtr.Zero, 0, 0, 0);

        public IWindowInfo WindowInfo { get; }
    }
}
*/

﻿//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2013 the Open Toolkit library.
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
#if !MINIMAL
using System.Drawing;
#endif
using System.Runtime.InteropServices;


namespace OpenTK.Graphics.ES20
{
    /// <summary>
    /// Provides access to OpenGL ES 2.0 methods.
    /// </summary>
    public sealed partial class GL : GraphicsBindingsBase
    {
#if IPHONE
        private const string Library = "/System/Library/Frameworks/OpenGLES.framework/OpenGLES";
#else
        //private const string Library = "libGLESv2.dll";
#endif
        private static readonly object sync_root = new object();
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public GL()
        {

        }
        public override void LoadEntryPoints()
        {
            GLDelInit.LoadAll();
        }
        /// <summary>
        /// Returns a synchronization token unique for the GL class.
        /// </summary>
        protected override object SyncRoot
        {
            get { return sync_root; }
        }

        //-----------------------------------------------------------------
        //
        public static int GenBuffer()
        {
            GL.GenBuffers(1, out int newBufferId);
            return newBufferId;
        }
        public static void DeleteBuffer(int bufferId)
        {
            unsafe
            {
                int* arr = stackalloc int[1];
                arr[0] = bufferId;
                GL.DeleteBuffers(1, arr);
            }
        }

        public static void BufferData(BufferTarget target, int byteLen, float[] points, BufferUsageHint usageHint)
        {
            unsafe
            {
                fixed (float* h = points)
                {
                    GL.BufferData(target, (IntPtr)byteLen, (IntPtr)h, usageHint);
                }
            }
        }

        //-----------------------------------------------------------------
#pragma warning disable 3019
#pragma warning disable 1591
#pragma warning disable 1572
#pragma warning disable 1573

        // Note: Mono 1.9.1 truncates StringBuilder results (for 'out string' parameters).
        // We work around this issue by doubling the StringBuilder capacity.

        public static void ClearColor(Color color)
        {
            GL.ClearColor(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
        }

        public static void ClearColor(Color4 color)
        {
            GL.ClearColor(color.R, color.G, color.B, color.A);
        }

        public static void BlendColor(Color color)
        {
            GL.BlendColor(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
        }

        public static void BlendColor(Color4 color)
        {
            GL.BlendColor(color.R, color.G, color.B, color.A);
        }

        [CLSCompliant(false)]
        public static void Uniform2(int location, ref Vector2 vector)
        {
            GL.Uniform2(location, vector.X, vector.Y);
        }

        [CLSCompliant(false)]
        public static void Uniform3(int location, ref Vector3 vector)
        {
            GL.Uniform3(location, vector.X, vector.Y, vector.Z);
        }

        [CLSCompliant(false)]
        public static void Uniform4(int location, ref Vector4 vector)
        {
            GL.Uniform4(location, vector.X, vector.Y, vector.Z, vector.W);
        }

        public static void Uniform2(int location, Vector2 vector)
        {
            GL.Uniform2(location, vector.X, vector.Y);
        }

        public static void Uniform3(int location, Vector3 vector)
        {
            GL.Uniform3(location, vector.X, vector.Y, vector.Z);
        }

        public static void Uniform4(int location, Vector4 vector)
        {
            GL.Uniform4(location, vector.X, vector.Y, vector.Z, vector.W);
        }

        public static void Uniform4(int location, Color4 color)
        {
            GL.Uniform4(location, color.R, color.G, color.B, color.A);
        }

        public static void Uniform4(int location, Quaternion quaternion)
        {
            GL.Uniform4(location, quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }

        public static void UniformMatrix2(int location, bool transpose, ref Matrix2 matrix)
        {
            unsafe
            {
                fixed (float* matrix_ptr = &matrix.Row0.X)
                {
                    GL.UniformMatrix2(location, 1, transpose, matrix_ptr);
                }
            }
        }

        public static void UniformMatrix3(int location, bool transpose, ref Matrix3 matrix)
        {
            unsafe
            {
                fixed (float* matrix_ptr = &matrix.Row0.X)
                {
                    GL.UniformMatrix3(location, 1, transpose, matrix_ptr);
                }
            }
        }

        public static void UniformMatrix4(int location, bool transpose, ref Matrix4 matrix)
        {
            unsafe
            {
                fixed (float* matrix_ptr = &matrix.Row0.X)
                {
                    GL.UniformMatrix4(location, 1, transpose, matrix_ptr);
                }
            }
        }


        public static string GetActiveAttrib(int program, int index, out int size, out ActiveAttribType type)
        {
            int length;
            GetProgram(program, ES20.GetProgramParameterName.ActiveAttributeMaxLength, out length);
            string str;

            GetActiveAttrib(program, index, length == 0 ? 1 : length * 2, out length, out size, out type, out str);
            return str;
        }

        public static string GetActiveUniform(int program, int uniformIndex, out int size, out ActiveUniformType type)
        {
            int length;
            GetProgram(program, ES20.GetProgramParameterName.ActiveUniformMaxLength, out length);

            string str;
            GetActiveUniform(program, uniformIndex, length == 0 ? 1 : length, out length, out size, out type, out str);
            return str;
        }

        public static void ShaderSource(Int32 shader, System.String @string)
        {
            unsafe
            {
                int length = @string.Length;
                GL.ShaderSource((UInt32)shader, 1, new string[] { @string }, &length);
            }
        }

        public static string GetShaderInfoLog(Int32 shader)
        {
            string info;
            GetShaderInfoLog(shader, out info);
            return info;
        }

        public static void GetShaderInfoLog(Int32 shader, out string info)
        {
            unsafe
            {
                int length;
                GL.GetShader(shader, ShaderParameter.InfoLogLength, out length);
                if (length == 0)
                {
                    info = String.Empty;
                    return;
                }
                GL.GetShaderInfoLog((UInt32)shader, length, &length, out info);
            }
        }

        public static string GetProgramInfoLog(Int32 program)
        {
            string info;
            GetProgramInfoLog(program, out info);
            return info;
        }

        public static void GetProgramInfoLog(Int32 program, out string info)
        {
            unsafe
            {
                int length;
                GL.GetProgram(program, ES20.GetProgramParameterName.InfoLogLength, out length); if (length == 0)
                {
                    info = String.Empty;
                    return;
                }
                GL.GetProgramInfoLog((UInt32)program, length * 2, &length, out info);
            }
        }

        [CLSCompliant(false)]
        public static void VertexAttrib2(Int32 index, ref Vector2 v)
        {
            GL.VertexAttrib2(index, v.X, v.Y);
        }

        [CLSCompliant(false)]
        public static void VertexAttrib3(Int32 index, ref Vector3 v)
        {
            GL.VertexAttrib3(index, v.X, v.Y, v.Z);
        }

        [CLSCompliant(false)]
        public static void VertexAttrib4(Int32 index, ref Vector4 v)
        {
            GL.VertexAttrib4(index, v.X, v.Y, v.Z, v.W);
        }

        public static void VertexAttrib2(Int32 index, Vector2 v)
        {
            GL.VertexAttrib2(index, v.X, v.Y);
        }

        public static void VertexAttrib3(Int32 index, Vector3 v)
        {
            GL.VertexAttrib3(index, v.X, v.Y, v.Z);
        }

        public static void VertexAttrib4(Int32 index, Vector4 v)
        {
            GL.VertexAttrib4(index, v.X, v.Y, v.Z, v.W);
        }

        public static void VertexAttribPointer(int index, int size, VertexAttribPointerType type, bool normalized, int stride, int offset)
        {
            VertexAttribPointer(index, size, type, normalized, stride, (IntPtr)offset);
        }


        public static void VertexAttribPointer(uint index, int size, VertexAttribPointerType type, bool normalized, int stride, int offset)
        {
            VertexAttribPointer(index, size, type, normalized, stride, (IntPtr)offset);
        }


        public static void GetFloat(GetPName pname, out Vector2 vector)
        {
            unsafe
            {
                fixed (Vector2* ptr = &vector)
                {
                    GetFloat(pname, (float*)ptr);
                }
            }
        }

        public static void GetFloat(GetPName pname, out Vector3 vector)
        {
            unsafe
            {
                fixed (Vector3* ptr = &vector)
                {
                    GetFloat(pname, (float*)ptr);
                }
            }
        }

        public static void GetFloat(GetPName pname, out Vector4 vector)
        {
            unsafe
            {
                fixed (Vector4* ptr = &vector)
                {
                    GetFloat(pname, (float*)ptr);
                }
            }
        }

        public static void GetFloat(GetPName pname, out Matrix4 matrix)
        {
            unsafe
            {
                fixed (Matrix4* ptr = &matrix)
                {
                    GetFloat(pname, (float*)ptr);
                }
            }
        }

        //public static void Viewport(Size size)
        //{
        //    GL.Viewport(0, 0, size.Width, size.Height);
        //}

        //public static void Viewport(Point location, Size size)
        //{
        //    GL.Viewport(location.X, location.Y, size.Width, size.Height);
        //}

        //public static void Viewport(Rectangle rectangle)
        //{
        //    GL.Viewport(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        //}
         

        //#if MINIMAL
        //        public static void Viewport(OpenTK.Point location, OpenTK.Size size)
        //        {
        //            GL.Viewport(location.X, location.Y, size.Width, size.Height);
        //        }

        //        public static void Viewport(OpenTK.Rectangle rectangle)
        //        {
        //            GL.Viewport(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        //        }
        //#endif

#pragma warning restore 3019
#pragma warning restore 1591
#pragma warning restore 1572
#pragma warning restore 1573
    }

#pragma warning disable 1574 // XML comment cref attribute could not be resolved, compiler bug in Mono 3.4.0

    /// <summary>
    /// Defines the signature of a debug callback for
    /// <see cref="GL.DebugMessageCallback"/>.
    /// </summary>
    /// <param name="source">The <see cref="DebugSource"/> for this debug message.</param>
    /// <param name="type">The <see cref="DebugType"/> for this debug message.</param>
    /// <param name="id">The id of this debug message.</param>
    /// <param name="severity">The <see cref="DebugSeverity"/> for this debug message.</param>
    /// <param name="length">The length of this debug message.</param>
    /// <param name="message">A pointer to a null-terminated ASCII C string, representing the content of this debug message.</param>
    /// <param name="userParam">A pointer to a user-specified parameter.</param>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void DebugProc(
        DebugSource source, DebugType type, int id,
        DebugSeverity severity, int length, IntPtr message,
        IntPtr userParam);

    /// <summary>
    /// Defines the signature of a debug callback for
    /// <see cref="GL.Khr.DebugMessageCallback"/>.
    /// </summary>
    /// <param name="source">The <see cref="DebugSource"/> for this debug message.</param>
    /// <param name="type">The <see cref="DebugType"/> for this debug message.</param>
    /// <param name="id">The id of this debug message.</param>
    /// <param name="severity">The <see cref="DebugSeverity"/> for this debug message.</param>
    /// <param name="length">The length of this debug message.</param>
    /// <param name="message">A pointer to a null-terminated ASCII C string, representing the content of this debug message.</param>
    /// <param name="userParam">A pointer to a user-specified parameter.</param>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void DebugProcKhr(
        DebugSource source, DebugType type, int id,
        DebugSeverity severity, int length, IntPtr message,
        IntPtr userParam);

#pragma warning restore 1574 // XML comment cref attribute could not be resolved, compiler bug in Mono 3.4.0



    partial class GL
    {
        public static void DrawArrays(BeginMode beginMode, int first, int count)
        {
            GL.DrawArrays((PrimitiveType)beginMode, first, count);
        }
        public static void DrawElements(BeginMode beginMode, int nelements, DrawElementsType type, int offset)
        {
            GL.DrawElements((PrimitiveType)beginMode, nelements, type, (IntPtr)offset);
        }
        public static int GenTexture()
        {
            GL.GenTextures(1, out int textureId);
            return textureId;
        }
        public static void DeleteTexture(int textureId)
        {
            unsafe
            {
                int* arr = stackalloc int[1];
                arr[0] = textureId;
                GL.DeleteTextures(1, arr);
            }
        }

        //-------------
        public static void VertexAttribPointer(int location,
            int size, VertexAttribPointerType
            type, bool normalize,
            int byteCount, byte[] vertices)
        {
            unsafe
            {
                fixed (byte* v_ptr = vertices)
                {
                    GL.VertexAttribPointer(location,
                         size,
                         type,
                         normalize,
                         byteCount, //total size
                        (IntPtr)v_ptr);
                }
            }
        }
        public static void VertexAttribPointer<T>(uint index, int size, VertexAttribPointerType type, bool normalized, int stride, T[] arr)
            where T : struct
        {
            System.Runtime.InteropServices.GCHandle pAddress_ptr = System.Runtime.InteropServices.GCHandle.Alloc(arr, System.Runtime.InteropServices.GCHandleType.Pinned);
            try
            {
                GL.VertexAttribPointer(index, size, type, normalized, stride, (IntPtr)pAddress_ptr.AddrOfPinnedObject());
            }
            finally
            {
                pAddress_ptr.Free();
            }
        }
        public static void VertexAttribPointer<T>(int index, int size, VertexAttribPointerType type, bool normalized, int stride, T[] arr)
            where T : struct
        {
            System.Runtime.InteropServices.GCHandle pAddress_ptr = System.Runtime.InteropServices.GCHandle.Alloc(arr, System.Runtime.InteropServices.GCHandleType.Pinned);
            try
            {
                GL.VertexAttribPointer(index, size, type, normalized, stride, (IntPtr)pAddress_ptr.AddrOfPinnedObject());
            }
            finally
            {
                pAddress_ptr.Free();
            }
        }
        public static void VertexAttribPointer(Int32 index, Int32 size,
            OpenTK.Graphics.ES20.VertexAttribPointerType type,
            bool normalized,
            Int32 stride,
            float[] arr)
        {
            unsafe
            {
                fixed (float* v_ptr = arr)
                {
                    GL.VertexAttribPointer(index,
                         size,
                         type,
                         normalized,
                         stride, //total size
                         (IntPtr)v_ptr);
                }
            }
        }
        public static void DrawElements(OpenTK.Graphics.ES20.PrimitiveType mode, Int32 count, OpenTK.Graphics.ES20.DrawElementsType type, byte[] indices)
        {
            unsafe
            {
                fixed (byte* v_ptr = indices)
                {
                    GL.DrawElements(mode, count, type, (IntPtr)v_ptr);
                }
            }
        }
        public static void DrawElements(BeginMode mode, int size, DrawElementsType elemType, ushort[] indices)
        {
            unsafe
            {
                fixed (ushort* v_ptr = indices)
                {
                    GL.DrawElements((PrimitiveType)mode, size, elemType, (IntPtr)v_ptr);
                }
            }
        }
        public static void TexImage2D(TextureTarget textureTarget, int level, PixelInternalFormat format,
            int width, int height, int b, PixelFormat pixelFormat, PixelType pxtype, byte[] buffer)
        {
            unsafe
            {
                fixed (byte* buffer_ptr = buffer)
                {
                    GL.TexImage2D(
                       (TextureTarget2d)textureTarget,
                       level,
                       (TextureComponentCount)format,
                       width, height, b, pixelFormat, pxtype,
                       (System.IntPtr)buffer_ptr);
                }
            }
        }
        public static void TexImage2D(TextureTarget textureTarget, int level, PixelInternalFormat format,
           int width, int height, int b, PixelFormat pixelFormat, PixelType pxtype, IntPtr buffer_ptr)
        {

            GL.TexImage2D(
               (TextureTarget2d)textureTarget,
               level,
               (TextureComponentCount)format,
               width, height, b, pixelFormat, pxtype,
               buffer_ptr);

        }
        public static void TexSubImage2D(TextureTarget textureTarget, int level,
            int x, int y,
            int width, int height, PixelFormat pixelFormat, PixelType pxtype, IntPtr buffer_ptr)
        {
            GL.TexSubImage2D((TextureTarget2d)textureTarget, 0,
                      x, y, width, height,
                      PixelFormat.Rgba, // 
                      PixelType.UnsignedByte, buffer_ptr);

        }
        public static void BufferData(OpenTK.Graphics.ES20.BufferTarget target,
            IntPtr size, [CountAttribute(Parameter = "size")] IntPtr data,
           BufferUsage usage)
        {
            GL.BufferData(target,
                size, //size in byte
                data,
                (BufferUsageHint)usage);   //this version we use static draw 
        }
        public static void FramebufferTexture2D(OpenTK.Graphics.ES20.FramebufferTarget target, FramebufferSlot attachment, OpenTK.Graphics.ES20.TextureTarget textarget, Int32 texture, Int32 level)
        {
            GL.FramebufferTexture2D(target, (FramebufferAttachment)attachment, (TextureTarget2d)textarget, texture, level);
        }
        public static void FramebufferRenderbuffer(OpenTK.Graphics.ES20.FramebufferTarget target, FramebufferSlot attachment, OpenTK.Graphics.ES20.RenderbufferTarget renderbuffertarget, Int32 renderbuffer)
        {
            GL.FramebufferRenderbuffer(target, (FramebufferAttachment)attachment, RenderbufferTarget.Renderbuffer, renderbuffer);
        }

        public static void GetProgram(Int32 program, ProgramParameter pname, out int linkStatus)
        {

            GL.GetProgram(program, (OpenTK.Graphics.ES20.GetProgramParameterName)pname, out linkStatus);
        }
    }
}




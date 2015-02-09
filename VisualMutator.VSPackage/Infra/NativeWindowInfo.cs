namespace PiotrTrzpil.VisualMutator_VSPackage.Infra
{
    namespace UsefulTools.Wpf
    {
        using System;
        using System.Windows.Forms;

        public class NativeWindowInfo : IWin32Window
        {
            private readonly IntPtr _handle;

            private readonly int _top;

            private readonly int _left;

            private readonly int _width;

            private readonly int _height;

            public IntPtr Handle
            {
                get
                {
                    return _handle;
                }
            }

            public int Top
            {
                get
                {
                    return _top;
                }
            }

            public int Left
            {
                get
                {
                    return _left;
                }
            }

            public int Width
            {
                get
                {
                    return _width;
                }
            }

            public int Height
            {
                get
                {
                    return _height;
                }
            }

            public NativeWindowInfo(IntPtr handle, int top = 0, int left = 0, int width = 0, int height = 0)
            {
                _handle = handle;
                _top = top;
                _left = left;
                _width = width;
                _height = height;
            }
        }
    }
}
namespace PiotrTrzpil.VisualMutator_VSPackage.Model
{
    using System;

    public class NativeWindowInfo
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

        public NativeWindowInfo(IntPtr handle, int top, int left, int width, int height)
        {
            _handle = handle;
            _top = top;
            _left = left;
            _width = width;
            _height = height;
        }
    }
}
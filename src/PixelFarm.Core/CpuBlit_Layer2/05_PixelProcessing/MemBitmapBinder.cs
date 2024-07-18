//MIT, 2014-present, WinterDev

namespace PixelFarm.Drawing
{
    public sealed class MemBitmapBinder : ImageBinder
    {
        private PixelFarm.CpuBlit.MemBitmap _memBmp;
        private bool _isMemBmpOwner;
        private bool _releaseLocalBmpIfRequired;

        public MemBitmapBinder(PixelFarm.CpuBlit.MemBitmap memBmp, bool isMemBmpOwner)
        {
            _memBmp = memBmp;
            _isMemBmpOwner = isMemBmpOwner;
        }

        public override void ReleaseLocalBitmapIfRequired()
        {
            _releaseLocalBmpIfRequired = true;
        }

        //
        public override int Width => _memBmp.Width;

        public override int Height => _memBmp.Height;

        //
        public override bool IsYFlipped => false;

        //
        public override void Dispose()
        {
            if (_memBmp != null)
            {
                if (_isMemBmpOwner)
                {
                    _memBmp.Dispose();
                }
                _memBmp = null;
            }
        }
    }
}
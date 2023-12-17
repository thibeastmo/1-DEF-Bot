using System.Drawing;

namespace NLBE_Bot.StatsPresentation
{
    class CenterDrawer
    {
        private Graphics _graphics;
        private Font _font;
        public CenterDrawer(Graphics graphics, Font font)
        {
            _graphics = graphics;
            _font = font;
        }

        public void Draw(string term, int x, int y, bool centerHeight = false)
        {
            var measurements = _graphics.MeasureString(term, _font);
            _graphics.DrawString(term, _font, Brushes.White, x - measurements.Width / 2, centerHeight ? y - measurements.Height / 2 : y);
        }
    }
}

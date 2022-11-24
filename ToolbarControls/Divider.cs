using ToolbarControls;

namespace SurfaceFiller.Components
{

    /// <summary>
    /// Implementuje obiekt rozdzielający opcje dostępne na pasku z nadzędziami
    /// </summary>
    internal class Divider : Label
    {
        private const int DividerHeight = 2;

        public Divider()
        {
            Text = string.Empty;
            BorderStyle = BorderStyle.Fixed3D;
            AutoSize = false;
            Height = DividerHeight;
            Width = ControlConstants.ToolbarWidth;
        }
    }
}

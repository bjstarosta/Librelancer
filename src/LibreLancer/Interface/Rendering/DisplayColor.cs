using LibreLancer;

namespace LibreLancer.Interface
{
    [UiLoadable]
    public class DisplayColor : DisplayElement
    {
        public InterfaceColor Color { get; set; }
        public override void Render(UiContext context, RectangleF clientRectangle)
        {
            if (Color == null) return;
            context.Mode2D();
            var rect = context.PointsToPixels(clientRectangle);
            context.Renderer2D.FillRectangle(rect, Color.GetColor(context.GlobalTime));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HTL.Grieskirchen.Edubot
{
    /// <summary>
    /// Interaction logic for DrawingPanel.xaml
    /// </summary>
    public partial class DrawingPanel : UserControl
    {
        Pen gridPen = new Pen(Brushes.LightGray, 1);
        Pen drawPen = new Pen(Brushes.Blue, 3);
        double lineWidth = 10;
        double lineHeight = 10;
        List<Point> points = new List<Point>();

        public DrawingPanel()
        {
            InitializeComponent();
            SnapsToDevicePixels = true;
            points.Add(new Point(50, 50));
            points.Add(new Point(70, 20));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {           
            base.OnRender(drawingContext);
            RenderGrid(drawingContext);
            RenderDrawing(drawingContext);
            if (this.IsMouseOver) {
                RenderMouse(drawingContext);
            }
        }

        protected void RenderMouse(DrawingContext drawingContext) {
            double mouseX = Mouse.GetPosition(this).X;
            double mouseY = Mouse.GetPosition(this).Y;
         
            drawingContext.DrawLine(drawPen, new Point(mouseX, mouseY), new Point(mouseX+2, mouseY+2));
        }

        /// <summary>
        /// Render the grid of the drawing panel
        /// </summary>
        /// <param name="drawingContext">The drawing context of this panel</param>
        protected void RenderGrid(DrawingContext drawingContext) {
            for (int i = 0; i < this.ActualWidth / lineWidth; i++)
            {
                drawingContext.DrawLine(gridPen, new Point(i * lineWidth, 0), new Point(i * lineWidth, this.ActualHeight));
            }
            for (int i = 0; i < this.ActualHeight / lineHeight; i++)
            {
                drawingContext.DrawLine(gridPen, new Point(0, i * lineHeight), new Point(this.ActualWidth, i * lineHeight));
            }
        }

        protected void RenderDrawing(DrawingContext drawingContext)
        {
            foreach (Point point in points) {
                drawingContext.DrawLine(drawPen, point, new Point(point.X+2,point.Y+2));
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            //this.InvalidateVisual();
        }

        private void UserControl_DragOver(object sender, DragEventArgs e)
        {
            //Console.WriteLine("2");
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!points.Contains(Mouse.GetPosition(this)))
                {
                    points.Add(Mouse.GetPosition(this));
                }
            }
            this.InvalidateVisual();
        }
    }
}

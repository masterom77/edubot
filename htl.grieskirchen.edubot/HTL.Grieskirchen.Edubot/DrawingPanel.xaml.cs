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
using System.Runtime.InteropServices;

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
        int changesLimit = 10;
        int changes = 0;
        List<Point> points = new List<Point>();
        System.Threading.Thread addingThread;
        bool dragging;


        public DrawingPanel()
        {
            InitializeComponent();
            SnapsToDevicePixels = true;
            Cursor = Cursors.Pen;
            //addingThread = new System.Threading.Thread(AddPoint);
        }

        //public static void AddPoint() {
        //    while(dragging){
            
        //        Mouse.get
        //        this.Dispatcher.Invoke(
        //  System.Windows.Threading.DispatcherPriority.Normal,
        //  new Action(
        //    delegate()
        //    {
        //        InvalidateVisual();
        //    }
        //));
        //    }
        //}

        protected override void OnRender(DrawingContext drawingContext)
        {           
            base.OnRender(drawingContext);
            drawingContext.DrawRectangle(Brushes.White, null, new Rect(0, 0, this.ActualWidth, this.ActualHeight));
            RenderGrid(drawingContext);
            RenderDrawing(drawingContext);
            if (this.IsMouseOver) {
                RenderMouse(drawingContext);
            }
        }

        /// <summary>
        /// Render the mouse-cursor
        /// </summary>
        /// <param name="drawingContext"></param>
        protected void RenderMouse(DrawingContext drawingContext) {
            double mouseX = Mouse.GetPosition(this).X;
            double mouseY = Mouse.GetPosition(this).Y;
            drawingContext.DrawRectangle(Brushes.LightGray, drawPen, new Rect(mouseX, mouseY, 1, 1));
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
                drawingContext.DrawRectangle(Brushes.LightGray, drawPen, new Rect(point.X, point.Y, 1, 1));
            }
        }

        private void UserControl_DragOver(object sender, DragEventArgs e)
        {
        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragging = true;
        }

        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point[] nPoints = new Point[64];
                int count = Mouse.GetIntermediatePoints(this, nPoints);
                
                points = points.Union(nPoints.ToList().GetRange(0,count)).ToList();
            }
        }

        private void UserControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragging = false;
            InvalidateVisual();
        }

    }
}

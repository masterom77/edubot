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
using System.Windows.Media.Media3D;
using HTL.Grieskirchen.Edubot.API.Interpolation;
using HTL.Grieskirchen.Edubot.API.Adapters;

namespace HTL.Grieskirchen.Edubot
{
    /// <summary>
    /// Interaction logic for Visualisation3D.xaml
    /// </summary>
    public partial class Visualisation2D : UserControl
    {
        public Visualisation2D()
        {
            InitializeComponent();

            posSecondaryEngine = MeshSecondaryEngine.Content.Bounds;
            drawnPoints = new List<Point>();
            AnglePrimaryAxis = 0;
            AnglePrimaryAxis = 0;
        }

        private Rect3D posSecondaryEngine;
        private double anglePrimaryAxis;
        private double angleSecondaryAxis;
        private List<Point> drawnPoints;
        private VirtualAdapter visualisationAdapter;

        public VirtualAdapter VisualisationAdapter
        {
            get { return visualisationAdapter; }
            set { visualisationAdapter = value;
            InvalidateVisual();
            }
        }

        private List<InterpolationStep> angles;

        public List<InterpolationStep> Angles
        {
            get { return angles; }
            set { angles = value;
            new System.Threading.Thread(startAnimation).Start();
            }
        }

        private void startAnimation() {
            updateCallback updatePrimaryAngle = new updateCallback(UpdatePrimaryAxis);
            updateCallback updateSecondaryAngle = new updateCallback(UpdateSecondaryAxis);

            foreach (InterpolationStep step in angles) {
                System.Threading.Thread.Sleep(10);
                Dispatcher.Invoke(updatePrimaryAngle, step.Alpha1);
                Dispatcher.Invoke(updateSecondaryAngle, step.Alpha2);
            }

            //visualisationAdapter.State = API.State.READY;
            
            
        
        }

        delegate void updateCallback(float val);


        //public void MoveAnglePrimaryAxis(long ticks, float speed) {
        //    primaryAxisSpeed = speed*40 / 16;
        //    primaryAxisTicks = ticks / 16;

        //    new System.Threading.Thread(MoveAnglePrimaryAxisAsync).Start();
        //}

        //public void MoveAngleSecondaryAxis(long ticks, float speed)
        //{
        //    secondaryAxisSpeed = speed*40 / 16;
        //    secondaryAxisTicks = ticks / 16;

        //    new System.Threading.Thread(MoveAngleSecondaryAxisAsync).Start();
        //}

        //private void MoveAnglePrimaryAxisAsync()
        //{
        //    updateCallback updateAngle = new updateCallback(UpdatePrimaryAxis);

        //    for (int i = 0; i < primaryAxisTicks; i++)
        //    {

        //        Dispatcher.Invoke(updateAngle);
        //        System.Threading.Thread.Sleep(Convert.ToInt32(1 / primaryAxisSpeed * 1000));
        //    }
        //}

        //private void MoveAngleSecondaryAxisAsync()
        //{
        //    updateCallback updateAngle = new updateCallback(UpdateSecondaryAxis);
        //    for (int i = 0; i < secondaryAxisTicks; i++)
        //    {

        //        Dispatcher.Invoke(updateAngle);
        //        System.Threading.Thread.Sleep(Convert.ToInt32(1 / secondaryAxisSpeed * 1000));
        //    }
        //}

        private void UpdatePrimaryAxis(float val)
        {
            AnglePrimaryAxis = val;
        }
        private void UpdateSecondaryAxis(float val)
        {
            AngleSecondaryAxis = val;
        }
     

        public double AnglePrimaryAxis
        {
            get { return anglePrimaryAxis; }
            set
            {
                Transform3DGroup transformGroup = new Transform3DGroup();
                transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), value), new Point3D(0, 0, 0)));


                MeshPrimaryAxis.Transform = transformGroup;
                MeshSecondaryEngine.Transform = transformGroup;
                MeshPen.Transform = transformGroup;
                //MeshPen2.Transform = transformGroup;
                MeshSecondaryAxis.Transform = transformGroup;



                posSecondaryEngine = transformGroup.TransformBounds(MeshSecondaryEngine.Content.Bounds);


                anglePrimaryAxis = value;
                AngleSecondaryAxis = AngleSecondaryAxis;
            }
        }


        public double AngleSecondaryAxis
        {
            get { return angleSecondaryAxis; }
            set
            {
                //int dir = value < 0 ? -1 : 1;
                Transform3DGroup transformGroup = new Transform3DGroup();
                transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), AnglePrimaryAxis)));
                transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), value), new Point3D(posSecondaryEngine.Location.X + posSecondaryEngine.SizeX / 2, posSecondaryEngine.Location.Y - posSecondaryEngine.SizeY, posSecondaryEngine.Location.Z + posSecondaryEngine.SizeZ / 2)));
                
                MeshPen.Transform = transformGroup;
                //MeshPen2.Transform = transformGroup;
                MeshSecondaryAxis.Transform = transformGroup;


                angleSecondaryAxis = value;
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            
            RenderGrid(drawingContext);
            base.OnRender(drawingContext);
            drawingContext.DrawLine(new Pen(Brushes.Gray, 3), new Point(0, ActualHeight / 2 - 1), new Point(ActualWidth, ActualHeight / 2 - 1));
            drawingContext.DrawLine(new Pen(Brushes.Gray, 3), new Point(ActualWidth / 2 - 1, 0), new Point(ActualWidth / 2 - 1, ActualHeight));
            RenderLabels(drawingContext);
        }

        protected void RenderGrid(DrawingContext drawingContext)
        {
            int stepSize = 25;
            for (int col = 0; col < ActualWidth / stepSize; col++)
            {
                drawingContext.DrawLine(new Pen(Brushes.LightGray, 1), new Point(0, stepSize * col), new Point(ActualWidth, stepSize * col));
            }
            for (int row = 0; row < ActualHeight / stepSize; row++)
            {
                drawingContext.DrawLine(new Pen(Brushes.LightGray, 1), new Point(stepSize * row, 0), new Point(stepSize * row, ActualHeight));
            }
        }

        protected void RenderLabels(DrawingContext drawingContext)
        {
            int stepSize = 25;
            int steps;
            if (visualisationAdapter != null)
                steps = (int)(visualisationAdapter.Length + visualisationAdapter.Length2) / stepSize;
            else
                steps = (int)ActualWidth / stepSize;

            for (int col = 0; col < steps+1; col++)
            {
                int y = (steps * 25 / 2) - stepSize * col;
                if (y == 0) {
                    continue;
                }
                FormattedText text = new FormattedText(y.ToString(), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 10, Brushes.Black);
                drawingContext.DrawText(text, new Point(ActualWidth / 2 + 5 , (col * stepSize)-text.Height/2));
            }
            for (int row = 0; row < steps + 1; row++)
            {
                int x = (-steps * 25 / 2) + stepSize * row;
                if (x == 0)
                {
                    continue;
                }
                FormattedText text = new FormattedText(x.ToString(), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 10, Brushes.Black);
                drawingContext.DrawText(text, new Point((row * stepSize) - text.Width / 2, ActualWidth / 2 + 5));
            }
        }
    }
}

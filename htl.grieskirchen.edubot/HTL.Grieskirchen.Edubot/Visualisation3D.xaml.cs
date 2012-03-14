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
    public partial class Visualisation3D : UserControl
    {
        public const int MAX_SPEED = 50;

       #region ---------------------Dependency Properties-------------------
        public static readonly DependencyProperty ShowGridProperty =
      DependencyProperty.Register("ShowGrid", typeof(bool), typeof(Visualisation3D), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShowLabelsProperty =
      DependencyProperty.Register("ShowLabels", typeof(bool), typeof(Visualisation3D), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShowAnimationProperty =
      DependencyProperty.Register("ShowAnimation", typeof(bool), typeof(Visualisation3D), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TicksProperty =
      DependencyProperty.Register("Ticks", typeof(int), typeof(Visualisation3D), new FrameworkPropertyMetadata(4, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty MaxTicksProperty =
      DependencyProperty.Register("MaxTicks", typeof(int), typeof(Visualisation3D), new FrameworkPropertyMetadata(20, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty MinTicksProperty =
      DependencyProperty.Register("MinTicks", typeof(int), typeof(Visualisation3D), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty SpeedProperty =
      DependencyProperty.Register("Speed", typeof(int), typeof(Visualisation3D), new PropertyMetadata(50, UpdateSpeed));

        private static void UpdateSpeed(DependencyObject obj, DependencyPropertyChangedEventArgs arg){
            speed = (int) obj.GetValue(SpeedProperty);
        } 

        public static readonly DependencyProperty MaxSpeedProperty =
      DependencyProperty.Register("MaxSpeed", typeof(int), typeof(Visualisation3D), new FrameworkPropertyMetadata(100, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty MinSpeedProperty =
      DependencyProperty.Register("MinSpeed", typeof(int), typeof(Visualisation3D), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        public Visualisation3D()
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

        #region ---------------------Properties---------------------
        
        public bool ShowGrid
        {
            get { return (bool)GetValue(ShowGridProperty); }
            set
            {
                SetValue(ShowGridProperty, value);
            }
        }
        public bool ShowLabels
        {
            get { return (bool)GetValue(ShowLabelsProperty); }
            set
            {
                SetValue(ShowLabelsProperty, value);
            }
        }
        private bool showAnimation;
        public bool ShowAnimation
        {
            get {
                showAnimation = (bool)GetValue(ShowAnimationProperty);
                return (bool)GetValue(ShowAnimationProperty); }
            set
            {
                showAnimation = value;
                SetValue(ShowAnimationProperty, value);
            }
        }

        public int Ticks
        {
            get { return (int)GetValue(TicksProperty); }
            set
            {
                SetValue(TicksProperty, value);
            }
        }
        public int MaxTicks
        {
            get { return (int)GetValue(MaxTicksProperty); }
            set
            {
                SetValue(MaxTicksProperty, value);
            }

        }
        public int MinTicks
        {
            get { return (int)GetValue(MinTicksProperty); }
            set
            {
                SetValue(MinTicksProperty, value);
            }
        }
        private static int speed;
        public int Speed
        {
            get {
                return (int)GetValue(SpeedProperty); 
            }
            set
            {
                SetValue(SpeedProperty, value);
            }
        }
        public int MaxSpeed
        {
            get { return (int)GetValue(MaxSpeedProperty); }
            set
            {
                SetValue(MaxSpeedProperty, value);
            }
        }
        public int MinSpeed
        {
            get { return (int)GetValue(MinSpeedProperty); }
            set
            {
                SetValue(MinSpeedProperty, value);
            }
        }

        private VirtualAdapter visualisationAdapter;
        public VirtualAdapter VisualisationAdapter
        {
            get { return visualisationAdapter; }
            set { visualisationAdapter = value;
            InvalidateVisual();
            }
        }

        private List<InterpolationStep> angles;
        /// <summary>
        /// Gets or sets the angles of the robot during the animation. Setting this property will cause the control to start the animation.
        /// </summary>
        public List<InterpolationStep> Angles
        {
            get { return angles; }
            set
            {
                angles = value;
                if (showAnimation)
                {
                    new System.Threading.Thread(StartAnimation).Start();
                }
            }
        }

        #endregion

        #region ---------------------Animation-----------------------

        /// <summary>
        /// A delegate used for updating the angle of an axis of the robot
        /// </summary>
        /// <param name="val">A float containing the new angle</param>
        delegate void UpdateCallback(float val);

        /// <summary>
        /// Gets or sets the angle of the primary Axis of the virtual robot
        /// </summary>
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

        /// <summary>
        /// Gets or sets the angle of the secondary Axis of the virtual robot
        /// </summary>
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

        /// <summary>
        /// Starts the animation of the virtual robot
        /// </summary>
        private void StartAnimation() {
            UpdateCallback updatePrimaryAngle = new UpdateCallback(UpdatePrimaryAxis);
            UpdateCallback updateSecondaryAngle = new UpdateCallback(UpdateSecondaryAxis);
            float ticks = MAX_SPEED + 1 - (((float)MAX_SPEED / 100) * speed);
            foreach (InterpolationStep step in angles) {
                System.Threading.Thread.Sleep((int)ticks);
                Dispatcher.Invoke(updatePrimaryAngle, step.Alpha1);
                Dispatcher.Invoke(updateSecondaryAngle, step.Alpha2);
            }
            visualisationAdapter.State = API.State.READY;
        }

        /// <summary>
        /// Updates the primary axis of the virtual robot
        /// </summary>
        /// <param name="val">A float, containing the new angle of the primary axis</param>
        private void UpdatePrimaryAxis(float val)
        {
            AnglePrimaryAxis = val;
        }

        /// <summary>
        /// Updates the secondary axis of the virtual robot
        /// </summary>
        /// <param name="val">A float, containing the new angle of the secondary axis</param>
        private void UpdateSecondaryAxis(float val)
        {
            AngleSecondaryAxis = val;
        }

        #endregion

        #region ---------------------Rendering---------------------
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (ShowAnimation)
            {
                vpVisualization.Visibility = Visibility.Visible;
                if (ShowGrid)
                {
                //    RenderGrid(drawingContext);
                }
                base.OnRender(drawingContext);
                //drawingContext.DrawLine(new Pen(Brushes.Gray, 3), new Point(0, ActualHeight / 2 - 1), new Point(ActualWidth, ActualHeight / 2 - 1));
                //drawingContext.DrawLine(new Pen(Brushes.Gray, 3), new Point(ActualWidth / 2 - 1, 0), new Point(ActualWidth / 2 - 1, ActualHeight));
                if (ShowLabels)
                {
                //    RenderLabels(drawingContext);
                }
            }
            else
            {
                base.OnRender(drawingContext);
                vpVisualization.Visibility = Visibility.Hidden;
                FormattedText text = new FormattedText("Visualisierung deaktiviert", System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 16, Brushes.Red);
                drawingContext.DrawText(text, new Point(ActualWidth / 2 - text.Width / 2, ActualHeight / 2 - text.Height / 2));
            }
        }

        protected void RenderGrid(DrawingContext drawingContext)
        {
            float stepSize = (float) ActualWidth / (Ticks*2);
            for (int col = 0; col < Ticks*2; col++)
            {
                drawingContext.DrawLine(new Pen(Brushes.LightGray, 1), new Point(0, stepSize * col), new Point(ActualWidth, stepSize * col));
            }
            for (int row = 0; row < Ticks*2; row++)
            {
                drawingContext.DrawLine(new Pen(Brushes.LightGray, 1), new Point(stepSize * row, 0), new Point(stepSize * row, ActualHeight));
            }
        }

        protected void RenderLabels(DrawingContext drawingContext)
        {
            double stepSize;
            double pixelPerPoint;
            double length;
            if (visualisationAdapter != null)
            {
                length = (visualisationAdapter.Length + visualisationAdapter.Length2) * 2;
                stepSize = (int)(length / (Ticks*2));
                pixelPerPoint = ActualWidth / length;
            }
            else
            {
                length = ActualWidth;
                stepSize = (int)(ActualWidth / Ticks);
                pixelPerPoint = 1;
            }
            //double offset = ActualWidth / steps;

            for (int col = 0; col < Ticks*2 +1; col++)
            {
                float yPos = (float) (col * stepSize * pixelPerPoint);
                int y = (int)(length/2-(col * stepSize));
                if (y == 0) {
                    continue;
                }
                FormattedText text = new FormattedText(y.ToString(), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 11, Brushes.Black);
                drawingContext.DrawText(text, new Point(ActualWidth / 2 + 5 , yPos-text.Height/2));
            }
            for (int row = 0; row < Ticks*2 + 1; row++)
            {
                float xPos = (float)(row * stepSize * pixelPerPoint);
                int x = (int)(length / 2 - (row * stepSize));
                if (x == 0)
                {
                    continue;
                }
                FormattedText text = new FormattedText(x.ToString(), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 11, Brushes.Black);
                drawingContext.DrawText(text, new Point(xPos - text.Width / 2, ActualWidth / 2 + 5));
            }
        }
        #endregion
    }
}

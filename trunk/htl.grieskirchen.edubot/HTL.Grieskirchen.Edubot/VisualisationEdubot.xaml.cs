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
using System.ComponentModel;
using HTL.Grieskirchen.Edubot.Settings;
using HTL.Grieskirchen.Edubot.API.EventArgs;

namespace HTL.Grieskirchen.Edubot
{
    /// <summary>
    /// Interaction logic for Visualisation3D.xaml
    /// </summary>
    public partial class VisualisationEdubot : UserControl
    {
        public const int MAX_SPEED = 50;

       #region ---------------------Dependency Properties-------------------
      //  public static readonly DependencyProperty ShowGridProperty =
      //DependencyProperty.Register("ShowGrid", typeof(bool), typeof(Visualisation3D), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

      //  public static readonly DependencyProperty ShowLabelsProperty =
      //DependencyProperty.Register("ShowLabels", typeof(bool), typeof(Visualisation3D), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

      //  public static readonly DependencyProperty ShowAnimationProperty =
      //DependencyProperty.Register("ShowAnimation", typeof(bool), typeof(Visualisation3D), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

      //  public static readonly DependencyProperty TicksProperty =
      //DependencyProperty.Register("Ticks", typeof(int), typeof(Visualisation3D), new FrameworkPropertyMetadata(4, FrameworkPropertyMetadataOptions.AffectsRender));

      //  public static readonly DependencyProperty MaxTicksProperty =
      //DependencyProperty.Register("MaxTicks", typeof(int), typeof(Visualisation3D), new FrameworkPropertyMetadata(20, FrameworkPropertyMetadataOptions.AffectsRender));

      //  public static readonly DependencyProperty MinTicksProperty =
      //DependencyProperty.Register("MinTicks", typeof(int), typeof(Visualisation3D), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsRender));

      //  public static readonly DependencyProperty SpeedProperty =
      //DependencyProperty.Register("Speed", typeof(int), typeof(Visualisation3D), new PropertyMetadata(50, UpdateSpeed));

      //  private static void UpdateSpeed(DependencyObject obj, DependencyPropertyChangedEventArgs arg){
      //      speed = (int) obj.GetValue(SpeedProperty);
      //  } 

      //  public static readonly DependencyProperty MaxSpeedProperty =
      //DependencyProperty.Register("MaxSpeed", typeof(int), typeof(Visualisation3D), new FrameworkPropertyMetadata(100, FrameworkPropertyMetadataOptions.AffectsRender));

      //  public static readonly DependencyProperty MinSpeedProperty =
      //DependencyProperty.Register("MinSpeed", typeof(int), typeof(Visualisation3D), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        public VisualisationEdubot()
        {
            InitializeComponent();

            posSecondaryEngine = Mesh5_Motor1_Model.Content.Bounds;
            drawnPoints = new List<Point>();
            AnglePrimaryAxis = 0;
            AnglePrimaryAxis = 0;
            configuration = new VisualizationConfig();
        }

        private Rect3D posSecondaryEngine;
        private System.Threading.Thread animationThread;
        private double anglePrimaryAxis;
        private double angleSecondaryAxis;
        private List<Point> drawnPoints;

        #region ---------------------Properties---------------------
        
        //public bool ShowGrid
        //{
        //    get { return (bool)GetValue(ShowGridProperty); }
        //    set
        //    {
        //        SetValue(ShowGridProperty, value);
        //    }
        //}
        //public bool ShowLabels
        //{
        //    get { return (bool)GetValue(ShowLabelsProperty); }
        //    set
        //    {
        //        SetValue(ShowLabelsProperty, value);
        //    }
        //}
        //private bool showAnimation;
        //public bool ShowAnimation
        //{
        //    get {
        //        showAnimation = (bool)GetValue(ShowAnimationProperty);
        //        return (bool)GetValue(ShowAnimationProperty); }
        //    set
        //    {
        //        showAnimation = value;
        //        SetValue(ShowAnimationProperty, value);
        //    }
        //}

        //public int Ticks
        //{
        //    get { return (int)GetValue(TicksProperty); }
        //    set
        //    {
        //        SetValue(TicksProperty, value);
        //    }
        //}
        //public int MaxTicks
        //{
        //    get { return (int)GetValue(MaxTicksProperty); }
        //    set
        //    {
        //        SetValue(MaxTicksProperty, value);
        //    }

        //}
        //public int MinTicks
        //{
        //    get { return (int)GetValue(MinTicksProperty); }
        //    set
        //    {
        //        SetValue(MinTicksProperty, value);
        //    }
        //}
        //private static int speed;
        //public int Speed
        //{
        //    get {
        //        return (int)GetValue(SpeedProperty); 
        //    }
        //    set
        //    {
        //        SetValue(SpeedProperty, value);
        //    }
        //}
        //public int MaxSpeed
        //{
        //    get { return (int)GetValue(MaxSpeedProperty); }
        //    set
        //    {
        //        SetValue(MaxSpeedProperty, value);
        //    }
        //}
        //public int MinSpeed
        //{
        //    get { return (int)GetValue(MinSpeedProperty); }
        //    set
        //    {
        //        SetValue(MinSpeedProperty, value);
        //    }
        //}
        private VisualizationConfig configuration;
        public VisualizationConfig Configuration
        {
            get { return configuration; }
            set
            {
                configuration = value;
                configuration.PropertyChanged += InvalidateDrawing;
            }
        }

        private VirtualAdapter visualisationAdapter;
        public VirtualAdapter VisualisationAdapter
        {
            get { return visualisationAdapter; }
            set { visualisationAdapter = value;
            visualisationAdapter.OnAbort += StopAnimation;
            visualisationAdapter.OnHoming += StartHoming;
            InvalidateVisual();
            }
        }

        private void StartHoming(object sender, EventArgs args)
        {
            new System.Threading.Thread(Home).Start(args);
        }

        private void Home(object args)
        {
            try
            {
                HomingEventArgs e = (HomingEventArgs)args;
                UpdateCallback updatePrimaryAngle = new UpdateCallback(UpdatePrimaryAxis);
                UpdateCallback updateSecondaryAngle = new UpdateCallback(UpdateSecondaryAxis);
                bool primaryCorrected = false;
                bool secondaryCorrected = false;
                float ticks = 5;// = MAX_SPEED + 10 - (((float)MAX_SPEED / 100) * configuration.Speed);
                while (!primaryCorrected || !secondaryCorrected)
                {
                    ticks = MAX_SPEED + 1 - (((float)MAX_SPEED / 100) * configuration.Speed);
                    System.Threading.Thread.Sleep((int)ticks);
                    if (anglePrimaryAxis > e.CorrectionAngle || anglePrimaryAxis < -e.CorrectionAngle)
                    {
                        if (anglePrimaryAxis > e.CorrectionAngle)
                        {
                            Dispatcher.Invoke(updatePrimaryAngle, (float)(anglePrimaryAxis - e.CorrectionAngle));
                        }
                        else
                        {
                            Dispatcher.Invoke(updatePrimaryAngle, (float)(anglePrimaryAxis + e.CorrectionAngle));
                        }
                    }
                    else
                    {
                        primaryCorrected = true;
                    }

                    if (angleSecondaryAxis > e.CorrectionAngle || angleSecondaryAxis < -e.CorrectionAngle)
                    {
                        if (angleSecondaryAxis > e.CorrectionAngle)
                        {
                            Dispatcher.Invoke(updateSecondaryAngle, (float)(angleSecondaryAxis - e.CorrectionAngle));
                        }
                        else
                        {
                            Dispatcher.Invoke(updateSecondaryAngle, (float)(angleSecondaryAxis + e.CorrectionAngle));
                        }
                    }
                    else
                    {
                        secondaryCorrected = true;
                    }
                }
                visualisationAdapter.State = API.State.READY;
            }
            catch (System.Threading.ThreadAbortException)
            {

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
                if (configuration.VisualizationEnabled)
                {
                    animationThread = new System.Threading.Thread(StartAnimation);
                    animationThread.Start();
                }
            }
        }

        #endregion

        #region ---------------------Animation-----------------------

        private void InvalidateDrawing(object sender, PropertyChangedEventArgs property)
        {
            InvalidateVisual();
        }

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
                transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), value), new System.Windows.Media.Media3D.Point3D(0, 0, 0)));


                Mesh4_Arm1_1_Model.Transform = transformGroup;
                Mesh5_Motor1_Model.Transform = transformGroup;
                Mesh6_G_2Arm1_Model.Transform = transformGroup;
                //MeshPen2.Transform = transformGroup;
                Mesh7_G_2Arm1_Model.Transform = transformGroup;
                //Mesh8_Model.Transform = transformGroup;
                posSecondaryEngine = transformGroup.TransformBounds(Mesh5_Motor1_Model.Content.Bounds);


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
                transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), value), new System.Windows.Media.Media3D.Point3D(posSecondaryEngine.Location.X + posSecondaryEngine.SizeX / 2, posSecondaryEngine.Location.Y - posSecondaryEngine.SizeY, posSecondaryEngine.Location.Z + posSecondaryEngine.SizeZ / 2)));

                Mesh6_G_2Arm1_Model.Transform = transformGroup;
                //MeshPen2.Transform = transformGroup;
                Mesh7_G_2Arm1_Model.Transform = transformGroup;


                angleSecondaryAxis = value;
            }
        }

        /// <summary>
        /// Starts the animation of the virtual robot
        /// </summary>
        private void StartAnimation() {
            try
            {
                UpdateCallback updatePrimaryAngle = new UpdateCallback(UpdatePrimaryAxis);
                UpdateCallback updateSecondaryAngle = new UpdateCallback(UpdateSecondaryAxis);
                float ticks = 5;// = MAX_SPEED + 10 - (((float)MAX_SPEED / 100) * configuration.Speed);
                foreach (InterpolationStep step in angles)
                {
                    ticks = MAX_SPEED + 1 - (((float)MAX_SPEED / 100) * configuration.Speed);
                    System.Threading.Thread.Sleep((int)ticks);
                    Dispatcher.Invoke(updatePrimaryAngle, step.Alpha1);
                    Dispatcher.Invoke(updateSecondaryAngle, step.Alpha2);
                }
                visualisationAdapter.State = API.State.READY;
            }
            catch (System.Threading.ThreadAbortException)
            {

            }
        }

        private void StopAnimation(object sender, EventArgs args)
        {
            animationThread.Abort();
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
            //if (configuration.VisualizationEnabled)
            //{
            //    Visualization.Visibility = Visibility.Visible;
            //    if (configuration.ShowGrid)
            //    {
            //    //    RenderGrid(drawingContext);
            //    }
            //    base.OnRender(drawingContext);
            //    //drawingContext.DrawLine(new Pen(Brushes.Gray, 3), new Point(0, ActualHeight / 2 - 1), new Point(ActualWidth, ActualHeight / 2 - 1));
            //    //drawingContext.DrawLine(new Pen(Brushes.Gray, 3), new Point(ActualWidth / 2 - 1, 0), new Point(ActualWidth / 2 - 1, ActualHeight));
            //    if (configuration.ShowLabels)
            //    {
            //    //    RenderLabels(drawingContext);
            //    }
            //}
            //else
            //{
            //    base.OnRender(drawingContext);
            //    vpVisualization.Visibility = Visibility.Hidden;
            //    FormattedText text = new FormattedText("Visualisierung deaktiviert", System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 16, Brushes.Red);
            //    drawingContext.DrawText(text, new Point(ActualWidth / 2 - text.Width / 2, ActualHeight / 2 - text.Height / 2));
            //}
        }

        protected void RenderGrid(DrawingContext drawingContext)
        {
            float stepSize = (float)ActualWidth / (configuration.Steps* 2);
            for (int col = 0; col < configuration.Steps * 2; col++)
            {
                drawingContext.DrawLine(new Pen(Brushes.LightGray, 1), new Point(0, stepSize * col), new Point(ActualWidth, stepSize * col));
            }
            for (int row = 0; row < configuration.Steps * 2; row++)
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
                stepSize = (int)(length / (configuration.Steps * 2));
                pixelPerPoint = ActualWidth / length;
            }
            else
            {
                length = ActualWidth;
                stepSize = (int)(ActualWidth / configuration.Steps);
                pixelPerPoint = 1;
            }
            //double offset = ActualWidth / steps;

            for (int col = 0; col < configuration.Steps * 2 + 1; col++)
            {
                float yPos = (float) (col * stepSize * pixelPerPoint);
                int y = (int)(length/2-(col * stepSize));
                if (y == 0) {
                    continue;
                }
                FormattedText text = new FormattedText(y.ToString(), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 11, Brushes.Black);
                drawingContext.DrawText(text, new Point(ActualWidth / 2 + 5 , yPos-text.Height/2));
            }
            for (int row = 0; row < configuration.Steps * 2 + 1; row++)
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

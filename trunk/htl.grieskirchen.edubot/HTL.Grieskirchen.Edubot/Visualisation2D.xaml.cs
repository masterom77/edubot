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
using HTL.Grieskirchen.Edubot.Settings;
using System.ComponentModel;
using HTL.Grieskirchen.Edubot.API.EventArgs;

namespace HTL.Grieskirchen.Edubot
{
    /// <summary>
    /// Interaction logic for Visualisation3D.xaml
    /// </summary>
    public partial class Visualisation2D : UserControl
    {
        public const int MAX_SPEED = 50;


        public Visualisation2D()
        {
            InitializeComponent();

            posSecondaryEngine = MeshSecondaryEngine.Content.Bounds;
            drawnPoints = new List<Point>();
            AnglePrimaryAxis = 0;
            AnglePrimaryAxis = 0;
            configuration = new VisualizationConfig();
        }

        private Rect3D posSecondaryEngine;
        private double anglePrimaryAxis;
        private double angleSecondaryAxis;
        private System.Threading.Thread animationThread;
        private List<Point> drawnPoints;
        private double tertiarySpeed;

        #region ---------------------Properties---------------------

        private VisualizationConfig configuration;

        public VisualizationConfig Configuration
        {
            get { return configuration; }
            set { configuration = value;
            configuration.PropertyChanged += ApplyConfiguration;
              
            }
        }

        private VirtualAdapter visualisationAdapter;
        public VirtualAdapter VisualisationAdapter
        {
            get { return visualisationAdapter; }
        }

        //public void SetVisualisationAdapter(VirtualAdapter adapter)
        //{
        //    if (visualisationAdapter != null) {
        //        RemoveVisualisationAdapter();
        //    }
        //    visualisationAdapter = adapter;
        //    visualisationAdapter.OnAbort += StopAnimation;
        //    visualisationAdapter.OnMovementStarted += StartMoving;
        //    visualisationAdapter.OnHoming += StartHoming;
        //    visualisationAdapter.OnToolUsed += UseTool;
        //    InvalidateVisual();
        //}

        //public VirtualAdapter RemoveVisualisationAdapter()
        //{
        //    visualisationAdapter.OnAbort -= StopAnimation;
        //    visualisationAdapter.OnMovementStarted -= StartMoving;
        //    visualisationAdapter.OnHoming -= StartHoming;
        //    visualisationAdapter.OnToolUsed -= UseTool;
        //    InvalidateVisual();
        //    return visualisationAdapter;
        //}

        private void UseTool(object sender, EventArgs args)
        {
            visualisationAdapter.State = API.State.READY;
        }

        private void StartHoming(object sender, EventArgs args)
        {
            new System.Threading.Thread(Home).Start(args);
        }

        private void StartMoving(object sender, EventArgs args)
        {
            if (configuration.VisualizationEnabled)
            {
                animationThread = new System.Threading.Thread(Move);
                animationThread.Start(((MovementStartedEventArgs)args).Result);
            }
           // new System.Threading.Thread(Move).Start(args);
        }

        private void Home(object args)
        {
            try
            {
                HomingEventArgs e = (HomingEventArgs)args;
                UpdateCallback updatePrimaryAngle = new UpdateCallback(UpdatePrimaryAxis);
                UpdateCallback updateSecondaryAngle = new UpdateCallback(UpdateSecondaryAxis);
                if (configuration.AnimateHoming)
                {
                    bool primaryCorrected = false;
                    bool secondaryCorrected = false;
                    float ticks = 5;
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
                }
                else
                {
                   
                    Dispatcher.Invoke(updatePrimaryAngle,new object[]{ 0f});
                    Dispatcher.Invoke(updateSecondaryAngle, new object[]{0f});
                
                }
                visualisationAdapter.State = API.State.READY;
            }
            catch (System.Threading.ThreadAbortException)
            {

            }
        }


        

        public void Move(object args) {
            InterpolationResult result = (InterpolationResult)args;
            try
            {
                UpdateCallback updatePrimaryAngle = new UpdateCallback(UpdatePrimaryAxis);
                UpdateCallback updateSecondaryAngle = new UpdateCallback(UpdateSecondaryAxis);
                float ticks = 5;// = MAX_SPEED + 10 - (((float)MAX_SPEED / 100) * configuration.Speed);
                foreach (InterpolationStep step in result.Angles)
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

        private List<InterpolationStep> angles;
        public List<InterpolationStep> Angles
        {
            get { return angles; }
            set
            {
                angles = value;
                
            }
        }

        public void Animate(InterpolationResult result) { 
            
            angles = result.Angles;
            tertiarySpeed = result.IncrZ;
            
            if (configuration.VisualizationEnabled)
                {
                    animationThread = new System.Threading.Thread(StartAnimation);
                    animationThread.Start();
                }
                //if (configuration.VisualizationEnabled)
                //{
                //    animationThread = new System.Threading.Thread(StartAnimation);
                //    animationThread.Start();
                //}
            
            
        }

        

        #endregion
        #region Settings

        private void ApplyConfiguration(object sender, PropertyChangedEventArgs property)
        {

            if (property.PropertyName == "Length" || property.PropertyName == "Length2")
            {
                if (visualisationAdapter != null)
                {
                    visualisationAdapter.Length = float.Parse(configuration.Length);
                    visualisationAdapter.Length2 = float.Parse(configuration.Length2);
                }
                //SCALE AXES
            }
            if (property.PropertyName == "IsEdubotModelSelected") {
                anglePrimaryAxis = 0;
                angleSecondaryAxis = 0;
            }
            if (property.PropertyName == "VisualizationEnabled")
            {
                if (configuration.VisualizationEnabled)
                {
                    visualisationAdapter = new VirtualAdapter(Tool.VIRTUAL, float.Parse(configuration.Length), float.Parse(configuration.Length2));
                    visualisationAdapter.OnAbort += StopAnimation;
                    visualisationAdapter.OnMovementStarted += StartMoving;
                    visualisationAdapter.OnHoming += StartHoming;
                    visualisationAdapter.OnToolUsed += UseTool;
                    API.Edubot.GetInstance().RegisterAdapter("2DVisualization", visualisationAdapter);
                }
                else
                {
                    if (visualisationAdapter != null)
                    {
                        visualisationAdapter.OnAbort -= StopAnimation;
                        visualisationAdapter.OnMovementStarted -= StartMoving;
                        visualisationAdapter.OnHoming -= StartHoming;
                        visualisationAdapter.OnToolUsed -= UseTool;
                        API.Edubot.GetInstance().DeregisterAdapter("2DVisualization");
                    }
                }
            }
            InvalidateVisual();
        }
        #endregion


        #region ---------------------Animation-----------------------


        delegate void UpdateCallback(float val);
        public double AnglePrimaryAxis
        {
            get { return anglePrimaryAxis; }
            set
            {
                Transform3DGroup transformGroup = new Transform3DGroup();
                transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), value), new System.Windows.Media.Media3D.Point3D(0, 0, 0)));


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
                transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), value), new System.Windows.Media.Media3D.Point3D(posSecondaryEngine.Location.X + posSecondaryEngine.SizeX / 2, posSecondaryEngine.Location.Y - posSecondaryEngine.SizeY, posSecondaryEngine.Location.Z + posSecondaryEngine.SizeZ / 2)));

                
                MeshPen.Transform = transformGroup;
                //MeshPen2.Transform = transformGroup;
                MeshSecondaryAxis.Transform = transformGroup;


                angleSecondaryAxis = value;
            }
        }

        public double PositionTertiaryAxis
        {
            get { return angleSecondaryAxis; }
            set
            {
                //int dir = value < 0 ? -1 : 1;
                Transform3DGroup transformGroup = new Transform3DGroup();
                transformGroup.Children.Add(new TranslateTransform3D(new Vector3D(0,0,value)));
                
                
                MeshPen.Transform = transformGroup;
                


                angleSecondaryAxis = value;
            }
        }

        private void StartAnimation()
        {
            try
            {
                UpdateCallback updatePrimaryAngle = new UpdateCallback(UpdatePrimaryAxis);
                UpdateCallback updateSecondaryAngle = new UpdateCallback(UpdateSecondaryAxis);
                UpdateCallback updateTertiaryPosition = new UpdateCallback(UpdateTertiaryAxis);
                float ticks = 5;// = MAX_SPEED + 10 - (((float)MAX_SPEED / 100) * configuration.Speed);
                foreach (InterpolationStep step in angles)
                {
                    ticks = MAX_SPEED + 1 - (((float)MAX_SPEED / 100) * configuration.Speed);
                    System.Threading.Thread.Sleep((int)ticks);
                    Dispatcher.Invoke(updatePrimaryAngle, step.Alpha1);
                    Dispatcher.Invoke(updateSecondaryAngle, step.Alpha2);
                    Dispatcher.Invoke(updateTertiaryPosition, tertiarySpeed);
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

        private void UpdatePrimaryAxis(float val)
        {
            AnglePrimaryAxis = val;
        }
        private void UpdateSecondaryAxis(float val)
        {
            AngleSecondaryAxis = val;
        }

        private void UpdateTertiaryAxis(float val)
        {
            PositionTertiaryAxis = val;
        }

        #endregion

        #region ---------------------Rendering---------------------
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (configuration.VisualizationEnabled)
            {
                vpVisualization.Visibility = Visibility.Visible;
                if (configuration.ShowGrid)
                {
                    RenderGrid(drawingContext);
                }
                base.OnRender(drawingContext);
                drawingContext.DrawLine(new Pen(Brushes.Gray, 3), new Point(0, ActualHeight / 2 - 1), new Point(ActualWidth, ActualHeight / 2 - 1));
                drawingContext.DrawLine(new Pen(Brushes.Gray, 3), new Point(ActualWidth / 2 - 1, 0), new Point(ActualWidth / 2 - 1, ActualHeight));
                if (configuration.ShowLabels)
                {
                    RenderLabels(drawingContext);
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
            float stepSize = (float) ActualWidth / (configuration.Steps*2);
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

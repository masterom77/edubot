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
    public partial class Visualisation3D : UserControl
    {
        public const int MAX_SPEED = 50;


        public Visualisation3D()
        {
            InitializeComponent();

            posSecondaryEngine = MeshSecondaryEngine.Content.Bounds;
            offsetZ = 0;
            drawnPoints = new List<Point>();
            AnglePrimaryAxis = 0;
            AnglePrimaryAxis = 0;
            configuration = new VisualizationConfig();
        }

        private Rect3D posSecondaryEngine;
        private double offsetZ;
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
                UpdateCallback updateTertiaryAngle = new UpdateCallback(UpdateTertiaryAxis);
                float ticks = 5;// = MAX_SPEED + 10 - (((float)MAX_SPEED / 100) * configuration.Speed);
                foreach (InterpolationStep step in result.Angles)
                {
                    ticks = MAX_SPEED + 1 - (((float)MAX_SPEED / 100) * configuration.Speed);
                    System.Threading.Thread.Sleep((int)ticks);
                    Dispatcher.Invoke(updatePrimaryAngle, step.Alpha1);
                    Dispatcher.Invoke(updateSecondaryAngle, step.Alpha2);
                    Dispatcher.Invoke(updateTertiaryAngle, result.Angles.Count);
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

        //public void Animate(InterpolationResult result) { 
            
        //    angles = result.Angles;
        //    tertiarySpeed = result.IncrZ;
            
        //    if (configuration.VisualizationEnabled)
        //        {
        //            animationThread = new System.Threading.Thread(StartAnimation);
        //            animationThread.Start();
        //        }
        //        //if (configuration.VisualizationEnabled)
        //        //{
        //        //    animationThread = new System.Threading.Thread(StartAnimation);
        //        //    animationThread.Start();
        //        //}
            
            
        //}



        #endregion

        #region ---------------------Animation-----------------------

        private void InvalidateDrawing(object sender, PropertyChangedEventArgs property) {
            InvalidateVisual();
        }

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

                double toolRange = MeshPen.Content.Bounds.Y + MeshPen.Content.Bounds.SizeY;
                offsetZ += value;
                Transform3DGroup transformGroup = new Transform3DGroup();
                transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), AnglePrimaryAxis)));
                transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), value), new System.Windows.Media.Media3D.Point3D(posSecondaryEngine.Location.X + posSecondaryEngine.SizeX / 2, posSecondaryEngine.Location.Y - posSecondaryEngine.SizeY, posSecondaryEngine.Location.Z + posSecondaryEngine.SizeZ / 2)));
                transformGroup.Children.Add(new TranslateTransform3D(new Vector3D(offsetZ, 0, 0)));
                
                //MeshPen.Transform = transformGroup;
                
                

                
            }
        }

        //private void StartAnimation()
        //{
        //    try
        //    {
        //        UpdateCallback updatePrimaryAngle = new UpdateCallback(UpdatePrimaryAxis);
        //        UpdateCallback updateSecondaryAngle = new UpdateCallback(UpdateSecondaryAxis);
        //        UpdateCallback updateTertiaryPosition = new UpdateCallback(UpdateTertiaryAxis);
        //        float ticks = 5;// = MAX_SPEED + 10 - (((float)MAX_SPEED / 100) * configuration.Speed);
        //        foreach (InterpolationStep step in angles)
        //        {
        //            ticks = MAX_SPEED + 1 - (((float)MAX_SPEED / 100) * configuration.Speed);
        //            System.Threading.Thread.Sleep((int)ticks);
        //            Dispatcher.Invoke(updatePrimaryAngle, step.Alpha1);
        //            Dispatcher.Invoke(updateSecondaryAngle, step.Alpha2);
        //            Dispatcher.Invoke(updateTertiaryPosition, tertiarySpeed);
        //        }
        //        visualisationAdapter.State = API.State.READY;
        //    }
        //    catch (System.Threading.ThreadAbortException)
        //    {
        //    }
        //}

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

        private void ApplyConfiguration(object sender, PropertyChangedEventArgs property)
        {

            if (visualisationAdapter != null)
            {
                if (property.PropertyName == "Length")
                {
                    visualisationAdapter.Length = float.Parse(configuration.Length);
                }
                if (property.PropertyName == "Length2")
                {
                    visualisationAdapter.Length2 = float.Parse(configuration.Length2);
                }
                if (property.PropertyName == "VerticalToolRange")
                {
                    visualisationAdapter.VerticalToolRange = float.Parse(configuration.VerticalToolRange);
                }
                if (property.PropertyName == "Transmission")
                {
                    visualisationAdapter.Transmission = float.Parse(configuration.Transmission);
                }
                if (property.PropertyName == "MaxPrimaryAngle")
                {
                    visualisationAdapter.MaxPrimaryAngle = float.Parse(configuration.MaxPrimaryAngle);
                }
                if (property.PropertyName == "MinPrimaryAngle")
                {
                    visualisationAdapter.MinPrimaryAngle = float.Parse(configuration.MinPrimaryAngle);
                }
                if (property.PropertyName == "MaxSecondaryAngle")
                {
                    visualisationAdapter.MaxSecondaryAngle = float.Parse(configuration.MaxSecondaryAngle);
                }
                if (property.PropertyName == "MinSecondaryAngle")
                {
                    visualisationAdapter.MinSecondaryAngle = float.Parse(configuration.MinSecondaryAngle);
                }
            }
            if (property.PropertyName == "VisualizationEnabled" || property.PropertyName == "IsEdubotModelSelected")
            {
                if (configuration.VisualizationEnabled && !configuration.IsEdubotModelSelected)
                {
                    configuration.MaxPrimaryAngle = float.MaxValue.ToString();
                    configuration.MinPrimaryAngle = float.MinValue.ToString();
                    configuration.MaxSecondaryAngle = float.MaxValue.ToString();
                    configuration.MinSecondaryAngle = float.MinValue.ToString();
                    configuration.Transmission = "0";
                    visualisationAdapter = new VirtualAdapter(Tool.VIRTUAL, float.Parse(configuration.Length), float.Parse(configuration.Length2),30,0);
                    visualisationAdapter.OnAbort += StopAnimation;
                    visualisationAdapter.OnMovementStarted += StartMoving;
                    visualisationAdapter.OnHoming += StartHoming;
                    visualisationAdapter.OnToolUsed += UseTool;
                    AnglePrimaryAxis = 0;
                    AngleSecondaryAxis = 0;
                    API.Edubot.GetInstance().RegisterAdapter("3DVisualization", visualisationAdapter);
                }
                else
                {
                    if (visualisationAdapter != null)
                    {
                        visualisationAdapter.OnAbort -= StopAnimation;
                        visualisationAdapter.OnMovementStarted -= StartMoving;
                        visualisationAdapter.OnHoming -= StartHoming;
                        visualisationAdapter.OnToolUsed -= UseTool;
                        API.Edubot.GetInstance().DeregisterAdapter("3DVisualization");
                    }
                }
            }
            InvalidateVisual();
        }

        #region ---------------------Rendering---------------------
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (configuration.VisualizationEnabled)
            {
                vpVisualization.Visibility = Visibility.Visible;

                base.OnRender(drawingContext);
            }
            else
            {
                base.OnRender(drawingContext);
                vpVisualization.Visibility = Visibility.Hidden;
                FormattedText text = new FormattedText("Visualisierung deaktiviert", System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 16, Brushes.Red);
                drawingContext.DrawText(text, new Point(ActualWidth / 2 - text.Width / 2, ActualHeight / 2 - text.Height / 2));
            }
        }

       
        #endregion
    }
}

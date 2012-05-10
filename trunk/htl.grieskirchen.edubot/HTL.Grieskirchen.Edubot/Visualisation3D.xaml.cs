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
using HTL.Grieskirchen.Edubot.API;
using System.Threading;

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
        private ScaleTransform3D primaryScale;
        private TranslateTransform3D primaryOffset;
        private ScaleTransform3D secondaryScale;
        private TranslateTransform3D secondaryOffset;

        #region ---------------------Properties---------------------

        private VisualizationConfig configuration;

        public VisualizationConfig Configuration
        {
            get { return configuration; }
            set { configuration = value;
            configuration.PropertyChanged += ApplyConfiguration;
            InvalidateVisual();
            }
        }

        private VirtualAdapter visualisationAdapter;
        public VirtualAdapter VisualisationAdapter
        {
            get { return visualisationAdapter; }
        }

        
        private void UseTool(object sender, EventArgs args)
        {
            visualisationAdapter.SetState(State.READY);
        }

        private void Shutdown(object sender, EventArgs args)
        {
            visualisationAdapter.SetState(State.SHUTDOWN);
        }

        private void StartHoming(object sender, EventArgs args)
        {
            animationThread = new System.Threading.Thread(Home);
            animationThread.Priority = ThreadPriority.AboveNormal;
            animationThread.Start(args);
        }

        private void StartMoving(object sender, EventArgs args)
        {
            if (configuration.VisualizationEnabled)
            {
                animationThread = new System.Threading.Thread(Move);
                animationThread.Priority = ThreadPriority.AboveNormal;
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
                visualisationAdapter.SetState(API.State.READY);
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
                //UpdateCallback updateTertiaryAngle = new UpdateCallback(UpdateTertiaryAxis);
                float ticks = 5;

                foreach (InterpolationStep step in result.Angles)
                {
                    ticks = MAX_SPEED + 1 - (((float)MAX_SPEED / 100) * configuration.Speed);
                    System.Threading.Thread.Sleep((int)ticks);
                    Dispatcher.Invoke(updatePrimaryAngle, step.Alpha1);
                    Dispatcher.Invoke(updateSecondaryAngle, step.Alpha2);
                    //Dispatcher.Invoke(updateTertiaryAngle, result.Angles.Count);
                }
                visualisationAdapter.SetState(API.State.READY);
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
                RotateTransform3D rotateTransform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), value), new System.Windows.Media.Media3D.Point3D(0, 0, 0));
                Transform3DGroup transformPrimary = new Transform3DGroup();
                if (primaryScale != null)
                    transformPrimary.Children.Add(primaryScale); //First Scaling
                transformPrimary.Children.Add(rotateTransform); //Then Rotating
                MeshPrimaryAxis.Transform = transformPrimary;

                Transform3DGroup transformSecondary = new Transform3DGroup();
                if (primaryOffset != null)
                    transformSecondary.Children.Add(primaryOffset);
                transformSecondary.Children.Add(rotateTransform);
                MeshSecondaryEngine.Transform = transformSecondary;
                MeshPen.Transform = transformSecondary;
                //MeshPen2.Transform = transformGroup;
                MeshSecondaryAxis.Transform = transformSecondary;
                posSecondaryEngine = transformSecondary.TransformBounds(MeshSecondaryEngine.Content.Bounds);


                anglePrimaryAxis = value;
                AngleSecondaryAxis = AngleSecondaryAxis;
            }
        }

        public double AngleSecondaryAxis
        {
            get { return angleSecondaryAxis; }
            set
            {
                RotateTransform3D rotateTransform1 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), AnglePrimaryAxis), new System.Windows.Media.Media3D.Point3D(0, 0, 0));
                RotateTransform3D rotateTransform2 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), value), new System.Windows.Media.Media3D.Point3D(posSecondaryEngine.Location.X + posSecondaryEngine.SizeX / 2, posSecondaryEngine.Location.Y - posSecondaryEngine.SizeY, posSecondaryEngine.Location.Z + posSecondaryEngine.SizeZ / 2));

                Transform3DGroup transformRotate = new Transform3DGroup();
                transformRotate.Children.Add(rotateTransform1);
                transformRotate.Children.Add(rotateTransform2);

                Transform3DGroup transformSecondary = new Transform3DGroup();
                if (secondaryScale != null)
                    transformSecondary.Children.Add(secondaryScale);
                if (primaryOffset != null)
                    transformSecondary.Children.Add(primaryOffset);
                transformSecondary.Children.Add(transformRotate);


                MeshPen.Transform = transformRotate;
                //MeshPen2.Transform = transformGroup;
                MeshSecondaryAxis.Transform = transformSecondary;


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

        private void StopAnimation(object sender, EventArgs args)
        {
            try
            {
                if (animationThread != null)
                {
                    animationThread.Abort();
                }
            }
            catch (ThreadAbortException)
            {
            }
            if (visualisationAdapter != null)
            {
                visualisationAdapter.SetState(State.SHUTDOWN);
            }
        }

        private void UpdatePrimaryAxis(float val)
        {
            AnglePrimaryAxis = val;
        }
        private void UpdateSecondaryAxis(float val)
        {
            AngleSecondaryAxis = val;
        }

        //private void UpdateTertiaryAxis(float val)
        //{
        //    PositionTertiaryAxis = val;
        //}

        #endregion

        public void ScaleAxes()
        {


            double primaryMeshRelation = MeshPrimaryAxis.Content.Bounds.SizeZ / (MeshPrimaryAxis.Content.Bounds.SizeZ + MeshSecondaryAxis.Content.Bounds.SizeZ);
            double secondaryMeshRelation = MeshSecondaryAxis.Content.Bounds.SizeZ / (MeshPrimaryAxis.Content.Bounds.SizeZ + MeshSecondaryAxis.Content.Bounds.SizeZ);

            double primaryScaleRatio = visualisationAdapter.Length / ((visualisationAdapter.Length + visualisationAdapter.Length2) * primaryMeshRelation);
            double secondaryScaleRatio = visualisationAdapter.Length2 / ((visualisationAdapter.Length + visualisationAdapter.Length2) * secondaryMeshRelation);

            double primaryAxisWidth = MeshPrimaryAxis.Content.Bounds.SizeZ;
            double secondaryAxisX = MeshSecondaryAxis.Content.Bounds.Z;

            primaryScale = new ScaleTransform3D(1, 1, primaryScaleRatio, MeshPrimaryAxis.Content.Bounds.X + MeshPrimaryAxis.Content.Bounds.SizeX / 2, MeshPrimaryAxis.Content.Bounds.Y + MeshPrimaryAxis.Content.Bounds.SizeY, MeshPrimaryAxis.Content.Bounds.Z + MeshPrimaryAxis.Content.Bounds.SizeZ);
            secondaryScale = new ScaleTransform3D(1, 1, secondaryScaleRatio, MeshSecondaryAxis.Content.Bounds.X + MeshSecondaryAxis.Content.Bounds.SizeX / 2, MeshSecondaryAxis.Content.Bounds.Y + MeshSecondaryAxis.Content.Bounds.SizeY, MeshSecondaryAxis.Content.Bounds.Z + MeshSecondaryAxis.Content.Bounds.SizeZ);
            MeshPrimaryAxis.Transform = primaryScale;
            double offset = primaryAxisWidth - primaryAxisWidth * primaryScaleRatio;
            primaryOffset = new TranslateTransform3D(0, 0, offset);
            secondaryOffset = new TranslateTransform3D(0, 0, offset);
            Transform3DGroup transform = new Transform3DGroup();
            transform.Children.Add(secondaryScale);
            transform.Children.Add(secondaryOffset);
            MeshSecondaryEngine.Transform = primaryOffset;
            MeshSecondaryAxis.Transform = transform;
            AnglePrimaryAxis = 0;
            AngleSecondaryAxis = 0;
        }


        private void ApplyConfiguration(object sender, PropertyChangedEventArgs property)
        {

            if (visualisationAdapter != null)
            {
                if (property.PropertyName == "Length")
                {
                    visualisationAdapter.Length = configuration.Length;
                    ScaleAxes();

                }
                if (property.PropertyName == "Length2")
                {
                    visualisationAdapter.Length2 = configuration.Length2;
                    ScaleAxes();
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
                    if (configuration.MaxPrimaryAngle == null)
                    {
                        visualisationAdapter.MaxPrimaryAngle = float.MaxValue;
                    }
                    else
                    {
                        visualisationAdapter.MaxPrimaryAngle = float.Parse(configuration.MaxPrimaryAngle);
                    }
                }
                if (property.PropertyName == "MinPrimaryAngle")
                {
                    if (configuration.MinPrimaryAngle == null)
                    {
                        visualisationAdapter.MinPrimaryAngle = float.MinValue;
                    }
                    else
                    {
                        visualisationAdapter.MinPrimaryAngle = float.Parse(configuration.MinPrimaryAngle);
                    }
                }
                if (property.PropertyName == "MaxSecondaryAngle")
                {
                    if (configuration.MaxSecondaryAngle == null)
                    {
                        visualisationAdapter.MaxSecondaryAngle = float.MaxValue;
                    }
                    else
                    {
                        visualisationAdapter.MaxSecondaryAngle = float.Parse(configuration.MaxSecondaryAngle);
                    }
                }
                if (property.PropertyName == "MinSecondaryAngle")
                {
                    if (configuration.MinSecondaryAngle == null)
                    {
                        visualisationAdapter.MinSecondaryAngle = float.MinValue;
                    }
                    else
                    {
                        visualisationAdapter.MinSecondaryAngle = float.Parse(configuration.MinSecondaryAngle);
                    }
                }
            }
            if (property.PropertyName == "VisualizationEnabled" || property.PropertyName == "IsEdubotModelSelected")
            {
                if (configuration.VisualizationEnabled && !configuration.IsEdubotModelSelected)
                {
                    configuration.Length = 200;
                    configuration.Length2 = 200;
                    configuration.MaxPrimaryAngle = null;
                    configuration.MinPrimaryAngle = null;
                    configuration.MaxSecondaryAngle = null;
                    configuration.MinSecondaryAngle = null;
                    configuration.Transmission = "0";
                    visualisationAdapter = new VirtualAdapter(Tool.VIRTUAL,configuration.Length, configuration.Length2,30,0);
                    visualisationAdapter.OnAbort += StopAnimation;
                    visualisationAdapter.OnMovementStarted += StartMoving;
                    visualisationAdapter.OnHoming += StartHoming;
                    visualisationAdapter.OnToolUsed += UseTool;
                    visualisationAdapter.OnShuttingDown += Shutdown;
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
                        visualisationAdapter.OnShuttingDown -= Shutdown;
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

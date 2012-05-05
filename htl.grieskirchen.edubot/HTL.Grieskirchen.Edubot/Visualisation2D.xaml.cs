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
using HTL.Grieskirchen.Edubot.API;
using System.Threading;

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
            drawnPaths = new List<Path>();
            AnglePrimaryAxis = 0;
            AnglePrimaryAxis = 0;
            configuration = new VisualizationConfig();
        }

        private Rect3D posSecondaryEngine;
        private double anglePrimaryAxis;
        private double angleSecondaryAxis;
        private System.Threading.Thread animationThread;
        private List<Path> drawnPaths;
        private Path currentPath;
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

        private void UseTool(object sender, EventArgs args)
        {
            usingTool = ((ToolUsedEventArgs)args).Activated;
            visualisationAdapter.SetState(State.READY);
        }

        private void Shutdown(object sender, EventArgs args)
        {
            visualisationAdapter.SetState(State.SHUTDOWN);
        }

        private void StartHoming(object sender, EventArgs args)
        {
            animationThread = new System.Threading.Thread(Home);
            animationThread.Start(args);
        }

        private void StartMoving(object sender, EventArgs args)
        {
            if (configuration.VisualizationEnabled)
            {
                animationThread = new System.Threading.Thread(Move);
                animationThread.Start(args);
            }
           // new System.Threading.Thread(Move).Start(args);
        }

        private void Home(object args)
        {
            try
            {
                drawnPaths.Clear();
                Dispatcher.Invoke(new Action(delegate { InvalidateVisual(); }));
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
            MovementStartedEventArgs eArgs = (MovementStartedEventArgs) args;
            InterpolationResult result = eArgs.Result;
            try
            {
                UpdateCallback updatePrimaryAngle = new UpdateCallback(UpdatePrimaryAxis);
                UpdateCallback updateSecondaryAngle = new UpdateCallback(UpdateSecondaryAxis);
                UpdateCallback updateTertiaryAngle = new UpdateCallback(UpdateTertiaryAxis);
                float ticks = 5;
                currentPath = new Path() { Start = result.Points.First() };
                currentPath.Direction = SweepDirection.Counterclockwise;
                if (result.InterpolationType == InterpolationType.Circular)
                {
                    if ((bool)result.MetaData["Clockwise"])
                    {
                        currentPath.Direction = SweepDirection.Clockwise;
                    }
                    currentPath.Radius = (float)(double)result.MetaData["Radius"];
                }
                currentPath.Dashed = !usingTool;
                currentPath.Type = result.InterpolationType;
                foreach (InterpolationStep step in result.Angles)
                {
                    ticks = MAX_SPEED + 1 - (((float)MAX_SPEED / 100) * configuration.Speed);       
                    currentPath.End = step.Target;
                    System.Threading.Thread.Sleep((int)ticks);
                    Dispatcher.Invoke(new Action(delegate { InvalidateVisual(); }));
                    Dispatcher.Invoke(updatePrimaryAngle, step.Alpha1);
                    Dispatcher.Invoke(updateSecondaryAngle, step.Alpha2);
                    //Dispatcher.Invoke(updateTertiaryAngle, step.Alpha3);
                }
                if (result.Angles.Count > 0)
                {
                    drawnPaths.Add(currentPath);
                }
                visualisationAdapter.SetState(State.READY);
            }
            catch (System.Threading.ThreadAbortException)
            {

            }
        }

        public void ScaleAxes() {

            float primaryScaleRatio = visualisationAdapter.Length / ((visualisationAdapter.Length + visualisationAdapter.Length2)/2);
            float secondaryScaleRatio = 2 - primaryScaleRatio;

            double primaryAxisWidth = MeshPrimaryAxis.Content.Bounds.SizeZ;
            double secondaryAxisX = MeshSecondaryAxis.Content.Bounds.Z;

            ScaleTransform3D primaryScale = new ScaleTransform3D(1, 1, primaryScaleRatio);
            ScaleTransform3D secondaryScale = new ScaleTransform3D(1, 1, secondaryScaleRatio, MeshSecondaryAxis.Content.Bounds.X + MeshSecondaryAxis.Content.Bounds.SizeX/2, MeshSecondaryAxis.Content.Bounds.Y + MeshSecondaryAxis.Content.Bounds.SizeY, MeshSecondaryAxis.Content.Bounds.Z + MeshSecondaryAxis.Content.Bounds.SizeZ);
            MeshPrimaryAxis.Transform = primaryScale;
            double offset = primaryAxisWidth - primaryAxisWidth*primaryScaleRatio;
            //if(relLength > 1){
            //    offset *= -1;
            //}

            TranslateTransform3D primaryOffset = new TranslateTransform3D(0, 0, offset);
            TranslateTransform3D secondaryOffset = new TranslateTransform3D(0,0,offset);
            Transform3DGroup transform = new Transform3DGroup();
            transform.Children.Add(secondaryScale);
            transform.Children.Add(secondaryOffset);
            MeshSecondaryEngine.Transform = primaryOffset;

            MeshSecondaryAxis.Transform = transform;
        }

        #endregion
        #region Settings

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
        
            if (property.PropertyName == "IsEdubotModelSelected") {
                AnglePrimaryAxis = 0;
                AngleSecondaryAxis = 0;
            }
            if (property.PropertyName == "VisualizationEnabled")
            {
                if (configuration.VisualizationEnabled)
                {
                    visualisationAdapter = new VirtualAdapter(Tool.VIRTUAL, configuration.Length, configuration.Length2);
                    visualisationAdapter.OnAbort += StopAnimation;
                    visualisationAdapter.OnMovementStarted += StartMoving;
                    visualisationAdapter.OnHoming += StartHoming;
                    visualisationAdapter.OnToolUsed += UseTool;
                    visualisationAdapter.OnShuttingDown += Shutdown;
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
                        visualisationAdapter.OnShuttingDown -= Shutdown;
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


        private void StopAnimation(object sender, EventArgs args)
        {
            try
            {
                if (animationThread != null)
                {
                    animationThread.Abort();
                }
            }
            catch (ThreadAbortException) { 
            }
            if (visualisationAdapter != null) {
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
                RenderPath(drawingContext);
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
            for (int col = 0; col <= configuration.Steps * 2; col++)
            {
                drawingContext.DrawLine(new Pen(Brushes.LightGray, 1), new Point(0, stepSize * col), new Point(ActualWidth, stepSize * col));
            }
            for (int row = 0; row <= configuration.Steps * 2; row++)
            {
                drawingContext.DrawLine(new Pen(Brushes.LightGray, 1), new Point(stepSize * row, 0), new Point(stepSize * row, ActualHeight));
            }
        }

        bool usingTool = false;
        //List<API.Interpolation.Point3D> drawnPoints;

        protected void RenderPath(DrawingContext drawingContext)
        {
            double axisLength = visualisationAdapter.Length + visualisationAdapter.Length2;
            double pixelPerPoint = ActualWidth / (axisLength * 2);
            double center = axisLength * pixelPerPoint;
            Pen pen = new Pen(Brushes.Black, 3);
            Pen dashedPen = new Pen(Brushes.Black, 2);
            dashedPen.DashStyle = new DashStyle(new double[] { 2, 3 }, 0);
            Path path;
            for(int i = drawnPaths.Count-1; i >= 0; i--){
                path = drawnPaths.ElementAt(i);
                if (path.Type == InterpolationType.Circular)
                {
                    DrawArc(drawingContext, null, path.Dashed ? dashedPen : pen, new Point(center + path.Start.X * pixelPerPoint, center - path.Start.Y * pixelPerPoint), new Point(center + path.End.X * pixelPerPoint, center - path.End.Y * pixelPerPoint),(float) (path.Radius * pixelPerPoint), path.Direction);
                }
                else
                {
                    drawingContext.DrawLine(path.Dashed ? dashedPen : pen, new Point(center + path.Start.X * pixelPerPoint, center - path.Start.Y * pixelPerPoint), new Point(center + path.End.X * pixelPerPoint, center - path.End.Y * pixelPerPoint));
                }
            }
            if (currentPath != null)
            {
                if (currentPath.Type == InterpolationType.Circular)
                {

                    DrawArc(drawingContext, null, currentPath.Dashed ? dashedPen : pen, new Point(center + currentPath.Start.X * pixelPerPoint, center - currentPath.Start.Y * pixelPerPoint), new Point(center + currentPath.End.X * pixelPerPoint, center - currentPath.End.Y * pixelPerPoint), (float)(currentPath.Radius*pixelPerPoint), currentPath.Direction);
                }
                else
                {
                    drawingContext.DrawLine(currentPath.Dashed ? dashedPen : pen, new Point(center + currentPath.Start.X * pixelPerPoint, center - currentPath.Start.Y * pixelPerPoint), new Point(center + currentPath.End.X * pixelPerPoint, center - currentPath.End.Y * pixelPerPoint));
                }
            }
        }

        protected void RenderLabels(DrawingContext drawingContext)
        {
            double axisLength = visualisationAdapter.Length + visualisationAdapter.Length2;
            double stepSize = axisLength / configuration.Steps;
            double pixelPerPoint = ActualWidth / (axisLength*2);

            for (int i = 0; i < configuration.Steps; i++) {
                double coordinate = Math.Round(axisLength - (i * stepSize),0);
                double posCoordinate = axisLength * pixelPerPoint - coordinate * pixelPerPoint;

                if (coordinate == 0)
                {
                    continue;
                }


                //Draw negative x/y labels
                FormattedText text = new FormattedText((-coordinate).ToString(), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 11, Brushes.Black);
                drawingContext.DrawText(text, new Point(posCoordinate - text.Width / 2, ActualWidth / 2 + 5)); 
                drawingContext.DrawText(text, new Point(ActualWidth / 2 + 5, ActualHeight - (posCoordinate + text.Height / 2)));
                //Draw positive x/y labels
                text = new FormattedText(coordinate.ToString(), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 11, Brushes.Black);
                drawingContext.DrawText(text, new Point(ActualWidth-(posCoordinate + text.Width / 2), ActualWidth / 2 + 5));
                //Draw y labels
                drawingContext.DrawText(text, new Point(ActualWidth / 2 + 5, posCoordinate - text.Height / 2));
                
            
            }
            
        }
        void DrawArc(DrawingContext drawingContext, Brush brush,
    Pen pen, Point start, Point end, float radius, SweepDirection direction)
        {
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            geometry.Figures.Add(figure);
            figure.StartPoint = start;
            figure.Segments.Add(new ArcSegment(end, new Size(radius,radius),
                0, false, direction, true));
            drawingContext.DrawGeometry(brush, pen, geometry);
        }
        #endregion

       
    }

    class Path
    {

        API.Interpolation.Point3D start;

        public API.Interpolation.Point3D Start
        {
            get { return start; }
            set { start = value; }
        }

        API.Interpolation.Point3D end;

        public API.Interpolation.Point3D End
        {
            get { return end; }
            set { end = value; }
        }

        InterpolationType type;

        public InterpolationType Type
        {
            get { return type; }
            set { type = value; }
        }

        bool dashed;

        public bool Dashed
        {
            get { return dashed; }
            set { dashed = value; }
        }

        SweepDirection direction;
        public SweepDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        float radius;

        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }
    }
}

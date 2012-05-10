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
using System.Windows.Ink;
using HTL.Grieskirchen.Edubot.API.Commands;
using HTL.Grieskirchen.Edubot.API.Adapters;
using HTL.Grieskirchen.Edubot.Settings;
using System.ComponentModel;
using HTL.Grieskirchen.Edubot.API.Interpolation;
using HTL.Grieskirchen.Edubot.API;

namespace HTL.Grieskirchen.Edubot.Controls
{
    /// <summary>
    /// Interaction logic for DrawingCanvas.xaml
    /// </summary>
    public partial class DrawingCanvas : InkCanvas
    {

        public DrawingCanvas()
            : base()
        {
            InitializeComponent();
            undoBuffer = new Stack<Memento>();
            redoBuffer = new Stack<Memento>();
            addMemento = true;
            index = 0;
            EditingMode = InkCanvasEditingMode.Select;
            StrokeCollected += AddMemento;
            StrokeErasing += AddMemento;
            Strokes.StrokesChanged += AddMemento;
            MouseLeftButtonDown += SetOrigin;
            MouseLeftButtonUp += AddStrokes;
            MouseMove += UpdateShape;
            VisualisationAdapter = new VirtualAdapter(Tool.VIRTUAL, 0, 0);
        }


        VirtualAdapter visualisationAdapter;

        public VirtualAdapter VisualisationAdapter
        {
            get { return visualisationAdapter; }
            set { visualisationAdapter = value;
            Remeasure();
            }
        }

        private VisualizationConfig configuration;

        public VisualizationConfig Configuration
        {
            get { return configuration; }
            set { configuration = value;
            configuration.PropertyChanged += ApplyConfiguration;
            }
        }

        private void ApplyConfiguration(object sender, PropertyChangedEventArgs args) {
            if (args.PropertyName == "Length")
            {
                visualisationAdapter.Length = configuration.Length;
            }
            if (args.PropertyName == "Length2") {

              
                visualisationAdapter.Length2 = configuration.Length2;
            }
            if (args.PropertyName == "MaxPrimaryAngle")
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
            if (args.PropertyName == "MinPrimaryAngle")
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
            if (args.PropertyName == "MaxSecondaryAngle")
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
            if (args.PropertyName == "MinSecondaryAngle")
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
            Remeasure();
            InvalidateVisual();
        }

        #region Undo/Redo
        Stack<Memento> undoBuffer;
        Stack<Memento> redoBuffer;
        bool addMemento;
        int index;

        class Memento
        {
            string operation;

            public string Operation
            {
                get { return operation; }
                set { operation = value; }
            }

            StrokeCollection strokes;

            public StrokeCollection Strokes
            {
                get { return strokes; }
                set { strokes = value; }
            }
        }

        public bool CanUndo {
            get { return undoBuffer.Count > 0; }
        }

        public bool CanRedo {
            get { return redoBuffer.Count > 0; }
        }

        private void AddMemento(object src, EventArgs e) {
            if (addMemento)
            {
                if (e is InkCanvasStrokeCollectedEventArgs)
                {
                    InkCanvasStrokeCollectedEventArgs icsce = (InkCanvasStrokeCollectedEventArgs)e;
                    undoBuffer.Push(new Memento() { Operation = "add", Strokes = new StrokeCollection() { icsce.Stroke } });
                    redoBuffer.Clear();
                }
                if (e is InkCanvasStrokeErasingEventArgs)
                {
                    InkCanvasStrokeErasingEventArgs icsee = (InkCanvasStrokeErasingEventArgs)e;
                    undoBuffer.Push(new Memento() { Operation = "del", Strokes = new StrokeCollection() { icsee.Stroke } });
                    redoBuffer.Clear();
                }
            }

        }

        public void Undo() {
            if (CanUndo)
            {
                addMemento = false;
                Memento lastAction = undoBuffer.Pop(); //undoBuffer.ElementAt(index - 1);
                if (lastAction.Operation == "add")
                {
                 
                    Strokes.Remove(lastAction.Strokes);
                }
                else
                {
                    Strokes.Add(lastAction.Strokes);
                }
                redoBuffer.Push(lastAction);
                addMemento = true;
                InvalidateVisual();
            }
        }

        public void Redo()
        {
            if (CanRedo)
            {
                addMemento = false;
                Memento nextAction = redoBuffer.Pop();
                if (nextAction.Operation == "add")
                {
                    Strokes.Add(nextAction.Strokes);
                }
                else
                {
                    Strokes.Remove(nextAction.Strokes);
                }
                undoBuffer.Push(nextAction);
                addMemento = true;
                InvalidateVisual();
                index++;
            }
        }

        #endregion

        #region Shape Drawing
        Point origin;
        InkCanvasDrawingMode drawingMode;
        bool displayGrid;

        public InkCanvasDrawingMode DrawingMode
        {
            get { return drawingMode; }
            set { drawingMode = value;
            if (value == InkCanvasDrawingMode.None)
            {
                ForceCursor = false;
                Cursor = null;
            }
            else
            {
                ForceCursor = true;
                Cursor = Cursors.Cross;
            }
            }
        }

        public bool DisplayGrid
        {
            get { return displayGrid; }
            set { displayGrid = value;
            InvalidateVisual();
            }
        }    

        public bool IsDrawingShape {
            get { return EditingMode == InkCanvasEditingMode.None; }
        }

        private void SetOrigin(object src, EventArgs e)
        {
            if (IsDrawingShape)
            {
                
                origin = Mouse.GetPosition(this);
            }
        }

        private void UpdateShape(object src, EventArgs e)
        {
            if (IsDrawingShape)
            {
                InvalidateVisual();
            }
        }

        private void AddStrokes(object src, EventArgs e) {
            if (IsDrawingShape)
            {
                Point currentPosition = Mouse.GetPosition(this);
                Stroke stroke;
                switch (drawingMode)
                {
                    case InkCanvasDrawingMode.Line:
                        StylusPointCollection line = new StylusPointCollection();
                        line.Add(new StylusPoint(origin.X, origin.Y));
                        line.Add(new StylusPoint(currentPosition.X, currentPosition.Y));
                        stroke = new Stroke(line);
                        Strokes.Add(stroke);
                        OnStrokeCollected(new InkCanvasStrokeCollectedEventArgs(stroke));
                        //buffer.Add(new Memento() { Operation = "add", Strokes = new StrokeCollection(){new Stroke(line)} });
                        //index++;
                        break;
                    case InkCanvasDrawingMode.Rectangle:
                        StylusPointCollection rect = new StylusPointCollection();
                        rect.Add(new StylusPoint(origin.X, origin.Y));
                        rect.Add(new StylusPoint(currentPosition.X, origin.Y));
                        rect.Add(new StylusPoint(currentPosition.X, currentPosition.Y));
                        rect.Add(new StylusPoint(origin.X, currentPosition.Y));
                        rect.Add(new StylusPoint(origin.X, origin.Y));
                        if (rect.Count > 0)
                        {
                            stroke = new Stroke(rect);
                            Strokes.Add(stroke);
                            OnStrokeCollected(new InkCanvasStrokeCollectedEventArgs(stroke));
                        }
                        //buffer.Add(new Memento() { Operation = "add", Strokes = new StrokeCollection() { new Stroke(rect) } });
                        //index++;
                        break;
                    case InkCanvasDrawingMode.Ellipse:
                        StylusPointCollection ellipse = new StylusPointCollection();
                        int radiusX = (int)(currentPosition.X - origin.X) / 2;
                        int radiusY = (int)(currentPosition.Y - origin.Y) / 2;
                        int segmentCount = (int)(Math.Sqrt(radiusX * radiusX + radiusY * radiusY)*4);
                        double dTheta = 2 * Math.PI / segmentCount;
                        double theta = 0;
                        Point center = new Point(origin.X + radiusX, origin.Y + radiusY);                        
                        int currentX = (int) center.X + radiusX;
                        int currentY = (int) center.Y;
                        for (int segment = 0; segment < segmentCount; segment++)
                        {
                            theta += dTheta;
                            currentX = (int) (center.X + radiusX * Math.Cos(theta));
                            currentY = (int) (center.Y + radiusY * Math.Sin(theta));
                            ellipse.Add(new StylusPoint(currentX, currentY));
                        }
                        if (ellipse.Count > 0)
                        {
                            stroke = new Stroke(ellipse);
                            Strokes.Add(stroke);
                            OnStrokeCollected(new InkCanvasStrokeCollectedEventArgs(stroke));
                        }
                        break;
                }
                
            }
        }

        public List<API.Commands.ICommand> GenerateMovementCommands() {
            List<API.Commands.ICommand> commands = new List<API.Commands.ICommand>();
            bool firstPoint;
            int sizePerQuadrant = (int)ActualHeight/2;
            Point3D prevPoint = visualisationAdapter.HomePoint;
            InterpolationStep[] kinResult;
            visualisationAdapter.AxisConfiguration = AxisConfiguration.Lefty;
            foreach (Stroke stroke in Strokes) {
                firstPoint = true;
                foreach (StylusPoint point in stroke.StylusPoints)
                {
                    int x = (int) point.X - sizePerQuadrant;
                    int y = (int)((ActualHeight - point.Y) - sizePerQuadrant);// -sizePerQuadrant;
                    kinResult = Kinematics.CalculateInverse(new Point3D(x, y, 0), visualisationAdapter.Length, visualisationAdapter.Length2, visualisationAdapter.VerticalToolRange, visualisationAdapter.Transmission);
                    if (!visualisationAdapter.AreAnglesValid(kinResult[(int)visualisationAdapter.AxisConfiguration].Alpha1, kinResult[(int)visualisationAdapter.AxisConfiguration].Alpha2, kinResult[(int)visualisationAdapter.AxisConfiguration].Alpha3)) {
                        AxisConfiguration otherConfig;
                        if (visualisationAdapter.AxisConfiguration == AxisConfiguration.Lefty)
                        {
                            otherConfig = AxisConfiguration.Righty;
                        }
                        else {
                            otherConfig = AxisConfiguration.Lefty;
                        }
                        if (visualisationAdapter.AreAnglesValid(kinResult[(int)otherConfig].Alpha1, kinResult[(int)otherConfig].Alpha2, kinResult[(int)otherConfig].Alpha3))
                        {
                            visualisationAdapter.AxisConfiguration = otherConfig;
                            commands.Add(new UseToolCommand(false));
                            commands.Add(new ChangeConfigurationCommand(otherConfig));
                            if (!firstPoint)
                                commands.Add(new UseToolCommand(true));
                        }
                    }
                    if (firstPoint)
                    {
                        //move to starting pointn of stroke without drawing a line --> Z = 0
                        commands.Add(new UseToolCommand(false));                        
                        commands.Add(new MVSCommand(new Point3D(x, y, 0)));
                        commands.Add(new UseToolCommand(true));
                        firstPoint = false;
                    }
                    else {
                        commands.Add(new MVSCommand(new Point3D(x, y, 0)));
                    }
                }
            }
            return commands;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.White, null, new Rect(0, 0, ActualWidth, ActualHeight));
            if (displayGrid)
            {
                RenderGrid(drawingContext);
            }
            RenderWorkingArea(drawingContext);
            //drawingContext.DrawEllipse(null, new Pen(Brushes.Gray, 1), new Point(300, 300), 300, 300);
            drawingContext.DrawLine(new Pen(Brushes.Gray, 1), new Point(0, ActualHeight / 2), new Point(ActualWidth, ActualHeight / 2));
            drawingContext.DrawLine(new Pen(Brushes.Gray, 1), new Point(ActualWidth / 2, 0), new Point(ActualWidth / 2, ActualHeight));
            if (IsDrawingShape && Mouse.LeftButton == MouseButtonState.Pressed){
                
                switch (drawingMode)
                {
                    case InkCanvasDrawingMode.Line:
                        RenderLine(drawingContext);
                        break;
                    case InkCanvasDrawingMode.Rectangle:
                        RenderRectangle(drawingContext);
                        break;
                    case InkCanvasDrawingMode.Ellipse:
                        RenderEllipse(drawingContext);
                        break;
                }
            }
            base.OnRender(drawingContext);

        }

        protected void RenderGrid(DrawingContext drawingContext){
            int stepSize = 10;
            for (int col = 0; col < ActualWidth / stepSize; col++)
            {
                drawingContext.DrawLine(new Pen(Brushes.LightGray, 1), new Point(0, stepSize * col), new Point(ActualWidth, stepSize * col));
            }
            for (int row = 0; row < ActualHeight / stepSize; row++)
            {
                drawingContext.DrawLine(new Pen(Brushes.LightGray, 1), new Point(stepSize * row, 0), new Point(stepSize * row, ActualHeight));
            }
        }

        protected void RenderLine(DrawingContext drawingContext)
        {
            Point currentPosition = Mouse.GetPosition(this);
            drawingContext.DrawLine(new Pen(Brushes.Black, DefaultDrawingAttributes.Width), origin, currentPosition);
        }

        protected void RenderRectangle(DrawingContext drawingContext) {
            Point currentPosition = Mouse.GetPosition(this);
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {

            }
            drawingContext.DrawRectangle(null, new Pen(Brushes.Black, DefaultDrawingAttributes.Width), new Rect(origin, currentPosition));
            
        }

        protected void RenderEllipse(DrawingContext drawingContext)
        {
            Point currentPosition = Mouse.GetPosition(this);
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {

            }
            int radiusX = (int)(currentPosition.X - origin.X) / 2;
            int radiusY = (int)(currentPosition.Y - origin.Y) / 2;
            drawingContext.DrawEllipse(null, new Pen(Brushes.Black, DefaultDrawingAttributes.Width), new Point(origin.X + radiusX, origin.Y + radiusY), Math.Abs(radiusX), Math.Abs(radiusY));
                        
        }

        protected void RenderWorkingArea(DrawingContext drawingContext)
        {
            if (visualisationAdapter != null)
            {
                float radius = visualisationAdapter.Length + visualisationAdapter.Length2;

                if (visualisationAdapter.MaxPrimaryAngle == float.MaxValue && visualisationAdapter.MaxSecondaryAngle == float.MaxValue && visualisationAdapter.MinPrimaryAngle == float.MinValue &&visualisationAdapter.MinSecondaryAngle == float.MinValue)
                {
                    drawingContext.DrawEllipse(null, new Pen(Brushes.Gray, 1), new Point(radius, radius), radius, radius);
                    drawingContext.DrawEllipse(null, new Pen(Brushes.Gray, 1), new Point(radius, radius), visualisationAdapter.Length - visualisationAdapter.Length2, visualisationAdapter.Length - visualisationAdapter.Length2);
                }
                else
                {
                    float radius1 = visualisationAdapter.Length;
                    float radius2 = visualisationAdapter.Length2;
//                    
                    Point startPoint = new Point(radius + radius1 * (float)Math.Cos(API.Interpolation.MathHelper.ConvertToRadians(visualisationAdapter.MaxPrimaryAngle)), radius + radius1 * (float)Math.Sin(API.Interpolation.MathHelper.ConvertToRadians(visualisationAdapter.MaxPrimaryAngle)));
                    Point endPoint = new Point(radius + radius1 * (float)Math.Cos(API.Interpolation.MathHelper.ConvertToRadians(visualisationAdapter.MinPrimaryAngle)), radius + radius1 * (float)Math.Sin(API.Interpolation.MathHelper.ConvertToRadians(visualisationAdapter.MinPrimaryAngle)));

                    drawingContext.DrawLine(new Pen(Brushes.Gray, 1), new Point(radius, radius), startPoint);
                    drawingContext.DrawLine(new Pen(Brushes.Gray, 1), new Point(radius, radius), endPoint);
                    
                    DrawArc(drawingContext, null, new Pen(Brushes.Gray, 1), startPoint, endPoint, new Size(radius1, radius1), visualisationAdapter.MaxPrimaryAngle+Math.Abs(visualisationAdapter.MinPrimaryAngle) >180, SweepDirection.Counterclockwise);

                    Point startPoint2 = new Point(startPoint.X + radius2 * (float)Math.Cos(API.Interpolation.MathHelper.ConvertToRadians(visualisationAdapter.MaxSecondaryAngle)), startPoint.Y + radius2 * (float)Math.Sin(API.Interpolation.MathHelper.ConvertToRadians(visualisationAdapter.MaxSecondaryAngle)));
                    Point endPoint2 = new Point(endPoint.X + radius2 * (float)Math.Cos(API.Interpolation.MathHelper.ConvertToRadians(visualisationAdapter.MinSecondaryAngle)), endPoint.Y + radius2 * (float)Math.Sin(API.Interpolation.MathHelper.ConvertToRadians(visualisationAdapter.MinSecondaryAngle)));
                    

                    drawingContext.DrawLine(new Pen(Brushes.Gray, 1), startPoint, startPoint2);
                    drawingContext.DrawLine(new Pen(Brushes.Gray, 1), endPoint, endPoint2);

                    //DrawArc(drawingContext, null, new Pen(Brushes.Gray, 1), startPoint2, startPoint, new Size(radius2, radius2), false, SweepDirection.Counterclockwise);
                    DrawArc(drawingContext, null, new Pen(Brushes.Gray, 1), startPoint2, endPoint2, new Size(radius, radius), visualisationAdapter.MaxSecondaryAngle + Math.Abs(visualisationAdapter.MinSecondaryAngle) > 180, SweepDirection.Counterclockwise);
                }
            }
        }

        void DrawArc(DrawingContext drawingContext, Brush brush,
    Pen pen, Point start, Point end, Size radius, bool isLargeArc, SweepDirection direction)
        {
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            geometry.Figures.Add(figure);
            figure.StartPoint = start;
            figure.Segments.Add(new ArcSegment(end, radius,
                0, isLargeArc, direction, true));
            drawingContext.DrawGeometry(brush, pen, geometry);
        }

        private void Remeasure() {
            addMemento = false;
            addMemento = true;
            this.Width = (visualisationAdapter.Length + visualisationAdapter.Length2) * 2;
            this.Height = (visualisationAdapter.Length + visualisationAdapter.Length2) * 2;
           
        }
        #endregion
    }
}

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
            buffer = new List<Memento>();
            index = 0;
            EditingMode = InkCanvasEditingMode.Select;
            StrokeCollected += AddMemento;
            StrokeErasing += AddMemento;
            Strokes.StrokesChanged += AddMemento;
            MouseLeftButtonDown += SetOrigin;
            MouseLeftButtonUp += AddStrokes;
            MouseMove += UpdateShape;
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
            if (args.PropertyName == "Length2") {
                
                VisualisationAdapter = new VirtualAdapter(Tool.VIRTUAL, float.Parse(configuration.Length), float.Parse(configuration.Length2),90,-90,45,-45);
            }
        }

        #region Undo/Redo
        List<Memento> buffer;
        int index;

        class Memento{
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
            get { return buffer.Count > 0 && index > 0; }
        }

        public bool CanRedo {
            get { return buffer.Count > 0 && index < buffer.Count; }
        }

        internal void CanUndoDelegate(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = buffer.Count > 0 && index > 0;
        }

        internal void CanRedoDelegate(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = buffer.Count > 0 && index < buffer.Count;
        }

        internal void UndoExecuted(object sender, ExecutedRoutedEventArgs e) {
            Undo();
        }

        internal void RedoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Redo();
        }

        private void AddMemento(object src, EventArgs e) {
            if (e is InkCanvasStrokeCollectedEventArgs)
            {
                InkCanvasStrokeCollectedEventArgs icsce = (InkCanvasStrokeCollectedEventArgs)e;
                buffer.Add(new Memento() { Operation = "add", Strokes = new StrokeCollection() { icsce.Stroke } });
                index++;

                if (buffer.Count > index)
                {
                    buffer.RemoveRange(index, buffer.Count - index);
                }
            }
            if (e is InkCanvasStrokeErasingEventArgs) {
                InkCanvasStrokeErasingEventArgs icsee = (InkCanvasStrokeErasingEventArgs)e;
                buffer.Add(new Memento() { Operation = "del", Strokes = new StrokeCollection() { icsee.Stroke } });
                index++;

                if (buffer.Count > index)
                {
                    buffer.RemoveRange(index, buffer.Count - index);
                }
            }
            //if (e is StrokeCollectionChangedEventArgs)
            //{
            //    //StrokeCollectionChangedEventArgs scce = (StrokeCollectionChangedEventArgs)e;
            //    //if (scce.Added.Count > 0)
            //    //{
            //    //    buffer.Add(new Memento() { Operation = "add", Strokes = scce.Added });
            //    //    index++;
            //    //}
            //    //if (scce.Removed.Count > 0)
            //    //{
            //    //    buffer.Add(new Memento() { Operation = "del", Strokes = scce.Removed });
            //    //    index--;
            //    //}
            //}

        }

        public void Undo() {
            if (CanUndo)
            {
                Memento lastAction = buffer.ElementAt(index - 1);
                if (lastAction.Operation == "add")
                {
                    Strokes.Remove(lastAction.Strokes);
                }
                else
                {
                    Strokes.Add(lastAction.Strokes);
                }
                InvalidateVisual();
                index--;
            }
        }

        public void Redo()
        {
            if (CanRedo)
            {
                Memento nextAction = buffer.ElementAt(index);
                if (nextAction.Operation == "add")
                {
                    Strokes.Add(nextAction.Strokes);
                }
                else
                {
                    Strokes.Remove(nextAction.Strokes);
                }
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
            set { drawingMode = value; }
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
            
            foreach (Stroke stroke in Strokes) {
                firstPoint = true;
                foreach (StylusPoint point in stroke.StylusPoints)
                {
                    int x = (int) point.X - sizePerQuadrant;
                    int y = (int)((ActualHeight - point.Y) - sizePerQuadrant);// -sizePerQuadrant;
                    if (firstPoint)
                    {
                        //move to starting pointn of stroke without drawing a line --> Z = 0
                        commands.Add(new UseToolCommand(false));
                        commands.Add(new MVSCommand(new API.Interpolation.Point3D(x, y, 0)));
                        commands.Add(new UseToolCommand(true));
                        firstPoint = false;
                    }
                    else {
                        commands.Add(new MVSCommand(new API.Interpolation.Point3D(x, y, 0)));
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
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                int x = (int)(currentPosition.X - origin.X);
                int y = (int)(currentPosition.Y - origin.Y);
                double angle;
                if (y != 0)
                {
                    angle = Math.Acos(x / Math.Abs(y)) * 180 / Math.PI;
                }
                else
                {
                    if (x > 0)
                        angle = 0;
                    else
                        angle = 180;
                }

                if ((angle > 60 && angle < 120) || (angle > 240 && angle < 300))
                {
                    currentPosition.X = origin.X;
                }
                else
                {
                    currentPosition.Y = origin.Y;
                }
            }
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

                if (visualisationAdapter.MaxPrimaryAngle == float.MaxValue && visualisationAdapter.MaxSecondaryAngle == float.MaxValue && visualisationAdapter.MinPrimaryAngle == float.MinValue && visualisationAdapter.MinSecondaryAngle == float.MinValue)
                {
                    drawingContext.DrawEllipse(null, new Pen(Brushes.Gray, 1), new Point(radius, radius), radius, radius);
                }
                else
                {
                    Point startPoint;
                    Point endPoint;

                    float xPrimMax = radius + radius * (float)Math.Cos(API.Interpolation.MathHelper.ConvertToRadians(visualisationAdapter.MaxPrimaryAngle));
                    float yPrimMax = radius + radius * (float)Math.Sin(API.Interpolation.MathHelper.ConvertToRadians(visualisationAdapter.MaxPrimaryAngle));
float xPrimMin = radius + radius * (float)Math.Cos(API.Interpolation.MathHelper.ConvertToRadians(visualisationAdapter.MinPrimaryAngle));
                    float yPrimMin = radius + radius * (float)Math.Sin(API.Interpolation.MathHelper.ConvertToRadians(visualisationAdapter.MinPrimaryAngle));
                    DrawArc(drawingContext, null, new Pen(Brushes.Gray, 1), new Point(xPrimMax, yPrimMax), new Point(xPrimMin, yPrimMin),new Size(radius,radius), visualisationAdapter.MaxPrimaryAngle+Math.Abs(visualisationAdapter.MinPrimaryAngle) > 180);

                    //drawingContext.DrawLine(new Pen(Brushes.Gray, 1), new Point(radius, radius), new Point(xPrimMax);
                    //drawingContext.DrawLine(new Pen(Brushes.Gray, 1), new Point(radius, radius), endPoint);

                    float xSecMax = radius + radius * (float)Math.Cos(API.Interpolation.MathHelper.ConvertToRadians(visualisationAdapter.MaxSecondaryAngle));
                    float ySecMax = radius + radius * (float)Math.Sin(API.Interpolation.MathHelper.ConvertToRadians(visualisationAdapter.MaxSecondaryAngle));
                    float xSecMin = radius + radius * (float)Math.Cos(API.Interpolation.MathHelper.ConvertToRadians(visualisationAdapter.MinSecondaryAngle));
                    float ySecMin = radius + radius * (float)Math.Sin(API.Interpolation.MathHelper.ConvertToRadians(visualisationAdapter.MinSecondaryAngle));
                    DrawArc(drawingContext, null, new Pen(Brushes.Gray, 1), new Point(xSecMax, ySecMax), new Point(xSecMin, ySecMin), new Size(visualisationAdapter.Length2, visualisationAdapter.Length2), visualisationAdapter.MaxSecondaryAngle > 180);
                   
                    //startPoint = new Point(x, y);
                    //x = radius + radius * (float)Math.Cos(API.Interpolation.MathHelper.ConvertToRadians(visualisationAdapter.MinPrimaryAngle + visualisationAdapter.MinSecondaryAngle));
                    //y = radius + radius * (float)Math.Sin(API.Interpolation.MathHelper.ConvertToRadians(visualisationAdapter.MinPrimaryAngle + visualisationAdapter.MinSecondaryAngle));
                    //endPoint = new Point(x, y);
                    //drawingContext.DrawLine(new Pen(Brushes.Gray, 1), new Point(radius, radius), startPoint);
                    //drawingContext.DrawLine(new Pen(Brushes.Gray, 1), new Point(radius, radius), endPoint);
                    
                    //DrawArc(drawingContext, null, new Pen(Brushes.Gray, 1), startPoint, endPoint, new Size(radius, radius), visualisationAdapter.MaxPrimaryAngle+visualisationAdapter.MaxSecondaryAngle + Math.Abs(visualisationAdapter.MinPrimaryAngle) + Math.Abs(visualisationAdapter.MinSecondaryAngle) >180);
                }
            }
        }


        //http://blogs.vertigo.com/personal/ralph/Blog/Lists/Posts/Post.aspx?ID=5
        void DrawArc(DrawingContext drawingContext, Brush brush,
    Pen pen, Point start, Point end, Size radius, bool isLargeArc)
        {
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            geometry.Figures.Add(figure);
            figure.StartPoint = start;
            figure.Segments.Add(new ArcSegment(end, radius,
                0, isLargeArc, SweepDirection.Counterclockwise, true));
            drawingContext.DrawGeometry(brush, pen, geometry);
        }

        private void Remeasure() {
            this.Width = (visualisationAdapter.Length + visualisationAdapter.Length2) * 2;
            this.Height = (visualisationAdapter.Length + visualisationAdapter.Length2) * 2;
        }
        #endregion
    }
}

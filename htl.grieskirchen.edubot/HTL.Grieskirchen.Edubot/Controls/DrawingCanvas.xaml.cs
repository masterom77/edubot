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
            MouseLeftButtonDown += SetOrigin;
            MouseLeftButtonUp += AddStrokes;
            MouseMove += UpdateShape;
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

        private void AddMemento(object src, EventArgs e) {
            if (e is InkCanvasStrokeCollectedEventArgs)
            {
                InkCanvasStrokeCollectedEventArgs icsce = (InkCanvasStrokeCollectedEventArgs)e;
                buffer.Add(new Memento() { Operation = "add", Strokes = new StrokeCollection() { icsce.Stroke } });
                index++;
            }
            if (e is InkCanvasStrokeErasingEventArgs) {
                InkCanvasStrokeErasingEventArgs icsee = (InkCanvasStrokeErasingEventArgs)e;
                buffer.Add(new Memento() { Operation = "del", Strokes = new StrokeCollection() { icsee.Stroke } });
                index++;
            }
            //if (e is StrokeCollectionChangedEventArgs) {
            //    StrokeCollectionChangedEventArgs scce = (StrokeCollectionChangedEventArgs)e;
            //    if (scce.Added.Count > 0)
            //    {
            //        buffer.Add(new Memento() { Operation = "add", Strokes = scce.Added });
            //        index++;
            //    }
            //    if (scce.Removed.Count > 0)
            //    {
            //        buffer.Add(new Memento() { Operation = "del", Strokes = scce.Removed });
            //        index++;
            //    }
            //}
            if (buffer.Count > index)
            {
                buffer.RemoveRange(index, buffer.Count - index);
            }

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
                switch (drawingMode)
                {
                    case InkCanvasDrawingMode.Line:
                        StylusPointCollection line = new StylusPointCollection();
                        line.Add(new StylusPoint(origin.X, origin.Y));
                        line.Add(new StylusPoint(currentPosition.X, currentPosition.Y));
                        Strokes.Add(new Stroke(line));
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
                        Strokes.Add(new Stroke(rect));
                        //buffer.Add(new Memento() { Operation = "add", Strokes = new StrokeCollection() { new Stroke(rect) } });
                        //index++;
                        break;
                    case InkCanvasDrawingMode.Ellipse:
                        StylusPointCollection ellipse = new StylusPointCollection();
                        int radiusX = (int)(currentPosition.X - origin.X) / 2;
                        int radiusY = (int)(currentPosition.Y - origin.Y) / 2;
                        int segmentCount = (int)((radiusX * 2 + radiusY * 2) * 1.5);
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
                        Strokes.Add(new Stroke(ellipse));
                        break;
                }
                
            }
        }

        public List<MoveCommand> GenerateMovementCommands() {
            List<MoveCommand> commands = new List<MoveCommand>();
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
                        commands.Add(new MoveCommand(new API.Point3D(x, y, 0)));
                        firstPoint = false;
                    }
                    else {
                        commands.Add(new MoveCommand(new API.Point3D(x, y, 1)));
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
            drawingContext.DrawEllipse(null, new Pen(Brushes.Gray, 1), new Point(300, 300), 300, 300);
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
            //if (Keyboard.IsKeyDown(Key.LeftShift))
            //{
            //    int x = (int) (currentPosition.X - origin.X);
            //    int y = (int) (currentPosition.Y - origin.Y);
            //    double angle;
            //    if (y != 0)
            //    {
            //        angle = Math.Acos(x / Math.Abs(y)) * 180 / Math.PI;
            //    }
            //    else {
            //        if (x > 0)
            //            angle = 0;
            //        else
            //            angle = 180;
            //    }

            //        if ((angle > 60 && angle < 120) || (angle > 240 && angle < 300))
            //        {
            //            currentPosition.X = origin.X;
            //        }
            //        else
            //        {
            //            currentPosition.Y = origin.Y;
            //        }
            //}
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
        #endregion
    }
}

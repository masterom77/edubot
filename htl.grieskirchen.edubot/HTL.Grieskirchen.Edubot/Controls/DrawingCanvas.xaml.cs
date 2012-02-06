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

namespace HTL.Grieskirchen.Edubot.Controls
{
    /// <summary>
    /// Interaction logic for DrawingCanvas.xaml
    /// </summary>
    public partial class DrawingCanvas : InkCanvas
    {
        List<Memento> buffer;
        int index;

        class Memento{
            string operation;

            public string Operation
            {
                get { return operation; }
                set { operation = value; }
            }

            Stroke stroke;

            public Stroke Stroke
            {
                get { return stroke; }
                set { stroke = value; }
            }
        }

        public DrawingCanvas() : base()
        {
            InitializeComponent();
            buffer = new List<Memento>();
            index = 0;
            StrokeCollected += AddMemento;
            StrokeErasing += AddMemento;
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
                buffer.Add(new Memento() { Operation = "add", Stroke = icsce.Stroke });
            }
            else {
                InkCanvasStrokeErasingEventArgs icsee = (InkCanvasStrokeErasingEventArgs)e;
                buffer.Add(new Memento() { Operation = "del", Stroke = icsee.Stroke });
            }
            index++;
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
                    Strokes.Remove(lastAction.Stroke);
                }
                else
                {
                    Strokes.Add(lastAction.Stroke);
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
                    Strokes.Add(nextAction.Stroke);
                }
                else
                {
                    Strokes.Remove(nextAction.Stroke);
                }
                InvalidateVisual();
                index++;
            }
        }


    }
}

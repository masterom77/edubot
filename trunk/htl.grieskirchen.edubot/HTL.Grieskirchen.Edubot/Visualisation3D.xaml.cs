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

namespace HTL.Grieskirchen.Edubot
{
    /// <summary>
    /// Interaction logic for Visualisation3D.xaml
    /// </summary>
    public partial class Visualisation3D : UserControl
    {
        public Visualisation3D()
        {
            InitializeComponent();

            posSecondaryEngine = MeshSecondaryEngine.Content.Bounds;
            AnglePrimaryAxis = 0;
            AnglePrimaryAxis = 0;
        }

        private Rect3D posSecondaryEngine;
        private double anglePrimaryAxis;
        private double angleSecondaryAxis;
        private float primaryAxisSpeed;
        private long primaryAxisTicks;
        private float secondaryAxisSpeed;
        private long secondaryAxisTicks;
        delegate void updateCallback();


        public void MoveAnglePrimaryAxis(long ticks, float speed) {
            primaryAxisSpeed = speed*40 / 16;
            primaryAxisTicks = ticks / 16;

            new System.Threading.Thread(MoveAnglePrimaryAxisAsync).Start();
        }

        public void MoveAngleSecondaryAxis(long ticks, float speed)
        {
            secondaryAxisSpeed = speed*40 / 16;
            secondaryAxisTicks = ticks / 16;

            new System.Threading.Thread(MoveAngleSecondaryAxisAsync).Start();
        }

        private void MoveAnglePrimaryAxisAsync() {
            updateCallback updateAngle = new updateCallback(UpdatePrimaryAxis);
           
            for (int i = 0; i < primaryAxisTicks; i++)
            {
                
                Dispatcher.Invoke(updateAngle);
                System.Threading.Thread.Sleep(Convert.ToInt32(1/primaryAxisSpeed*1000));
            }
        }

        private void MoveAngleSecondaryAxisAsync()
        {
            updateCallback updateAngle = new updateCallback(UpdateSecondaryAxis);
            for (int i = 0; i < secondaryAxisTicks; i++)
            {

                Dispatcher.Invoke(updateAngle);
                System.Threading.Thread.Sleep(Convert.ToInt32(1 / secondaryAxisSpeed * 1000));
            }
        }

        private void UpdatePrimaryAxis() {
            AnglePrimaryAxis += 0.9;
        }
        private void UpdateSecondaryAxis()
        {
            AngleSecondaryAxis += 0.9;
        }
     

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
                MeshPen2.Transform = transformGroup;
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
                Transform3DGroup transformGroup = new Transform3DGroup();
                transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), AnglePrimaryAxis)));
                transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), value), new Point3D(posSecondaryEngine.Location.X + posSecondaryEngine.SizeX / 2, posSecondaryEngine.Location.Y - posSecondaryEngine.SizeY, posSecondaryEngine.Location.Z + posSecondaryEngine.SizeZ / 2)));


                MeshPen.Transform = transformGroup;
                MeshPen2.Transform = transformGroup;
                MeshSecondaryAxis.Transform = transformGroup;

                angleSecondaryAxis = value;
            }
        }
    }
}

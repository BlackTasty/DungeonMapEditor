using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DungeonMapEditor.Controls
{
    /// <summary>
    /// Interaction logic for ClipCanvas.xaml
    /// </summary>
    public partial class ClipCanvas : Canvas
    {
        #region ClipX dependency property
        public double ClipX
        {
            get { return (double)GetValue(ClipXProperty); }
            set { SetValue(ClipXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClipX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClipXProperty =
            DependencyProperty.Register("ClipX", typeof(double), typeof(ClipCanvas), new PropertyMetadata(5));
        #endregion

        #region ClipY dependency property
        public double ClipY
        {
            get { return (double)GetValue(ClipYProperty); }
            set { SetValue(ClipYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClipY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClipYProperty =
            DependencyProperty.Register("ClipY", typeof(double), typeof(ClipCanvas), new PropertyMetadata(5));
        #endregion

        #region GridBackground
        public ImageSource GridBackground
        {
            get { return (ImageSource)GetValue(GridBackgroundProperty); }
            set { SetValue(GridBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridBackgroundProperty =
            DependencyProperty.Register("GridBackground", typeof(ImageSource), typeof(ClipCanvas), new PropertyMetadata(null));
        #endregion

        public ClipCanvas()
        {
            InitializeComponent();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}

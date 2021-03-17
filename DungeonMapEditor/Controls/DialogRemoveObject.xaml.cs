using DungeonMapEditor.Core.Dialog;
using DungeonMapEditor.ViewModel;
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
    /// Interaction logic for DialogRemoveObject.xaml
    /// </summary>
    public partial class DialogRemoveObject : Border
    {
        public event EventHandler<DialogButtonClickedEventArgs> DialogCompleted;

        private object data;

        public DialogRemoveObject(string name)
        {
            InitializeComponent();
            (DataContext as DialogRemoveObjectViewModel).Name = name;
        }

        public DialogRemoveObject(string name, object data) : this(name)
        {
            this.data = data;
        }

        private void CreateFloor_Click(object sender, RoutedEventArgs e)
        {
            OnDialogCompleted(new DialogButtonClickedEventArgs(DialogResult.OK, data));
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            OnDialogCompleted(new DialogButtonClickedEventArgs(DialogResult.Abort));
        }

        protected virtual void OnDialogCompleted(DialogButtonClickedEventArgs e)
        {
            DialogCompleted?.Invoke(this, e);
        }
    }
}

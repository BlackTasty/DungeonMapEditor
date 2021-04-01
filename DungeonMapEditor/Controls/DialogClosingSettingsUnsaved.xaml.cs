using DungeonMapEditor.Core.Dialog;
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
    /// Interaction logic for DialogClosingSettingsUnsaved.xaml
    /// </summary>
    public partial class DialogClosingSettingsUnsaved : Border, IClosingDialog
    {
        public event EventHandler<ClosingUnsavedDialogButtonClickedEventArgs> DialogCompleted;

        public DialogClosingSettingsUnsaved()
        {
            InitializeComponent();
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            OnDialogCompleted(new ClosingUnsavedDialogButtonClickedEventArgs(null, null, DialogResult.Yes));
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            OnDialogCompleted(new ClosingUnsavedDialogButtonClickedEventArgs(null, null, DialogResult.No));
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            OnDialogCompleted(new ClosingUnsavedDialogButtonClickedEventArgs(DialogResult.Abort));
        }

        protected virtual void OnDialogCompleted(ClosingUnsavedDialogButtonClickedEventArgs e)
        {
            DialogCompleted?.Invoke(this, e);
        }
    }
}

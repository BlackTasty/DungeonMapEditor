using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Events;
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

namespace DungeonMapEditor.UI
{
    /// <summary>
    /// Interaction logic for ProjectOverview.xaml
    /// </summary>
    public partial class ProjectOverview : TabControl
    {
        public event EventHandler<NameChangedEventArgs> ProjectNameChanged;

        public ProjectOverview() : this(new ProjectFile("Untitled dungeon"))
        {
        }

        public ProjectOverview(ProjectFile projectFile)
        {
            InitializeComponent();
            ProjectOverviewViewModel vm = DataContext as ProjectOverviewViewModel;
            vm.ProjectFile = projectFile;
            vm.ProjectNameChanged += ProjectOverview_ProjectNameChanged;
        }

        public string GetProjectName()
        {
            return (DataContext as ProjectOverviewViewModel).ProjectFile?.Name;
        }

        private void ProjectOverview_ProjectNameChanged(object sender, NameChangedEventArgs e)
        {
            OnProjectNameChanged(e);
        }

        private void AddFloor_Click(object sender, RoutedEventArgs e)
        {
            OpenFloorPlan(new FloorPlan());
        }

        private void FloorDataGridRow_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                if (row.DataContext is FloorAssignment floorAssignment)
                {
                    OpenFloorPlan(floorAssignment.FloorPlan);
                }
            }
        }

        private void OpenFloorPlan(FloorPlan floorPlan)
        {
            FloorPlanGrid floorPlanGrid = new FloorPlanGrid(floorPlan);
            floorPlanGrid.FloorNameChanged += FloorPlanGrid_FloorNameChanged;

            tabControl.SelectedIndex = AddTab(floorPlanGrid, floorPlan.Name, Guid.NewGuid().ToString());
        }

        private void FloorPlanGrid_FloorNameChanged(object sender, NameChangedEventArgs e)
        {
            if (sender is FloorPlanGrid floorPlanGrid)
            {
                TabItem target = tabControl.Items.OfType<TabItem>().FirstOrDefault(x => x.Tag == floorPlanGrid.Tag);
                if (target != null && target.Header is StackPanel headerStack)
                {
                    TextBlock headerText = headerStack.Children.OfType<TextBlock>().FirstOrDefault();

                    if (headerText != null)
                    {
                        headerText.Text = e.NewName;
                    }
                }
            }
        }

        private int AddTab(FrameworkElement element, string headerString, object tabItemTag = null)
        {
            StackPanel header = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            Button tabCloseButton = new Button()
            {
                Content = "x",
                Margin = new Thickness(8, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(4, 0, 4, 0)
            };
            tabCloseButton.Click += TabCloseButton_Click;

            header.Children.Add(new TextBlock() { Text = headerString });
            header.Children.Add(tabCloseButton);

            TabItem tabItem = new TabItem
            {
                Header = header,
                Content = element,
                Tag = tabItemTag
            };
            tabCloseButton.Tag = tabItem;

            tabControl.Items.Add(tabItem);

            return tabControl.Items.Count - 1;
        }

        protected virtual void OnProjectNameChanged(NameChangedEventArgs e)
        {
            ProjectNameChanged?.Invoke(this, e);
        }

        private void TabCloseButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is TabItem targetTab)
            {
                if (targetTab.Content is FloorPlanGrid floorPlanGrid)
                {
                    floorPlanGrid.FloorNameChanged -= FloorPlanGrid_FloorNameChanged;
                }

                tabControl.Items.Remove(targetTab);
            }
        }
    }
}

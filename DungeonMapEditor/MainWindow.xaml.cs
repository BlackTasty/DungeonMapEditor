using DungeonMapEditor.Core;
using DungeonMapEditor.Core.Enum;
using DungeonMapEditor.Core.Events;
using DungeonMapEditor.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

namespace DungeonMapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AddTab(new HomeScreen(), "Home");
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

            if (tabItem.Content is HomeScreen homeScreen)
            {
                homeScreen.SelectionMade += HomeScreen_SelectionMade;
            }

            return tabControl.Items.Count - 1;
        }

        private void HomeScreen_SelectionMade(object sender, HomeScreenSelectionMadeEventArgs e)
        {
            int selectedTabIndex = -1;
            switch (e.Selection)
            {
                case HomeScreenSelectionType.NewProject:
                    ProjectOverview projectOverview = new ProjectOverview();
                    projectOverview.Tag = Guid.NewGuid().ToString();
                    projectOverview.ProjectNameChanged += ProjectOverview_ProjectNameChanged;
                    selectedTabIndex = AddTab(projectOverview, projectOverview.GetProjectName(), projectOverview.Tag);
                    break;
                case HomeScreenSelectionType.LoadProject:
                    break;
                case HomeScreenSelectionType.OpenTileManager:
                    selectedTabIndex = AddTab(new TileManager(), "Tile manager");
                    break;
            }

            if (selectedTabIndex >= 0)
            {
                tabControl.SelectedIndex = selectedTabIndex;
            }
        }

        private void ProjectOverview_ProjectNameChanged(object sender, NameChangedEventArgs e)
        {
            if (sender is ProjectOverview projectOverview)
            {
                TabItem target = tabControl.Items.OfType<TabItem>().FirstOrDefault(x => x.Tag == projectOverview.Tag);
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

        private void TabCloseButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is TabItem targetTab)
            {
                if (targetTab.Content is HomeScreen homeScreen)
                {
                    homeScreen.SelectionMade -= HomeScreen_SelectionMade;
                }
                else if (targetTab.Content is ProjectOverview projectOverview)
                {
                    projectOverview.ProjectNameChanged -= ProjectOverview_ProjectNameChanged;
                }
                tabControl.Items.Remove(targetTab);
            }
        }
    }
}

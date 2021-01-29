using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Vim.LinqArray;

namespace Vim.Explorer.Plugin
{
    /// <summary>
    /// Interaction logic for SliderView.xaml
    /// </summary>
    public partial class SliderListView : Window
    {
        public enum GroupingType
        {
            Category,
            Family,
            Room,
            Level,
            Model,
            Workset,
            DesignOption,
            Assembly,
            Element,
            Node,
        }

        public SliderListView()
        {
            InitializeComponent();
            foreach (var gt in Enum.GetValues(typeof(GroupingType)))
            {
                GroupingComboBox.Items.Add(gt);
            }
            GroupingComboBox.SelectedIndex = 0;
            GroupingComboBox.SelectionChanged += GroupingComboBox_SelectionChanged;

            ListBox.SelectionChanged += ListBox_SelectionChanged;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = ListBox.SelectedIndex;
            if (Slider.Value != index)
                Slider.Value = index;
        }

        private void GroupingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Init(Helper);
            Slider.Value = 0;
        }

        public VimHelper Helper { get; set; }

        public int[] Ids { get; set; }

        public static int GetRoom(VimSceneNode node)
        {
            var r = node?.Element?._Room.Index ?? -1;
            if (r >= 0) return r;
            r = node?.FamilyInstance?._FromRoom.Index ?? -1;
            if (r >= 0) return r;
            return node?.FamilyInstance?._ToRoom.Index ?? -1;
        }

        public IArray<int> NodesToId(VimScene vim, GroupingType gt)
        {
            switch (gt)
            {
                case GroupingType.Category:
                    return vim.VimNodes.Select(n => n.Category?.Index ?? -1);
                case GroupingType.Family:
                    return vim.VimNodes.Select(n => n.Family?.Index ?? -1);
                case GroupingType.Room:
                    return vim.VimNodes.Select(GetRoom);
                case GroupingType.Level:
                    return vim.VimNodes.Select(n => n.Element?._Level.Index ?? -1);
                case GroupingType.Model:
                    return vim.VimNodes.Select(n => n.Element?._Model.Index ?? -1);
                case GroupingType.Workset:
                    return vim.VimNodes.Select(n => n.Element?._Workset.Index ?? -1);
                case GroupingType.DesignOption:
                    return vim.VimNodes.Select(n => n.Element?._DesignOption.Index ?? -1);
                case GroupingType.Assembly:
                    return vim.VimNodes.Select(n => n.Element?._AssemblyInstance.Index ?? -1);
                case GroupingType.Element:
                    return vim.VimNodes.Select(n => n.Element?.Index ?? -1);
                case GroupingType.Node:
                default:
                    return vim.VimNodes.Select(n => n.Id);
            }
        }

        public IEnumerable<string> GroupingNames(VimScene vim, GroupingType gt)
        {
            switch (gt)
            {
                // TODO: this should only show names where we can find a node with geometry that uses it. 
                case GroupingType.Category:
                    return vim.Model.CategoryList.Select(x => x.Name ?? "<unnamed>").ToEnumerable();
                case GroupingType.Family:
                    return vim.Model.FamilyList.Select(x => x.Element?.Name ?? "<unnamed>").ToEnumerable();
                case GroupingType.Room:
                    return vim.Model.RoomList.Select(x => x.Element?.Name ?? "<unnamed>").ToEnumerable();
                case GroupingType.Level:
                    return vim.Model.LevelList.Select(x => x.Element?.Name ?? "<unnamed>").ToEnumerable();
                case GroupingType.Model:
                    return vim.Model.ModelList.Select(x => x.Title ?? "<unnamed>").ToEnumerable();
                case GroupingType.Workset:
                    return vim.Model.WorksetList.Select(x => x.Element?.Name ?? "<unnamed>").ToEnumerable();
                case GroupingType.DesignOption:
                    return vim.Model.DesignOptionList.Select(x => x.Element?.Name ?? "<unnamed>").ToEnumerable();
                case GroupingType.Assembly:
                    return vim.Model.AssemblyInstanceList.Select(x => x.Element?.Name ?? "<unnamed>").ToEnumerable();
                case GroupingType.Element:
                    return vim.Model.ElementList.Select(x => x?.Name ?? "<unnamed>").ToEnumerable();
                case GroupingType.Node:
                default:
                    return vim.Model.NodeList.Select(x => x.Element?.Name ?? "<unnamed>").ToEnumerable();
            }
        }

        public void Init(VimHelper helper)
        {
            Show();
            Helper = helper;
            var gt = (GroupingType)GroupingComboBox.SelectedIndex;
            var names = GroupingNames(helper.Vim, gt).ToList();
            ListBox.ItemsSource = names;
            Slider.Maximum = names.Count;
            Slider.SmallChange = 1;
            Slider.LargeChange = 5;
            Ids = NodesToId(helper.Vim, gt).ToArrayInParallel();
            Slider.ValueChanged += Slider_ValueChanged;
        }

        public void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var val = (int)Slider.Value;
            if (ListBox.SelectedIndex != val)
                ListBox.SelectedIndex = val;
            Helper.ShowNodes(n => Ids[n.Id] == val);
        }
    }
}

using MapEditor.Command;
using MapEditor.Model.Property;
using MapEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
using System.Windows.Shapes;

namespace MapEditor.Model.MapItems
{
    public class NodeModel : MapItem
    {
        public double editor_x { 
            get { return can_move_x; }
            set
            {
                can_move_x = value;
                // X 좌료 변경되도록 ? 
                //X = can_move_x - (Width / 2);
                OnPropertyChanged(nameof(editor_x));
            }
         }
        public double editor_y
        {
            get { return can_move_y; }
            set
            {
                can_move_y = value;

                //Y = can_move_y - (Height / 2);
                OnPropertyChanged(nameof(editor_y));
            }
        }
        public double X
        {
            get { return view_position_x; }
            set
            {
                view_position_x = value;
                OnPropertyChanged(nameof(X));
            }
        }
        public double Y
        {
            get { return view_position_y; }
            set
            {
                view_position_y = value;
                OnPropertyChanged(nameof(Y));
            }
        }

        public double Width { get; set; } = 16;
        public double Height { get; set; } = 16;
        public string Fill { get; set; } = "Red";
        public string Stroke { get; set; } = "Black";
        public double StrokeThickness { get; set; } = 2;

        public string real_x { get; set; } = "";
        public string real_y { get; set; } = "";

        public string whirls { get; set; } = "";
        // $"{name}"
        public ICommand ClickNode { get; private set; }
        public ICommand DragNode { get; private set; }
        public ICommand DropNode { get; private set; }
        public ICommand DeleteNode { get; private set; }


        private double can_move_x { get; set; }
        private double can_move_y { get; set; }

        private double view_position_x { get; set; }
        private double view_position_y { get; set; }

        public NodeModel(double x, double y)
        {
            itemName = MapItemNames.node;
            ClickNode = new DelegateCommand((object obj) => SelectNodeItem(obj));
            DragNode = new DelegateCommand((object obj) => DragNodeItem(obj));
            DropNode = new DelegateCommand((object obj) => DropNodeItem(obj));
            DeleteNode = new DelegateCommand((object obj) => DeleteNodeInShape(obj));
            editor_x = x;
            editor_y = y;
            // 가운데 값 맞추기 위함.
            X = editor_x - (Width/2);
            Y = editor_y - (Height/2);
        }

        //public NodeModel(string name, double editor_x, double editor_y, string real_x, string real_y, string whirls)
        public NodeModel(string[] words)
        {
            itemName = MapItemNames.node;
            this.name = words[0];
            this.real_x = words[1];
            this.real_y = words[2];
            this.editor_x = Double.Parse(words[3]);
            this.editor_y = Double.Parse(words[4]);
            this.whirls = words[5];
            ClickNode = new DelegateCommand((object obj) => SelectNodeItem(obj));
            DragNode = new DelegateCommand((object obj) => DragNodeItem(obj));
            DropNode = new DelegateCommand((object obj) => DropNodeItem(obj));
            DeleteNode = new DelegateCommand((object obj) => DeleteNodeInShape(obj));
            X = editor_x - (Width / 2);
            Y = editor_y - (Height / 2);
        }

        private void SelectNodeItem(object param)
        {
            Type thisType = param.GetType();
            //System.Diagnostics.Debug.WriteLine($"type :  ({thisType.Name})");
            if (param is NodeModel node)
            {
                MainViewModel.selectedNM = node;
                MainViewModel.selectedEG = null;
                MainViewModel.Properties.Clear();

                SetProperties(node);

            }
        }
        private void DragNodeItem(object param)
        {            
            if (param is NodeModel node)
            {
                node.editor_x = Math.Round(MainViewModel.NowMouseLocation.X);
                node.editor_y = Math.Round(MainViewModel.NowMouseLocation.Y);
                // 가운데 값 맞추기 위함.
                //node.X = editor_x - (Width / 2);
                //node.Y = editor_y - (Height / 2);
            }
        }

        private void DropNodeItem(object param)
        {
            System.Diagnostics.Debug.WriteLine($"move position : ({MainViewModel.NowMouseLocation})");
            if (param is NodeModel node)
            {
                //node.editor_x = Math.Round(MainViewModel.NowMouseLocation.X);
                //node.editor_y = Math.Round(MainViewModel.NowMouseLocation.Y);
                // 가운데 값 맞추기 위함.
                System.Diagnostics.Debug.WriteLine($"move position : ({MainViewModel.NowMouseLocation})");
                node.X = node.editor_x - (node.Width / 2);
                node.Y = node.editor_y - (node.Height / 2);
            }
        }

        public string setCfg()
        {
            string reurnString = "";
            string returnName = name;
            string returnReal_x = real_x;
            string returnReal_y = real_y;
            string returnEditor_x = editor_x + "";
            string returnEditor_y = editor_y + "";
            string returnWhirls = whirls;


            if (returnReal_x == "")
            {
                returnReal_x = "\t";
            }
            if (returnReal_y == "")
            {
                returnReal_y = "\t";
            }
            if (returnWhirls == "")
            {
                returnWhirls = "\t";
            }
            if (returnName == "")
            {
                returnName = "\t";
            }
            reurnString = $"{returnName} {returnReal_x} {returnReal_y} {returnEditor_x} {returnEditor_y} {returnWhirls}";
            return reurnString;
        }
        public static NodeModel FileToNodeModel(string lines)
        {
            //TODO :
            // string to nodemodel
            //
            string replace = lines.Replace(" ", ",");
            string[] words = replace.Split(',');            

            return new NodeModel(words);
        }
        private void DeleteNodeInShape(object param)
        {          
            if (param is NodeModel node)
            {
                foreach (var item in MainViewModel.arrows)
                {
                    if (item.StartNode.Equals(node) || item.EndNode.Equals(node))
                    {
                        //item.StartNodeName = MI.name;
                        MessageBox.Show("Edge have this node. please delete Edge first. then excute it ");
                        return;
                    }
                }
                MainViewModel.shapes.Remove(node);
                MainViewModel.Shapes = MainViewModel.shapes;
                MainViewModel.selectedNM = null;
            }
        }

    }
}

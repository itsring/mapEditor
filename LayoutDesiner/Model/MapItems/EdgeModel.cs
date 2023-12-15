using MapEditor.Command;
using MapEditor.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace MapEditor.Model.MapItems
{
    public class EdgeModel : MapItem
    {
        public NodeModel StartNode { get; set; }
        public NodeModel EndNode { get; set; }

        //public double arrowHead_1_x { get; set; }
        //public double arrowHead_1_y { get; set; }
        //public double arrowHead_2_x { get; set; }
        //public double arrowHead_2_y { get; set; }

        public string distance { get; set; } = "";
        public string rail_type { get; set; } = "";
        public string direction_Left_or_Right { get; set; } = "";
        private string settingStartNodeName{ get; set; }
        private string settingEndNodeName { get; set; }
        //public PointCollection point { get; set; }
        public ICommand ClickEdge { get; private set; }
        public ICommand DeleteEdge { get; set; }

        public EdgeModel(NodeModel start, NodeModel end)
        {
            StartNode = start;
            EndNode = end;
            this.itemName = MapItemNames.edge;
            System.Diagnostics.Debug.WriteLine($"Clicked!!!! Original StartNode: ({StartNode.X}, {StartNode.Y}) EndNode: ({EndNode.X}, {EndNode.Y})");
            ClickEdge = new DelegateCommand((object obj) => SelectNodeItem(obj));
            DeleteEdge = new DelegateCommand((object obj) => DeleteNodeInShape(obj));
            //point = setArrow();
        }
        private void SelectNodeItem(object param)
        {
            Type thisType = param.GetType();
            if (param is EdgeModel node)
            {
                MainViewModel.selectedEG = node;
                MainViewModel.selectedNM = null;
                MainViewModel.Properties.Clear();

                SetProperties(node);
            }
        }
        //private PointCollection setArrow()
        //{
        //    Vector dirVector = new Vector(EndNode.X - StartNode.X, EndNode.Y - StartNode.Y);
        //    dirVector.Normalize();

        //    // Rotate direction vector by 135 degrees to get one side of the arrow head.
        //    Vector rotateVector = new Vector(dirVector.Y, -dirVector.X);
        //    rotateVector *= Math.Sqrt(2) / 2;

        //    double arrowHeadSize = 10;
        //    // Calculate points of the arrow head.
        //    Point ptEnd = new Point(EndNode.X, EndNode.Y);
        //    Point ptArrowHeadA = ptEnd + (rotateVector * arrowHeadSize);
        //    rotateVector.Negate();
        //    Point ptArrowHeadB = ptEnd + (rotateVector * arrowHeadSize);
        //    // Set points to polygon (arrow head).
            
        //    return new PointCollection { ptArrowHeadA, ptEnd, ptArrowHeadB };
        //}

        public string setCfg()
        {
            string returnStartName = StartNode.name;
            string returnEnd = EndNode.name;
            string returndistance = distance;
            string returnRail_type = rail_type;
            string returnDirection_Left_or_Right = direction_Left_or_Right;

            if (returnStartName == "")
            {
                returnStartName = "\t";
            }
            if (returnEnd == "")
            {
                returnEnd = "\t";
            }
            if (returndistance == "")
            {
                returndistance = "\t";
            }
            if (returnRail_type == "")
            {
                returnRail_type = "\t";
            }
            if (returnDirection_Left_or_Right == "")
            {
                returnDirection_Left_or_Right = "\t";
            }


            return $"{returnStartName} {returnEnd} {returndistance} {returnRail_type} {returnDirection_Left_or_Right}";
        }

        public static EdgeModel FileToNodeModel(string lines, ObservableCollection<NodeModel> nodes)
        {
            if(!(nodes.Count > 0))
            {
                return null;
            }
            string replace = lines.Replace(" ", ",");
            string[] words = replace.Split(',');
            EdgeModel em = null;
            NodeModel start = null;
            NodeModel end = null;
            int findIndex = 0; // 2가 되면 foreach break;
            
            foreach (NodeModel node in nodes)
            {               
                if(node.name == null)
                {
                    continue;
                }
                if(findIndex ==2)
                {
                    break;
                }
                if (node.name.Equals(words[0]))
                {
                    start = node;
                   
                    findIndex++;
                }
                if (node.name.Equals(words[1]))
                {
                    end = node;
                    findIndex++;
                }
            }

            em = new EdgeModel(start, end);
            em.distance = words[2];
            em.rail_type = words[3];
            em.direction_Left_or_Right = words[4];

            return em;
        }

        private void DeleteNodeInShape(object param)
        {
            if (param is EdgeModel Edge)
            {                
                MainViewModel.arrows.Remove(Edge);
                MainViewModel.Edges = MainViewModel.arrows;
                MainViewModel.selectedEG = null;
            }
        }
    }
}

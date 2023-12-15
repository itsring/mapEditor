using MapEditor.Model.MapItems;
using MapEditor.Model.Property;
using MapEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Xml.Linq;

namespace MapEditor.Model
{
    public class MapItem : ViewModelBase
    {
        public string itemName { get; set; }
        public string name { get { return settingName; }
            set
            {
                settingName = value;
                OnPropertyChanged(nameof(name));
            } 
        }

        private string settingName { get; set; }
        public MainViewModel MainViewModel { get; set; }
        public MapItem()
        {
        }

        public void SetProperties(MapItem item)
        {
            // 클래스의 속성을 Reflection을 사용하여 가져오고 배열에 저장
            PropertyInfo[] properties = item.GetType().GetProperties();
            List<string> keys = new List<string>();
            List<object> values = new List<object>();

            foreach (PropertyInfo property in properties)
            {
                PropertyModel propertyModel = new PropertyModel();
                if (property.GetValue(item) is MapItem obj)
                {
                    propertyModel.Name = property.Name;
                    propertyModel.Value = obj.name;
                    MainViewModel.PropertyList.Add(propertyModel);
                    continue;
                }
                if (property.GetValue(item) is ICommand || property.GetValue(item) is ViewModelBase )
                {
                    continue;
                }
                //if(property.Name == "editor_x" || property.Name == "editor_y")
                //{
                //    continue;
                //}
                propertyModel.Name = property.Name;
                propertyModel.Value = property.GetValue(item);
                MainViewModel.PropertyList.Add(propertyModel);
                //if (property.GetValue(item) is MapItem obj)
                //{
                //    if (obj != null && obj is NodeModel node)
                //    {
                //        SetProperties(node);
                //    }
                //continue;
                //}

                //else
                //{
                //if (property.GetValue(item) is ICommand || property.GetValue(item) is ViewModelBase)
                //{
                //    continue;
                //}
                //propertyModel.Name = property.Name;
                //propertyModel.Value = property.GetValue(item);
                //MainViewModel.PropertyList.Add(propertyModel);
                //}
                //keys.Add(property.Name);
                //values.Add(property.GetValue(node));
            }
            MainViewModel.Properties = MainViewModel.PropertyList;
        }

        public void UpdateProperties(MapItem MI)
        {
            Type type = this.GetType();
            foreach (var item in MainViewModel.Properties)
            {
                PropertyInfo propertyInfo = type.GetProperty(item.Name);
                if(MI is EdgeModel em)
                {
                    if(propertyInfo.Name == "StartNode")
                    {
                        //em.StartNode.name = item.Value;
                        string nodeName = em.StartNode.name;
                        em.StartNode.name = item.Value.ToString();
                        continue;
                        //propertyInfo.SetValue(nodeName, Convert.ChangeType(item.Value, typeof(string)));
                    }
                    else if(propertyInfo.Name == "EndNode")
                    {
                        string nodeName = em.EndNode.name;
                        em.EndNode.name = item.Value.ToString();
                        continue;
                        //propertyInfo.SetValue(nodeName, Convert.ChangeType(item.Value, typeof(string)));
                    }
                }
                propertyInfo.SetValue(MI, Convert.ChangeType(item.Value, propertyInfo.PropertyType));
            }
            updateComplete(MI);
        }

        private void updateComplete(MapItem MI)
        {
            if(MI is NodeModel node)
            {
                node.X = node.editor_x - (node.Width / 2);
                node.Y = node.editor_y - (node.Height / 2);
                System.Diagnostics.Debug.WriteLine($"Selected Item: {node.GetHashCode()}");
                //findAndUpdate(node);
            }
            if(MI is EdgeModel edge)
            {                
                //edge.StartNode.name = edge.StartNodeName;
                //edge.EndNode.name = edge.EndNodeName;
                System.Diagnostics.Debug.WriteLine($"Selected Item: {edge.StartNode.GetHashCode()}");
                //findAndUpdate(edge.StartNode);
                //findAndUpdate(edge.EndNode);
            }
        }

        private void findAndUpdate(MapItem MI)
        {
            ObservableCollection<EdgeModel> edges = MI.MainViewModel.Edges;
            //ObservableCollection<NodeModel> nodes = MI.MainViewModel.Shapes;
            if (MI.MainViewModel.Edges.Count > 0)
            {
                foreach (var item in edges)
                {
                    if (item.StartNode.Equals(MI))
                    {
                        //item.StartNodeName = MI.name;
                        System.Diagnostics.Debug.WriteLine($"Equals Item: {MI.name}, startNode.name : {item.StartNode.name}");
                    }
                    if (item.EndNode.Equals(MI))
                    {
                        //item.EndNodeName = MI.name;
                        System.Diagnostics.Debug.WriteLine($"Equals Item: {MI.name}, startNode.name : {item.EndNode.name}");
                    }
                    //item.StartNode;
                }
            }

        }
    }
}

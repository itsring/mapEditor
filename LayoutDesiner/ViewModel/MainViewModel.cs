
using MapEditor.Command;
using MapEditor.Model;
using MapEditor.Model.MapItems;
using MapEditor.Model.Property;
using MapEditor.Util;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using ZmLib.Util.Setting;

namespace MapEditor.ViewModel
{
    public class MainViewModel : ViewModelBase
    {   
        // commands
        public ICommand MouseMoveOnImg { get; private set; }
        public ICommand ClickBackgoundImg { get; private set; }
        public ICommand ClickDrawPanel { get; private set; }
        public ICommand ClickNode { get; private set; }
        public ICommand ClickMenuOpen { get; private set; }
        public ICommand ClickMenuSave { get; private set; }
        public ICommand SaveProperty { get; private set; }


        // for binding
        public ObservableCollection<NodeModel> shapes = new ObservableCollection<NodeModel>();
        private string clickStatus { get; set; } = "NONE"; 
        private ObservableCollection<string> DrawItems = new ObservableCollection<string>();
        public ObservableCollection<PropertyModel> PropertyList = new ObservableCollection<PropertyModel>();
        // selected item
        public NodeModel selectedNM { get; set; } 
        public EdgeModel selectedEG { get; set; } 

        // for new EdgeModel
        public NodeModel EdgeStart { get; set; } = null;
        public NodeModel EdgeEnd { get; set; } = null;

        public ObservableCollection<EdgeModel> arrows = new ObservableCollection<EdgeModel>();

        // Lacation Text
        private string LT { get;  set; }

        // for move node pointer 
        public Point NowMouseLocation { get; set; }


        private double[] lineData { get; set; }

        // click mouse location
        private double[] clickLocation { get; set; }

        // 0 == startNode , 1 == endNode, 2 == 0
        private int CountForCreateEdge = 0;

        public MainViewModel()
        {
            //MouseMoveOnImg = new RelayCommand<MouseEventArgs>((obj) => OnMouseOverImage(obj));
            MouseMoveOnImg = new DelegateCommand((object obj) => OnMouseOverImage(obj));
            ClickBackgoundImg = new DelegateCommand((object obj) => ClickBackImg(obj));
            ClickDrawPanel = new DelegateCommand((object obj) => SelectListItem(obj));
            ClickNode = new DelegateCommand((object obj) => SelectNodeItem(obj));
            ClickMenuOpen = new SimpleCommand(OpenDialog);
            ClickMenuSave = new SimpleCommand(OpenDialogAndSaveToFile);
            SaveProperty = new DelegateCommand((object obj) => SavePropertyCommand(obj));
            //ClickNode = new RelayCommand<NodeModel>(SelectNodeItem);
            clickLocation = new double[2];
            lineData = new double[4];
            LT = "";
            MouseLocationText = "";
            DrawItemList = setListView();
        }


        /// Binding List
        public string MouseLocationText
        {
            get { return LT; }
            private set { 
                LT = value;
                OnPropertyChanged(nameof(MouseLocationText));
            }
        }
        
        public double mouse_x
        {
            get { return lineData[0]; }
            set {
                lineData[0] = value;
                OnPropertyChanged(nameof(mouse_x));
            }
        }
        public double mouse_y
        {
            get { return lineData[1]; }
            set
            {
                lineData[1] = value;
                OnPropertyChanged(nameof(mouse_y));
            }
        }
        public double image_x
        {
            get { return lineData[2]; }
            set
            {
                lineData[2] = value;
                OnPropertyChanged(nameof(image_x));
            }
        }
        public double image_y
        {
            get { return lineData[3]; }
            set
            {
                lineData[3] = value;
                OnPropertyChanged(nameof(image_y));
            }
        }
        public ObservableCollection<NodeModel> Shapes
        {
            get { return shapes; }
            set {
                shapes = value;
                OnPropertyChanged(nameof(Shapes));
            }
        }
        public ObservableCollection<EdgeModel> Edges
        {
            get { return arrows; }
            set
            {
                arrows = value;
                OnPropertyChanged(nameof(Edges));
            }
        }
        public ObservableCollection<string> DrawItemList
        {
            get { return DrawItems; }
            set
            {
                DrawItems = value;
                OnPropertyChanged(nameof(DrawItemList));
            }
        }
        public ObservableCollection<PropertyModel> Properties
        {
            get { return PropertyList; }
            set
            {
                PropertyList = value;
                OnPropertyChanged(nameof(Properties));
            }
        }
        
        ///
        /// Event Start 
        ///         
        // image위에 마우스가 있을 경우 마우스 위치 찍어주는 함수
        private void OnMouseOverImage(object param)
        {
            Grid grid = (Grid) param;
            NowMouseLocation = Mouse.GetPosition(grid);
            LocationModel LM = LocationUtil.SetLocationModel(grid);
            double[] originalPosition = LocationUtil.CalOriginalPosition(grid, LM);
            double[] actualSize = LocationUtil.ReturnActualSize(grid);

            mouse_x = LM.editor_x;
            mouse_y = LM.editor_y;
            image_x = actualSize[0];
            image_y = actualSize[1];           

            MouseLocationText = $"" +
                $"{mouse_x}, {mouse_y} " +
                $"(original {originalPosition[0]}, {originalPosition[1]} )" +
                $"(view size : {actualSize[0]} ,  {actualSize[1]}) " +
                $"( original size : {LM.image_W} ,  {LM.image_H} ) "
                ;
        }

        // 이미지 선택시 여러 분기에 의해 맵에 아이템 적용 됨.
        private void ClickBackImg(object param)
        {
            Grid grid = (Grid)param;

            LocationModel LM = LocationUtil.SetLocationModel(grid);
            double[] originalPosition = LocationUtil.CalOriginalPosition(grid, LM);

            clickLocation[0] = originalPosition[0];
            clickLocation[1] = originalPosition[1];

            // 위치 정보를 바탕으로 노드 생성! 
            if (clickStatus.Equals("NODE")) // 그리기 패널에서 노드 선택하면 노드 생성 
            {
                SetNode(LM, null);
            }

            // 첫번째인지 두번째인지 확인 필요, 엣지를 선택했을 경우 화살표 머리를 어떻게 할지 등... 필요함.
            if (clickStatus.Equals("EDGE"))
            {
                if(CountForCreateEdge == 1)
                {
                    EdgeEnd = SetNode(LM , selectedNM);
                    selectedNM = null;
                    EdgeModel em = new EdgeModel(EdgeStart, EdgeEnd);
                    em.MainViewModel = this;
                    arrows.Add(em);
                    CountForCreateEdge = 0;
                    Edges = arrows;
                    return;
                }
                if(CountForCreateEdge == 0)
                {
                   EdgeStart = SetNode(LM, selectedNM);
                   selectedNM = null;
                   CountForCreateEdge++;
                }              
            }
            //System.Diagnostics.Debug.WriteLine($"Clicked!!!! Original Image Position: ({clickLocation[0]}, {clickLocation[1]})");
        }

        // ListItem 선택 이벤트 = clickStatus 변경.
        private void SelectListItem(object param)
        {
            Type thisType = param.GetType();
            System.Diagnostics.Debug.WriteLine($"type :  ({thisType.Name})");
            if (param is ListView listView)
            {
                if (listView.SelectedItem is string selected)
                {
                    System.Diagnostics.Debug.WriteLine($"Selected Item: {selected}, clickstatus : {clickStatus}");
                    if (clickStatus.Equals((string)selected))
                    {
                        listView.SelectedItem = null;
                        clickStatus = "NONE";
                        return;
                    }
                    clickStatus = selected;
                    
                }
            }
        }

        private void SelectNodeItem(object param)
        {
            Type thisType = param.GetType();
            System.Diagnostics.Debug.WriteLine($"type :  ({thisType.Name})");            
        }

        private void SavePropertyCommand(object param)        
        {
            // ItemsControl을 가져옴. 
            // 뿌려준 label , text를 순서대로 가져와서 key value로 바꿔주기.
            if(param is ItemsControl itemsControl)
            {
                ObservableCollection<PropertyModel> itemsource = (ObservableCollection<PropertyModel>)itemsControl.ItemsSource;
                // 선택된 객체가 무엇인지 확인해야됨.
                if(selectedNM != null)
                {
                    foreach (NodeModel NM in shapes)
                    {
                        if (NM.editor_x == selectedNM.editor_x && NM.editor_y == selectedNM.editor_y)
                        {
                            NM.UpdateProperties(NM);
                        }
                    }
                    Shapes = shapes;
                }
                if (selectedEG != null)
                {
                    foreach (EdgeModel EM in arrows)
                    {            
                        if( selectedEG.StartNode.Equals(EM.StartNode) && selectedEG.EndNode.Equals(EM.EndNode) )
                        {
                            EM.UpdateProperties(EM);
                        }
                    }
                    Edges = arrows;
                }
            }
        }
        private void OpenDialog()
        {
            // 파일을 하나하나 열 때마다 물어보기때문에 주석처리.
            //if(Shapes.Count > 0 || Edges.Count > 0)
            //{
            //   if( MessageBox.Show("경고","저장되지 않았습니다. 새로 불러 오시겠습니까?" , MessageBoxButton.YesNo) == MessageBoxResult.OK)
            //    {
            //        // nodes 가져와서 저장
            //        // edges 가져와서 저장                    
            //        resetModelItems();
            //    }
            //    else
            //    {
            //        return;
            //    }
            //}
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "cfg files (*.cfg) | *.*";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                string[] filePaths = openFileDialog.FileNames;
                string[] fileName = openFileDialog.SafeFileNames;

                string file_name_contain_node = "node";
                int findIndex = Array.FindIndex(filePaths, x => x.Contains(file_name_contain_node));                
                


                int fileIndex = 0;
                // 파일을 동시에 가져 올 때 node데이터가 있고 edge가 있어야 됨. => 순서를 어떻게 할지 ... 고민 해야됨.
                foreach (string filePath in filePaths) {
                    string[] allText = File.ReadAllLines(filePath);
                    if (fileName[fileIndex].Contains("node"))
                    {
                        // node 작업
                        for (int i = 0; i < allText.Length; i ++)
                        {
                            if(i ==0 || i == allText.Length - 1 || allText[i][0] == '#')
                            {
                                // 첫줄과 마지막 줄이 주석이기 때문 
                                continue;
                            }
                            NodeModel nm = NodeModel.FileToNodeModel(allText[i]);
                            nm.MainViewModel = this;
                            shapes.Add(nm);
                        }
                        Shapes = shapes;
                    }
                    else if(fileName[fileIndex].Contains("edge"))
                    {
                        for (int i = 0; i < allText.Length; i++)
                        {
                            if (i == 0 || i == allText.Length - 1 || allText[i][0] == '#')
                            {
                                continue;
                            }
                            System.Diagnostics.Debug.WriteLine($"allText[{i}][0] :  ({allText[i][0]})");
                            EdgeModel em = EdgeModel.FileToNodeModel(allText[i], shapes);
                            if(em != null)
                            {
                                em.MainViewModel = this;
                                arrows.Add(em);
                            }
                        }
                        Edges = arrows;
                    }
                    else
                    {
                        throw new Exception("파일이름이 node 혹은 edge가 포함된 파일을 선택해주세요. 미구현");
                    }
                    fileIndex++;
                }                
            }
        }
        private void OpenDialogAndSaveToFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            int fileSaveIndex = 0;
            string edge_file_text = SetEdgeFileText(Edges);
            if(shapes.Count >0)
            {
                saveFileDialog.ShowDialog();
                if (saveFileDialog.FileName != "")
                {
                    saveFileDialog.Title = $"Save an {MapItemNames.getNames()[fileSaveIndex + 1]}";
                    // 일단 저장
                    saveNodesFile(shapes, saveFileDialog.FileName);
                }
            }
            if(Edges.Count >0) {
                saveFileDialog.ShowDialog();
                if (saveFileDialog.FileName != "")
                {
                    saveFileDialog.Title = $"Save an {MapItemNames.getNames()[fileSaveIndex + 1]}";
                    // 일단 저장
                    saveEdgesFile(Edges, saveFileDialog.FileName);
                }
            }

            
        }

        ///////// 
        /// 일반 메서드 리스트

        // 그리기 패널의 아이템 뿌려주기.
        private ObservableCollection<string> setListView()
        {
            ObservableCollection<string> returnValue = new();

            foreach(string a in MapItemNames.getNames())
            {
                returnValue.Add( a );
            }
            return returnValue;
        }


        //private void AddNode(LocationModel LM)
        //{
        //    NodeModel nodeModel = new NodeModel(LM.mouse_x, LM.mouse_y);
        //    nodeModel.MainViewModel = this;
        //    shapes.Add(nodeModel);
        //    Shapes = shapes;
        //}

        private NodeModel SetNode(LocationModel LM, NodeModel selectedNode)
        {
            NodeModel item = new NodeModel(LM.editor_x, LM.editor_y);
            if (selectedNode != null)
            {
               return selectedNode;
            }
            else
            {
                item.MainViewModel = this;
            }
            
            bool isContain = false;
            foreach(NodeModel NM in shapes)
            {
                // equals 다시 정의 하기...
                if( NM.editor_x == LM.editor_x && NM.editor_y == LM.editor_y)
                {
                    item = NM;
                    isContain = true;
                    break;
                }
            }
            if (!isContain)
            {
                shapes.Add(item);
                Shapes = shapes;
            }
          
            return item;
        }
        public void resetModelItems()
        {
            Shapes.Clear();
            Edges.Clear();
            clickStatus = "NONE";
            EdgeStart = null;
            EdgeEnd = null;
            selectedNM = null;
            selectedEG = null;
            CountForCreateEdge = 0;
        }

        public string SetNodeFileText(ObservableCollection<NodeModel> shapes)
        {
            string node_start = "# define nodes, node_name point point_pre editor_x editor_y whirls\n";
            string node_end = "# end of file";
            for (int i = 0; i < shapes.Count; i++)
            {
                node_start += shapes[i].setCfg();
            }
            node_start += node_end;

            return node_start;
        }

        public string SetEdgeFileText(ObservableCollection<EdgeModel> edges)
        {
            string node_start = "# define edges\r\n# start end distance rail_type directon\n";
            string node_end = "# end of file";
            for (int i = 0; i < edges.Count; i++)
            {
                node_start += edges[i].setCfg();
            }
            node_start += node_end;

            return node_start;
        }

        private void saveNodesFile(ObservableCollection<NodeModel> shapes, string fileName)
        {
            StreamWriter sw = new StreamWriter(fileName);
            string node_start = "# define nodes, node_name point point_pre editor_x editor_y whirls";
            string node_end = "# end of file";
            sw.WriteLine(node_start);
            for (int i = 0; i < shapes.Count; i++)
            {
                sw.WriteLine(shapes[i].setCfg());
            }
            
            sw.WriteLine(node_end);
            sw.Close();            
        }

        private void saveEdgesFile(ObservableCollection<EdgeModel> shapes, string fileName)
        {
            StreamWriter sw = new StreamWriter(fileName);
            string node_start = "# define edges\r\n# start end distance rail_type directon";
            string node_end = "# end of file";
            sw.WriteLine(node_start);
            for (int i = 0; i < shapes.Count; i++)
            {
                sw.WriteLine(shapes[i].setCfg());
            }

            sw.WriteLine(node_end);
            sw.Close();
        }
    }
}

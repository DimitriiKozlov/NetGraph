using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CourseProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        readonly Graph _graph = new Graph();
        private int _moveIndex = -1;
        private int _removeIndex = -1;
        //private bool _addEdge;

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var p = new Point(e.GetPosition(CanvasForm).X - Node.DefaultRadius * 0.5, e.GetPosition(CanvasForm).Y - Node.DefaultRadius * 0.5);
            //if (p.X < 0)
            //{
            //    _moveIndex = -1;
            //    return;
            //}

            var index = _graph.GetNum(p);

            if (RbAddEdge.IsChecked.Value) 
                if ((index != _moveIndex) && (index != -1) && (_moveIndex != -1) && (_graph.AddEdge(new Edge(_graph.Nodes[index].Id, _graph.Nodes[_moveIndex].Id, CanvasForm.Children.Count, Convert.ToInt32(LChannelWeight.Content), Convert.ToInt32(LChannelCapacity.Content)))))
                {
                    var line = new Line
                    {
                        X1 = _graph.Nodes[index].Coordinate.X + Node.DefaultRadius * 0.5,
                        Y1 = _graph.Nodes[index].Coordinate.Y + Node.DefaultRadius * 0.5,
                        X2 = _graph.Nodes[_moveIndex].Coordinate.X + Node.DefaultRadius * 0.5,
                        Y2 = _graph.Nodes[_moveIndex].Coordinate.Y + Node.DefaultRadius * 0.5,
                        Stroke = new SolidColorBrush(Colors.LightBlue),
                        StrokeThickness = 3
                    };
                    CanvasForm.Children.Add(line);
                    Panel.SetZIndex(line, 100);
                    return;
                }
                else
                    return;

            if (RbSetEnable.IsChecked.Value)
                if (index == _moveIndex)
                {
                    if (index == -1)
                        return;

                    var lst = _graph.GetNodeEdges(_graph.Nodes[index].Id);

                    var ellipse = (Ellipse) ((Grid) CanvasForm.Children[_graph.Nodes[index].ChildId]).Children[0];

                    if (_graph.Nodes[index].Enable)
                    {
                        _graph.Nodes[index].Enable = false;
                        ellipse.Fill = new SolidColorBrush(Colors.DarkSlateGray);

                        foreach (var edge in lst)
                        {
                            var l = (Line)CanvasForm.Children[edge.ChildId];
                            edge.Enable = false;
                            l.Stroke = new SolidColorBrush(Colors.DarkSlateGray);
                        }
                    }
                    else
                    {
                        _graph.Nodes[index].Enable = true;
                        ellipse.Fill = new SolidColorBrush(Color.FromRgb(7, 7, 89));

                        foreach (var edge in lst)
                            if (_graph.Nodes[_graph.GetNum((_graph.Nodes[index].Id == edge.IdVertex1) ? edge.IdVertex2 : edge.IdVertex1)].Enable)
                            {
                                var l = (Line)CanvasForm.Children[edge.ChildId];
                                edge.Enable = true;
                                l.Stroke = new SolidColorBrush(Colors.LightBlue);
                            }
                    }
                    return;
                }
                else
                {
                    if ((index == -1) || (_moveIndex == -1))
                        return;


                    var lst = _graph.GetNodeEdges(_graph.Nodes[index].Id);

                    foreach (var edge in lst)
                        if ((edge.IdVertex1 == _graph.Nodes[_moveIndex].Id) || (edge.IdVertex2 == _graph.Nodes[_moveIndex].Id))
                        {
                            var l = (Line) CanvasForm.Children[edge.ChildId];
                            if (edge.Enable)
                            {
                                edge.Enable = false;
                                l.Stroke = new SolidColorBrush(Colors.DarkSlateGray);
                            }
                            else
                                if (_graph.Nodes[_graph.GetNum(edge.IdVertex1)].Enable && _graph.Nodes[_graph.GetNum(edge.IdVertex2)].Enable)
                                {
                                    edge.Enable = true;
                                    l.Stroke = new SolidColorBrush(Colors.LightBlue);
                                }
                        }
                    return;
                }
            


            if ((index != -1) || (p.X < 0))
            {
                if (_moveIndex != -1)
                {
                    var el = (Grid)CanvasForm.Children[_graph.Nodes[_moveIndex].ChildId];
                    el.Margin = new Thickness(_graph.Nodes[_moveIndex].Coordinate.X, _graph.Nodes[_moveIndex].Coordinate.Y, 0, 0);
                    RedrawNode(_moveIndex);
                }

                _moveIndex = -1;
                return;
            }

            if (_moveIndex != -1)
            {
                _graph.Nodes[_moveIndex].Coordinate = p;
                _moveIndex = -1;
                return;
            }

            var indexOfLast = CanvasForm.Children.Count;
            _graph.AddNode(p, indexOfLast);


            var ellipseNode = new Ellipse
            {
                Stroke = new SolidColorBrush(Colors.LightBlue),
                Fill = new SolidColorBrush(Color.FromRgb(7, 7, 89)),
                Width = Node.DefaultRadius,
                Height = Node.DefaultRadius,
                Name = "_" + _graph.GetId(p),
                //Margin = new Thickness(p.X, p.Y, 0, 0)
            };

            var text = new TextBlock
            {
                Text = _graph.Nodes.Last().Id.ToString(),
                Foreground = new SolidColorBrush(Colors.LightBlue),
                //Width = Node.DefaultRadius,
                //Height = Node.DefaultRadius,
                //Margin = new Thickness(p.X, p.Y, 0, 0),
                FontSize = Node.DefaultRadius * 0.5,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                //Background = new SolidColorBrush(Colors.Wheat),
                TextAlignment = TextAlignment.Center,
            };


            var grd = new Grid
            {
                Margin = new Thickness(p.X, p.Y, 0, 0)
            };

            grd.Children.Add(ellipseNode);
            grd.Children.Add(text);

            CanvasForm.Children.Add(grd);
            Panel.SetZIndex(grd, 200);
            Panel.SetZIndex(ellipseNode, 210);
            Panel.SetZIndex(text, 220);
            _moveIndex = -1;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = new Point(e.GetPosition(CanvasForm).X - Node.DefaultRadius * 0.5, e.GetPosition(CanvasForm).Y - Node.DefaultRadius * 0.5);
            if (p.X < 0)
                return;

            _moveIndex = _graph.GetNum(p);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (RbAddNode.IsChecked != null && !RbAddNode.IsChecked.Value)
                return;

            var p = new Point(e.GetPosition(CanvasForm).X - Node.DefaultRadius * 0.5, e.GetPosition(CanvasForm).Y - Node.DefaultRadius * 0.5);

            if ((_moveIndex == -1) || (p.X < 0))
                return;

            var index = _graph.Nodes[_moveIndex].ChildId;

            var el = (Grid)CanvasForm.Children[index];
            el.Margin = new Thickness(p.X, p.Y, 0, 0);
            RedrawNode(_moveIndex, p);
        }

        public void RedrawNode(int number, Point p = new Point())
        {
            if (number == -1) return;
            if (p == new Point())
                p = _graph.Nodes[number].Coordinate;
            var id = _graph.Nodes[number].Id;

            //var id = graph.Nodes[number].Id;
            var lst = _graph.GetNodeEdges(id);

            foreach (var ed in lst)
            {
                var line = (Line)CanvasForm.Children[ed.ChildId];
                if (ed.IdVertex1 == id)
                {
                    line.X1 = p.X + Node.DefaultRadius * 0.5;
                    line.Y1 = p.Y + Node.DefaultRadius * 0.5;
                }
                else
                {
                    line.X2 = p.X + Node.DefaultRadius * 0.5;
                    line.Y2 = p.Y + Node.DefaultRadius * 0.5;
                }
            }
        }

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = new Point(e.GetPosition(CanvasForm).X - Node.DefaultRadius * 0.5, e.GetPosition(CanvasForm).Y - Node.DefaultRadius * 0.5);
            _removeIndex = _graph.GetNum(p);
        }

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var p = new Point(e.GetPosition(CanvasForm).X - Node.DefaultRadius * 0.5, e.GetPosition(CanvasForm).Y - Node.DefaultRadius * 0.5);
            var index = _graph.GetNum(p);

            if ((index == -1) || (_removeIndex == -1))
                return;

            var lst = _graph.GetNodeEdges(_graph.Nodes[index].Id);

            if (index == _removeIndex)
            {
                while (lst.Count > 0)
                {
                    CanvasForm.Children.RemoveAt(lst[0].ChildId);
                    _graph.DeleteEdge(lst[0]);
                    lst.RemoveAt(0);
                }
                CanvasForm.Children.RemoveAt(_graph.Nodes[index].ChildId);
                _graph.DeleteNode(index);
                return;
            }

            foreach (var ed in lst.Where(ed => (ed.IdVertex1 == _graph.Nodes[_removeIndex].Id) || (ed.IdVertex2 == _graph.Nodes[_removeIndex].Id)))
            {
                CanvasForm.Children.RemoveAt(ed.ChildId);
                _graph.DeleteEdge(ed);
                return;
            }

        }

        private void RBAddNode_Click(object sender, RoutedEventArgs e)
        {
            _moveIndex = _removeIndex = -1;
        }

        private void RBAddEdge_Click(object sender, RoutedEventArgs e)
        {
            _moveIndex = _removeIndex = -1;
        }

        private void RBSetEnable_Click(object sender, RoutedEventArgs e)
        {
            _moveIndex = _removeIndex = -1;
        }

        private void BRandomLChannelWeight_Click(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            LChannelWeight.Content = random.Next(1, 100);
            //
        }

        private void BRandomLChannelCapacity_Click(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            LChannelCapacity.Content = random.Next(1, 100);
        }

        private void LChannelWeight_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var s = ((Convert.ToInt32(LChannelWeight.Content.ToString()) + 1) % 100).ToString();
            LChannelWeight.Content = (s == "0") ? "1" : s;
        }

        private void LChannelWeight_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var s = (Convert.ToInt32(LChannelWeight.Content.ToString()) - 1).ToString();
            LChannelWeight.Content = (s == "0") ? "99" : s;
        }

        private void LChannelCapacity_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var s = ((Convert.ToInt32(LChannelCapacity.Content.ToString()) + 1) % 100).ToString();
            LChannelCapacity.Content = (s == "0") ? "1" : s;
        }

        private void LChannelCapacity_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var s = (Convert.ToInt32(LChannelCapacity.Content.ToString()) - 1).ToString();
            LChannelCapacity.Content = (s == "0") ? "99" : s;
        }
    }
}

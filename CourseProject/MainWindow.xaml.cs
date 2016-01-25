using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
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
        private Message _message;

        //readonly Message _message = new Message();
        //private bool _addEdge;

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var p = new Point(e.GetPosition(CanvasForm).X - Node.DefaultRadius * 0.5, e.GetPosition(CanvasForm).Y - Node.DefaultRadius * 0.5);

            var index = _graph.GetNum(p);

            if (RbAddEdge.IsChecked.Value) 
                if ((index != _moveIndex) && (index != -1) && (_moveIndex != -1) && (_graph.AddEdge(new Edge(_graph.Nodes[index].Id, _graph.Nodes[_moveIndex].Id, CanvasForm.Children.Count, Convert.ToInt32(LChannelWeightValue.Content), Convert.ToInt32(LChannelCapacityValue.Content)))))
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
            LChannelWeightValue.Content = random.Next(1, 100);
        }

        private void BRandomLChannelCapacity_Click(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            LChannelCapacityValue.Content = random.Next(Message.MessageBlockSize, 10000);
        }

        private void LChannelWeight_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var s = ((Convert.ToInt32(LChannelWeightValue.Content.ToString()) + 1) % 100).ToString();
            LChannelWeightValue.Content = (s == "0") ? "1" : s;
        }

        private void LChannelWeight_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var s = (Convert.ToInt32(LChannelWeightValue.Content.ToString()) - 1).ToString();
            LChannelWeightValue.Content = (s == "0") ? "99" : s;
        }

        private void LChannelCapacity_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var s = ((Convert.ToInt32(LChannelCapacityValue.Content.ToString()) + 1) % 10000).ToString();
            LChannelCapacityValue.Content = (s == "0") ? Message.MessageBlockSize.ToString() : s;
        }

        private void LChannelCapacity_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var s = (Convert.ToInt32(LChannelCapacityValue.Content.ToString()) - 1).ToString();
            LChannelCapacityValue.Content = (s == Message.MessageBlockSize.ToString()) ? "9999" : s;
        }

        private void BRandomMessageSizeValue_Click(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            LMessageSizeValue.Content = random.Next(Message.MessageBlockSize, Message.MessageMaxSize);
        }

        private void LMessageSizeValue_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var s = ((Convert.ToInt32(LMessageSizeValue.Content.ToString()) + 1) % Message.MessageMaxSize).ToString();
            LMessageSizeValue.Content = (s == "0") ? Message.MessageBlockSize.ToString() : s;
        }

        private void LMessageSizeValue_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var s = (Convert.ToInt32(LMessageSizeValue.Content.ToString()) - 1).ToString();
            LMessageSizeValue.Content = (s == Message.MessageBlockSize.ToString()) ? Message.MessageMaxSize.ToString() : s;
        }

        private void LMessageFromValue_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ("--" == LMessageFromValue.Content.ToString())
            {
                LMessageFromValue.Content = (_graph.Nodes.Count <= 0) ? "--" : _graph.Nodes[0].Id.ToString();
                return;
            }

            LMessageFromValue.Content = (_graph.Nodes[(_graph.GetNum(Convert.ToInt32(LMessageFromValue.Content.ToString())) + 1) % _graph.Nodes.Count].Id).ToString();
        }

        private void LMessageFromValue_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ("--" == LMessageFromValue.Content.ToString())
            {
                LMessageFromValue.Content = (_graph.Nodes.Count <= 0) ? "--" : _graph.Nodes[0].Id.ToString();
                return;
            }

            LMessageFromValue.Content = (_graph.Nodes[(_graph.GetNum(Convert.ToInt32(LMessageFromValue.Content.ToString())) + _graph.Nodes.Count - 1) % _graph.Nodes.Count].Id).ToString();
        }

        private void LMessageToValue_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ("--" == LMessageToValue.Content.ToString())
            {
                LMessageToValue.Content = (_graph.Nodes.Count <= 0) ? "--" : _graph.Nodes[0].Id.ToString();
                return;
            }

            LMessageToValue.Content = (_graph.Nodes[(_graph.GetNum(Convert.ToInt32(LMessageToValue.Content.ToString())) + 1) % _graph.Nodes.Count].Id).ToString();
        }

        private void LMessageToValue_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ("--" == LMessageToValue.Content.ToString())
            {
                LMessageToValue.Content = (_graph.Nodes.Count <= 0) ? "--" : _graph.Nodes[0].Id.ToString();
                return;
            }

            LMessageToValue.Content = (_graph.Nodes[(_graph.GetNum(Convert.ToInt32(LMessageToValue.Content.ToString())) + _graph.Nodes.Count - 1) % _graph.Nodes.Count].Id).ToString();
        }

        private void BSend_Click(object sender, RoutedEventArgs e)
        {
            if ((LMessageFromValue.Content.ToString() == "--") || (LMessageToValue.Content.ToString() == "--"))
                return;

            _message = new Message(Convert.ToInt32(LMessageSizeValue.Content.ToString()), CBDatagram.IsChecked != null && CBDatagram.IsChecked.Value);

            PbSend.Maximum = _message.Size;
            PbSend.Value = 0;

            BSendFinish.IsEnabled = true;
            BSendNext.IsEnabled = true;
            BSendPlay.IsEnabled = true;
            BSendReset.IsEnabled = true;
            BSend.IsEnabled = false;
        }

        private void BPowerAverageRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (_graph.Nodes.Count == 0)
            {
                LPowerAverageValue.Content = "0";
                return;
            }

            LPowerAverageValue.Content = ((double)2*_graph.Edges.Count/_graph.Nodes.Count).ToString(CultureInfo.InvariantCulture);
        }

        private void BSendNext_Click(object sender, RoutedEventArgs e)
        {
            _graph.RestoreEdgeCapacity();
            for (var i = 0; i < _message.PackagesList.Count; i++)
            {
                var packagese = _message.PackagesList[i];

                if (packagese.WaitForAnswer)
                {
                    packagese.WaitForAnswer = false;
                    continue;
                }
                packagese.WaitForAnswer = true;

                if (packagese.IdNext == -1)
                {
                    var waySet = _graph.GetWayToNode(Convert.ToInt32(LMessageFromValue.Content.ToString()),
                        Convert.ToInt32(LMessageToValue.Content.ToString()), packagese.Size);
                    if (waySet == null)
                        continue;
                    packagese.SetWay(waySet);
                    packagese.WaitForAnswer = true;
                    continue;
                }


                if (packagese.IsDatagtam)
                {
                    var edgeDG =
                       (_graph.GetNodeEdges(packagese.Id)
                           .Where(
                               edg =>
                                   ((edg.IdVertex1 == packagese.Id) && (edg.IdVertex2 == packagese.IdNext)) ||
                                   ((edg.IdVertex1 == packagese.IdNext) && (edg.IdVertex2 == packagese.Id)))).ToList()[0];
                    if (edgeDG.CurentCapacity >= packagese.Size)
                    {
                        edgeDG.CurentCapacity -= packagese.Size;

                        //if (packagese.WaitForAnswer)
                        //{
                        //    packagese.WaitForAnswer = false;
                        //    continue;
                        //}
                        //packagese.WaitForAnswer = true;
                        if (packagese.Next())
                        {
                            PbSend.Value += packagese.Size;
                            _message.PackagesList.RemoveAt(i--);
                        }
                    }
                    continue;
                }

                var way = _graph.GetWayToNode(packagese.Id, Convert.ToInt32(LMessageToValue.Content.ToString()),
                    packagese.Size);

                if (way == null) continue;
                var edge =
                    (_graph.GetNodeEdges(packagese.Id)
                        .Where(
                            edg =>
                                ((edg.IdVertex1 == way[0]) && (edg.IdVertex2 == way[1])) ||
                                ((edg.IdVertex1 == way[1]) && (edg.IdVertex2 == way[0])))).ToList()[0];
                edge.CurentCapacity -= packagese.Size;

                //if (packagese.WaitForAnswer)
                //{
                //    packagese.WaitForAnswer = false;
                //    continue;
                //}
                //packagese.WaitForAnswer = true;
                if (!packagese.Next(way)) continue;
                PbSend.Value += packagese.Size;
                _message.PackagesList.RemoveAt(i--);
            }

            textBox1.Text = "Num\tFrom\tTo\tServer\tSize\n";
            for (var i = 0; i < _message.PackagesList.Count; i++)
                textBox1.Text += i.ToString() + '\t' + _message.PackagesList[i].Id.ToString() + '\t' + _message.PackagesList[i].IdNext.ToString() + '\t' + _message.PackagesList[i].WaitForAnswer.ToString() + '\t' + _message.PackagesList[i].Size + '\n';
        }

        private async void BSendPlay_Click(object sender, RoutedEventArgs e)
        {
            while (_message.PackagesList.Count > 0)
            {
                await Task.Delay(50);
                BSendNext_Click(sender, e);
            }
        }

        private void BSendFinish_Click(object sender, RoutedEventArgs e)
        {
            while (_message.PackagesList.Count > 0)
                BSendNext_Click(sender, e);
        }

        private void BSendReset_Click(object sender, RoutedEventArgs e)
        {
            LMessageFromValue.Content = "--";
            LMessageToValue.Content = "--";

            PbSend.Maximum = 100;
            PbSend.Value = 0;

            BSendFinish.IsEnabled = false;
            BSendNext.IsEnabled = false;
            BSendPlay.IsEnabled = false;
            BSendReset.IsEnabled = false;
            BSend.IsEnabled = true;
        }
    }
}

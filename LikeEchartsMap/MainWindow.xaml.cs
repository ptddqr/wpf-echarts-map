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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LikeEchartsMap
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //所有的动画 都在这个方法里 AddPointToStoryboard

        #region 成员变量
        /// <summary>
        /// 跑动的点里的Path的Data
        /// </summary>
        private string m_PointData;
        /// <summary>
        /// 点的运动速度 单位距离/秒
        /// </summary>
        private double m_Speed;
        /// <summary>
        /// 运动轨迹弧线的正弦角度
        /// </summary>
        private double m_Angle;
        /// <summary>
        /// 数据源
        /// </summary>
        private List<MapItem> m_Source;
        /// <summary>
        /// 故事版
        /// </summary>
        private Storyboard m_Sb = new Storyboard();
        #endregion

        #region 事件
        #region loaded
        /// <summary>
        /// loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //参数设置部分
            m_PointData = "M244.5,98.5 L273.25,93.75 C278.03113,96.916667 277.52785,100.08333 273.25,103.25 z";
            m_Speed = 50;
            m_Angle = 15;

            MakeData();
            InitRadioButton();
            AddAnimation(m_Source[0]);
        }
        #endregion
        #region RadioButton点击
        /// <summary>
        /// RadioButton点击
        /// </summary>
        void rbtn_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rbtn = (RadioButton)sender;
            if (rbtn.IsChecked == true)
            {
                foreach (MapItem item in m_Source)
                {
                    if ((string)rbtn.Content == item.From.ToString())
                    {
                        AddAnimation(item);
                        return;
                    }
                }
            }
        }
        #endregion
        #endregion

        #region 方法
        #region 做数据
        /// <summary>
        /// 做数据
        /// </summary>
        private void MakeData()
        {
            Random rd = new Random();
            List<MapItem> list = new List<MapItem>();
            List<MapToItem> toList = new List<MapToItem>();
            toList.Add(new MapToItem() { To = ProvincialCapital.成都, Diameter = rd.Next(10, 51) });
            toList.Add(new MapToItem() { To = ProvincialCapital.西宁, Diameter = rd.Next(10, 51) });
            toList.Add(new MapToItem() { To = ProvincialCapital.哈尔滨, Diameter = rd.Next(10, 51) });
            toList.Add(new MapToItem() { To = ProvincialCapital.海口, Diameter = rd.Next(10, 51), Tip = "美丽的大海" });
            toList.Add(new MapToItem() { To = ProvincialCapital.呼和浩特, Diameter = rd.Next(10, 51) });
            toList.Add(new MapToItem() { To = ProvincialCapital.重庆, Diameter = 50, Tip = "山鸡大神在这" });
            toList.Add(new MapToItem() { To = ProvincialCapital.台北, Diameter = rd.Next(10, 51) });
            toList.Add(new MapToItem() { To = ProvincialCapital.乌鲁木齐, Diameter = rd.Next(10, 51) });
            toList.Add(new MapToItem() { To = ProvincialCapital.广州, Diameter = rd.Next(10, 51) });
            toList.Add(new MapToItem() { To = ProvincialCapital.上海, Diameter = 50, Tip = "雷叔的地盘!" });
            list.Add(new MapItem() { From = ProvincialCapital.北京, To = toList });
            list.Add(new MapItem() { From = ProvincialCapital.西安, To = toList });
            list.Add(new MapItem() { From = ProvincialCapital.拉萨, To = toList });
            m_Source = list;
        }
        #endregion
        #region 加载菜单RadioButton
        /// <summary>
        /// 加载RadioButton
        /// </summary>
        private void InitRadioButton()
        {
            Random rd = new Random();
            for (int i = 0; i < m_Source.Count; i++)
            {
                byte r = (byte)rd.Next(0, 256);
                byte g = (byte)rd.Next(0, 256);
                byte b = (byte)rd.Next(0, 256);
                RadioButton rbtn = new RadioButton();
                rbtn.Content = m_Source[i].From.ToString();
                rbtn.Margin = new Thickness(0, 20, 0, 0);
                rbtn.Background = new SolidColorBrush(Color.FromArgb(200, r, g, b));
                rbtn.BorderBrush = new SolidColorBrush(Color.FromArgb(255, r, g, b));
                if (i == 0)
                    rbtn.IsChecked = true;
                else
                    rbtn.IsChecked = false;
                rbtn.Click += rbtn_Click;
                spnl_Radio.Children.Add(rbtn);
            }
        }
        #endregion
        #region 添加控件和动画到容器
        /// <summary>
        /// 添加控件和动画到容器
        /// </summary>
        /// <param name="item">数据项</param>
        private void AddAnimation(MapItem item)
        {
            grid_Animation.Children.Clear();
            m_Sb.Children.Clear();
            Random rd = new Random();
            foreach (MapToItem toItem in item.To)
            {
                if (item.From == toItem.To)
                    continue;

                //颜色
                byte[] rgb = new byte[] { (byte)rd.Next(0, 255), (byte)rd.Next(0, 255), (byte)rd.Next(0, 255) };

                //运动轨迹
                double l = 0;
                Path particlePath = GetParticlePath(item.From, toItem, rgb, out l);
                grid_Animation.Children.Add(particlePath);
                // 跑动的点
                Grid grid = GetRunPoint(rgb);
                //到达城市的圆
                Ellipse ell = GetToEllipse(toItem, rgb);
                AddPointToStoryboard(grid, ell, m_Sb, particlePath, l, item.From, toItem);
                grid_Animation.Children.Add(grid);
                grid_Animation.Children.Add(ell);

                m_Sb.Begin(this);
            }
        }
        #endregion
        #region 获取省会,直辖市,特别行政区的坐标
        /// <summary>
        /// 获取省会,直辖市,特别行政区的坐标
        /// </summary>
        /// <param name="pc">城市</param>
        /// <returns>Point(Left,Top)</returns>
        private Point GetProvincialCapitalPoint(ProvincialCapital city)
        {
            Point point = new Point(0, 0);
            switch (city)
            {
                case ProvincialCapital.北京:
                    point.X = 625.71145;
                    point.Y = 265.20515;
                    break;
                case ProvincialCapital.天津:
                    point.X = 646.648895;
                    point.Y = 277.719215;
                    break;
                case ProvincialCapital.上海:
                    point.X = 730.844;
                    point.Y = 425.208;
                    break;
                case ProvincialCapital.重庆:
                    point.X = 487.123;
                    point.Y = 469.796;
                    break;
                case ProvincialCapital.石家庄:
                    point.X = 605.527;
                    point.Y = 300.853;
                    break;
                case ProvincialCapital.太原:
                    point.X = 575.685;
                    point.Y = 310.961;
                    break;
                case ProvincialCapital.沈阳:
                    point.X = 725.375;
                    point.Y = 214.217;
                    break;
                case ProvincialCapital.长春:
                    point.X = 742.702;
                    point.Y = 173.786;
                    break;
                case ProvincialCapital.哈尔滨:
                    point.X = 751.847;
                    point.Y = 137.687;
                    break;
                case ProvincialCapital.南京:
                    point.X = 691.682;
                    point.Y = 418.295;
                    break;
                case ProvincialCapital.杭州:
                    point.X = 706.603;
                    point.Y = 446.211;
                    break;
                case ProvincialCapital.合肥:
                    point.X = 661.841;
                    point.Y = 418.295;
                    break;
                case ProvincialCapital.福州:
                    point.X = 706.603;
                    point.Y = 528.516;
                    break;
                case ProvincialCapital.南昌:
                    point.X = 646.439;
                    point.Y = 486.16;
                    break;
                case ProvincialCapital.济南:
                    point.X = 648.845;
                    point.Y = 327.807;
                    break;
                case ProvincialCapital.郑州:
                    point.X = 596.382;
                    point.Y = 371.126;
                    break;
                case ProvincialCapital.武汉:
                    point.X = 617.078;
                    point.Y = 451.506;
                    break;
                case ProvincialCapital.长沙:
                    point.X = 593.975;
                    point.Y = 497.231;
                    break;
                case ProvincialCapital.广州:
                    point.X = 611.303;
                    point.Y = 592.531;
                    break;
                case ProvincialCapital.海口:
                    point.X = 553.545;
                    point.Y = 663.766;
                    break;
                case ProvincialCapital.成都:
                    point.X = 445.73;
                    point.Y = 453.912;
                    break;
                case ProvincialCapital.贵阳:
                    point.X = 492.417;
                    point.Y = 533.811;
                    break;
                case ProvincialCapital.昆明:
                    point.X = 420.22;
                    point.Y = 563.171;
                    break;
                case ProvincialCapital.西安:
                    point.X = 522.259;
                    point.Y = 384.121;
                    break;
                case ProvincialCapital.兰州:
                    point.X = 442.842;
                    point.Y = 354.28;
                    break;
                case ProvincialCapital.西宁:
                    point.X = 408.668;
                    point.Y = 340.321;
                    break;
                case ProvincialCapital.拉萨:
                    point.X = 235.394;
                    point.Y = 452.949;
                    break;
                case ProvincialCapital.南宁:
                    point.X = 520.815;
                    point.Y = 605.046;
                    break;
                case ProvincialCapital.呼和浩特:
                    point.X = 557.877;
                    point.Y = 255.128;
                    break;
                case ProvincialCapital.银川:
                    point.X = 479.422;
                    point.Y = 299.891;
                    break;
                case ProvincialCapital.乌鲁木齐:
                    point.X = 220.474;
                    point.Y = 179.562;
                    break;
                case ProvincialCapital.香港:
                    point.X = 623.817;
                    point.Y = 611.784;
                    break;
                case ProvincialCapital.澳门:
                    point.X = 600.714;
                    point.Y = 615.634;
                    break;
                case ProvincialCapital.台北:
                    point.X = 747.515;
                    point.Y = 545.844;
                    break;
            }
            return point;
        }
        #endregion
        #region 获取运动轨迹
        /// <summary>
        /// 获取运动轨迹
        /// </summary>
        /// <param name="from">来自</param>
        /// <param name="toItem">去</param>
        /// <param name="rgb">颜色:r,g,b</param>
        /// <param name="l">两点间的直线距离</param>
        /// <returns>Path</returns>
        private Path GetParticlePath(ProvincialCapital from, MapToItem toItem, byte[] rgb, out double l)
        {
            Point startPoint = GetProvincialCapitalPoint(from);
            Point endPoint = GetProvincialCapitalPoint(toItem.To);

            Path path = new Path();
            Style style = (Style)FindResource("ParticlePathStyle");
            path.Style = style;
            PathGeometry pg = new PathGeometry();
            PathFigure pf = new PathFigure();
            pf.StartPoint = startPoint;
            ArcSegment arc = new ArcSegment();
            arc.SweepDirection = SweepDirection.Clockwise;//顺时针弧
            arc.Point = endPoint;
            //半径 正弦定理a/sinA=2r r=a/2sinA 其中a指的是两个城市点之间的距离 角A指a边的对角
            double sinA = Math.Sin(Math.PI * m_Angle / 180.0);
            //计算距离 勾股定理
            double x = startPoint.X - endPoint.X;
            double y = startPoint.Y - endPoint.Y;
            double aa = x * x + y * y;
            l = Math.Sqrt(aa);
            double r = l / (sinA * 2);
            arc.Size = new Size(r, r);
            pf.Segments.Add(arc);
            pg.Figures.Add(pf);
            path.Data = pg;
            path.Stroke = new SolidColorBrush(Color.FromArgb(255, rgb[0], rgb[1], rgb[2]));
            path.Stretch = Stretch.None;
            path.ToolTip = string.Format("{0}=>{1}", from.ToString(), toItem.To.ToString());

            return path;
        }
        #endregion
        #region 获取跑动的点
        /// <summary>
        /// 获取跑动的点
        /// </summary>
        /// <param name="rgb">颜色:r,g,b</param>
        /// <returns>Grid</returns>
        private Grid GetRunPoint(byte[] rgb)
        {
            //一个Grid里包含一个椭圆 一个Path 椭圆做阴影
            //Grid
            Grid grid = new Grid();
            grid.IsHitTestVisible = false;//不参与命中测试
            grid.HorizontalAlignment = HorizontalAlignment.Left;
            grid.VerticalAlignment = VerticalAlignment.Top;
            grid.Width = 40;
            grid.Height = 15;
            grid.RenderTransformOrigin = new Point(0.5, 0.5);
            //Ellipse
            Ellipse ell = new Ellipse();
            ell.Width = 40;
            ell.Height = 15;
            ell.Fill = new SolidColorBrush(Color.FromArgb(255, rgb[0], rgb[1], rgb[2]));
            RadialGradientBrush rgbrush = new RadialGradientBrush();
            rgbrush.GradientOrigin = new Point(0.8, 0.5);
            rgbrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 0, 0, 0), 0));
            rgbrush.GradientStops.Add(new GradientStop(Color.FromArgb(22, 0, 0, 0), 1));
            ell.OpacityMask = rgbrush;
            grid.Children.Add(ell);
            //Path
            Path path = new Path();
            path.Data = Geometry.Parse(m_PointData);
            path.Width = 30;
            path.Height = 4;
            LinearGradientBrush lgb = new LinearGradientBrush();
            lgb.StartPoint = new Point(0, 0);
            lgb.EndPoint = new Point(1, 0);
            lgb.GradientStops.Add(new GradientStop(Color.FromArgb(88, rgb[0], rgb[1], rgb[2]), 0));
            lgb.GradientStops.Add(new GradientStop(Color.FromArgb(255, 255, 255, 255), 1));
            path.Fill = lgb;
            path.Stretch = Stretch.Fill;
            grid.Children.Add(path);
            return grid;
        }
        #endregion
        #region 获取到达城市的圆
        /// <summary>
        /// 获取到达城市的圆
        /// </summary>
        /// <param name="toItem">数据项</param>
        /// <param name="rgb">颜色</param>
        /// <returns>Ellipse</returns>
        private Ellipse GetToEllipse(MapToItem toItem, byte[] rgb)
        {
            Ellipse ell = new Ellipse();
            ell.HorizontalAlignment = HorizontalAlignment.Left;
            ell.VerticalAlignment = VerticalAlignment.Top;
            ell.Width = toItem.Diameter;
            ell.Height = toItem.Diameter;
            ell.Fill = new SolidColorBrush(Color.FromArgb(255, rgb[0], rgb[1], rgb[2]));
            Point toPos = GetProvincialCapitalPoint(toItem.To);
            TranslateTransform ttf = new TranslateTransform(toPos.X - ell.Width / 2, toPos.Y - ell.Height / 2);//定位到城市所在的点
            ell.RenderTransform = ttf;
            ell.ToolTip = string.Format("{0} {1}", toItem.To.ToString(), toItem.Tip);
            ell.Opacity = 0;
            return ell;
        }
        #endregion
        #region 将点加入到动画故事版
        /// <summary>
        /// 将点加入到动画故事版
        /// </summary>
        /// <param name="runPoint">运动的点</param>
        /// <param name="toEll">达到城市的圆</param>
        /// <param name="sb">故事版</param>
        /// <param name="particlePath">运动轨迹</param>
        /// <param name="l">运动轨迹的直线距离</param>
        /// <param name="from">来自</param>
        /// <param name="toItem">去</param>
        private void AddPointToStoryboard(Grid runPoint, Ellipse toEll, Storyboard sb, Path particlePath, double l, ProvincialCapital from, MapToItem toItem)
        {
            double pointTime = l / m_Speed;//点运动所需的时间
            double particleTime = pointTime / 2;//轨迹呈现所需时间(跑的比点快两倍)

            #region 运动的点
            TransformGroup tfg = new TransformGroup();
            MatrixTransform mtf = new MatrixTransform();
            tfg.Children.Add(mtf);
            TranslateTransform ttf = new TranslateTransform(-runPoint.Width / 2, -runPoint.Height / 2);//纠正最上角沿path运动到中心沿path运动
            tfg.Children.Add(ttf);
            runPoint.RenderTransform = tfg;

            MatrixAnimationUsingPath maup = new MatrixAnimationUsingPath();
            maup.PathGeometry = particlePath.Data.GetFlattenedPathGeometry();
            maup.Duration = new Duration(TimeSpan.FromSeconds(pointTime));
            maup.RepeatBehavior = RepeatBehavior.Forever;
            maup.AutoReverse = false;
            maup.IsOffsetCumulative = false;
            maup.DoesRotateWithTangent = true;
            Storyboard.SetTarget(maup, runPoint);
            Storyboard.SetTargetProperty(maup, new PropertyPath("(Grid.RenderTransform).Children[0].(MatrixTransform.Matrix)"));
            sb.Children.Add(maup);
            #endregion

            #region 达到城市的圆
            //轨迹到达圆时 圆呈现
            DoubleAnimation ellda = new DoubleAnimation();
            ellda.From = 0.2;//此处值设置0-1会有不同的呈现效果
            ellda.To = 1;
            ellda.Duration = new Duration(TimeSpan.FromSeconds(particleTime));
            ellda.BeginTime = TimeSpan.FromSeconds(particleTime);//推迟动画开始时间 等轨迹连接到圆时 开始播放圆的呈现动画
            ellda.FillBehavior = FillBehavior.HoldEnd;
            Storyboard.SetTarget(ellda, toEll);
            Storyboard.SetTargetProperty(ellda, new PropertyPath(Ellipse.OpacityProperty));
            sb.Children.Add(ellda);
            //圆呈放射状
            RadialGradientBrush rgBrush = new RadialGradientBrush();
            GradientStop gStop0 = new GradientStop(Color.FromArgb(255, 0, 0, 0), 0);
            //此为控制点 color的a值设为0 off值走0-1 透明部分向外放射 初始设为255是为了初始化效果 开始不呈放射状 等跑动的点运动到城市的圆后 color的a值才设为0开始呈现放射动画
            GradientStop gStopT = new GradientStop(Color.FromArgb(255, 0, 0, 0), 0);
            GradientStop gStop1 = new GradientStop(Color.FromArgb(255, 0, 0, 0), 1);
            rgBrush.GradientStops.Add(gStop0);
            rgBrush.GradientStops.Add(gStopT);
            rgBrush.GradientStops.Add(gStop1);
            toEll.OpacityMask = rgBrush;
            //跑动的点达到城市的圆时 控制点由不透明变为透明 color的a值设为0 动画时间为0
            ColorAnimation ca = new ColorAnimation();
            ca.To = Color.FromArgb(0, 0, 0, 0);
            ca.Duration = new Duration(TimeSpan.FromSeconds(0));
            ca.BeginTime = TimeSpan.FromSeconds(pointTime);
            ca.FillBehavior = FillBehavior.HoldEnd;
            Storyboard.SetTarget(ca, toEll);
            Storyboard.SetTargetProperty(ca, new PropertyPath("(Ellipse.OpacityMask).(GradientBrush.GradientStops)[1].(GradientStop.Color)"));
            sb.Children.Add(ca);
            //点达到城市的圆时 呈现放射状动画 控制点的off值走0-1 透明部分向外放射
            DoubleAnimation eda = new DoubleAnimation();
            eda.To = 1;
            eda.Duration = new Duration(TimeSpan.FromSeconds(2));
            eda.RepeatBehavior = RepeatBehavior.Forever;
            eda.BeginTime = TimeSpan.FromSeconds(particleTime);
            Storyboard.SetTarget(eda, toEll);
            Storyboard.SetTargetProperty(eda, new PropertyPath("(Ellipse.OpacityMask).(GradientBrush.GradientStops)[1].(GradientStop.Offset)"));
            sb.Children.Add(eda);
            #endregion

            #region 运动轨迹
            //找到渐变的起点和终点
            Point startPoint = GetProvincialCapitalPoint(from);
            Point endPoint = GetProvincialCapitalPoint(toItem.To);
            Point start = new Point(0, 0);
            Point end = new Point(1, 1);
            if (startPoint.X > endPoint.X)
            {
                start.X = 1;
                end.X = 0;
            }
            if (startPoint.Y > endPoint.Y)
            {
                start.Y = 1;
                end.Y = 0;
            }
            LinearGradientBrush lgBrush = new LinearGradientBrush();
            lgBrush.StartPoint = start;
            lgBrush.EndPoint = end;
            GradientStop lgStop0 = new GradientStop(Color.FromArgb(255, 0, 0, 0), 0);
            GradientStop lgStop1 = new GradientStop(Color.FromArgb(0, 0, 0, 0), 0);
            lgBrush.GradientStops.Add(lgStop0);
            lgBrush.GradientStops.Add(lgStop1);
            particlePath.OpacityMask = lgBrush;
            //运动轨迹呈现
            DoubleAnimation pda0 = new DoubleAnimation();
            pda0.To = 1;
            pda0.Duration = new Duration(TimeSpan.FromSeconds(particleTime));
            pda0.FillBehavior = FillBehavior.HoldEnd;
            Storyboard.SetTarget(pda0, particlePath);
            Storyboard.SetTargetProperty(pda0, new PropertyPath("(Path.OpacityMask).(GradientBrush.GradientStops)[0].(GradientStop.Offset)"));
            sb.Children.Add(pda0);
            DoubleAnimation pda1 = new DoubleAnimation();
            //pda1.From = 0.5; //此处解开注释 值设为0-1 会有不同的轨迹呈现效果
            pda1.To = 1;
            pda1.Duration = new Duration(TimeSpan.FromSeconds(particleTime));
            pda1.FillBehavior = FillBehavior.HoldEnd;
            Storyboard.SetTarget(pda1, particlePath);
            Storyboard.SetTargetProperty(pda1, new PropertyPath("(Path.OpacityMask).(GradientBrush.GradientStops)[1].(GradientStop.Offset)"));
            sb.Children.Add(pda1);
            #endregion
        }
        #endregion
        #endregion
    }
    /// <summary>
    /// 省会,直辖市,特别行政区
    /// </summary>
    public enum ProvincialCapital
    {
        北京,
        天津,
        上海,
        重庆,
        石家庄,
        太原,
        沈阳,
        长春,
        哈尔滨,
        南京,
        杭州,
        合肥,
        福州,
        南昌,
        济南,
        郑州,
        武汉,
        长沙,
        广州,
        海口,
        成都,
        贵阳,
        昆明,
        西安,
        兰州,
        西宁,
        拉萨,
        南宁,
        呼和浩特,
        银川,
        乌鲁木齐,
        香港,
        澳门,
        台北
    }
    /// <summary>
    /// 地图数据项
    /// </summary>
    public class MapItem
    {
        /// <summary>
        /// 出发城市
        /// </summary>
        public ProvincialCapital From { get; set; }
        /// <summary>
        /// 到达城市
        /// </summary>
        public List<MapToItem> To { get; set; }
    }
    /// <summary>
    /// 地图到达城市数据项
    /// </summary>
    public class MapToItem
    {
        /// <summary>
        /// 到达城市
        /// </summary>
        public ProvincialCapital To { get; set; }
        /// <summary>
        /// 到达城市圆点的直径
        /// </summary>
        public double Diameter { get; set; }
        /// <summary>
        /// 提示的值
        /// </summary>
        public string Tip { get; set; }
    }
}

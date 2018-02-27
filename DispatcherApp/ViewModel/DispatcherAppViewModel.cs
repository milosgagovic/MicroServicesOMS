using DispatcherApp.Model;
using DispatcherApp.View;
using DispatcherApp;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using PubSubscribe;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DMSCommon.Model;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO;
using System.Threading;
using DMSCommon.TreeGraph;
using DispatcherApp.Model.Properties;
using TransactionManagerContract;
using System.ServiceModel;
using System.Windows.Data;
using IMSContract;
using DispatcherApp.View.CustomControls;
using DispatcherApp.View.CustomControls.NetworkElementsControls;
using DispatcherApp.Model.Measurements;
using OMSSCADACommon;
using System.Threading.Tasks;
using DispatcherApp.View.CustomControls.TabContentControls;
using GravityAppsMandelkowMetroCharts;

namespace DispatcherApp.ViewModel
{
    public class DispatcherAppViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Task blinkTask;

        public CancellationTokenSource tokenSource = new CancellationTokenSource();

        private IOMSClient proxyToTransactionManager;

        private FrameworkElement frameworkElement = new FrameworkElement();

        private ObservableCollection<ChartSeries> chartSeries = new ObservableCollection<ChartSeries>();
        private ObservableCollection<UIElement> chartBorderItems = new ObservableCollection<UIElement>();
        private string chartTitle = "";
        private string chartSubtitle = "";

        #region Subscriber
        private Subscriber subscriber;
        #endregion

        #region Bindings
        private Dictionary<long, Element> Network = new Dictionary<long, Element>();
        private List<long> Sources = new List<long>();
        private ObservableCollection<Element> breakers = new ObservableCollection<Element>();

        private Dictionary<long, ElementProperties> properties = new Dictionary<long, ElementProperties>();
        private Dictionary<long, ResourceDescription> resourceProperties = new Dictionary<long, ResourceDescription>();
        private ElementProperties currentProperty = new ElementProperties();
        private long currentPropertyMRID;

        private int commandsIndex = 0;
        private bool test = true;

        private ObservableCollection<IncidentReport> incidentReports = new ObservableCollection<IncidentReport>();
        private ObservableCollection<Crew> crews = new ObservableCollection<Crew>();

        private Dictionary<long, Measurement> measurements = new Dictionary<long, Measurement>();

        private Dictionary<long, ObservableCollection<UIElement>> uiNetworks = new Dictionary<long, ObservableCollection<UIElement>>();
        private ObservableCollection<UIElement> mainCanvases = new ObservableCollection<UIElement>();
        private Dictionary<long, int> networkDepth = new Dictionary<long, int>();
        private Canvas mainCanvas = new Canvas();
        private double startHeight = 20;
        private double startWidth = 3;
        private double currentHeight = 20;
        private double currentWidth = 3;

        private ObservableCollection<BorderTabItem> leftTabControlTabs = new ObservableCollection<BorderTabItem>();
        private int leftTabControlIndex = 0;
        private Visibility leftTabControlVisibility = Visibility.Collapsed;

        private ObservableCollection<BorderTabItem> rightTabControlTabs = new ObservableCollection<BorderTabItem>();
        private int rightTabControlIndex = 0;
        private Visibility rightTabControlVisibility = Visibility.Collapsed;

        private ObservableCollection<BorderTabItem> bottomTabControlTabs = new ObservableCollection<BorderTabItem>();
        private int bottomTabControlIndex = 0;
        private Visibility bottomTabControlVisibility = Visibility.Collapsed;

        private ObservableCollection<TreeViewItem> networkMapsBySource = new ObservableCollection<TreeViewItem>();
        private ObservableCollection<Button> networkMapsBySourceButton = new ObservableCollection<Button>();

        private ObservableCollection<TabItem> centerTabControlTabs = new ObservableCollection<TabItem>();
        private int centerTabControlIndex = 0;

        private NetworkExplorerControl networkExplorer = new NetworkExplorerControl();
        private IncidentExplorer incidentExplorer = new IncidentExplorer();
        private OutputControl output = new OutputControl();
        private Dictionary<long, NetworkModelControlExtended> networModelControls = new Dictionary<long, NetworkModelControlExtended>();

        #endregion

        #region Commands
        private RelayCommand _measCommand;

        private RelayCommand _openControlCommand;
        private RelayCommand _closeControlCommand;

        private RelayCommand _propertiesCommand;

        private RelayCommand _sendCrewCommand;

        private RelayCommand _executeSwitchCommand;

        private RelayCommand _generateIncidentByDateChartCommand;
        private RelayCommand _generateIncidentByBreakerChartCommand;
        private RelayCommand _generateStatesByBreakerChartCommand;

        #endregion

        #region Constructor
        public DispatcherAppViewModel()
        {
            Thread.Sleep(5000);

            subscriber = new Subscriber();
            subscriber.Subscribe();
            subscriber.publishUpdateEvent += GetUpdate;
            subscriber.publishCrewEvent += GetCrewUpdate;
            subscriber.publishIncident += GetIncident;
            subscriber.publishCall += GetCallFromConsumers;
            subscriber.publiesBreakers += SearchForIncident;

            NetTcpBinding binding = new NetTcpBinding();
            binding.CloseTimeout = new TimeSpan(1, 0, 0, 0);
            binding.OpenTimeout = new TimeSpan(1, 0, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0, 0);
            binding.MaxReceivedMessageSize = Int32.MaxValue;

            ChannelFactory<IOMSClient> factoryToTMS = new ChannelFactory<IOMSClient>(binding,
                new EndpointAddress("net.tcp://localhost:7090/TMServiceEndpoint"));
            ProxyToTransactionManager = factoryToTMS.CreateChannel();
            TMSAnswerToClient answerFromTransactionManager = new TMSAnswerToClient();

            try
            {
                answerFromTransactionManager = ProxyToTransactionManager.GetNetwork(); 

            }
            catch (Exception e) { }
            InitNetwork();
            InitElementsAndProperties(answerFromTransactionManager);
            DrawElementsOnGraph(answerFromTransactionManager.GraphDeep);

        }

        public void InitNetwork()
        {
            this.Network.Clear();
            this.properties.Clear();
            //this.UINetworks.Clear();
            //this.networModelControls.Clear();
            this.NetworkMapsBySourceButton.Clear();
            this.networkDepth.Clear();
            this.Sources.Clear();
            //this.MainCanvases.Clear();
            this.IncidentReports.Clear();
            this.mainCanvas.Children.Clear();
            this.Crews.Clear();
            this.Breakers.Clear();

            #region FakeNetwork
            //Source s1 = new Source(0, -1, "ES_2") { ElementGID = 0 };
            //Source s2 = new Source(0, -1, "ES_3") { ElementGID = 23 };
            //s2.Marker = false;
            //Node n10 = new Node(24, "CN_10");
            //n10.Parent = s2.ElementGID;
            //s2.End2 = n10.ElementGID;
            //ACLine b15 = new ACLine(25, "ACLS_1");
            //b15.End1 = n10.ElementGID;
            //n10.Children.Add(b15.ElementGID);
            //Consumer b16 = new Consumer(27, "EC_4");
            //b16.End1 = n10.ElementGID;
            //b16.End2 = -1;
            //n10.Children.Add(b16.ElementGID);
            //Node n11 = new Node(26, "CN_2");
            //b15.End2 = n11.ElementGID;
            //n11.Parent = b15.ElementGID;
            //Consumer b17 = new Consumer(28, "EC_4");
            //b17.End1 = n11.ElementGID;
            //b17.End2 = -1;
            //n11.Children.Add(b17.ElementGID);
            //Consumer b18 = new Consumer(29, "EC_4");
            //b18.End1 = n11.ElementGID;
            //b18.End2 = -1;
            //n11.Children.Add(b18.ElementGID);
            //Node n1 = new Node(1, "CN_1") { ElementGID = 1 };
            //n1.Parent = s1.ElementGID;
            //s1.End2 = n1.ElementGID;
            //ACLine b1 = new ACLine(2, "ACLS_1");
            //b1.End1 = n1.ElementGID;
            //n1.Children.Add(b1.ElementGID);
            //Node n2 = new Node(3, "CN_2");
            //b1.End2 = n2.ElementGID;
            //n2.Parent = b1.ElementGID;
            //Switch b2 = new Switch(4, "BR_1");
            //b2.End1 = n2.ElementGID;
            //n2.Children.Add(b2.ElementGID);
            //ACLine b3 = new ACLine(5, "ACLS_3");
            //b3.End1 = n2.ElementGID;
            //n2.Children.Add(b3.ElementGID);
            //Consumer b10 = new Consumer(17, "EC_4");
            //b10.End1 = n2.ElementGID;
            //b10.End2 = -1;
            //n2.Children.Add(b10.ElementGID);
            //Node n3 = new Node(6, "CN_3");
            //b2.End2 = n3.ElementGID;
            //n3.Parent = b2.ElementGID;
            //Switch b4 = new Switch(7, "BR_2");
            //b4.End1 = n3.ElementGID;
            //n3.Children.Add(b4.ElementGID);
            //ACLine b5 = new ACLine(8, "ACLS_2");
            //b5.End1 = n3.ElementGID;
            //b5.Marker = false;
            //n3.Children.Add(b5.ElementGID);
            //Node n4 = new Node(9, "CN_4");
            //b4.End2 = n4.ElementGID;
            //n4.Parent = b4.ElementGID;
            //Consumer b6 = new Consumer(10, "EC_1");
            //b6.End1 = n4.ElementGID;
            //n4.Children.Add(b6.ElementGID);
            //Node n5 = new Node(11, "CN_5");
            //b5.End2 = n5.ElementGID;
            //n5.Parent = b5.ElementGID;
            //Consumer b7 = new Consumer(12, "EC_2");
            //b7.End1 = n5.ElementGID;
            //n5.Children.Add(b7.ElementGID);
            //b7.Marker = false;
            //Node n6 = new Node(13, "CN_6");
            //b3.End2 = n6.ElementGID;
            //n6.Parent = b3.ElementGID;
            //n6.Marker = false;
            //Switch b8 = new Switch(14, "BR_3");
            //b8.End1 = n6.ElementGID;
            //b8.Marker = false;
            //n6.Children.Add(b8.ElementGID);
            //Node n7 = new Node(15, "CN_7");
            //b8.End2 = n7.ElementGID;
            //n7.Parent = b8.ElementGID;
            //Consumer b9 = new Consumer(16, "EC_3");
            //b9.End1 = n7.ElementGID;
            //b9.End2 = -1;
            //n7.Children.Add(b9.ElementGID);
            //Consumer b11 = new Consumer(20, "EC_5");
            //b11.End1 = n7.ElementGID;
            //b11.End2 = -1;
            //n7.Children.Add(b11.ElementGID);
            ////Consumer b12 = new Consumer(21, "EC_5");
            ////b12.End1 = n1.ElementGID;
            ////b12.End2 = -1;
            ////n1.Children.Add(b12.ElementGID);

            //Sources.Add(s1.ElementGID);
            //Sources.Add(s2.ElementGID);

            //Network.Add(s1.ElementGID, s1);
            //Network.Add(s2.ElementGID, s2);
            //Network.Add(n1.ElementGID, n1);
            //Network.Add(n2.ElementGID, n2);
            //Network.Add(n3.ElementGID, n3);
            //Network.Add(n4.ElementGID, n4);
            //Network.Add(n5.ElementGID, n5);
            //Network.Add(n6.ElementGID, n6);
            //Network.Add(n7.ElementGID, n7);
            //Network.Add(b1.ElementGID, b1);
            //Network.Add(b2.ElementGID, b2);
            //Network.Add(b3.ElementGID, b3);
            //Network.Add(b4.ElementGID, b4);
            //Network.Add(b5.ElementGID, b5);
            //Network.Add(b6.ElementGID, b6);
            //Network.Add(b7.ElementGID, b7);
            //Network.Add(b8.ElementGID, b8);
            //Network.Add(b9.ElementGID, b9);
            //Network.Add(b10.ElementGID, b10);
            //Network.Add(b11.ElementGID, b11);
            //Network.Add(b15.ElementGID, b15);
            //Network.Add(n10.ElementGID, n10);
            //Network.Add(n11.ElementGID, n11);
            //Network.Add(b16.ElementGID, b16);
            //Network.Add(b17.ElementGID, b17);
            //Network.Add(b18.ElementGID, b18);
            ////Network.Add(b12.ElementGID, b12);
            ////Network.Add(n8.ElementGID, n8);
            #endregion

        }

        public void InitElementsAndProperties(TMSAnswerToClient answerFromTransactionManager)
        {
            if (answerFromTransactionManager != null && answerFromTransactionManager.Elements != null && answerFromTransactionManager.ResourceDescriptions != null)
            {
                foreach (Element element in answerFromTransactionManager.Elements)
                {
                    this.Network.Add(element.ElementGID, element);
                }

                foreach (ResourceDescription rd in answerFromTransactionManager.ResourceDescriptions)
                {
                    Element element = null;
                    this.Network.TryGetValue(rd.GetProperty(ModelCode.IDOBJ_GID).AsLong(), out element);

                    if (element != null)
                    {
                        if (element is Source)
                        {
                            this.Sources.Add(element.ElementGID);
                            EnergySourceProperties properties = new EnergySourceProperties() { IsEnergized = element.Marker, IsUnderScada = element.UnderSCADA };
                            properties.ReadFromResourceDescription(rd);
                            this.properties.Add(element.ElementGID, properties);
                        }
                        else if (element is Consumer)
                        {
                            EnergyConsumerProperties properties = new EnergyConsumerProperties() { IsEnergized = element.Marker, IsUnderScada = element.UnderSCADA };
                            properties.ReadFromResourceDescription(rd);
                            this.properties.Add(element.ElementGID, properties);
                        }
                        else if (element is ACLine)
                        {
                            ACLineSegmentProperties properties = new ACLineSegmentProperties() { IsEnergized = element.Marker, IsUnderScada = element.UnderSCADA };
                            properties.ReadFromResourceDescription(rd);
                            this.properties.Add(element.ElementGID, properties);
                        }
                        else if (element is Node)
                        {
                            ConnectivityNodeProperties properties = new ConnectivityNodeProperties() { IsEnergized = element.Marker, IsUnderScada = element.UnderSCADA };
                            properties.ReadFromResourceDescription(rd);
                            this.properties.Add(element.ElementGID, properties);
                        }
                        else if (element is Switch)
                        {
                            Switch breaker = element as Switch;
                            this.Breakers.Add(element);
                            BreakerProperties properties = new BreakerProperties() { IsEnergized = element.Marker, IsUnderScada = element.UnderSCADA, Incident = element.Incident, CanCommand = breaker.CanCommand };
                            properties.ValidCommands.Add(CommandTypes.CLOSE);
                            this.CommandIndex = 0;

                            properties.ReadFromResourceDescription(rd);
                            this.properties.Add(element.ElementGID, properties);
                        }
                    }
                }

                foreach (ElementProperties properties in this.properties.Values)
                {
                    if (properties is BreakerProperties)
                    {
                        Element element = null;
                        this.Network.TryGetValue(properties.GID, out element);

                        if (element != null)
                        {
                            Element node = null;
                            Switch breaker = (Switch)element;
                            this.Network.TryGetValue(breaker.End1, out node);

                            if (node != null)
                            {
                                Element branch = null;
                                Node parent = (Node)node;
                                this.Network.TryGetValue(parent.Parent, out branch);

                                if (branch != null)
                                {
                                    ElementProperties parentProperties = null;
                                    this.properties.TryGetValue(branch.ElementGID, out parentProperties);

                                    if (parentProperties != null)
                                    {
                                        properties.Parent = parentProperties;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (ResourceDescription rd in answerFromTransactionManager.ResourceDescriptionsOfMeasurment)
            {
                ResourceDescription meas;
                try
                {
                    meas = answerFromTransactionManager.ResourceDescriptions.Where(p => p.GetProperty(ModelCode.IDOBJ_MRID).AsString() == rd.GetProperty(ModelCode.IDOBJ_MRID).AsString()).FirstOrDefault();
                }
                catch { continue; }

                if (meas != null)
                {
                    try
                    {
                        long psr = meas.GetProperty(ModelCode.MEASUREMENT_PSR).AsLong();
                        DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(psr);

                        if (type == DMSType.BREAKER)
                        {
                            meas.UpdateProperty(rd.GetProperty(ModelCode.DISCRETE_NORMVAL));
                            DigitalMeasurement measurement = new DigitalMeasurement();
                            measurement.ReadFromResourceDescription(meas);

                            ElementProperties properties;
                            Properties.TryGetValue(psr, out properties);

                            if (properties != null)
                            {
                                properties.Measurements.Add(measurement);
                            }
                            
                            this.Measurements.Add(measurement.GID, measurement);
                        }
                        else if (type == DMSType.ENERGCONSUMER)
                        {
                            meas.UpdateProperty(rd.GetProperty(ModelCode.ANALOG_NORMVAL));
                            AnalogMeasurement measurement = new AnalogMeasurement();
                            measurement.ReadFromResourceDescription(meas);

                            ElementProperties properties;
                            Properties.TryGetValue(psr, out properties);

                            if (properties != null)
                            {
                                properties.Measurements.Add(measurement);
                            }

                            properties.IsUnderScada = true;
                            this.Measurements.Add(measurement.GID, measurement);
                        }
                    }
                    catch { }
                }
            }

            if (answerFromTransactionManager.Crews != null)
            {
                foreach (Crew crew in answerFromTransactionManager.Crews)
                {
                    this.Crews.Add(crew);
                }
            }

            if (answerFromTransactionManager.IncidentReports != null)
            {
                foreach (IncidentReport incident in answerFromTransactionManager.IncidentReports)
                {
                    this.IncidentReports.Insert(0, incident);
                }
            }
        }

        public void DrawElementsOnGraph(int depth)
        {
            foreach (long sourceGid in Sources)
            {
                Element element = null;
                Network.TryGetValue(sourceGid, out element);

                if (element != null)
                {
                    Source source = element as Source;

                    ObservableCollection<UIElement> temp = new ObservableCollection<UIElement>();
                    if (!this.UINetworks.TryGetValue(source.ElementGID, out temp))
                    {
                        this.UINetworks.Add(source.ElementGID, new ObservableCollection<UIElement>());
                    }

                    Canvas canvas = new Canvas() { Width = 400, Height = 400 };

                    if (this.UINetworks[source.ElementGID].Count > 0)
                    {
                        this.mainCanvas = (Canvas)this.UINetworks[source.ElementGID][0];
                    }
                    else
                    {
                        this.UINetworks[source.ElementGID].Add(canvas);
                        this.MainCanvases.Add(canvas);
                        this.mainCanvas = canvas;
                    }

                    NetworkModelControlExtended nmc = new NetworkModelControlExtended() { ItemsSourceForCanvas = this.UINetworks[source.ElementGID] };
                    if (this.networModelControls.TryGetValue(source.ElementGID, out nmc))
                    {
                        nmc = this.networModelControls[source.ElementGID];
                    }
                    else
                    {
                        nmc = new NetworkModelControlExtended() { ItemsSourceForCanvas = this.UINetworks[source.ElementGID] };
                        this.networModelControls.Add(source.ElementGID, nmc);
                    }

                    Button but = new Button() { Content = source.MRID, Command = OpenControlCommand, CommandParameter = source.ElementGID, Style = (Style)frameworkElement.FindResource("ButtonStyle") };
                    this.NetworkMapsBySourceButton.Add(but);

                    this.networkDepth.Add(source.ElementGID, depth);

                    if (source != null)
                    {
                        DrawGraph(source);
                    }
                }
            }
        }
        #endregion

        #region DrawGraph
        private void DrawGraph(Source source)
        {
            double cellHeight = mainCanvas.Height / networkDepth[source.ElementGID];

            for (int i = 1; i < networkDepth[source.ElementGID]; i++)
            {
                PlaceGridLines(i, cellHeight);
            }

            Point point1 = new Point()
            {
                X = mainCanvas.Width / 2,
                Y = 5
            };
            Point point2 = new Point()
            {
                X = mainCanvas.Width / 2,
                Y = cellHeight
            };

            PlaceBranch(point1, point2, cellHeight, 0, source, source.ElementGID);

            Element end2 = null;

            Network.TryGetValue(source.End2, out end2);

            if (end2 != null)
            {
                PlaceGraph(end2 as Node, 1, 1, mainCanvas.Width, cellHeight, 1, 0, null);
            }
        }

        private void PlaceGraph(Node currentNode, int x, int y, double cellWidth, double cellHeight, double currentDivision, double offset, Point? point1)
        {
            if (currentNode == null)
            {
                return;
            }

            Point? point2 = PlaceNode(x, y++, cellWidth, cellHeight, offset, currentNode.ElementGID, currentNode.MRID);

            if (point1 != null)
            {
                Element parent = null;

                Network.TryGetValue(currentNode.Parent, out parent);

                if (parent != null)
                {
                    PlaceBranch((Point)point1, (Point)point2, cellHeight, cellWidth, parent as Branch, parent.ElementGID);
                }
            }

            cellWidth = mainCanvas.Width / (currentDivision * currentNode.Children.Count);

            for (int x1 = 1; x1 <= currentNode.Children.Count; x1++)
            {
                double localOffset = offset + (x1 - 1) * cellWidth;

                Element branch = null;

                Network.TryGetValue(currentNode.Children[x1 - 1], out branch);

                if (branch != null)
                {
                    Branch branch1 = branch as Branch;

                    Element end2 = null;

                    Network.TryGetValue(branch1.End2, out end2);

                    if (end2 != null)
                    {
                        PlaceGraph(end2 as Node, x1, y, cellWidth, cellHeight, currentDivision * currentNode.Children.Count, localOffset, point2);
                    }

                    Element consumer = null;

                    Network.TryGetValue(currentNode.Children[x1 - 1], out consumer);

                    if (consumer is Consumer)
                    {
                        PlaceConsumer(y, cellWidth, cellHeight, localOffset, (Point)point2, consumer as Consumer, consumer.ElementGID, consumer.MRID);
                    }
                }
            }
        }

        private void PlaceConsumer(int y, double cellWidth, double cellHeight, double offset, Point point1, Consumer consumer, long id, string mrid)
        {
            ElementProperties prop;
            properties.TryGetValue(id, out prop);

            if (prop != null)
            {
                if (currentHeight >= cellWidth)
                {
                    currentHeight = cellWidth;
                }
                else
                {
                    currentHeight = startHeight;
                }
                if (currentHeight >= cellHeight * 25 / 100)
                {
                    currentHeight = cellHeight * 25 / 100;
                }

                ConsumerControl consumercontr = new ConsumerControl(prop, currentHeight);

                Canvas.SetLeft(consumercontr, offset + /*x * */cellWidth - cellWidth / 2 - currentHeight / 2);
                Canvas.SetTop(consumercontr, y * cellHeight - currentHeight - 4);
                Canvas.SetZIndex(consumercontr, 5);

                consumercontr.Button.Command = PropertiesCommand;
                consumercontr.Button.CommandParameter = prop.GID;
                consumercontr.ToolTip = prop.MRID;

                Point point2 = new Point()
                {
                    X = offset + /*x * */cellWidth - cellWidth / 2,
                    Y = y * cellHeight - currentHeight - 4
                };

                PlaceBranch(point1, point2, cellHeight, cellWidth, consumer, id);

                mainCanvas.Children.Add(consumercontr);
            }
        }

        private Point PlaceSwitch(double cellHeight, double cellWidth, Point point1, Point point2, long id, bool isEnergized, string mrid)
        {
            ElementProperties prop;
            properties.TryGetValue(id, out prop);

            if (prop != null)
            {
                if (currentHeight >= cellWidth)
                {
                    currentHeight = cellWidth;
                }
                else
                {
                    currentHeight = startHeight;
                }
                if (currentHeight >= cellHeight * 25 / 100)
                {
                    currentHeight = cellHeight * 25 / 100;
                }

                SwitchControl switchControl = new SwitchControl(prop, currentHeight);

                Canvas.SetLeft(switchControl, point2.X - (currentHeight) / 2 - currentHeight - 2);
                Canvas.SetTop(switchControl, point2.Y - (cellHeight / 3) - (currentHeight) / 2);
                Canvas.SetZIndex(switchControl, 5);

                switchControl.Button.Command = PropertiesCommand;
                switchControl.Button.CommandParameter = prop.GID;
                switchControl.ButtonCanvas.ToolTip = prop.MRID;

                mainCanvas.Children.Add(switchControl);
            }

            return new Point(point2.X, point2.Y - (cellHeight / 3));
        }

        private void PlaceACLine(double cellHeight, double cellWidth, Point point1, Point point2, long id, bool isEnergized, string mrid)
        {
            ElementProperties prop;
            properties.TryGetValue(id, out prop);

            if (currentWidth >= cellWidth * 80 / 100)
            {
                currentWidth = cellWidth * 80 / 100;
            }
            else
            {
                currentWidth = startWidth;
            }
            currentHeight = startHeight;
            if (currentHeight >= cellHeight * 25 / 100)
            {
                currentHeight = cellHeight * 25 / 100;
            }

            if (prop != null)
            {
                ACLineControl lineControl = new ACLineControl(prop, currentWidth, currentHeight);

                Canvas.SetLeft(lineControl, point2.X - lineControl.Width / 2);
                Canvas.SetTop(lineControl, point2.Y - (cellHeight / 3) - lineControl.Height / 2);
                Canvas.SetZIndex(lineControl, 5);

                lineControl.Command = PropertiesCommand;
                lineControl.CommandParameter = prop.GID;
                lineControl.ToolTip = prop.MRID;

                mainCanvas.Children.Add(lineControl);
            }
        }

        private void PlaceBranch(Point point1, Point point2, double cellHeight, double cellWidth, Branch branch, long id)
        {
            ElementProperties prop;
            properties.TryGetValue(id, out prop);

            Point point4 = new Point();

            if (prop != null)
            {
                if (branch != null)
                {
                    if (branch is Source)
                    {
                        PlaceSource(branch.ElementGID, branch.MRID);
                    }
                    else if (branch is Switch)
                    {
                        point4 = PlaceSwitch(cellHeight, cellWidth, point1, point2, branch.ElementGID, branch.Marker, branch.MRID);
                    }
                    else if (branch is ACLine)
                    {
                        PlaceACLine(cellHeight, cellWidth, point1, point2, branch.ElementGID, branch.Marker, branch.MRID);
                    }

                    Point point3 = new Point()
                    {
                        X = point2.X,
                        Y = point1.Y + (cellHeight / 3)
                    };

                    if (branch is Switch)
                    {
                        Polyline polyline1 = new Polyline();
                        Polyline polyline2 = new Polyline();
                        polyline1.Points.Add(point4);
                        polyline1.Points.Add(point2);

                        polyline1.StrokeThickness = 0.5;
                        Canvas.SetZIndex(polyline1, 0);
                        polyline1.DataContext = prop;

                        polyline2.Points.Add(point1);
                        polyline2.Points.Add(point3);
                        polyline2.Points.Add(point4);

                        polyline2.StrokeThickness = 0.5;
                        Canvas.SetZIndex(polyline2, 0);
                        polyline2.DataContext = prop;

                        Style style1 = new Style();
                        Style style2 = new Style();
                        Setter setter1 = new Setter() { Property = Polyline.StrokeProperty, Value = (SolidColorBrush)frameworkElement.FindResource("SwitchColorClosed") };

                        DataTrigger trigger1 = new DataTrigger() { Binding = new Binding("IsEnergized"), Value = false };
                        Setter setter2 = new Setter() { Property = Polyline.StrokeProperty, Value = Brushes.Blue };

                        trigger1.Setters.Add(setter2);

                        DataTrigger trigger2 = new DataTrigger() { Binding = new Binding("Parent.IsEnergized"), Value = false };

                        trigger2.Setters.Add(setter2);

                        style1.Setters.Add(setter1);
                        style1.Triggers.Add(trigger1);

                        style2.Setters.Add(setter1);
                        style2.Triggers.Add(trigger2);

                        polyline1.Style = style1;
                        polyline2.Style = style2;

                        mainCanvas.Children.Add(polyline1);
                        mainCanvas.Children.Add(polyline2);
                    }
                    else
                    {
                        Polyline polyline1 = new Polyline();
                        polyline1.Points.Add(point1);
                        polyline1.Points.Add(point3);
                        polyline1.Points.Add(point2);

                        polyline1.StrokeThickness = 0.5;
                        Canvas.SetZIndex(polyline1, 0);
                        polyline1.DataContext = prop;

                        Style style1 = new Style();
                        Setter setter1 = new Setter() { Property = Polyline.StrokeProperty, Value = (SolidColorBrush)frameworkElement.FindResource("SwitchColorClosed") };

                        DataTrigger trigger1 = new DataTrigger() { Binding = new Binding("IsEnergized"), Value = false };
                        Setter setter2 = new Setter() { Property = Polyline.StrokeProperty, Value = Brushes.Blue };

                        trigger1.Setters.Add(setter2);

                        style1.Setters.Add(setter1);
                        style1.Triggers.Add(trigger1);

                        polyline1.Style = style1;

                        mainCanvas.Children.Add(polyline1);
                    }
                }
            }
        }

        private Point? PlaceNode(int x, int y, double cellWidth, double cellHeight, double offset, long id, string mrid)
        {
            ElementProperties prop;
            properties.TryGetValue(id, out prop);

            if (prop != null)
            {
                NodeControl node = new NodeControl(3, 3);
                Canvas.SetLeft(node, offset + /*x * */cellWidth - cellWidth / 2 - node.Width / 2);
                Canvas.SetTop(node, y * cellHeight - node.Height / 2);
                Canvas.SetZIndex(node, 5);

                node.Command = PropertiesCommand;
                node.CommandParameter = prop.GID;
                node.ToolTip = prop.MRID;

                mainCanvas.Children.Add(node);

                return new Point()
                {
                    X = offset + cellWidth - cellWidth / 2,
                    Y = y * cellHeight
                };
            }
            else
            {
                return null;
            }
        }

        private void PlaceSource(long id, string mrid)
        {
            Button sourceButton = new Button() { Width = 18, Height = 18 };
            sourceButton.Background = Brushes.Transparent;
            sourceButton.BorderThickness = new Thickness(0);
            sourceButton.BorderBrush = Brushes.Transparent;
            sourceButton.ToolTip = mrid;
            sourceButton.Content = new Image() { Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "/../../View/Resources/Images/triangle.png")) };
            Canvas.SetLeft(sourceButton, mainCanvas.Width / 2 - sourceButton.Width / 2);
            Canvas.SetZIndex(sourceButton, 5);
            mainCanvas.Children.Add(sourceButton);

            SetProperties(sourceButton, id);
        }

        private void PlaceGridLines(int i, double cellHeight)
        {
            Line line = new Line();
            line.Stroke = Brushes.Gray;
            line.StrokeThickness = 0.5;
            line.X1 = 0;
            line.X2 = mainCanvas.Width;
            line.Y1 = i * cellHeight;
            line.Y2 = i * cellHeight;

            mainCanvas.Children.Add(line);
        }

        private void SetProperties(Button button, long id)
        {
            ElementProperties property;
            Element element;
            this.properties.TryGetValue(id, out property);
            this.Network.TryGetValue(id, out element);

            button.Command = PropertiesCommand;
            button.CommandParameter = id;
        }
        #endregion

        #region Command execution
        public RelayCommand GenerateIncidentByDateChartCommand
        {
            get
            {
                return _generateIncidentByDateChartCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteGenerateIncidentByDateChartCommand(parameter);
                    });
            }
        }

        public RelayCommand GenerateIncidentByBreakerChartCommand
        {
            get
            {
                return _generateIncidentByBreakerChartCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteGenerateIncidentByBreakerChartCommand(parameter);
                    });
            }
        }

        public RelayCommand GenerateStatesByBreakerChartCommand
        {
            get
            {
                return _generateStatesByBreakerChartCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteGenerateStatesByBreakerChartCommand(parameter);
                    });
            }
        }

        public RelayCommand OpenControlCommand
        {
            get
            {
                return _openControlCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteOpenControlCommand(parameter);
                    });
            }
        }

        public RelayCommand CloseControlCommand
        {
            get
            {
                return _closeControlCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteCloseControlCommand(parameter);
                    });
            }
        }

        public RelayCommand PropertiesCommand
        {
            get
            {
                return _propertiesCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecutePropertiesCommand(parameter);
                    });
            }
        }

        public RelayCommand SendCrewCommand
        {
            get
            {
                return _sendCrewCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteSendCrewCommand(parameter);
                    });
            }
        }

        public RelayCommand ExecuteSwitchCommand
        {
            get
            {
                return _executeSwitchCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteSwitchCommandd(parameter);
                    });
            }
        }

        private void ExecuteGenerateIncidentByDateChartCommand(object parameter)
        {
            try
            {
                bool allDates = false;
                DateTime date;
                try
                {
                    date = (DateTime)parameter;
                }
                catch
                {
                    allDates = true;
                    date = DateTime.UtcNow;
                }
                 
                List<List<IncidentReport>> reportsByBreaker = new List<List<IncidentReport>>();
                List<string> mrids = new List<string>();

                foreach (Switch breaker in this.Breakers)
                {
                    mrids.Add(breaker.MRID);
                }

                if (!allDates)
                {
                    reportsByBreaker = ProxyToTransactionManager.GetReportsForSpecificDateSortByBreaker(mrids, date);
                }
                else
                {
                    reportsByBreaker = ProxyToTransactionManager.GetAllReportsSortByBreaker(mrids);
                }

                ClusteredColumnChart chart = new ClusteredColumnChart();
                this.ChartSeries.Clear();
                this.ChartBorderItems.Clear();

                int i = 0;
                foreach (List<IncidentReport> reports in reportsByBreaker)
                {
                    if (reports.Count == 0)
                    {
                        reports.Add(new IncidentReport() { LostPower = 0, MrID = "a" });
                    }
                    else
                    {
                        foreach (IncidentReport report in reports)
                        {
                            report.LostPower = reports.Count;
                        }
                    }

                    ChartSeries series = new ChartSeries();
                    series.SeriesTitle = mrids[i++];
                    series.ItemsSource = reports;
                    series.DisplayMember = "MrID";
                    series.ValueMember = "LostPower";

                    this.ChartSeries.Add(series);
                }
                
                this.ChartTitle = "Number of Incidents for Breakers";
                if (!allDates)
                {
                    this.ChartSubtitle = "Date: " + date.Day + "/" + date.Month + "/" + date.Year;
                }
                else
                {
                    this.ChartSubtitle = "All days";
                }

                chart.Series = this.ChartSeries;
                chart.HorizontalAlignment = HorizontalAlignment.Stretch;
                chart.VerticalAlignment = VerticalAlignment.Stretch;
                chart.MinHeight = 400;
                this.ChartBorderItems.Add(chart);
            }
            catch { }
        }

        private void ExecuteGenerateIncidentByBreakerChartCommand(object parameter)
        {
            try
            {
                Switch breaker;
                try
                {
                    breaker = (Switch)parameter;
                }
                catch
                {
                    return;
                }

                List<List<IncidentReport>> reportsByBreaker = new List<List<IncidentReport>>();

                reportsByBreaker = ProxyToTransactionManager.GetReportsForMrID(breaker.MRID);

                ClusteredColumnChart chart = new ClusteredColumnChart();
                this.ChartSeries.Clear();
                this.ChartBorderItems.Clear();

                int i = 0;
                foreach (List<IncidentReport> reports in reportsByBreaker)
                {
                    if (reports.Count == 0)
                    {
                        reports.Add(new IncidentReport() { LostPower = 0, MrID = "a" });
                    }
                    else
                    {
                        foreach (IncidentReport report in reports)
                        {
                            report.LostPower = reports.Count;
                        }
                    }

                    ChartSeries series = new ChartSeries();
                    series.SeriesTitle = reports[0].Time.Day + "/" + reports[0].Time.Month + "/" + reports[0].Time.Year;
                    series.ItemsSource = reports;
                    series.DisplayMember = "MrID";
                    series.ValueMember = "LostPower";

                    this.ChartSeries.Add(series);
                }

                this.ChartTitle = "Number of Incidents by Days";
                this.ChartSubtitle = "Breaker: " + breaker.MRID;

                chart.Series = this.ChartSeries;
                chart.HorizontalAlignment = HorizontalAlignment.Stretch;
                chart.VerticalAlignment = VerticalAlignment.Stretch;
                chart.MinHeight = 400;
                this.ChartBorderItems.Add(chart);
            }
            catch { }
        }

        private void ExecuteGenerateStatesByBreakerChartCommand(object parameter)
        {
            try
            {
                Switch breaker;
                try
                {
                    breaker = (Switch)parameter;
                }
                catch
                {
                    return;
                }

                List<List<ElementStateReport>> reportsByBreaker = new List<List<ElementStateReport>>();

                reportsByBreaker = ProxyToTransactionManager.GetElementStateReportsForMrID(breaker.MRID);

                ClusteredColumnChart chart = new ClusteredColumnChart();
                this.ChartSeries.Clear();
                this.ChartBorderItems.Clear();

                int i = 0;
                foreach (List<ElementStateReport> reports in reportsByBreaker)
                {
                    ChartSeries series = new ChartSeries();
                    series.SeriesTitle = string.Format("{0}/{1}/{2}\n{3}:{4}:{5}", reports[0].Time.Day, reports[0].Time.Month, reports[0].Time.Year, reports[0].Time.Hour, reports[0].Time.Minute, reports[0].Time.Second);
                    series.ItemsSource = reports;
                    series.DisplayMember = "MrID";
                    series.ValueMember = "State";

                    this.ChartSeries.Add(series);
                }

                this.ChartTitle = "States of a Breaker (0 - Closed, 1 - Opened)";
                this.ChartSubtitle = "Breaker: " + breaker.MRID;

                chart.Series = this.ChartSeries;
                chart.HorizontalAlignment = HorizontalAlignment.Stretch;
                chart.VerticalAlignment = VerticalAlignment.Stretch;
                chart.MinHeight = 400;
                this.ChartBorderItems.Add(chart);
            }
            catch { }
        }

        private void ExecuteSwitchCommandd(object parameter)
        {
            Measurement measurement = this.Measurements.Where(m => m.Value.MRID == (string)parameter).FirstOrDefault().Value;
            if (measurement != null)
            {
                ElementProperties elementProperties;
                Properties.TryGetValue(measurement.Psr, out elementProperties);

                if (elementProperties != null)
                {
                    elementProperties.CanCommand = false;
                }
            }

            ProxyToTransactionManager.SendCommandToSCADA(TypeOfSCADACommand.WriteDigital, (string)parameter, CommandTypes.CLOSE, 0);
        }

        private void ExecuteSendCrewCommand(object parameter)
        {
            var values = (object[])parameter;
            var datetime = (DateTime)values[0];
            var crew = (Crew)values[1];

            IncidentReport report = new IncidentReport();
            foreach (IncidentReport ir in IncidentReports)
            {
                if (DateTime.Compare(ir.Time, (DateTime)datetime) == 0)
                {
                    report = ir;
                    break;
                }
            }

           // report.Crew = crew;
            report.CrewSent = true;
            report.IncidentState = IncidentState.PENDING;

            ProxyToTransactionManager.SendCrew(report);

            ElementProperties element = Properties.Where(p => p.Value.MRID == report.MrID).FirstOrDefault().Value;
            element.CrewSent = true;
        }

        private void ExecutePropertiesCommand(object parameter)
        {
            Element element;
            properties.TryGetValue((long)parameter, out currentProperty);
            Network.TryGetValue((long)parameter, out element);

            if (currentProperty != null)
            {
                CurrentPropertyMRID = currentProperty.GID;
                bool exists = false;
                int i = 0;

                for (i = 0; i < RightTabControlTabs.Count; i++)
                {
                    if (RightTabControlTabs[i].Header as string == "Properties")
                    {
                        SetTabContent(RightTabControlTabs[i], element);
                        exists = true;
                        this.RightTabControlIndex = i;
                        break;
                    }
                }

                if (!exists)
                {
                    BorderTabItem ti = new BorderTabItem() { Header = "Properties", Style = (Style)frameworkElement.FindResource("TabItemRightStyle") };
                    ti.Title.Text = "Properties";
                    SetTabContent(ti, element);
                    //if (!RightTabControlTabs.Contains(ti))
                    //{
                    this.RightTabControlTabs.Add(ti);
                    this.RightTabControlIndex = this.RightTabControlTabs.Count - 1;
                    //}
                }

                this.RightTabControlVisibility = Visibility.Visible;
            }
        }

        private void SetTabContent(BorderTabItem ti, Element element)
        {
            if (element != null)
            {
                if (element is Node)
                {
                    ti.Scroll.Content = new NodePropertiesControl();
                }
                else if (element is Switch)
                {
                    ti.Scroll.Content = new SwitchPropertiesControl();
                }
                else if (element is Consumer)
                {
                    ti.Scroll.Content = new ConsumerPropertiesControl();
                }
                else if (element is Source)
                {
                    ti.Scroll.Content = new SourcePropertiesControl();
                }
                else if (element is ACLine)
                {
                    ti.Scroll.Content = new ACLinePropertiesControl();
                }
            }
            else
            {
                ti.Scroll.Content = new EmptyPropertiesControl();
            }
        }

        private void ExecuteOpenControlCommand(object parameter)
        {
            if (parameter as string == "Network Explorer")
            {
                bool exists = false;
                int i = 0;

                for (i = 0; i < LeftTabControlTabs.Count; i++)
                {
                    if (LeftTabControlTabs[i].Header == parameter)
                    {
                        exists = true;
                        this.LeftTabControlIndex = i;
                        break;
                    }
                }

                if (!exists)
                {
                    BorderTabItem ti = new BorderTabItem() { Header = parameter, Style = (Style)frameworkElement.FindResource("TabItemLeftStyle") };
                    ti.Scroll.Content = networkExplorer;
                    ti.Title.Text = (string)parameter;

                    if (!leftTabControlTabs.Contains(ti))
                    {
                        this.LeftTabControlTabs.Add(ti);
                        this.LeftTabControlIndex = this.LeftTabControlTabs.Count - 1;
                    }
                }

                this.LeftTabControlVisibility = Visibility.Visible;
            }
            else if (parameter as string == "Properties")
            {
                bool exists = false;
                int i = 0;

                for (i = 0; i < RightTabControlTabs.Count; i++)
                {
                    if (RightTabControlTabs[i].Header == parameter)
                    {
                        exists = true;
                        this.RightTabControlIndex = i;
                        break;
                    }
                }

                if (!exists)
                {
                    BorderTabItem ti = new BorderTabItem() { Header = parameter, Style = (Style)frameworkElement.FindResource("TabItemRightStyle") };
                    if (!RightTabControlTabs.Contains(ti))
                    {
                        ti.Title.Text = (string)parameter;
                        SetTabContent(ti, null);
                        this.RightTabControlTabs.Add(ti);
                        this.RightTabControlIndex = this.RightTabControlTabs.Count - 1;
                    }
                }

                this.RightTabControlVisibility = Visibility.Visible;
            }
            else if (parameter as string == "Incident Explorer" || parameter as string == "Output")
            {
                bool exists = false;
                int i = 0;

                for (i = 0; i < BottomTabControlTabs.Count; i++)
                {
                    if (BottomTabControlTabs[i].Header == parameter)
                    {
                        exists = true;
                        this.BottomTabControlIndex = i;
                        break;
                    }
                }

                if (!exists)
                {
                    BorderTabItem ti = new BorderTabItem() { Header = parameter, Style = (Style)frameworkElement.FindResource("TabItemBottomStyle") };
                    if (parameter as string == "Incident Explorer")
                    {
                        ti.Scroll.Content = incidentExplorer;
                        ti.Title.Text = (string)parameter;
                    }
                    else if (parameter as string == "Output")
                    {
                        ti.Scroll.Content = output;
                        ti.Title.Text = (string)parameter;
                    }

                    if (!BottomTabControlTabs.Contains(ti))
                    {
                        this.BottomTabControlTabs.Add(ti);
                        this.BottomTabControlIndex = this.BottomTabControlTabs.Count - 1;
                    }
                }

                this.BottomTabControlVisibility = Visibility.Visible;
            }
            else if (parameter as string == "Report Explorer")
            {
                bool exists = false;
                int i = 0;

                for (i = 0; i < CenterTabControlTabs.Count; i++)
                {
                    if (CenterTabControlTabs[i].Header == parameter)
                    {
                        exists = true;
                        this.CenterTabControlIndex = i;
                        break;
                    }
                }

                if (!exists)
                {
                    BorderTabItem ti = new BorderTabItem()
                    {
                        Header = parameter,
                        Style = (Style)frameworkElement.FindResource("TabItemCenterStyle")
                    };

                    ti.Scroll.Content = new ReportExplorer();
                    ti.Title.Text = "";

                    if (!CenterTabControlTabs.Contains(ti))
                    {
                        this.CenterTabControlTabs.Add(ti);
                        this.CenterTabControlIndex = this.CenterTabControlTabs.Count - 1;
                    }
                }
            }
            else
            {
                bool exists = false;
                int i = 0;
                Element element = null;
                if (parameter != null)
                {
                    Network.TryGetValue((long)parameter, out element);

                    if (element != null)
                    {
                        for (i = 0; i < CenterTabControlTabs.Count; i++)
                        {
                            if (CenterTabControlTabs[i].Header as string == element.MRID)
                            {
                                exists = true;
                                this.CenterTabControlIndex = i;
                                break;
                            }
                        }
                    }

                    if (!exists)
                    {
                        BorderTabItem ti = new BorderTabItem()
                        {
                            Header = element.MRID,
                            Style = (Style)frameworkElement.FindResource("TabItemCenterStyle")
                        };

                        ti.Scroll.Content = networModelControls[(long)parameter];
                        ti.Title.Text = "";

                        if (!CenterTabControlTabs.Contains(ti))
                        {
                            this.CenterTabControlTabs.Add(ti);
                            this.CenterTabControlIndex = this.CenterTabControlTabs.Count - 1;
                        }
                    }
                }
            }
        }

        private void ExecuteCloseControlCommand(object parameter)
        {
            if ((string)parameter == "Network Explorer")
            {
                int i = 0;

                for (i = 0; i < LeftTabControlTabs.Count; i++)
                {
                    if ((string)LeftTabControlTabs[i].Header == (string)parameter)
                    {
                        LeftTabControlTabs[i].Content = null;
                        LeftTabControlTabs[i].Visibility = Visibility.Collapsed;
                        LeftTabControlTabs.RemoveAt(i);
                        break;
                    }
                }

                if (LeftTabControlTabs.Count == 0)
                {
                    this.LeftTabControlVisibility = Visibility.Collapsed;
                }
            }
            else if ((string)parameter == "Properties")
            {
                int i = 0;

                for (i = 0; i < RightTabControlTabs.Count; i++)
                {
                    if ((string)RightTabControlTabs[i].Header == (string)parameter)
                    {
                        RightTabControlTabs[i].Content = null;
                        RightTabControlTabs[i].Visibility = Visibility.Collapsed;
                        RightTabControlTabs.RemoveAt(i);
                        break;
                    }
                }

                if (RightTabControlTabs.Count == 0)
                {
                    this.RightTabControlVisibility = Visibility.Collapsed;
                }
            }
            else if ((string)parameter == "Incident Explorer" || (string)parameter == "Output")
            {
                int i = 0;

                for (i = 0; i < BottomTabControlTabs.Count; i++)
                {
                    if ((string)BottomTabControlTabs[i].Header == (string)parameter)
                    {
                        BottomTabControlTabs[i].Content = null;
                        BottomTabControlTabs[i].Visibility = Visibility.Collapsed;
                        BottomTabControlTabs.RemoveAt(i);
                        break;
                    }
                }

                if (BottomTabControlTabs.Count == 0)
                {
                    this.BottomTabControlVisibility = Visibility.Collapsed;
                }
            }
        }
        #endregion

        #region Properties
        public ObservableCollection<BorderTabItem> LeftTabControlTabs
        {
            get
            {
                return leftTabControlTabs;
            }
            set
            {
                leftTabControlTabs = value;
            }
        }

        public int LeftTabControlIndex
        {
            get
            {
                return leftTabControlIndex;
            }
            set
            {
                leftTabControlIndex = value;
                RaisePropertyChanged("LeftTabControlIndex");
            }
        }

        public ObservableCollection<BorderTabItem> RightTabControlTabs
        {
            get
            {
                return rightTabControlTabs;
            }
            set
            {
                rightTabControlTabs = value;
            }
        }

        public int RightTabControlIndex
        {
            get
            {
                return rightTabControlIndex;
            }
            set
            {
                rightTabControlIndex = value;
                RaisePropertyChanged("RightTabControlIndex");
            }
        }

        public ObservableCollection<BorderTabItem> BottomTabControlTabs
        {
            get
            {
                return bottomTabControlTabs;
            }
            set
            {
                bottomTabControlTabs = value;
            }
        }

        public int BottomTabControlIndex
        {
            get
            {
                return bottomTabControlIndex;
            }
            set
            {
                bottomTabControlIndex = value;
                RaisePropertyChanged("BottomTabControlIndex");
            }
        }

        public ObservableCollection<TabItem> CenterTabControlTabs
        {
            get
            {
                return centerTabControlTabs;
            }
            set
            {
                centerTabControlTabs = value;
            }
        }

        public int CenterTabControlIndex
        {
            get
            {
                return centerTabControlIndex;
            }
            set
            {
                centerTabControlIndex = value;
                RaisePropertyChanged("CenterTabControlIndex");
            }
        }

        public int CommandIndex
        {
            get
            {
                return commandsIndex;
            }
            set
            {
                commandsIndex = value;
                RaisePropertyChanged("CommandIndex");
            }
        }

        public Visibility LeftTabControlVisibility
        {
            get
            {
                return leftTabControlVisibility;
            }
            set
            {
                leftTabControlVisibility = value;
                RaisePropertyChanged("LeftTabControlVisibility");
            }
        }

        public Visibility RightTabControlVisibility
        {
            get
            {
                return rightTabControlVisibility;
            }
            set
            {
                rightTabControlVisibility = value;
                RaisePropertyChanged("RightTabControlVisibility");
            }
        }

        public Visibility BottomTabControlVisibility
        {
            get
            {
                return bottomTabControlVisibility;
            }
            set
            {
                bottomTabControlVisibility = value;
                RaisePropertyChanged("BottomTabControlVisibility");
            }
        }

        public ObservableCollection<IncidentReport> IncidentReports
        {
            get
            {
                return incidentReports;
            }
            set
            {
                incidentReports = value;
            }
        }

        public ObservableCollection<Crew> Crews
        {
            get
            {
                return crews;
            }
            set
            {
                crews = value;
            }
        }

        public ObservableCollection<TreeViewItem> NetworkMapsBySource
        {
            get
            {
                return networkMapsBySource;
            }
            set
            {
                networkMapsBySource = value;
            }
        }

        public ObservableCollection<UIElement> MainCanvases
        {
            get
            {
                return mainCanvases;
            }
            set
            {
                mainCanvases = value;
            }
        }

        public ObservableCollection<Button> NetworkMapsBySourceButton
        {
            get
            {
                return networkMapsBySourceButton;
            }
            set
            {
                networkMapsBySourceButton = value;
            }
        }

        public ObservableCollection<ChartSeries> ChartSeries
        {
            get
            {
                return chartSeries;
            }
            set
            {
                chartSeries = value;
            }
        }

        public ObservableCollection<UIElement> ChartBorderItems
        {
            get
            {
                return chartBorderItems;
            }
            set
            {
                chartBorderItems = value;
            }
        }

        public ObservableCollection<Element> Breakers
        {
            get
            {
                return breakers;
            }
            set
            {
                breakers = value;
            }
        }

        public Dictionary<long, ObservableCollection<UIElement>> UINetworks
        {
            get
            {
                return uiNetworks;
            }
            set
            {
                uiNetworks = value;
            }
        }

        public Dictionary<long, ElementProperties> Properties
        {
            get
            {
                return properties;
            }
            set
            {
                properties = value;
                RaisePropertyChanged("Properties");
            }
        }

        public ElementProperties CurrentProperty
        {
            get
            {
                return currentProperty;
            }
            set
            {
                currentProperty = value;
                RaisePropertyChanged("CurrentProperty");
            }
        }

        public long CurrentPropertyMRID
        {
            get
            {
                return currentPropertyMRID;
            }
            set
            {
                currentPropertyMRID = value;
                RaisePropertyChanged("CurrentPropertyMRID");
            }
        }

        public Dictionary<long, Measurement> Measurements
        {
            get
            {
                return measurements;
            }
            set
            {
                measurements = value;
                RaisePropertyChanged("Measurements");
            }
        }

        public string ChartTitle
        {
            get
            {
                return chartTitle;
            }
            set
            {
                chartTitle = value;
                RaisePropertyChanged("ChartTitle");
            }
        }

        public string ChartSubtitle
        {
            get
            {
                return chartSubtitle;
            }
            set
            {
                chartSubtitle = value;
                RaisePropertyChanged("ChartSubtitle");
            }
        }

        public IOMSClient ProxyToTransactionManager
        {
            get { return proxyToTransactionManager; }
            set { proxyToTransactionManager = value; }
        }

        #endregion Properties

        #region Miscelaneous
        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion

        #region Publish methods
        private void GetUpdate(List<SCADAUpdateModel> update)
        {
            if (update != null)
            {
                if (update.ElementAt(0).IsElementAdded == true)
                {
                    NetTcpBinding binding = new NetTcpBinding();
                    binding.CloseTimeout = new TimeSpan(1, 0, 0, 0);
                    binding.OpenTimeout = new TimeSpan(1, 0, 0, 0);
                    binding.ReceiveTimeout = new TimeSpan(1, 0, 0, 0);
                    binding.SendTimeout = new TimeSpan(1, 0, 0, 0);
                    binding.MaxReceivedMessageSize = Int32.MaxValue;

                    ChannelFactory<IOMSClient> factoryToTMS = new ChannelFactory<IOMSClient>(binding,
                        new EndpointAddress("net.tcp://localhost:6080/TransactionManagerService"));
                    ProxyToTransactionManager = factoryToTMS.CreateChannel();
                    TMSAnswerToClient answerFromTransactionManager = new TMSAnswerToClient();

                    try
                    {
                        answerFromTransactionManager = ProxyToTransactionManager.GetNetwork();
                    }
                    catch (Exception e) { }

                    InitNetwork();
                    InitElementsAndProperties(answerFromTransactionManager);
                    DrawElementsOnGraph(answerFromTransactionManager.GraphDeep);

                    return;
                }

                int i = 0;
                foreach (SCADAUpdateModel sum in update)
                {
                    ElementProperties property;
                    properties.TryGetValue(sum.Gid, out property);
                    if (property != null)
                    {
                        property.IsEnergized = sum.IsEnergized;

                        if (property is BreakerProperties && i == 0)
                        {
                            Measurement measurement;
                            Measurements.TryGetValue(property.Measurements[0].GID, out measurement);
                            DigitalMeasurement digitalMeasurement = (DigitalMeasurement)measurement;

                            if (digitalMeasurement != null)
                            {
                                digitalMeasurement.State = sum.State;
                            }
                        }
                    }
                    i++;
                }
            }
        }

        private void GetCrewUpdate(SCADAUpdateModel update)
        {
            Console.WriteLine(update.Response.ToString());
        }

        private void GetIncident(IncidentReport report)
        {
            IncidentReport temp = new IncidentReport();
            bool found = false;
            foreach (IncidentReport ir in IncidentReports)
            {
                if (DateTime.Compare(ir.Time, report.Time) == 0)
                {
                    temp = ir;
                    found = true;
                    break;
                }
            }
            if (found)
            {
                temp.Reason = report.Reason;
                temp.RepairTime = report.RepairTime;
                temp.IncidentState = report.IncidentState;
            }
            else
            {
                IncidentReports.Insert(0, report);
            }

            try
            {
                ElementProperties element = Properties.Where(p => p.Value.MRID == report.MrID).FirstOrDefault().Value;
                if (report.IncidentState == IncidentState.REPAIRED)
                {
                    element.Incident = false;
                    element.CanCommand = true;
                }
                else
                {
                    element.Incident = true;
                }
                element.CrewSent = false;
            }
            catch { }
        }

        private void GetCallFromConsumers(SCADAUpdateModel call)
        {
            ElementProperties property;
            properties.TryGetValue(call.Gid, out property);
            if (property != null)
            {
                property.IsEnergized = call.IsEnergized;
            }
        }
        private void SearchForIncident(bool isIncident, long incidentBreaker)
        {

            ElementProperties propBr;
            properties.TryGetValue(incidentBreaker, out propBr);
            if (propBr != null)
            {
                try
                {
                    if (isIncident == false)
                    {
                        tokenSource = new CancellationTokenSource();
                        CancellationToken token = tokenSource.Token;

                        blinkTask = Task.Factory.StartNew(() => Blink(propBr, token), token);
                    }
                    else if (isIncident)
                    {
                        tokenSource.Cancel();
                        propBr.IsCandidate = true;
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
        }
        private async Task Blink(ElementProperties sw,CancellationToken ct)
        {
            while (true)
            {
                await Task.Delay(800);
                sw.IsCandidate = sw.IsCandidate == true ? false : true;

                if (ct.IsCancellationRequested)
                {
                    ct.ThrowIfCancellationRequested();
                }
            }
        }
        #endregion
    }
}


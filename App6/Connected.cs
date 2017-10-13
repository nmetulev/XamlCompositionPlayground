using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace ToolkitPreview
{
    public class Connected
    {
        private static List<ConnectedAnimationProperties> _connectedAnnimationsProps = new List<ConnectedAnimationProperties>();
        private static Dictionary<string, ConnectedAnimationProperties> _previousPageConnectedAnimationProps = new Dictionary<string, ConnectedAnimationProperties>();
        private static Dictionary<UIElement, List<UIElement>> _coordinatedAnimationElements = new Dictionary<UIElement, List<UIElement>>();

        private static Frame _navigationFrame;

        public static Frame NavigationFrame
        {
            get
            {
                return _navigationFrame;
            }

            set
            {
                _navigationFrame = value;
                _navigationFrame.Navigating += _frame_Navigating;
                _navigationFrame.Navigated += _frame_Navigated;
            }
        }

        private static void _frame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            // make sure all bindings and properties have been set
            RoutedEventHandler handler = null;
            handler = (s, args) =>
            {
                var page = s as Page;
                page.Loaded -= handler;

                object parameter;
                if (e.NavigationMode == Windows.UI.Xaml.Navigation.NavigationMode.Back)
                {
                    var sourcePage = (sender as Frame).ForwardStack.LastOrDefault();
                    parameter = sourcePage?.Parameter ?? null;
                }
                else
                {
                    parameter = e.Parameter;
                }

                var cas = ConnectedAnimationService.GetForCurrentView();

                foreach (var props in _connectedAnnimationsProps)
                {
                    var connectedAnimation = cas.GetAnimation(props.Key);
                    if (connectedAnimation != null)
                    {
                        if (props.IsListAnimation && parameter != null)
                        {
                            props.ListViewBase.ScrollIntoView(parameter);
                            // give time to the ui thread to scroll the list
                            var t = props.ListViewBase.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                            {
                                try
                                {
                                    var success = await props.ListViewBase.TryStartConnectedAnimationAsync(connectedAnimation, parameter, props.ElementName);
                                }
                                catch (Exception)
                                {
                                    connectedAnimation.Cancel();
                                }
                            });
                        }
                        else if (!props.IsListAnimation)
                        {
                            if (_coordinatedAnimationElements.TryGetValue(props.Element, out var coordinatedElements))
                            {
                                connectedAnimation.TryStart(props.Element, coordinatedElements);
                            }
                            else
                            {
                                connectedAnimation.TryStart(props.Element);
                            }
                        }
                    }

                    if (_previousPageConnectedAnimationProps.ContainsKey(props.Key))
                    {
                        _previousPageConnectedAnimationProps.Remove(props.Key);
                    }
                }

                // if there are animations that were prepared on previous page but no elements on this page have the same key - cancel
                foreach (var _previousProps in _previousPageConnectedAnimationProps)
                {
                    var connectedAnimation = cas.GetAnimation(_previousProps.Key);
                    if (connectedAnimation != null)
                    {
                        connectedAnimation.Cancel();
                    }
                }

                _previousPageConnectedAnimationProps.Clear();
            };

            var navigatedPage = NavigationFrame.Content as Page;
            navigatedPage.Loaded += handler;
            
        }

        private static void _frame_Navigating(object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
        {
            var parameter = e.Parameter != null && !(e.Parameter is string str && string.IsNullOrEmpty(str)) ? e.Parameter : null;

            var cas = ConnectedAnimationService.GetForCurrentView();
            foreach (var props in _connectedAnnimationsProps)
            {
                if (props.IsListAnimation && parameter != null)
                {
                    props.ListViewBase.PrepareConnectedAnimation(props.Key, e.Parameter, props.ElementName);
                }
                else if (!props.IsListAnimation)
                {
                    cas.PrepareToAnimate(props.Key, props.Element);
                }
                else
                {
                    continue;
                }

                _previousPageConnectedAnimationProps[props.Key] = props;
            }

            _connectedAnnimationsProps.Clear();
            _coordinatedAnimationElements.Clear();
        }

        public static string GetKey(DependencyObject obj)
        {
            return (string)obj.GetValue(KeyProperty);
        }

        public static void SetKey(DependencyObject obj, string value)
        {
            obj.SetValue(KeyProperty, value);
        }

        public static UIElement GetAnchorElement(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(AnchorElementProperty);
        }

        public static void SetAnchorElement(DependencyObject obj, UIElement value)
        {
            obj.SetValue(AnchorElementProperty, value);
        }

        public static string GetListItemKey(DependencyObject obj)
        {
            return (string)obj.GetValue(ListItemKeyProperty);
        }

        public static void SetListItemKey(DependencyObject obj, string value)
        {
            obj.SetValue(ListItemKeyProperty, value);
        }

        public static string GetListItemElementName(DependencyObject obj)
        {
            return (string)obj.GetValue(ListItemElementNameProperty);
        }

        public static void SetListItemElementName(DependencyObject obj, string value)
        {
            obj.SetValue(ListItemElementNameProperty, value);
        }

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.RegisterAttached("Key", 
                                                typeof(string), 
                                                typeof(Connected), 
                                                new PropertyMetadata(null, OnKeyChanged));

        public static readonly DependencyProperty AnchorElementProperty =
            DependencyProperty.RegisterAttached("AnchorElement", 
                                                typeof(UIElement), 
                                                typeof(Connected), 
                                                new PropertyMetadata(null, OnAnchorElementChanged));

        public static readonly DependencyProperty ListItemKeyProperty =
            DependencyProperty.RegisterAttached("ListItemKey", 
                                                typeof(string), 
                                                typeof(Connected), 
                                                new PropertyMetadata(null, OnListItemKeyChanged));


        public static readonly DependencyProperty ListItemElementNameProperty =
            DependencyProperty.RegisterAttached("ListItemElementName", 
                                                typeof(string), 
                                                typeof(Connected), 
                                                new PropertyMetadata(null, OnListItemElementNameChanged));

        private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetupFrame();

            if (d is FrameworkElement element)
            {
                var animation = new ConnectedAnimationProperties()
                {
                    Key = e.NewValue as string,
                    Element = element,
                };

                _connectedAnnimationsProps.Add(animation);
            }
        }

        private static void OnAnchorElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is UIElement element))
            {
                return;
            }

            //TODO handle removal ie newvalue == null

            if (!(e.NewValue is UIElement anchorElement))
            {
                return;
            }

            if (!_coordinatedAnimationElements.TryGetValue(anchorElement, out var list))
            {
                list = new List<UIElement>();
                _coordinatedAnimationElements[anchorElement] = list;
            }

            list.Add(element);
        }

        private static void OnListItemKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetupFrame();

            var props = GetListViewBaseItemAnimationDetails(d);
            if (props == null)
            {
                return;
            }

            _connectedAnnimationsProps.Add(props);
        }

        private static void OnListItemElementNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetupFrame();

            var props = GetListViewBaseItemAnimationDetails(d);
            if (props == null)
            {
                return;
            }

            _connectedAnnimationsProps.Add(props);

            //TODO, need to make sure changed are updated on all attached properties 
        }

        private static void SetupFrame()
        {
            if (NavigationFrame == null && Window.Current.Content is Frame frame)
            {
                NavigationFrame = frame;
            }
        }

        private static ConnectedAnimationProperties GetListViewBaseItemAnimationDetails(DependencyObject d)
        {
            if (!(d is Windows.UI.Xaml.Controls.ListViewBase listViewBase))
            {
                return null;
            }
            var elementName = GetListItemElementName(d);
            var key = GetListItemKey(d);

            if (string.IsNullOrWhiteSpace(elementName) ||
                string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            return new ConnectedAnimationProperties()
            {
                Key = key,
                IsListAnimation = true,
                ElementName = elementName,
                ListViewBase = listViewBase
            };
        }

        internal class ConnectedAnimationProperties
        {
            public string Key { get; set; }
            public UIElement Element { get; set; }
            public List<UIElement> CoordinatedElements { get; set; }
            public string ElementName { get; set; }
            public Windows.UI.Xaml.Controls.ListViewBase ListViewBase { get; set; }
            public bool IsListAnimation { get; set; } = false;
        }
    }

}

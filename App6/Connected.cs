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
        private static List<ConnectedAnimationDetails> FromBuffer = new List<ConnectedAnimationDetails>();
        private static List<ConnectedAnimationDetails> ToBuffer = new List<ConnectedAnimationDetails>();

        private static Dictionary<string, ConnectedAnimationDetails> oldFromBuffer = new Dictionary<string, ConnectedAnimationDetails>();

        private static List<ConnectedListViewBaseAnimationDetails> ListViewBaseFromBuffer = new List<ConnectedListViewBaseAnimationDetails>();
        private static List<ConnectedListViewBaseAnimationDetails> ListViewBaseToBuffer = new List<ConnectedListViewBaseAnimationDetails>();

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
            RoutedEventHandler handler = null;
            handler = (s, args) =>
            {
                var page = s as Page;
                page.Loaded -= handler;

                var cas = ConnectedAnimationService.GetForCurrentView();

                var currentPage = NavigationFrame.CurrentSourcePageType;
                if (e.NavigationMode == Windows.UI.Xaml.Navigation.NavigationMode.Forward ||
                    e.NavigationMode == Windows.UI.Xaml.Navigation.NavigationMode.New)
                {
                    var i = 0;
                    while (i < ToBuffer.Count)
                    {
                        var anim = ToBuffer[i];

                        var connectedAnimation = cas.GetAnimation(anim.Key);
                        if (connectedAnimation != null)
                        {
                            if (_coordinatedAnimationElements.TryGetValue(anim.Element, out var coordinatedElements))
                            {
                                connectedAnimation.TryStart(anim.Element, coordinatedElements);
                            }
                            else
                            {
                                connectedAnimation.TryStart(anim.Element);
                            }
                            i++;
                        }
                        else
                        {
                            ToBuffer.Remove(anim);
                        }

                        if (oldFromBuffer.ContainsKey(anim.Key))
                        {
                            oldFromBuffer.Remove(anim.Key);
                        }
                    }

                    // if there are animations that were prepared on previous page but no elements on this page have the same key - cancel
                    foreach (var oldAnim in oldFromBuffer)
                    {
                        var connectedAnimation = cas.GetAnimation(oldAnim.Key);
                        if (connectedAnimation != null)
                        {
                            connectedAnimation.Cancel();
                        }
                    }
                }
                else if (e.NavigationMode == Windows.UI.Xaml.Navigation.NavigationMode.Back)
                {
                    foreach (var anim in FromBuffer)
                    {
                        var connectedAnimation = cas.GetAnimation(anim.Key);
                        if (connectedAnimation != null)
                        {
                            if (_coordinatedAnimationElements.TryGetValue(anim.Element, out var coordinatedElements))
                            {
                                connectedAnimation.TryStart(anim.Element, coordinatedElements);
                            }
                            else
                            {
                                connectedAnimation.TryStart(anim.Element);
                            }
                        }
                    }

                    var sourcePage = (sender as Frame).ForwardStack.LastOrDefault();
                    if (sourcePage != null && sourcePage.Parameter != null)
                    {
                        foreach (var anim in ListViewBaseFromBuffer)
                        {
                            anim.ListViewBase.ScrollIntoView(sourcePage.Parameter, ScrollIntoViewAlignment.Leading);
                            var connectedAnimation = cas.GetAnimation(anim.Key);
                            if (connectedAnimation != null)
                            {
                                var t = anim.ListViewBase.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                                {
                                    var success = await anim.ListViewBase.TryStartConnectedAnimationAsync(connectedAnimation, sourcePage.Parameter, anim.ElementName);
                                });
                            }

                        }
                    }
                }

                oldFromBuffer.Clear();
            };

            var navigatedPage = NavigationFrame.Content as Page;
            navigatedPage.Loaded += handler;
            
        }

        private static void _frame_Navigating(object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
        {
            var cas = ConnectedAnimationService.GetForCurrentView();

            if (e.NavigationMode == Windows.UI.Xaml.Navigation.NavigationMode.Forward ||
                e.NavigationMode == Windows.UI.Xaml.Navigation.NavigationMode.New)
            {
                foreach (var anim in FromBuffer)
                {
                    cas.PrepareToAnimate(anim.Key, anim.Element);
                    oldFromBuffer[anim.Key] = anim;
                }

                if (e.Parameter != null)
                {
                    foreach (var anim in ListViewBaseFromBuffer)
                    {
                        anim.ListViewBase.PrepareConnectedAnimation(anim.Key, e.Parameter, anim.ElementName);
                    }
                }
            }
            else if (e.NavigationMode == Windows.UI.Xaml.Navigation.NavigationMode.Back)
            {
                foreach (var anim in ToBuffer)
                {
                    cas.PrepareToAnimate(anim.Key, anim.Element);
                }
            }

            

            FromBuffer.Clear();
            ToBuffer.Clear();
            ListViewBaseFromBuffer.Clear();
            _coordinatedAnimationElements.Clear();
        }

        public static string GetFromKey(DependencyObject obj)
        {
            return (string)obj.GetValue(FromKeyProperty);
        }

        public static void SetFromKey(DependencyObject obj, string value)
        {
            obj.SetValue(FromKeyProperty, value);
        }

        // Using a DependencyProperty as the backing store for FromKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FromKeyProperty =
            DependencyProperty.RegisterAttached("FromKey", typeof(string), typeof(Connected), new PropertyMetadata(null, OnFromKeyChanged));

        private static void OnFromKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (NavigationFrame == null && Window.Current.Content is Frame frame)
            {
                NavigationFrame = frame;
            }

            if (d is FrameworkElement element)
            {
                var animation = new ConnectedAnimationDetails()
                {
                    Key = e.NewValue as string,
                    Element = element,
                };

                FromBuffer.Add(animation);
            }
        }



        public static string GetToKey(DependencyObject obj)
        {
            return (string)obj.GetValue(ToKeyProperty);
        }

        public static void SetToKey(DependencyObject obj, string value)
        {
            obj.SetValue(ToKeyProperty, value);
        }

        // Using a DependencyProperty as the backing store for ToKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToKeyProperty =
            DependencyProperty.RegisterAttached("ToKey", typeof(string), typeof(Connected), new PropertyMetadata(null, OnToKeyChanged));

        private static void OnToKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (NavigationFrame == null && Window.Current.Content is Frame frame)
            {
                NavigationFrame = frame;
            }

            if (d is FrameworkElement element)
            {
                var animation = new ConnectedAnimationDetails()
                {
                    Key = e.NewValue as string,
                    Element = element
                };

                ToBuffer.Add(animation);
            }
        }

        public static UIElement GetAnchorElement(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(AnchorElementProperty);
        }

        public static void SetAnchorElement(DependencyObject obj, UIElement value)
        {
            obj.SetValue(AnchorElementProperty, value);
        }

        // Using a DependencyProperty as the backing store for AnchorElement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnchorElementProperty =
            DependencyProperty.RegisterAttached("AnchorElement", typeof(UIElement), typeof(Connected), new PropertyMetadata(null, OnAnchorElementChanged));

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

        public static string GetListItemFromKey(DependencyObject obj)
        {
            return (string)obj.GetValue(ListItemFromKeyProperty);
        }

        public static void SetListItemFromKey(DependencyObject obj, string value)
        {
            obj.SetValue(ListItemFromKeyProperty, value);
        }

        // Using a DependencyProperty as the backing store for ListItemFromKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListItemFromKeyProperty =
            DependencyProperty.RegisterAttached("ListItemFromKey", typeof(string), typeof(Connected), new PropertyMetadata(null, OnListItemFromKeyChanged));


        public static string GetListItemElementName(DependencyObject obj)
        {
            return (string)obj.GetValue(ListItemElementNameProperty);
        }

        public static void SetListItemElementName(DependencyObject obj, string value)
        {
            obj.SetValue(ListItemElementNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for ListItemElementName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListItemElementNameProperty =
            DependencyProperty.RegisterAttached("ListItemElementName", typeof(string), typeof(Connected), new PropertyMetadata(null, OnListItemElementNameChanged));

        private static void OnListItemFromKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetupFrame();

            var clad = GetListViewBaseItemAnimationDetails(d);
            if (clad == null)
            {
                return;
            }

            ListViewBaseFromBuffer.Add(clad);
        }


        private static void OnListItemElementNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetupFrame();

            var clad = GetListViewBaseItemAnimationDetails(d);
            if (clad == null)
            {
                return;
            }

            ListViewBaseFromBuffer.Add(clad);

            //TODO, need to make sure changed are updated on all attached properties 
        }
        private static void SetupFrame()
        {
            if (NavigationFrame == null && Window.Current.Content is Frame frame)
            {
                NavigationFrame = frame;
            }
        }

        private static ConnectedListViewBaseAnimationDetails GetListViewBaseItemAnimationDetails(DependencyObject d)
        {
            // TODO, this only works for FROMKEY
            if (!(d is Windows.UI.Xaml.Controls.ListViewBase listViewBase))
            {
                return null;
            }
            var elementName = GetListItemElementName(d);
            var key = GetListItemFromKey(d);

            if (string.IsNullOrWhiteSpace(elementName) ||
                string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            return new ConnectedListViewBaseAnimationDetails()
            {
                Key = key,
                ElementName = elementName,
                ListViewBase = listViewBase
            };
        }

        private static void ListViewBase_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!(sender is Windows.UI.Xaml.Controls.ListViewBase listViewBase))
            {
                return;
            }

            ConnectedAnimationService cas = ConnectedAnimationService.GetForCurrentView();
            
            var elementName = GetListItemElementName(listViewBase);
            var key = GetListItemFromKey(listViewBase);

            if (string.IsNullOrWhiteSpace(elementName) ||
                string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            listViewBase.PrepareConnectedAnimation(key, e.ClickedItem, elementName);
        }

        internal class ConnectedListViewBaseAnimationDetails
        {
            public string Key { get; set; }
            public string ElementName { get; set; }
            public Windows.UI.Xaml.Controls.ListViewBase ListViewBase { get; set; }
        }

        internal class ConnectedAnimationDetails
        {
            public string Key { get; set; }
            public UIElement Element { get; set; }
            public List<UIElement> CoordinatedElements { get; set; }
        }
    }

}

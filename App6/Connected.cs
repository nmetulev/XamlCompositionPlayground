using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace App6
{
    public class Connected
    {
        private static Frame _frame;
        private static Dictionary<Type, Stack<ConnectedAnimationDetails>> _animations = new Dictionary<Type, Stack<ConnectedAnimationDetails>>();
        private static Stack<ConnectedAnimationDetails> _orphanAnimations = new Stack<ConnectedAnimationDetails>();
        private static Dictionary<UIElement, List<UIElement>> _coordinatedAnimationElements = new Dictionary<UIElement, List<UIElement>>();

        private static void AddAnimation(Type pageType, ConnectedAnimationDetails animation)
        {
            if (!_animations.TryGetValue(pageType, out var animationsList))
            {
                animationsList = new Stack<ConnectedAnimationDetails>();
                _animations[pageType] = animationsList;
            }

            animationsList.Push(animation);
        }

        private static void HandleTheOrphans()
        {
            if (_frame != null && _frame.CurrentSourcePageType != null)
            {
                while (_orphanAnimations.Count > 0)
                {
                    AddAnimation(_frame.CurrentSourcePageType, _orphanAnimations.Pop());
                }
            }
        }

        private static void SetupFrame(Frame frame)
        {
            if (frame == null)
            {
                return;
            }

            _frame = frame;
            
        }

        private static void _frame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var page = _frame.Content as Page;
            page.Loaded += Page_Loaded;

            
        }

        private static void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var page = sender as Page;
            page.Loaded -= Page_Loaded;

            HandleTheOrphans();
            var cas = ConnectedAnimationService.GetForCurrentView();

            var currentPage = _frame.CurrentSourcePageType;

            if (_animations.TryGetValue(currentPage, out var animationsList))
            {
                foreach (var anim in animationsList)
                {
                    var connectedAnimation = cas.GetAnimation(anim.Key);
                    if (connectedAnimation != null)
                    {
                        if (_coordinatedAnimationElements.TryGetValue(anim.Element, out var coordinatedElements))
                        {
                            connectedAnimation.TryStart(anim.Element, coordinatedElements);
                            _coordinatedAnimationElements.Remove(anim.Element);
                        }
                        else
                        {
                            connectedAnimation.TryStart(anim.Element);
                        }
                    }
                }
            }
        }

        private static void _frame_Navigating(object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
        {
            var cas = ConnectedAnimationService.GetForCurrentView();

            var currentPage = _frame.CurrentSourcePageType;
            var targetPage = e.SourcePageType;
            
            if (_animations.TryGetValue(currentPage, out var animationsList))
            {
                while (animationsList.Count > 0)
                {
                    var anim = animationsList.Pop();
                    cas.PrepareToAnimate(anim.Key, anim.Element);
                }
            }
        }

        public static string GetKey(DependencyObject obj)
        {
            return (string)obj.GetValue(KeyProperty);
        }

        public static void SetKey(DependencyObject obj, string value)
        {
            obj.SetValue(KeyProperty, value);
        }

        // Using a DependencyProperty as the backing store for Key.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.RegisterAttached("Key", typeof(string), typeof(Connected), new PropertyMetadata(null, OnKeyChanged));

        private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (_frame == null && Window.Current.Content is Frame frame)
            {
                _frame = frame;
                _frame.Navigating += _frame_Navigating;
                _frame.Navigated += _frame_Navigated;
            }

            if (d is FrameworkElement element)
            {
                var animation = new ConnectedAnimationDetails()
                {
                    Key = e.NewValue as string,
                    Element = element
                };

                _orphanAnimations.Push(animation);
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

        internal class ConnectedAnimationDetails
        {
            public string Key { get; set; }
            public UIElement Element { get; set; }
            public List<UIElement> CoordinatedElements { get; set; }
        }
    }

}

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Markup;

namespace ToolkitPreview
{
    public class Implicit
    {
        public static CAnimationCollection GetShowAnimations(DependencyObject obj)
        {
            var collection = (CAnimationCollection)obj.GetValue(ShowAnimationsProperty);

            if (collection == null)
            {
                collection = new CAnimationCollection();
                obj.SetValue(ShowAnimationsProperty, collection);
            }

            return collection;
        }

        public static void SetShowAnimations(DependencyObject obj, CAnimationCollection value)
        {
            obj.SetValue(ShowAnimationsProperty, value);
        }

        public static CAnimationCollection GetHideAnimations(DependencyObject obj)
        {
            var collection = (CAnimationCollection)obj.GetValue(HideAnimationsProperty);

            if (collection == null)
            {
                collection = new CAnimationCollection();
                obj.SetValue(HideAnimationsProperty, collection);
            }
            return collection;
        }

        public static void SetHideAnimations(DependencyObject obj, CAnimationCollection value)
        {
            obj.SetValue(HideAnimationsProperty, value);
        }

        public static CAnimationCollection GetAnimations(DependencyObject obj)
        {
            var collection = (CAnimationCollection)obj.GetValue(AnimationsProperty);

            if (collection == null)
            {
                collection = new CAnimationCollection();
                obj.SetValue(AnimationsProperty, collection);
            }
            return collection;
        }

        public static void SetAnimations(DependencyObject obj, CAnimationCollection value)
        {
            obj.SetValue(AnimationsProperty, value);
        }

        //public static CAnimationCollection GetListShowAnimations(DependencyObject obj)
        //{
        //    var collection = (CAnimationCollection)obj.GetValue(ListShowAnimationsProperty);

        //    if (collection == null)
        //    {
        //        collection = new CAnimationCollection();
        //        obj.SetValue(ListShowAnimationsProperty, collection);
        //    }

        //    return collection;
        //}

        //public static void SetListShowAnimations(DependencyObject obj, CAnimationCollection value)
        //{
        //    obj.SetValue(ListShowAnimationsProperty, value);
        //}

        // Using a DependencyProperty as the backing store for ShowAnimations.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowAnimationsProperty =
            DependencyProperty.RegisterAttached("ShowAnimations", 
                                                typeof(CAnimationCollection), 
                                                typeof(Implicit), 
                                                new PropertyMetadata(null, ShowAnimationsChanged));

        // Using a DependencyProperty as the backing store for HideAnimations.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HideAnimationsProperty =
            DependencyProperty.RegisterAttached("HideAnimations", 
                                                typeof(CAnimationCollection), 
                                                typeof(Implicit), 
                                                new PropertyMetadata(null, HideAnimationsChanged));

        // Using a DependencyProperty as the backing store for Animations.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimationsProperty =
            DependencyProperty.RegisterAttached("Animations", 
                                                typeof(CAnimationCollection), 
                                                typeof(Implicit), 
                                                new PropertyMetadata(null, AnimationsChanged));



        // Using a DependencyProperty as the backing store for ListShowAnimations.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ListShowAnimationsProperty =
        //    DependencyProperty.RegisterAttached("ListShowAnimations", 
        //                                        typeof(CAnimationCollection), 
        //                                        typeof(Implicit), 
        //                                        new PropertyMetadata(null, ListShowAnimationChanged));

        //private static void ListShowAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (e.OldValue is CAnimationCollection oldCollection)
        //    {
        //        //oldCollection.CollectionChanged -= ShowCollectionChanged; //TODO
        //    }

        //    if (!(d is ListViewBase listViewBase))
        //    {
        //        return;
        //    }

        //    if (!(e.NewValue is CAnimationCollection animationCollection))
        //    {
        //        return;
        //    }

        //    //listViewBase.ContainerContentChanging += ListViewBase_ContainerContentChanging;
        //    listViewBase.ChoosingItemContainer += ListViewBase_ChoosingItemContainer;
        //    listViewBase.ItemContainerTransitions = null;
        //}

        //private static void ListViewBase_ChoosingItemContainer(ListViewBase sender, ChoosingItemContainerEventArgs args)
        //{
        //    if (args.ItemContainer == null)
        //    {
        //        // only support ListView and GridView
        //        if (sender is ListView lv)
        //        {
        //            args.ItemContainer = new ListViewItem();
        //        }
        //        else if (sender is GridView gv)
        //        {
        //            args.ItemContainer = new GridViewItem();
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    }

        //    SetShowAnimations(args.ItemContainer, GetListShowAnimations(sender));
        //}

        //private static void ListViewBase_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        //{
        //    SetShowAnimations(args.ItemContainer, GetListShowAnimations(sender));
        //}

        private static void ShowAnimationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is CAnimationCollection oldCollection)
            {
                oldCollection.CollectionChanged -= ShowCollectionChanged;
            }

            if (!(e.NewValue is CAnimationCollection animationCollection))
            {
                return;
            }

            if (!(d is UIElement element))
            {
                return;
            }

            animationCollection.CollectionChanged += ShowCollectionChanged;
            animationCollection.Element = element;

            var showAnimationGroup = GetGroupFromCAnimationCollection(element, animationCollection);
            ElementCompositionPreview.SetImplicitShowAnimation(element, showAnimationGroup);
        }

        private static void HideAnimationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is CAnimationCollection oldCollection)
            {
                oldCollection.CollectionChanged -= HideCollectionChanged;
            }

            if (!(e.NewValue is CAnimationCollection animationCollection))
            {
                return;
            }

            if (!(d is UIElement element))
            {
                    return;
            }

            animationCollection.CollectionChanged += HideCollectionChanged;
            animationCollection.Element = element;

            var hideAnimationGroup = GetGroupFromCAnimationCollection(element, animationCollection);
            ElementCompositionPreview.SetImplicitHideAnimation(element, hideAnimationGroup);
        }

        private static void AnimationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is CAnimationCollection oldCollection)
            {
                oldCollection.CollectionChanged -= AnimationsCollectionChanged;
            }

            if (!(e.NewValue is CAnimationCollection animationCollection))
            {
                return;
            }

            if (!(d is UIElement element))
            {
                return;
            }

            animationCollection.CollectionChanged += AnimationsCollectionChanged;
            animationCollection.Element = element;

            
            var implicitAnimationCollection = GetImplicitAnimationCollectionFromCAnimationCollection(element, animationCollection);
            ElementCompositionPreview.GetElementVisual(element).ImplicitAnimations = implicitAnimationCollection;
        }

        private static CompositionAnimationGroup GetGroupFromCAnimationCollection(UIElement element, CAnimationCollection animationCollection)
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);

            var compositor = visual.Compositor;
            var animationGroup = compositor.CreateAnimationGroup();

            foreach (var cAnim in animationCollection)
            {
                var compositionAnimation = cAnim.GetCompositionAnimation(visual);
                if (compositionAnimation != null)
                {
                    animationGroup.Add(compositionAnimation);

                    if (cAnim.Target.StartsWith("Translation"))
                    {
                        ElementCompositionPreview.SetIsTranslationEnabled(element, true);
                    }
                }
            }

            return animationGroup;
        }

        private static ImplicitAnimationCollection GetImplicitAnimationCollectionFromCAnimationCollection(UIElement element, CAnimationCollection animationCollection)
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);

            var compositor = visual.Compositor;
            var implicitAnimations = compositor.CreateImplicitAnimationCollection();

            var animations = new Dictionary<string, CompositionAnimationGroup>();

            foreach (var cAnim in animationCollection)
            {
                CompositionAnimation animation;
                if (!string.IsNullOrWhiteSpace(cAnim.Target) 
                    && (animation = cAnim.GetCompositionAnimation(visual)) != null)
                {
                    var target = cAnim.ImplicitTarget ?? cAnim.Target;
                    if (!animations.ContainsKey(target))
                    {
                        animations[target] = compositor.CreateAnimationGroup();
                    }
                    animations[target].Add(animation);

                    if (cAnim.Target.StartsWith("Translation"))
                    {
                        ElementCompositionPreview.SetIsTranslationEnabled(element, true);
                    }
                }
            }

            foreach (var kv in animations)
            {
                implicitAnimations[kv.Key] = kv.Value;
            }

            return implicitAnimations;
        }

        private static void ShowCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
            {
                return;
            }

            var collection = sender as CAnimationCollection;
            if (collection.Element == null)
            {
                return;
            }

            var showAnimationGroup = GetGroupFromCAnimationCollection(collection.Element, collection);
            ElementCompositionPreview.SetImplicitShowAnimation(collection.Element, showAnimationGroup);

        }

        private static void HideCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
            {
                return;
            }

            var collection = sender as CAnimationCollection;
            if (collection.Element == null)
            {
                return;
            }

            var hideAnimationGroup = GetGroupFromCAnimationCollection(collection.Element, collection);
            ElementCompositionPreview.SetImplicitHideAnimation(collection.Element, hideAnimationGroup);
        }

        private static void AnimationsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
            {
                return;
            }

            var collection = sender as CAnimationCollection;
            if (collection.Element == null)
            {
                return;
            }

            var implicitAnimationCollection = GetImplicitAnimationCollectionFromCAnimationCollection(collection.Element, collection);
            ElementCompositionPreview.GetElementVisual(collection.Element).ImplicitAnimations = implicitAnimationCollection;
        }
    }

    [ContentProperty(Name = nameof(KeyFrames))]
    public abstract class CAnimation : DependencyObject
    {
        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public KeyFrameCollection KeyFrames
        {
            get
            {
                var collection = (KeyFrameCollection)GetValue(ScalarKeyFramesProperty);
                if (collection == null)
                {
                    collection = new KeyFrameCollection();
                    SetValue(ScalarKeyFramesProperty, collection);
                }

                return collection;
            }
            set { SetValue(ScalarKeyFramesProperty, value); }
        }

        public string Target
        {
            get { return (string)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public string ImplicitTarget
        {
            get { return (string)GetValue(ImplicitTargetProperty); }
            set { SetValue(ImplicitTargetProperty, value); }
        }

        public TimeSpan Delay
        {
            get { return (TimeSpan)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Target.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target",
                                        typeof(string),
                                        typeof(CAnimation),
                                        new PropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", 
                                        typeof(TimeSpan), 
                                        typeof(CAnimation), 
                                        new PropertyMetadata(TimeSpan.FromMilliseconds(400)));

        // Using a DependencyProperty as the backing store for ScalarKeyFrames.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScalarKeyFramesProperty =
            DependencyProperty.Register("KeyFrames",
                                        typeof(KeyFrameCollection),
                                        typeof(CAnimation),
                                        new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for ImplicitTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImplicitTargetProperty =
            DependencyProperty.Register("ImplicitTarget", 
                                        typeof(string), 
                                        typeof(CAnimation), 
                                        new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Delay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DelayProperty =
            DependencyProperty.Register("Delay", 
                                        typeof(TimeSpan), 
                                        typeof(CAnimation), 
                                        new PropertyMetadata(TimeSpan.Zero));



        public abstract CompositionAnimation GetCompositionAnimation(Visual visual);
    }

    public class CScalarAnimation : CAnimation
    {
        public double From
        {
            get { return (double)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        // Using a DependencyProperty as the backing store for From.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(double), typeof(CScalarAnimation), new PropertyMetadata(double.NaN));

        public double To
        {
            get { return (double)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        // Using a DependencyProperty as the backing store for To.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(double), typeof(CScalarAnimation), new PropertyMetadata(double.NaN));

        private ScalarKeyFrame FromKeyFrame;
        private ScalarKeyFrame ToKeyFrame;
        
        public override CompositionAnimation GetCompositionAnimation(Visual visual)
        {
            var compositor = visual?.Compositor;
            if (compositor == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(Target))
            {
                return null;
            }

            PrepareKeyFrames();
            var animation = compositor.CreateScalarKeyFrameAnimation();
            animation.Target = Target;
            animation.Duration = Duration;
            animation.DelayTime = Delay;

            if (KeyFrames.Count == 0)
            {
                animation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
                return animation;
            }

            foreach(var keyFrame in KeyFrames)
            {
                if (keyFrame is ScalarKeyFrame scalarKeyFrame)
                {
                    animation.InsertKeyFrame((float)keyFrame.Key, (float)scalarKeyFrame.Value);
                }
                else if (keyFrame is ExpressionKeyFrame expressionKeyFrame)
                {
                    animation.InsertExpressionKeyFrame((float)keyFrame.Key, expressionKeyFrame.Value);
                }
            }

            return animation;
        }

        public void PrepareKeyFrames()
        {
            if (FromKeyFrame != null)
                KeyFrames.Remove(FromKeyFrame);

            if (ToKeyFrame != null)
                KeyFrames.Remove(ToKeyFrame);

            if (!double.IsNaN(From))
            {
                FromKeyFrame = new ScalarKeyFrame();
                FromKeyFrame.Key = 0f;
                FromKeyFrame.Value = From;
                KeyFrames.Add(FromKeyFrame);
            }

            if (!double.IsNaN(To))
            {
                ToKeyFrame = new ScalarKeyFrame();
                ToKeyFrame.Key = 1f;
                ToKeyFrame.Value = To;
                KeyFrames.Add(ToKeyFrame);
            }
        }
    }

    public class CVector3Animation : CAnimation
    {
        public string From
        {
            get { return (string)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public string To
        {
            get { return (string)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        // Using a DependencyProperty as the backing store for From.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(string), typeof(CVector3Animation), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for To.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(string), typeof(CVector3Animation), new PropertyMetadata(null));

        private Vector3KeyFrame FromKeyFrame;
        private Vector3KeyFrame ToKeyFrame;

        public override CompositionAnimation GetCompositionAnimation(Visual visual)
        {
            var compositor = visual.Compositor;

            if (string.IsNullOrWhiteSpace(Target))
            {
                return null;
            }

            PrepareKeyFrames();
            var animation = compositor.CreateVector3KeyFrameAnimation();
            animation.Target = Target;
            animation.Duration = Duration;
            animation.DelayTime = Delay;

            if (KeyFrames.Count == 0)
            {
                animation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
                return animation;
            }

            foreach (var keyFrame in KeyFrames)
            {
                if (keyFrame is Vector3KeyFrame vectorKeyFrame)
                {
                    animation.InsertKeyFrame((float)keyFrame.Key, vectorKeyFrame.Value);
                }
                else if (keyFrame is ExpressionKeyFrame expressionKeyFrame)
                {
                    animation.InsertExpressionKeyFrame((float)keyFrame.Key, expressionKeyFrame.Value);
                }
            }

            return animation;
        }

        

        public void PrepareKeyFrames()
        {
            if (FromKeyFrame != null)
                KeyFrames.Remove(FromKeyFrame);

            if (ToKeyFrame != null)
                KeyFrames.Remove(ToKeyFrame);

            if (From != null)
            {
                FromKeyFrame = new Vector3KeyFrame();
                FromKeyFrame.Key = 0f;
                FromKeyFrame.Value = From.ToVector3();
                KeyFrames.Add(FromKeyFrame);
            }

            if (To != null)
            {
                ToKeyFrame = new Vector3KeyFrame();
                ToKeyFrame.Key = 1f;
                ToKeyFrame.Value = To.ToVector3();
                KeyFrames.Add(ToKeyFrame);
            }
        }
    }

    public class OffsetAnimation : CVector3Animation
    {
        public OffsetAnimation()
        {
            Target = "Offset";
        }
    }

    public class ScaleAnimation : CVector3Animation
    {
        public ScaleAnimation()
        {
            Target = "Scale";
        }
    }

    public class OpacityAnimation : CScalarAnimation
    {
        public OpacityAnimation()
        {
            Target = "Opacity";
        }
    }

    public class RotationAnimation : CScalarAnimation
    {
        public RotationAnimation()
        {
            Target = "RotationAngle";
        }
    }

    public class TranslationAnimation : CVector3Animation
    {
        public TranslationAnimation()
        {
            Target = "Translation";
        }
    }

    public class RotationDegreesAnimation : CScalarAnimation
    {
        public RotationDegreesAnimation()
        {
            Target = "RotationAngleInDegrees";
        }
    }

    public abstract class KeyFrame : DependencyObject
    {
        public double Key
        {
            get { return (double)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Key.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(double), typeof(KeyFrame), new PropertyMetadata(0.0));
    }

    public class ScalarKeyFrame: KeyFrame
    {
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ScalarKeyFrame), new PropertyMetadata(0.0));
    }

    public class ExpressionKeyFrame : KeyFrame
    {
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(ExpressionKeyFrame), new PropertyMetadata(string.Empty));
    }

    public class Vector3KeyFrame : KeyFrame
    {
        public Vector3 Value
        {
            get { return (Vector3)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(Vector3), typeof(Vector3KeyFrame), new PropertyMetadata(Vector3.Zero));


    }

    public class CAnimationCollection : ObservableCollection<CAnimation>
    {
        //public UIElement Element { get; set; }
    }

    public class KeyFrameCollection : List<KeyFrame>
    {

    }
}

using System;
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
using Windows.UI.Xaml.Media;

namespace ToolkitPreview
{
    public class Implicit
    {

        public static readonly DependencyProperty ShowAnimationsProperty =
            DependencyProperty.RegisterAttached("ShowAnimations", 
                                                typeof(CAnimationCollection), 
                                                typeof(Implicit), 
                                                new PropertyMetadata(null, ShowAnimationsChanged));

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

        private static void ShowAnimationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is CAnimationCollection oldCollection)
            {
                oldCollection.AnimationCollectionChanged -= ShowCollectionChanged;
            }

            if (!(e.NewValue is CAnimationCollection animationCollection))
            {
                return;
            }

            if (!(d is UIElement element))
            {
                return;
            }

            animationCollection.Element = element;
            animationCollection.AnimationCollectionChanged -= ShowCollectionChanged;
            animationCollection.AnimationCollectionChanged += ShowCollectionChanged;

            ElementCompositionPreview.SetImplicitShowAnimation(element, GetCompositionAnimationGroup(animationCollection, element));
        }

        private static void HideAnimationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is CAnimationCollection oldCollection)
            {
                oldCollection.AnimationCollectionChanged -= HideCollectionChanged;
            }

            if (!(e.NewValue is CAnimationCollection animationCollection))
            {
                return;
            }

            if (!(d is UIElement element))
            {
                    return;
            }

            animationCollection.Element = element;
            animationCollection.AnimationCollectionChanged -= HideCollectionChanged;
            animationCollection.AnimationCollectionChanged += HideCollectionChanged;

            ElementCompositionPreview.SetImplicitHideAnimation(element, GetCompositionAnimationGroup(animationCollection, element));
        }

        private static void AnimationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is CAnimationCollection oldCollection)
            {
                oldCollection.AnimationCollectionChanged -= AnimationsCollectionChanged;
            }

            if (!(e.NewValue is CAnimationCollection animationCollection))
            {
                return;
            }

            if (!(d is UIElement element))
            {
                return;
            }

            animationCollection.Element = element;
            animationCollection.AnimationCollectionChanged -= AnimationsCollectionChanged;
            animationCollection.AnimationCollectionChanged += AnimationsCollectionChanged;

            ElementCompositionPreview.GetElementVisual(element).ImplicitAnimations = GetImplicitAnimationCollection(animationCollection, element);
        }

        private static void ShowCollectionChanged(object sender, EventArgs e)
        {
            var collection = sender as CAnimationCollection;
            if (collection.Element == null)
            {
                return;
            }

            ElementCompositionPreview.SetImplicitShowAnimation(collection.Element, GetCompositionAnimationGroup(collection, collection.Element));
        }

        private static void HideCollectionChanged(object sender, EventArgs e)
        {
            var collection = sender as CAnimationCollection;
            if (collection.Element == null)
            {
                return;
            }

            ElementCompositionPreview.SetImplicitHideAnimation(collection.Element, GetCompositionAnimationGroup(collection, collection.Element));

        }

        private static void AnimationsCollectionChanged(object sender, EventArgs e)
        {
            var collection = sender as CAnimationCollection;
            if (collection.Element == null)
            {
                return;
            }
            
            ElementCompositionPreview.GetElementVisual(collection.Element).ImplicitAnimations = 
                                            GetImplicitAnimationCollection(collection, collection.Element);
        }

        private static CompositionAnimationGroup GetCompositionAnimationGroup(CAnimationCollection collection, UIElement element)
        {
            if (collection.ContainsTranslationAnimation)
            {
                ElementCompositionPreview.SetIsTranslationEnabled(element, true);
            }

            return collection.GetCompositionAnimationGroup(element);
        }

        private static ImplicitAnimationCollection GetImplicitAnimationCollection(CAnimationCollection collection, UIElement element)
        {
            if (collection.ContainsTranslationAnimation)
            {
                ElementCompositionPreview.SetIsTranslationEnabled(element, true);
            }

            return collection.GetImplicitAnimationCollection(element);
        }
    }

    [ContentProperty(Name = nameof(KeyFrames))]
    public abstract class CAnimation : DependencyObject
    {
        public event EventHandler AnimationChanged;

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
                                        new PropertyMetadata(string.Empty, OnAnimationPropertyChanged));

        // Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", 
                                        typeof(TimeSpan), 
                                        typeof(CAnimation), 
                                        new PropertyMetadata(TimeSpan.FromMilliseconds(400), OnAnimationPropertyChanged));

        // Using a DependencyProperty as the backing store for ScalarKeyFrames.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScalarKeyFramesProperty =
            DependencyProperty.Register("KeyFrames",
                                        typeof(KeyFrameCollection),
                                        typeof(CAnimation),
                                        new PropertyMetadata(null, OnAnimationPropertyChanged));

        // Using a DependencyProperty as the backing store for ImplicitTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImplicitTargetProperty =
            DependencyProperty.Register("ImplicitTarget", 
                                        typeof(string), 
                                        typeof(CAnimation), 
                                        new PropertyMetadata(null, OnAnimationPropertyChanged));

        // Using a DependencyProperty as the backing store for Delay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DelayProperty =
            DependencyProperty.Register("Delay", 
                                        typeof(TimeSpan), 
                                        typeof(CAnimation), 
                                        new PropertyMetadata(TimeSpan.Zero, OnAnimationPropertyChanged));

        protected static void OnAnimationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CAnimation).AnimationChanged?.Invoke(d, null);
        }

        public abstract CompositionAnimation GetCompositionAnimation(Visual visual);
    }

    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <typeparam name="T">Type of <see cref="TypedKeyFrame{U}" to use/></typeparam>
    /// <typeparam name="U">Type of value being animated. Only nullable types supported</typeparam>
    public abstract class CTypedAnimation<T, U> : CAnimation where T : TypedKeyFrame<U>, new()
    {
        private T FromKeyFrame;
        private T ToKeyFrame;

        // Using a DependencyProperty as the backing store for From.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(U), typeof(CVector3Animation), new PropertyMetadata(GetDefaultValue(), OnAnimationPropertyChanged));

        // Using a DependencyProperty as the backing store for To.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(U), typeof(CVector3Animation), new PropertyMetadata(GetDefaultValue(), OnAnimationPropertyChanged));

        public U From
        {
            get { return (U)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public U To
        {
            get { return (U)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public override CompositionAnimation GetCompositionAnimation(Visual visual)
        {
            var compositor = visual.Compositor;

            if (string.IsNullOrWhiteSpace(Target))
            {
                return null;
            }

            PrepareKeyFrames();
            var animation = GetTypedAnimationFromCompositor(compositor);
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
                if (keyFrame is T typedKeyFrame)
                {
                    //animation.InsertKeyFrame((float)keyFrame.Key, vectorKeyFrame.Value);
                    InsertKeyFrameToTypedAnimation(animation, typedKeyFrame);
                }
                else if (keyFrame is ExpressionKeyFrame expressionKeyFrame)
                {
                    animation.InsertExpressionKeyFrame((float)keyFrame.Key, expressionKeyFrame.Value);
                }
            }

            return animation;
        }

        protected void PrepareKeyFrames()
        {
            if (FromKeyFrame != null)
                KeyFrames.Remove(FromKeyFrame);

            if (ToKeyFrame != null)
                KeyFrames.Remove(ToKeyFrame);

            if (!IsValueNull(From))
            {
                FromKeyFrame = new T();
                FromKeyFrame.Key = 0f;
                FromKeyFrame.Value = From;
                KeyFrames.Add(FromKeyFrame);
            }

            if (!IsValueNull(To))
            {
                ToKeyFrame = new T();
                ToKeyFrame.Key = 1f;
                ToKeyFrame.Value = To;
                KeyFrames.Add(ToKeyFrame);
            }
        }

        protected abstract KeyFrameAnimation GetTypedAnimationFromCompositor(Compositor compositor);

        protected abstract void InsertKeyFrameToTypedAnimation(KeyFrameAnimation animation, T keyFrame);

        // these two methods are required to support double (non nullable type)
        private static object GetDefaultValue()
        {
            if (typeof(U) == typeof(double))
                return double.NaN;

            return default(U);
        }

        private static bool IsValueNull(U value)
        {
            if (typeof(U) == typeof(double))
                return double.IsNaN((double)(object)value);

            return value == null;
        }
    }

    public class CScalarAnimation : CTypedAnimation<ScalarKeyFrame, double>
    {
        protected override KeyFrameAnimation GetTypedAnimationFromCompositor(Compositor compositor)
        {
            return compositor.CreateScalarKeyFrameAnimation();
        }

        protected override void InsertKeyFrameToTypedAnimation(KeyFrameAnimation animation, ScalarKeyFrame keyFrame)
        {
            (animation as ScalarKeyFrameAnimation).InsertKeyFrame((float)keyFrame.Key, (float)keyFrame.Value);
        }
    }

    public class CVector2Animation : CTypedAnimation<Vector2KeyFrame, string>
    {
        protected override KeyFrameAnimation GetTypedAnimationFromCompositor(Compositor compositor)
        {
            return compositor.CreateVector2KeyFrameAnimation();
        }

        protected override void InsertKeyFrameToTypedAnimation(KeyFrameAnimation animation, Vector2KeyFrame keyFrame)
        {
            (animation as Vector2KeyFrameAnimation).InsertKeyFrame((float)keyFrame.Key, keyFrame.Value.ToVector2());
        }
    }

    public class CVector3Animation : CTypedAnimation<Vector3KeyFrame, string>
    {
        protected override KeyFrameAnimation GetTypedAnimationFromCompositor(Compositor compositor)
        {
            return compositor.CreateVector3KeyFrameAnimation();
        }

        protected override void InsertKeyFrameToTypedAnimation(KeyFrameAnimation animation, Vector3KeyFrame keyFrame)
        {
            (animation as Vector3KeyFrameAnimation).InsertKeyFrame((float)keyFrame.Key, keyFrame.Value.ToVector3());
        }
    }

    public class CVector4Animation : CTypedAnimation<Vector4KeyFrame, string>
    {
        protected override KeyFrameAnimation GetTypedAnimationFromCompositor(Compositor compositor)
        {
            return compositor.CreateVector4KeyFrameAnimation();
        }

        protected override void InsertKeyFrameToTypedAnimation(KeyFrameAnimation animation, Vector4KeyFrame keyFrame)
        {
            (animation as Vector4KeyFrameAnimation).InsertKeyFrame((float)keyFrame.Key, keyFrame.Value.ToVector4());
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

    public abstract class TypedKeyFrame<T> : KeyFrame
    {
        public T Value
        {
            get { return (T)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(T), typeof(ScalarKeyFrame), new PropertyMetadata(0.0));
    }

    public class ScalarKeyFrame: TypedKeyFrame<double> { }

    public class ExpressionKeyFrame : TypedKeyFrame<string> { }

    public class Vector2KeyFrame : TypedKeyFrame<string> { }

    public class Vector3KeyFrame : TypedKeyFrame<string> { }

    public class Vector4KeyFrame : TypedKeyFrame<string> { }

    public class CAnimationCollection : ObservableCollection<CAnimation>
    {
        public UIElement Element { get; set; }

        public event EventHandler AnimationCollectionChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                return;
            }

            if (e.NewItems != null)
            {
                foreach (CAnimation newAnim in e.NewItems)
                {
                    newAnim.AnimationChanged += AnimationChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (CAnimation oldAnim in e.OldItems)
                {
                    oldAnim.AnimationChanged -= AnimationChanged;
                }
            }

            AnimationCollectionChanged?.Invoke(this, null);
        }

        private void AnimationChanged(object sender, EventArgs e)
        {
            AnimationCollectionChanged?.Invoke(this, null);
        }

        public CompositionAnimationGroup GetCompositionAnimationGroup(UIElement element)
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);

            var compositor = visual.Compositor;
            var animationGroup = compositor.CreateAnimationGroup();

            foreach (var cAnim in this)
            {
                var compositionAnimation = cAnim.GetCompositionAnimation(visual);
                if (compositionAnimation != null)
                {
                    animationGroup.Add(compositionAnimation);
                }
            }

            return animationGroup;
        }

        public ImplicitAnimationCollection GetImplicitAnimationCollection(UIElement element)
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);

            var compositor = visual.Compositor;
            var implicitAnimations = compositor.CreateImplicitAnimationCollection();

            var animations = new Dictionary<string, CompositionAnimationGroup>();

            foreach (var cAnim in this)
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
                }
            }

            foreach (var kv in animations)
            {
                implicitAnimations[kv.Key] = kv.Value;
            }

            return implicitAnimations;
        }

        public bool ContainsTranslationAnimation => this.Where(anim => anim.Target.StartsWith("Translation")).Count() > 0;
    }

    public class KeyFrameCollection : List<KeyFrame>
    {

    }
}

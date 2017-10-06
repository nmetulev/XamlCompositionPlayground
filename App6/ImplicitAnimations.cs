using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace App6
{
    public class ImplicitAnimations
    {
        public static CompAnimationCollection GetImplicitShowAnimations(DependencyObject obj)
        {
            return (CompAnimationCollection)obj.GetValue(ImplicitShowAnimationsProperty);
        }

        public static void SetImplicitShowAnimations(DependencyObject obj, CompAnimationCollection value)
        {
            obj.SetValue(ImplicitShowAnimationsProperty, value);
        }

        // Using a DependencyProperty as the backing store for ImplicitShowAnimations.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImplicitShowAnimationsProperty =
            DependencyProperty.RegisterAttached("ImplicitShowAnimations", typeof(CompAnimationCollection), typeof(ImplicitAnimations), new PropertyMetadata(null, ImplicitShowAnimationsChanged));

        private static void ImplicitShowAnimationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is CompAnimationCollection animationCollection))
            {
                return;
            }

            if (!(d is UIElement element))
            {
                return;
            }

            var showAnimationGroup = GetGroupFromCompAnimationCollection(element, animationCollection);
            ElementCompositionPreview.SetImplicitShowAnimation(element, showAnimationGroup);
        }

        public static CompAnimationCollection GetImplicitHideAnimations(DependencyObject obj)
        {
            return (CompAnimationCollection)obj.GetValue(ImplicitHideAnimationsProperty);
        }

        public static void SetImplicitHideAnimations(DependencyObject obj, CompAnimationCollection value)
        {
            obj.SetValue(ImplicitHideAnimationsProperty, value);
        }

        // Using a DependencyProperty as the backing store for ImplicitHideAnimations.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImplicitHideAnimationsProperty =
            DependencyProperty.RegisterAttached("ImplicitHideAnimations", typeof(CompAnimationCollection), typeof(ImplicitAnimations), new PropertyMetadata(null, ImplicitHideAnimationsChanged));

        private static void ImplicitHideAnimationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is CompAnimationCollection animationCollection))
            {
                return;
            }

            if (!(d is UIElement element))
            {
                    return;
            }

            var hideAnimationGroup = GetGroupFromCompAnimationCollection(element, animationCollection);
            ElementCompositionPreview.SetImplicitHideAnimation(element, hideAnimationGroup);
        }

        private static CompositionAnimationGroup GetGroupFromCompAnimationCollection(UIElement element, CompAnimationCollection animationCollection)
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
                }
            }

            return animationGroup;
        }
    }

    public class CompAnimation : DependencyObject
    {
        public string Target
        {
            get { return (string)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Target.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(string), typeof(CompAnimation), new PropertyMetadata(string.Empty));

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(CompAnimation), new PropertyMetadata(TimeSpan.Zero));

        public virtual CompositionAnimation GetCompositionAnimation(Visual visual)
        {
            return null;
        }
    }

    public class CompScalarKeyFrameAnimation : CompAnimation
    {
        public CompScalarKeyFrameAnimation()
        {
            ScalarKeyFrames = new ScalarKeyFrameCollection();
        }

        public ScalarKeyFrameCollection ScalarKeyFrames
        {
            get { return (ScalarKeyFrameCollection)GetValue(ScalarKeyFramesProperty); }
            set { SetValue(ScalarKeyFramesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScalarKeyFrames.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScalarKeyFramesProperty =
            DependencyProperty.Register("ScalarKeyFrames", typeof(ScalarKeyFrameCollection), typeof(CompScalarKeyFrameAnimation), new PropertyMetadata(null));

        public double From
        {
            get { return (double)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        // Using a DependencyProperty as the backing store for From.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(double), typeof(CompScalarKeyFrameAnimation), new PropertyMetadata(double.NaN));

        public double To
        {
            get { return (double)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        // Using a DependencyProperty as the backing store for To.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(double), typeof(CompScalarKeyFrameAnimation), new PropertyMetadata(double.NaN));

        private ScalarKeyFrame FromKeyFrame;
        private ScalarKeyFrame ToKeyFrame;

        public override CompositionAnimation GetCompositionAnimation(Visual visual)
        {
            var compositor = visual.Compositor;

            if (string.IsNullOrWhiteSpace(Target))
            {
                return null;
            }

            if (FromKeyFrame != null)
                ScalarKeyFrames.Remove(FromKeyFrame);

            if (ToKeyFrame != null)
                ScalarKeyFrames.Remove(ToKeyFrame);

            if (!double.IsNaN(From))
            {
                FromKeyFrame = new ScalarKeyFrame();
                FromKeyFrame.NormalizedProgressKey = 0f;
                FromKeyFrame.Value = From;
                ScalarKeyFrames.Add(FromKeyFrame);
            }

            if (!double.IsNaN(To))
            {
                ToKeyFrame = new ScalarKeyFrame();
                ToKeyFrame.NormalizedProgressKey = 1f;
                ToKeyFrame.Value = To;
                ScalarKeyFrames.Add(ToKeyFrame);
            }

            if (ScalarKeyFrames.Count < 0)
            {
                return null;
            }

            var animation = compositor.CreateScalarKeyFrameAnimation();
            animation.Target = Target;
            animation.Duration = Duration;

            foreach(var keyFrame in ScalarKeyFrames)
            {
                animation.InsertKeyFrame((float)keyFrame.NormalizedProgressKey, (float)keyFrame.Value);
            }

            return animation;
        }
    }

    public class ScalarKeyFrame: DependencyObject
    {
        public double NormalizedProgressKey
        {
            get { return (double)GetValue(NormalizedProgressKeyProperty); }
            set { SetValue(NormalizedProgressKeyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NormalizedProgressKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NormalizedProgressKeyProperty =
            DependencyProperty.Register("NormalizedProgressKey", typeof(double), typeof(ScalarKeyFrame), new PropertyMetadata(0.0));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ScalarKeyFrame), new PropertyMetadata(0.0));
    }

    public class ScalarKeyFrameCollection : List<ScalarKeyFrame>
    {

    }

    public class CompAnimationCollection : List<CompAnimation>
    {

    }
}

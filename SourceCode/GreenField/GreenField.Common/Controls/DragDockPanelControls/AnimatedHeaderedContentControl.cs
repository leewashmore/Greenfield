using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace GreenField.Common.DragDockPanelControls
{
    /// <summary>
    /// Animated headered content control base class.
    /// </summary>
    public class AnimatedHeaderedContentControl : HeaderedContentControl
    {
        #region Private memebers

        /// <summary>
        /// Stores the position animation.
        /// </summary>
        private readonly Storyboard positionAnimation;

        /// <summary>
        /// Stores the posisition X key frame.
        /// </summary>
        private readonly SplineDoubleKeyFrame positionAnimationXKeyFrame;

        /// <summary>
        /// Stores the position Y keyframe.
        /// </summary>
        private readonly SplineDoubleKeyFrame positionAnimationYKeyFrame;

        /// <summary>
        /// Stores the size animation.
        /// </summary>
        private readonly Storyboard sizeAnimation;
        
        //TBRemoved..............
        ///// <summary>
        ///// Stores the height key frame.
        ///// </summary>
        //private readonly SplineDoubleKeyFrame sizeAnimationHeightKeyFrame;

        ///// <summary>
        ///// Stores the width key frame.
        ///// </summary>
        //private readonly SplineDoubleKeyFrame sizeAnimationWidthKeyFrame;

        /// <summary>
        /// Stores the size key frame.
        /// </summary>
        private readonly SplineDoubleKeyFrame sizeAnimationKeyFrame;

        private Size _initial;
        private double _stepHeight;
        private double _stepWidth;

        /// <summary>
        /// Stores a flag storing if the position is animating.
        /// </summary>
        private bool positionAnimating;

        /// <summary>
        /// Stores the position animation time span.
        /// </summary>
        private TimeSpan positionAnimationTimespan = new TimeSpan(0, 0, 0, 0, 500);

        /// <summary>
        /// Stores a flag indicating if the size is animating.
        /// </summary>
        private bool sizeAnimating;

        /// <summary>
        /// Stores the size animation timespan.
        /// </summary>
        private TimeSpan sizeAnimationTimespan = new TimeSpan(0, 0, 0, 0, 200);

        #region TargetSize Property

        public static readonly DependencyProperty TargetSizeProperty =
            DependencyProperty.Register(
                "TargetSize", typeof(Size), typeof(AnimatedHeaderedContentControl),
                new PropertyMetadata(new Size(), OnTargetSizeChanged));

        /// <summary>
        /// The size of the panel used in the animation.
        /// </summary>
        public Size TargetSize
        {
            get { return (Size)GetValue(TargetSizeProperty); }
            set { SetValue(TargetSizeProperty, value); }
        }

        private static void OnTargetSizeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((AnimatedHeaderedContentControl)o).OnTargetSizeChanged((Size)e.NewValue, (Size)e.OldValue);
        }

        private void OnTargetSizeChanged(Size newValue, Size oldValue)
        {
            double left = Canvas.GetLeft(this);
            double top = Canvas.GetTop(this);
            this.Width = newValue.Width; this.Height = newValue.Height;//Added to tackle bug in size animation.
            Arrange(new Rect(new Point(left, top), newValue));
        }

        #endregion

        #region Tick Property

        public static readonly DependencyProperty TickProperty =
            DependencyProperty.Register(
                "Tick", typeof(double), typeof(AnimatedHeaderedContentControl),
                new PropertyMetadata(OnTickChanged));

        /// <summary>
        /// Used to animate the size. 0 - start animation 1 - end of the animation.
        /// </summary>
        /// <remarks>
        /// Current size = (initial size) + (size difference) * Tick;
        /// </remarks>
        public double Tick
        {
            get { return (double)GetValue(TickProperty); }
            set { SetValue(TickProperty, value); }
        }

        private static void OnTickChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((AnimatedHeaderedContentControl)o).OnTickChanged((double)e.NewValue, (double)e.OldValue);
        }

        private void OnTickChanged(double newValue, double oldValue)
        {
            double width = _stepWidth * newValue;
            double height = _stepHeight * newValue;
            TargetSize = new Size(_initial.Width + width, _initial.Height + height);
        }

        #endregion

        #endregion

        /// <summary>
        /// Blank Constructor
        /// </summary>
        public AnimatedHeaderedContentControl()
        {
            sizeAnimation = new Storyboard();
            var animation = new DoubleAnimationUsingKeyFrames();

            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(AnimatedHeaderedContentControl.Tick)"));
            sizeAnimationKeyFrame = new SplineDoubleKeyFrame();
            sizeAnimationKeyFrame.KeySpline = new KeySpline
            {
                ControlPoint1 = new Point(0.528, 0),
                ControlPoint2 = new Point(0.142, 0.847)
            };
            sizeAnimationKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(500));
            sizeAnimationKeyFrame.Value = 1;
            animation.KeyFrames.Add(sizeAnimationKeyFrame);

            sizeAnimation.Children.Add(animation);
            //TBRemoved..........
            //sizeAnimation = new Storyboard();

            //var widthAnimation = new DoubleAnimationUsingKeyFrames();
            //Storyboard.SetTarget(widthAnimation, this);
            //Storyboard.SetTargetProperty(widthAnimation, new PropertyPath("(FrameworkElement.Width)"));
            //sizeAnimationWidthKeyFrame = new SplineDoubleKeyFrame
            //{
            //    KeySpline = new KeySpline
            //    {
            //        ControlPoint1 = new Point(0.528, 0),
            //        ControlPoint2 = new Point(0.142, 0.847)
            //    },
            //    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(500)),
            //    Value = 0
            //};
            //widthAnimation.KeyFrames.Add(sizeAnimationWidthKeyFrame);
            //var heightAnimation = new DoubleAnimationUsingKeyFrames();
            //Storyboard.SetTarget(heightAnimation, this);
            //Storyboard.SetTargetProperty(heightAnimation, new PropertyPath("(FrameworkElement.Height)"));
            //sizeAnimationHeightKeyFrame = new SplineDoubleKeyFrame
            //{
            //    KeySpline = new KeySpline
            //    {
            //        ControlPoint1 = new Point(0.528, 0),
            //        ControlPoint2 = new Point(0.142, 0.847)
            //    },
            //    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(500)),
            //    Value = 0
            //};
            //heightAnimation.KeyFrames.Add(sizeAnimationHeightKeyFrame);

            //sizeAnimation.Children.Add(widthAnimation);
            //sizeAnimation.Children.Add(heightAnimation);
            sizeAnimation.Completed += SizeAnimation_Completed;


            positionAnimation = new Storyboard();

            var positionXAnimation = new DoubleAnimationUsingKeyFrames();

            Storyboard.SetTarget(positionXAnimation, this);
            Storyboard.SetTargetProperty(positionXAnimation, new PropertyPath("(Canvas.Left)"));
            positionAnimationXKeyFrame = new SplineDoubleKeyFrame();
            positionAnimationXKeyFrame.KeySpline = new KeySpline
            {
                ControlPoint1 = new Point(0.528, 0),
                ControlPoint2 = new Point(0.142, 0.847)
            };
            positionAnimationXKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(500));
            positionAnimationXKeyFrame.Value = 0;
            positionXAnimation.KeyFrames.Add(positionAnimationXKeyFrame);

            var positionYAnimation = new DoubleAnimationUsingKeyFrames();

            Storyboard.SetTarget(positionYAnimation, this);
            Storyboard.SetTargetProperty(positionYAnimation, new PropertyPath("(Canvas.Top)"));
            positionAnimationYKeyFrame = new SplineDoubleKeyFrame();
            positionAnimationYKeyFrame.KeySpline = new KeySpline
            {
                ControlPoint1 = new Point(0.528, 0),
                ControlPoint2 = new Point(0.142, 0.847)
            };
            positionAnimationYKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(500));
            positionAnimationYKeyFrame.Value = 0;
            positionYAnimation.KeyFrames.Add(positionAnimationYKeyFrame);

            positionAnimation.Children.Add(positionXAnimation);
            positionAnimation.Children.Add(positionYAnimation);

            positionAnimation.Completed += PositionAnimation_Completed;
        }

        #region Public members

        /// <summary>
        /// Gets or sets the size animation duration.
        /// </summary>
        [Category("Animation Properties"), Description("The size animation duration.")]
        public TimeSpan SizeAnimationDuration
        {
            get { return sizeAnimationTimespan; }

            set
            {
                sizeAnimationTimespan = value;
                if (sizeAnimationKeyFrame != null)
                {
                    sizeAnimationKeyFrame.KeyTime = KeyTime.FromTimeSpan(sizeAnimationTimespan);
                }
            }
        }

        /// <summary>
        /// Gets or sets the position animation duration.
        /// </summary>
        [Category("Animation Properties"), Description("The position animation duration.")]
        public TimeSpan PositionAnimationDuration
        {
            get { return positionAnimationTimespan; }

            set
            {
                positionAnimationTimespan = value;
                if (positionAnimationXKeyFrame != null)
                {
                    positionAnimationXKeyFrame.KeyTime = KeyTime.FromTimeSpan(positionAnimationTimespan);
                }

                if (positionAnimationYKeyFrame != null)
                {
                    positionAnimationYKeyFrame.KeyTime = KeyTime.FromTimeSpan(positionAnimationTimespan);
                }
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Animates the size of the control
        /// </summary>
        /// <param name="width">The target width</param>
        /// <param name="height">The target height</param>
        public void AnimateSize(double width, double height)
        {
            if (sizeAnimating)
            {
                sizeAnimation.Pause();
            }

            if (Parent != null)
            {
                sizeAnimating = true;
                _initial = new Size(ActualWidth, ActualHeight);
                Size _destination = new Size(width, height);
                
                _stepWidth = _destination.Width - _initial.Width;
                _stepHeight = _destination.Height - _initial.Height;
                Tick = 0;
                //TBRemoved..............
                //sizeAnimationHeightKeyFrame.Value = height;
                //sizeAnimationWidthKeyFrame.Value = width;
                sizeAnimation.Begin();
            }
        }
        /// <summary>
        /// Animates the Canvas.Left and Canvas.Top of the control
        /// </summary>
        /// <param name="x">New X position</param>
        /// <param name="y">New Y position</param>
        public void AnimatePosition(double x, double y)
        {
            if (positionAnimating)
            {
                positionAnimation.Pause();
            }

            // Ensure we are in the tree
            if (Parent != null)
            {
                positionAnimating = true;
                positionAnimationXKeyFrame.Value = x;
                positionAnimationYKeyFrame.Value = y;
                positionAnimation.Begin();
            }
        }

        #endregion

        /// <summary>
        /// Stores the position
        /// </summary>
        /// <param name="sender">The position animation.</param>
        /// <param name="e">Event args.</param>
        private void PositionAnimation_Completed(object sender, EventArgs e)
        {
            Canvas.SetLeft(this, positionAnimationXKeyFrame.Value);
            Canvas.SetTop(this, positionAnimationYKeyFrame.Value);
        }

        /// <summary>
        /// Stores the values once the animation has completed.
        /// </summary>
        /// <param name="sender">The animated content control.</param>
        /// <param name="e">The event args.</param>
        private void SizeAnimation_Completed(object sender, EventArgs e)
        {
            sizeAnimationKeyFrame.Value = 1;
            //TBRemoved....................
            //Width = sizeAnimationWidthKeyFrame.Value;
            //Height = sizeAnimationHeightKeyFrame.Value;
        }
    }
}
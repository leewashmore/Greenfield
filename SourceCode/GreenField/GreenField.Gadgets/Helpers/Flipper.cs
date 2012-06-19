using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GreenField.Gadgets.Helpers
{
    public class Flipper
    {
        public enum Direction { LeftToRight, RightToLeft }
                
        /// <summary>
        /// Method to flip the Controls
        /// </summary>
        /// <param name="over">The Control which is currently in View</param>
        /// <param name="under">The Control which is currently hidden.</param>
        /// <param name="direction"></param>
        /// <param name="duration"></param>
        public static void FlipItem(UIElement over, UIElement under, Direction direction = Direction.LeftToRight, int duration = 2000)
        {
            // setup visible plane
            over.Visibility = Visibility.Visible;
            over.Projection = new PlaneProjection { CenterOfRotationY = 0 };
            // setup hidden plane
            under.Visibility = Visibility.Collapsed;
            under.Projection = new PlaneProjection { CenterOfRotationY = 0 };

            // gen storyboard
            var _StoryBoard = new System.Windows.Media.Animation.Storyboard();
            var _Duration = TimeSpan.FromMilliseconds(duration);

            // add animation: hide-n-show items
            _StoryBoard.Children.Add(CreateVisibility(_Duration, over, false));
            _StoryBoard.Children.Add(CreateVisibility(_Duration, under, true));

            // add animation: rotate items
            if (direction == Direction.LeftToRight)
            {
                _StoryBoard.Children.Add(CreateRotation(_Duration, 0, -90, -180, (PlaneProjection)over.Projection));
                _StoryBoard.Children.Add(CreateRotation(_Duration, 180, 90, 0, (PlaneProjection)under.Projection));
            }
            else if (direction == Direction.RightToLeft)
            {
                _StoryBoard.Children.Add(CreateRotation(_Duration, 0, 90, 180, (PlaneProjection)over.Projection));
                _StoryBoard.Children.Add(CreateRotation(_Duration, -180, -90, 0, (PlaneProjection)under.Projection));
            }

            // start animation
            _StoryBoard.Begin();
        }

        private static DoubleAnimationUsingKeyFrames CreateRotation(TimeSpan duration, double degreesFrom, double degreesMid, double degreesTo, PlaneProjection projection)
        {
            var _One = new EasingDoubleKeyFrame { KeyTime = new TimeSpan(0), Value = degreesFrom, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn } };
            var _Two = new EasingDoubleKeyFrame { KeyTime = new TimeSpan(duration.Ticks / 2), Value = degreesMid, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn } };
            var _Three = new EasingDoubleKeyFrame { KeyTime = new TimeSpan(duration.Ticks), Value = degreesTo, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } };

            var _Animation = new DoubleAnimationUsingKeyFrames { BeginTime = new TimeSpan(0) };
            _Animation.KeyFrames.Add(_One);
            _Animation.KeyFrames.Add(_Two);
            _Animation.KeyFrames.Add(_Three);
            Storyboard.SetTargetProperty(_Animation, new PropertyPath("RotationY"));
            Storyboard.SetTarget(_Animation, projection);
            return _Animation;
        }

        private static ObjectAnimationUsingKeyFrames CreateVisibility(Duration duration, UIElement element, bool show)
        {
            var _One = new DiscreteObjectKeyFrame { KeyTime = new TimeSpan(0), Value = (show ? Visibility.Collapsed : Visibility.Visible) };
            var _Two = new DiscreteObjectKeyFrame { KeyTime = new TimeSpan(duration.TimeSpan.Ticks / 2), Value = (show ? Visibility.Visible : Visibility.Collapsed) };

            var _Animation = new ObjectAnimationUsingKeyFrames { BeginTime = new TimeSpan(0) };
            _Animation.KeyFrames.Add(_One);
            _Animation.KeyFrames.Add(_Two);
            Storyboard.SetTargetProperty(_Animation, new PropertyPath("Visibility"));
            Storyboard.SetTarget(_Animation, element);
            return _Animation;
        }
    }
}

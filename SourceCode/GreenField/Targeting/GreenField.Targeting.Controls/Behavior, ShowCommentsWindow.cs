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
using System.Windows.Interactivity;
using System.Collections.ObjectModel;
using TopDown.FacingServer.Backend.Targeting;

namespace GreenField.Targeting.Controls
{
    public class ShowCommentsWindowBehavior : Behavior<UserControl>
    {
        public static readonly DependencyProperty CommentsProperty = DependencyProperty.Register("Comments", typeof(ObservableCollection<CommentModel>), typeof(ShowCommentsWindowBehavior), new PropertyMetadata(WhenCommentsAreAvailable));
        public ObservableCollection<CommentModel> Comments
        {
            get { return (ObservableCollection<CommentModel>)this.GetValue(CommentsProperty); }
            set { this.SetValue(CommentsProperty, value); }
        }

        private static void WhenCommentsAreAvailable(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as ShowCommentsWindowBehavior;
            var comments = e.NewValue as ObservableCollection<CommentModel>;
            self.Dispatcher.BeginInvoke(delegate
            {
                self.ShowComments(comments);
            });
        }

        private void ShowComments(ObservableCollection<CommentModel> comments)
        {
            var window = new CommentsWindow();
            window.DataContext = comments;
            window.Show();
        }
    }
}

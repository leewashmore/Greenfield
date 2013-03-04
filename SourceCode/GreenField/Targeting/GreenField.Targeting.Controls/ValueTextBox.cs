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
using System.Diagnostics;
using System.Windows.Threading;

namespace GreenField.Targeting.Controls
{
    public class ValueTextBox : TextBox
    {
        public const String ValueFormat = "#0.00";

        private DispatcherTimer finalCountdown;
        public ValueTextBox()
        {
            this.finalCountdown = new DispatcherTimer();
            this.finalCountdown.Stop();
            this.finalCountdown.Tick += new EventHandler(Timeout);
            this.finalCountdown.Interval = TimeSpan.FromMilliseconds(100);
        }


        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Decimal?), typeof(ValueTextBox), new PropertyMetadata(null, OnValueChanged));
        public Decimal? Value
        {
            get { return (Decimal?)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        protected static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as ValueTextBox;
            var before = (Decimal?)e.OldValue;
            var after = (Decimal?)e.NewValue;
            self.PushText(false);
        }

        public static readonly DependencyProperty AssumedValueProperty = DependencyProperty.Register("AssumedValue", typeof(Decimal?), typeof(ValueTextBox), new PropertyMetadata(null, OnAssumedValueChanged));


        public Decimal? AssumedValue
        {
            get { return (Decimal?)this.GetValue(AssumedValueProperty); }
            set { this.SetValue(AssumedValueProperty, value); }
        }

        protected static void OnAssumedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as ValueTextBox;
            var before = (Decimal?)e.OldValue;
            var after = (Decimal?)e.NewValue;
            self.PushText(false);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            this.inFocus = true;
            base.OnGotFocus(e);
            this.PushText(true);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Enter)
            {
                var binding = this.GetBindingExpression(TextBox.TextProperty);
                if (binding != null)
                {
                    binding.UpdateSource();
                }
                this.InitiateFinishing(false);
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            this.inFocus = false;
            base.OnLostFocus(e);
            this.InitiateFinishing(true);
        }

        private Boolean inFocus;
        private void PushText(Boolean destructively)
        {
            var value = this.Value;
            if (value.HasValue)
            {
                var text = this.ToText(value.Value);
                this.SetText(text, false);
            }
            else
            {
                if (destructively)
                {
                    this.SetText(String.Empty, false);
                }
                else
                {
                    if (this.inFocus)
                    {
                        // do nothing
                    }
                    else
                    {
                        this.SetAssumed();
                    }
                }
            }
        }

        public Boolean IsAssumed { get; private set; }
        private void SetText(String value, Boolean isAssumed)
        {
            this.Text = value;
            this.IsAssumed = isAssumed;
            if (isAssumed)
            {
                this.Foreground = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                this.Foreground = new SolidColorBrush(Colors.Black);
            }
            this.OnGotText();
        }

        public event EventHandler GotText;
        protected virtual void OnGotText()
        {
            var handler = this.GotText;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }


        private Decimal? PullText()
        {
            var text = this.Text;
            if (this.IsAssumed)
            {
                // do nothing
                return null;
            }
            else
            {
                Decimal? value;
                if (String.IsNullOrEmpty(text))
                {
                    if (this.inFocus)
                    {
                        // do nothing
                    }
                    else
                    {
                        this.SetAssumed();
                    }
                    value = null;
                }
                else
                {
                    var valueOpt = ToNumber(text);
                    if (valueOpt == null)
                    {
                        if (this.inFocus)
                        {
                            if (this.Value.HasValue)
                            {
                                text = this.ToText(this.Value.Value);
                                this.SetText(text, false);
                            }
                            else
                            {
                                this.SetText(String.Empty, false);
                            }
                        }
                        else
                        {
                            this.SetAssumed();
                        }
                        value = null;
                    }
                    else
                    {
                        value = valueOpt.Value;
                    }
                }

                return value;
            }
        }

        public void InitiateFinishing(Boolean cancellable)
        {
            var args = new CancellableEventArgs(false);
            this.OnFinishing(args);

            this.finalCountdown.Start();
            if (cancellable && args.IsCancelled)
            {
                this.CancelFinishing();
            }
        }

        public void CancelFinishing()
        {
            this.finalCountdown.Stop();
        }

        private void Timeout(object sender, EventArgs e)
        {
            this.Finish();
        }

        public void Finish()
        {
            this.CancelFinishing();
            var value = this.PullText();
            this.Value = value;
        }

        public event CancellableEventHandler Finishing;
        
        protected virtual void OnFinishing(CancellableEventArgs args)
        {
            var handler = this.Finishing;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void SetAssumed()
        {
            String text;
            if (this.AssumedValue.HasValue)
            {
                text = this.ToText(this.AssumedValue.Value);
            }
            else
            {
                text = String.Empty;
            }
            this.SetText(text, true);
        }

        private String ToText(Decimal value)
        {
            var input = System.Convert.ToDecimal(value);
            var output = Math.Round(input * 100000.0m) / 100000.0m * 100.00m;
            var result = output.ToString(ValueFormat);
            return result;
        }

        private Decimal? ToNumber(String value)
        {
            Decimal parsed;
            if (Decimal.TryParse(value, out parsed))
            {
                var scaled = parsed / 100.0m;
                return scaled;
            }
            else
            {
                return null;
            }
        }


    }
}

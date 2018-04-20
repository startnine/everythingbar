using System.Windows;

namespace Superbar
{
    public class QuadContentWindow : Window
    {
        public QuadContentWindow()
        {

        }

        public object SecondContent
        {
            get => GetValue(SecondContentProperty);
            set => SetValue(SecondContentProperty, (value));
        }

        public static readonly DependencyProperty SecondContentProperty =
            DependencyProperty.Register("SecondContent", typeof(object), typeof(QuadContentWindow), new PropertyMetadata());

        public object ThirdContent
        {
            get => GetValue(ThirdContentProperty);
            set => SetValue(ThirdContentProperty, (value));
        }

        public static readonly DependencyProperty ThirdContentProperty =
            DependencyProperty.Register("ThirdContent", typeof(object), typeof(QuadContentWindow), new PropertyMetadata());

        public object FourthContent
        {
            get => GetValue(FourthContentProperty);
            set => SetValue(FourthContentProperty, (value));
        }

        public static readonly DependencyProperty FourthContentProperty =
            DependencyProperty.Register("FourthContent", typeof(object), typeof(QuadContentWindow), new PropertyMetadata());
    }
}

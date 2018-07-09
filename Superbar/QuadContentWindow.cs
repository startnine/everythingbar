using Start9.Api.AppBar;
using System.Windows;

namespace Superbar
{
    public class QuadContentWindow : AppBarWindow
    {
        public QuadContentWindow()
        {

        }

        public System.Object SecondContent
        {
            get => GetValue(SecondContentProperty);
            set => SetValue(SecondContentProperty, value);
        }

        public static readonly DependencyProperty SecondContentProperty =
            DependencyProperty.Register("SecondContent", typeof(System.Object), typeof(QuadContentWindow), new PropertyMetadata());

        public System.Object ThirdContent
        {
            get => GetValue(ThirdContentProperty);
            set => SetValue(ThirdContentProperty, value);
        }

        public static readonly DependencyProperty ThirdContentProperty =
            DependencyProperty.Register("ThirdContent", typeof(System.Object), typeof(QuadContentWindow), new PropertyMetadata());

        public System.Object FourthContent
        {
            get => GetValue(FourthContentProperty);
            set => SetValue(FourthContentProperty, value);
        }

        public static readonly DependencyProperty FourthContentProperty =
            DependencyProperty.Register("FourthContent", typeof(System.Object), typeof(QuadContentWindow), new PropertyMetadata());
    }
}

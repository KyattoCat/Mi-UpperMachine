using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MiUpperMachine.UserControls
{
    /// <summary>
    /// TitledTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class TitledTextBox : UserControl
    {
        public string Title
        {
            get
            {
                return data.Title;
            }
            set
            {
                data.Title = value;
            }
        }
        public string Text
        {
            get
            {
                return data.Text;
            }
            set
            {
                data.Text = value;
            }
        }

        private TitledTextBoxData data;
        
        public TitledTextBox()
        {
            InitializeComponent();
            data = new TitledTextBoxData();
            DataContext = data;
        }

        private void tbInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            data.Text = tbInput.Text;
        }
    }

    public class TitledTextBoxData
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public TitledTextBoxData()
        {
            Title = "";
            Text = "";
        }
    }
}

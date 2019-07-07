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

namespace Projeto_PII_noCanvas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        Jogo jogo;
        public MainWindow()
        {
            this.KeyDown += new KeyEventHandler(windows_KeyDown);
            this.KeyUp += new KeyEventHandler(windows_KeyUp);
            InitializeComponent();
        }
        public void OnAddElement(Frame obj) => canvas.Children.Add(obj);
        public void OnRemoveElement(Frame obj)
        {
            canvas.Children.Remove(obj);
        }

        public void OnEnd()
        {
            int count = canvas.Children.Count;
            /* Remove apenas a partir da posição 7
             * Porque até essa posicao estão as labels e botões
             */
            canvas.Children.RemoveRange(7, count);
            btn_Start.Visibility = Visibility.Visible;
        }
        public void OnChangeScreen(string label, string update)
        {
            switch (label)
            {
                case "nivel": lbl_nivel.Content = update; break;
                case "vida": lbl_vidas.Content = update; break;
                case "pontos": lbl_pontuacao.Content = update; break;
            }
        }
        private void windows_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    jogo.j.isPressedUp = true;
                    break;
                case Key.A:
                    jogo.j.isPressedLeft = true;
                    break;
                case Key.S:
                    jogo.j.isPressedDown = true;
                    break;
                case Key.D:
                    jogo.j.isPressedRight = true;
                    break;
                case Key.Space:
                    if (jogo.timer.IsEnabled)
                        jogo.timer.Stop();
                    else
                        jogo.timer.Start();
                    break;
            }
        }
        private void windows_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    jogo.j.isPressedUp = false;
                    break;
                case Key.A:
                    jogo.j.isPressedLeft = false;
                    break;
                case Key.S:
                    jogo.j.isPressedDown = false;
                    break;
                case Key.D:
                    jogo.j.isPressedRight = false;
                    break;
            }
        }
        private void btnStart_click(object sender, EventArgs e)
        {
            btn_Start.Visibility = Visibility.Hidden;
            jogo = new Jogo(800, 800);
            jogo.OnAddElement += OnAddElement;
            jogo.OnRemoveElement += OnRemoveElement;
            jogo.OnEnd += OnEnd;
            jogo.Start();
            lbl_nivel.Content = "1";
            lbl_vidas.Content = "10";
            lbl_pontuacao.Content = "0";
            jogo.OnChangeScreen += OnChangeScreen;
            Focus();
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Controls;


namespace Projeto_PII_noCanvas
{
    public delegate void onAddElement_Handler(Frame elm);
    public delegate void onRemoveElement_Handler(Frame elm);
    public delegate void Event_with_Label_Handler(string s);
    public delegate void OnChangeScreen_Handler(string opc, string s);
    public delegate void Empty_Handler();
    public class Jogo
    {
        public int pontuacao = 0,
            nivel = 1,
            x_window,
            y_window,
            contadorCriacaoObstaculos = 0,
            nextObstaculo = 100,
            obCounter = 0;
        public Jogador j;
        List<Objeto> ob = new List<Objeto>();
        public DispatcherTimer timer;
        public event onAddElement_Handler OnAddElement;
        public event onRemoveElement_Handler OnRemoveElement;
        public event OnChangeScreen_Handler OnChangeScreen;
        public event Empty_Handler OnEnd;
        public double GetDiffFuncEvolution()
        {
            return Math.Sqrt(nivel);
        }
        public Jogo(int _x_window, int _y_window)
        {
            x_window = _x_window;
            y_window = _y_window;
        }

        public void Start()
        {

            j = new Jogador(x_window, y_window);
            j.OnDefeat += End;
            
            OnAddElement(j);
            timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 1)
            };

            timer.Tick += new EventHandler(Tick);
            timer.Start();
        }
        public void End()
        {
            timer.Stop();
            OnEnd();
        }
        private void CriarObstáculo(bool isRoxo, bool isAzul)
        {

            Random r = new Random();
            double diff = GetDiffFuncEvolution();
            nextObstaculo = (int)(r.Next(50) + (100 / diff));
            contadorCriacaoObstaculos = 0;
            Obstaculo dp;
                 if (isRoxo)     dp = new Roxo(0, x_window, y_window);
            else if (isAzul)     dp = new Azul(0, x_window, y_window);
            else                 dp = new Branco((int)(diff), x_window, y_window);
            dp.OnColizao += AddPontos;
            if(!isRoxo)
                dp.OnOutOfWindow += TiraVida;
            dp.index = 7 + obCounter;
            obCounter++;
            ob.Add(dp);
            OnAddElement(dp);
            contadorCriacaoObstaculos = 0;
        }
        public void AddPontos(Obstaculo sender)
        {
            /*
             * Incrementa a pontuação
             * */
            pontuacao += sender.valor;
            OnChangeScreen("pontos", pontuacao.ToString());
        }
        public void TiraVida(Objeto sender)
        {
            OnChangeScreen("vida", j.tiraVida(sender.valor));
        }
        public void RemoveObstaculo(Objeto sender)
        {
            OnRemoveElement(sender);
            ob.Remove(sender);
        }
        private void Tick(object sender, object e)
        {
            /*
             * Incrementa o nivel
             * */
            if (pontuacao > nivel * nivel * 5)
            {
                OnChangeScreen("nivel", nivel++.ToString());
                CriarObstáculo(true, true);
                CriarObstáculo(false, true);
                CriarObstáculo(false, false);
            }

            //Move o jogador

            j.MoveMe();
            /*
             * Adiciona Novos Obstaculos 
             */
            contadorCriacaoObstaculos++;
            if (contadorCriacaoObstaculos >= nextObstaculo)
            {
                bool isAzul = (nivel >= 3 && new Random().Next((int)(40 / Math.Sqrt(nivel-2))) == 0)? true:false;
                bool isRoxo = (nivel >= 6 && new Random().Next((int)(25 / Math.Sqrt(nivel-5))) == 0)? true:false;
                CriarObstáculo(isRoxo, isAzul);
            }
            if (ob != null)
            {
                Obstaculo toRemove = null;
                foreach (Obstaculo obs in ob)
                {
                    //Verifica se colide com o jogador
                    if (obs.VerificaSeColideCom(j))
                    {
                        toRemove = obs;
                    }

                    /*Move o obstaculo
                     * Se se mover para fora remove-o
                     * Observação: Apenas remove um de cada vez
                     * caso aja mais do que um para remover
                     * irá remover apenas no proximo Tick
                     */
                    obs.MoveMe();
                    if (obs.VerificaRender())
                    {
                        obs.colisao = true;
                        toRemove = obs;
                    }

                }
                if (toRemove!=null)
                    RemoveObstaculo(toRemove);
                        
            }

        }

    }
}

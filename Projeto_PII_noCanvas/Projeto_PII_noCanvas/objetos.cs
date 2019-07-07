using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Projeto_PII_noCanvas
{

    class objetos
    {
        /*
        * Classe criada para evitar um bug de visual studio
        * em que ao derivar a primeira classe de um elemento visual
        * autocompila o ficheiro como Design
        * */
    }

    public delegate void Objeto_Handler(Objeto sender);
    public delegate void Obstaculo_Handler(Obstaculo sender);
    public delegate void void_Handler();
    public class Objeto : Frame
    {
        protected int posicao,
                      direcao,
                      x_max,
                      y_max,
                      x,
                      y,
                      speedVertical = 0,
                      speedHorizontal = 0;

        public int valor = 0,
            index;

        public event Objeto_Handler OnOutOfWindow;
        public bool Contains(Objeto r)
        {
            int x = r.x;
            int y = r.y;

            bool resp = false;
            if (
                    this.x < x + r.Width &&
                    this.x + this.Width > x &&
                    this.y < y + r.Height &&
                    this.y + this.Height > y
                )
            {
                resp = true;
            }
            return resp;
        }

        public virtual void MoveMe()
        {
            this.x += this.speedHorizontal;
            this.y += this.speedVertical;
            this.Margin = new System.Windows.Thickness(
                this.x,
                this.y,
                ((800 - x - Width)==0)? (800 - x - Width) : 1,
                ((800 - y - Height) ==0)? (800 - y - Height) : 1

            );
        }
        public bool VerificaRender()
        {
            if (this.x < -100 ||
                    this.x > x_max + 100 ||
                    this.y < -100 ||
                    this.y > y_max + 100
              )
            {
                if (this.valor <= 49)
                    OnOutOfWindow(this);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class Jogador : Objeto
    {

        public bool isPressedUp = false,
                isPressedDown = false,
                isPressedLeft = false,
                isPressedRight = false;

        readonly double friccao = .875; //Limitador de velocidade
        int speed = 2;
        public event void_Handler OnDefeat;

        public Jogador(int x_ecra, int y_ecra)
        {
            valor = 10;
            this.Height = 30;
            this.Width = 30;
            this.x = x_max / 2;
            this.y = y_max / 2;
            MoveMe();

            this.Background = Brushes.Yellow;
            //this.Image = System.Drawing.Image.FromFile("Pics\\square.png");
            //this.SizeMode = PictureBoxSizeMode.StretchImage;
        }
        public string tiraVida(int vlr)
        {
            this.valor -= vlr;
            if (this.valor <= 0)
                OnDefeat();
            return this.valor.ToString();
        }
        public override void MoveMe()
        {
            /* Vertical */
            if (isPressedUp && speedVertical <= 0) { speedVertical -= speed; }
            else if (isPressedDown && speedVertical >= 0) { speedVertical += speed; }

            /* Horizontal */
            if (isPressedRight && speedHorizontal >= 0) { speedHorizontal += speed; }
            else if (isPressedLeft && speedHorizontal <= 0) { speedHorizontal -= speed; }

            /* Aplicação de fricção*/
            speedVertical = (int)(speedVertical * friccao);
            speedHorizontal = (int)(speedHorizontal * friccao);
            base.MoveMe();
        }
    }
    public class Obstaculo : Objeto
    {
        public bool colisao = false;
        public event Obstaculo_Handler OnColizao;

        public Obstaculo(int delta, int x_ecra, int y_ecra)
        {

            x_max = x_ecra;
            y_max = y_ecra;
            Random r = new Random();
            direcao = r.Next(2);
            posicao = r.Next(2);
            switch (posicao)
            {
                case 0:
                    int posicao_y = 0;
                    switch (direcao)
                    {
                        case 0: posicao_y = y_ecra + 25; break;
                        case 1: posicao_y = -25; break;
                    }
                    this.Height = 20;

                    this.x = 0;
                    this.y = posicao_y;
                    MoveMe();
                    break;
                case 1:
                    int posicao_x = 0;
                    switch (direcao)
                    {
                        case 0: posicao_x = x_ecra + 25; break;
                        case 1: posicao_x = -25; break;
                    }
                    this.Width = 20;
                    this.x = posicao_x;
                    this.y = 0;
                    MoveMe();
                    break;

            }
        }
        public virtual bool VerificaSeColideCom(Objeto o)
        {
            if (o.Contains(this) && !colisao)
            {
                OnColizao(this);
                this.colisao = true;
            }
            else
            {
                this.colisao = false;

            }
            return colisao;
        }
    }
    public class Branco : Obstaculo
    {
        public Branco(int delta, int x_ecra, int y_ecra) : base(delta, x_ecra, y_ecra)
        {
            this.valor = 1;
            Random r = new Random();

            this.Background = Brushes.White;

            switch (posicao)
            {
                case 0:
                    speedVertical = r.Next(3) + 1;
                    if (direcao == 0) speedVertical *= -1;

                    this.Width = r.Next(delta) + 100;
                    this.x = (int)r.Next(x_ecra - (int)this.Width);
                    MoveMe();
                    break;
                case 1:
                    speedHorizontal = r.Next(3) + 1;
                    if (direcao == 0) speedHorizontal *= -1;

                    this.Height = r.Next(delta) + 100;

                    this.y = r.Next(y_ecra - (int)this.Height);
                    MoveMe();
                    break;

            }
        }
    }
    public class Azul : Obstaculo
    {
        int moverCount = 0;
        int moverSpeed = 0;
        public Azul(int delta, int x_ecra, int y_ecra) : base(delta, x_ecra, y_ecra)
        {
            this.valor = 5;
            this.Background = Brushes.Blue;
            this.moverSpeed = new Random().Next(4) * 50 + 200;
            int speed = new Random().Next(5) * 25 + 70;

            switch (posicao)
            {
                case 0:
                    speedVertical = speed;
                    if (direcao == 0) speedVertical *= -1;

                    this.Width = x_ecra;
                    this.Height = 20;
                    this.x = 0;
                    MoveMe();
                    break;

                case 1:
                    speedHorizontal = speed;
                    if (direcao == 0) speedHorizontal *= -1;

                    this.Width = 20;
                    this.Height = y_ecra;
                    this.y = 0;
                    MoveMe();
                    break;

            }
        }
        override public void MoveMe()
        {
            moverCount++;
            if (moverCount >= moverSpeed)
            {
                moverCount = 0;
                base.MoveMe();
            }
        }
    }
    public class Roxo : Obstaculo
    {
        public Roxo(int delta, int x_ecra, int y_ecra) : base(delta, x_ecra, y_ecra)
        {
            this.valor = 150;
            Random r = new Random();

            this.Background = Brushes.Purple;
            this.Width = 50;
            this.Height = 50;

            switch (posicao)
            {
                case 0:
                    speedVertical = 1;
                    if (direcao == 0) speedVertical *= -1;
                    this.x = r.Next(x_ecra - (int)this.Width);
                    MoveMe();
                    break;
                case 1:
                    speedHorizontal = 1;
                    if (direcao == 0) speedHorizontal *= -1;
                    this.y = r.Next(y_ecra - (int)this.Height);
                    MoveMe();
                    break;

            }
        }
    }
}

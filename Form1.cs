using System;
using System.Threading;
using System.Windows.Forms;

namespace BarberoDormilon
{
    public partial class FormBarberia : Form
    {
        Thread barberShop = null;//Hilo Principal
        int clientesEspera = 1;
        bool Barbero = false;
        bool[] arrayEspera = new bool[3] { false, false, false };
        bool work;
        int numeroAleatorio = 5;
        int ClientesCont = 0;
        public FormBarberia()
        {
            InitializeComponent();
        }

        private void btnNewProcess_Click(object sender, EventArgs e)
        {
            if (arrayEspera[0] && arrayEspera[1] && arrayEspera[2])
            {
                this.Invoke((MethodInvoker)delegate
                {
                    setIteraciones(clientesEspera.ToString() + " cliente perdido\n");
                });
            }
            nuevoProceso();
            
        }

        //_____________::Proceso 1:__________
        public void start()
        {
            work = true;
            while (work)
            {
                if (!Barbero)
                {
                    quitarCliente();
                    dormir();
                    numeroAleatorio = 1;
                }
                else
                {
                    activar();
                }
                for (int x = 0; x < arrayEspera.Length; x++)
                {
                    if (!arrayEspera[x])
                    {
                        desocuparSillaEspera(x + 1);
                    }
                }
                Thread.Sleep(numeroAleatorio * 1000);
            }
        }
        //SAve UI
        public void setIteraciones(string status)
        {
            this.rTXiteraciones.Text += status;
        }

        private void nuevoProceso()
        {
            int contador = -1;
            bool bandera = false;

            if (!Barbero)
            {
                Barbero = true;
                contador = 0;
                bandera = true;
            }

            if (bandera)
            {
                despertar();
            }
            else
            {
                //Si todos el barbero está ocupado, examina las sillas
                for (int x = 0; x < arrayEspera.Length; x++)
                {
                    if (!arrayEspera[x])
                    {
                        arrayEspera[x] = true;
                        contador = x + 1;
                        bandera = true;
                        break;
                    }
                }
                if (bandera)
                {
                    // Asignar silla
                    ocuparSilla(contador);
                }
                else
                {
                    clientesEspera++;
                }
            }
        }

        private void ocuparSilla(int silla)
        {
            switch (silla)
            {
                case 1:
                    pBCliente1.Visible = true;
                    break;
                case 2:
                    pBCliente2.Visible = true;
                    break;
                case 3:
                    pBCliente3.Visible = true;
                    break;
                default:
                    break;
            }

            try
            {
             Thread.Sleep(100);
            }
            catch (ThreadInterruptedException ex)
            {
                Console.WriteLine("Error" + ex);
            }
        }

        //_____________::Proceso 1 FIN::__________

        //_____________::Proceso 2 ::__________
        public void run()//atender.start();
        {
            Random rnd = new Random();
            this.numeroAleatorio = rnd.Next(5, 10);  // creates a number between 10 and 16

            try
            {
                Thread.Sleep(numeroAleatorio * 1000);
            }
            catch (ThreadInterruptedException ex)
            {
                Console.WriteLine("Error" + ex);
            }
            clienteSatisfecho();
        }

        private void clienteSatisfecho()
        {
            Barbero = false;
            ClientesCont++;
            string status = "Cliente "+ ClientesCont+" Satisfecho\n";
            this.Invoke((MethodInvoker)delegate
            {
                setIteraciones(status);
            });
            this.Invoke((MethodInvoker)delegate
            {
                actualizarCobro();
            });
            verificarEspera();
        }

        private void actualizarCobro()
        {
            int sum = 10 * ClientesCont;
            lblTotalAcum.Text =  "$" + sum.ToString();
        }
        private void verificarEspera()
        {
            if (arrayEspera[0] == true)
            {
                if (arrayEspera[1] == true)
                {
                    if (arrayEspera[2] == true)
                    {
                        arrayEspera[2] = false;
                        desocuparSillaEspera(3);
                    }
                    else
                    {
                        arrayEspera[1] = false;
                        desocuparSillaEspera(2);
                    }
                }
                else
                {
                    arrayEspera[0] = false;
                    desocuparSillaEspera(1);
                }
                Barbero = true;
                despertar();
            }
        }

        //_____________::Proceso 2 FIN ::__________

        //_____________::Interfaz Update::_________
        public void despertar()
        {
            setDespertar();
           Thread atender = new Thread(new ThreadStart(run));
            atender.Start();
        }

        //_________::UI Safe hilo ::_____________
        private delegate void setDelegateDespertar();
        public void setDespertar()
        {
            if (pBBarbero.InvokeRequired && pBZZ.InvokeRequired)
            {
                setDelegateDespertar delegado = new setDelegateDespertar(setDespertar);
                pBBarbero.Invoke(delegado, new object[] { });
                pBZZ.Invoke(delegado, new object[] { });
            }
            else
            {
                pBBarbero.Visible = false;
                pBZZ.Visible = false;
            }

        }

        public void dormir()
        {
            setDormir();
        }
        //_________::UI Safe hilo::_____________
        private delegate void setDelegateDormido();
        public void setDormir()
        {

            if (pBBarbero.InvokeRequired || pBZZ.InvokeRequired || lblStatus.InvokeRequired)
            {
                setDelegateDormido delegado = new setDelegateDormido(setDormir);
                pBBarbero.Invoke(delegado, new object[] { });
            }
            else
            {
                pBBarbero.Visible = true;
                pBZZ.Visible = true;
                lblStatus.Text = "Durmiendo";
            }

        }

        public void activar()
        {
            setActivar();
        }

        //_________::UI Safe hilo::_____________
        private delegate void setDelegateActivar();
        public void setActivar()
        {

            if (pBSilla.InvokeRequired || lblStatus.InvokeRequired)
            {
                setDelegateActivar delegado = new setDelegateActivar(setActivar);
                pBSilla.Invoke(delegado, new object[] { });
            }
            else
            {
                this.pBSilla.Image = global::BarberoDormilon.Properties.Resources.barbero_trabjando;
                lblStatus.Text = "Atendiendo";
            }

        }

        public void desocuparSillaEspera(int silla)
        {
            setDesocuparSilla(silla);
        }

        //_________::UI Safe hilo::_____________
        private delegate void setDelegateDesocuparSilla(int silla);
        public void setDesocuparSilla(int silla)
        {

            if (pBCliente1.InvokeRequired || pBCliente2.InvokeRequired)
            {
                setDelegateDesocuparSilla delegado = new setDelegateDesocuparSilla(setDesocuparSilla);
                pBCliente1.Invoke(delegado, new object[] { silla });
            }
            else
            {
                switch (silla)
                {
                    case 1:
                        pBCliente1.Visible = false;
                        break;
                    case 2:
                        pBCliente2.Visible = false;
                        break;
                    case 3:
                        pBCliente3.Visible = false;
                        break;
                    default:
                        break;
                }
            }

        }

        public void quitarCliente()
        {
            setQuitarCliente();
        }
        //_________::UI Safe hilo::_____________
        private delegate void setDelegateQuitarCliente();
        public void setQuitarCliente()
        {

            if (pBSilla.InvokeRequired)
            {
                setDelegateQuitarCliente delegado = new setDelegateQuitarCliente(setQuitarCliente);
                pBSilla.Invoke(delegado, new object[] { });
            }
            else
            {
                this.pBSilla.Image = global::BarberoDormilon.Properties.Resources.barberchair;
            }

        }


        private void FormBarberia_FormClosing(object sender, FormClosingEventArgs e)
        {
           if (barberShop != null)
            {
                this.work = false;
                try
                {
                    //barberShop.Join();
                    barberShop.Abort();

                }
                catch (ThreadAbortException ex)
                {
                    Console.WriteLine(ex);
                }
                //DestroyHandle  
            }

        }

        private void FormBarberia_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.work = false;
                this.rTXiteraciones.Text = "Proceso 1 finalizado";
            }
            if (e.KeyData == Keys.Return)
            {
                this.work = false;
                this.rTXiteraciones.Text = "Proceso 1 finalizado";
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (!Barbero)
            {
                this.btnClose.Enabled = false;
                this.btnNewProcess.Enabled = false;
                this.btnOpen.Enabled = true;
                this.work = false;
                this.rTXiteraciones.Text = "Cerrado\n";
                clientesEspera = 1;
                ClientesCont = 0;
            }
            else
            {
                this.rTXiteraciones.Text += "No Puedes cerrar hasta que acaben los procesos\n";
                this.btnNewProcess.Enabled = false;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            this.work = true;
            this.btnClose.Enabled = true;
            this.btnNewProcess.Enabled = true;
            this.btnOpen.Enabled = false;
            this.Invoke((MethodInvoker)delegate
            {
                setStatus("");
            });
            this.Invoke((MethodInvoker)delegate
            {
                changeAcumulado("$0");
            });

            barberShop = new Thread(new ThreadStart(start));
            barberShop.Start();
        }

        private void changeAcumulado(string str)
        {
            this.lblTotalAcum.Text = str;
        }
        private void setStatus(string str)
        {
            this.rTXiteraciones.Text = str;
        }
    }
}

using cansat_app;
using Cansat2021;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//Prueba
using System.Threading;

using System.IO;
namespace cansat_app
{
    public partial class Resolution : Form
    {
        public static List<byte> buffer = new List<byte>();
        public static List<byte> bufferout = new List<byte>();
        public static List<string> telemetry = new List<string>();
        public static SerialPort _serialPort;
        public static string simfile;
        public static string export;
        public static int line;
        public static System.IO.StreamReader file ;
        public Resolution()
        {
            InitializeComponent();
            simfile = textBox3.Text;
            export = textBox4.Text;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }
        
        static bool _continue;
        
        public  void init()
        {
            //TimerClass timer = new TimerClass();
            //TimerClass.Main();


            //CsvHelper csvHelper = new CsvHelper();
            //var myFile = csvHelper.readExampleFile();
            //string name = "";
            //string message = "";
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            //Thread readThread = new Thread(Read);

            // Create a new SerialPort object with default settings.  
            //serialPort1 = new SerialPort();

            // Allow the user to set the appropriate properties.  
            serialPort1.PortName = SetPortName(Form1.portname );
            //serialPort1.BaudRate = SetPortBaudRate(19200);
            //serialPort1.Parity = SetPortParity(_serialPort.Parity);
            //serialPort1.DataBits = SetPortDataBits(_serialPort.DataBits);
            //serialPort1.StopBits = SetPortStopBits(_serialPort.StopBits);
            //serialPort1.Handshake = SetPortHandshake(_serialPort.Handshake);


            //Set the read / write timeouts
            //serialPort1.ReadTimeout = 500;
            //serialPort1.WriteTimeout = 500;
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(port_OnReceiveDatazz);
            }

            _continue = true;
           

         
        }

        private  void port_OnReceiveDatazz(object sender,
                                  SerialDataReceivedEventArgs e)
        {
            //SerialPort sp = (SerialPort)sender;
            
            while (serialPort1.BytesToRead >1){
                var byteReaded = serialPort1.ReadByte();
                if (byteReaded == 0x7E)
                {
                    buffer.Clear();
                    telemetry.Clear();
                }
                buffer.Add((byte)byteReaded);

                if (buffer.Count >= 9)
                {
                    var buffer2 = buffer[2];
                    byte aux;
                    aux = (byte)(buffer[2] + 0x04);
                    if (aux == (byte)buffer.Count) //pregunta si ya tenemos toda la trama dentro de buffer
                    {
                        var message = "";
                        for (int i = 8; i < (buffer.Count - 1); i++)
                        {
                            message += (char)buffer[i];
                        }

                        // this.textBox1.Text = message;
                        SetText(message);
                        //Split message and send to CsvHelper class to create or append 
                        telemetry = message.Split(',').ToList();
                        Cansat2021.CsvHelper.writeCsvFromList(telemetry,export);

                        //fillForm(telemetry);
                        //Send message to Mqtt server
                        Mqtt.Publish(message);
                    }
                }

            }
        }

        public  void fillForm(List<string> telemetry)
        {
            textBox1.Text = "hola";
        }
        public static string SetPortName(string defaultPortName)
        {
            try
            {
                string portName = "";

                Console.WriteLine("Available Ports:");
                foreach (string s in SerialPort.GetPortNames())
                {
                    Console.WriteLine("   {0}", s);
                }

                Console.Write("COM port({0}): ", defaultPortName);
                //portName = Console.ReadLine();

                if (portName == "")
                {
                    portName = defaultPortName;
                }
                return portName;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static int SetPortBaudRate(int defaultPortBaudRate)
        {
            string baudRate = "";

            Console.Write("Baud Rate({0}): ", defaultPortBaudRate);
            //baudRate = Console.ReadLine();

            if (baudRate == "")
            {
                baudRate = defaultPortBaudRate.ToString();
            }

            return int.Parse(baudRate);
        }

        public static Parity SetPortParity(Parity defaultPortParity)
        {
            string parity = "";

            Console.WriteLine("Available Parity options:");
            foreach (string s in Enum.GetNames(typeof(Parity)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Parity({0}):", defaultPortParity.ToString());
            //parity = Console.ReadLine();

            if (parity == "")
            {
                parity = defaultPortParity.ToString();
            }

            return (Parity)Enum.Parse(typeof(Parity), parity);
        }

        public static int SetPortDataBits(int defaultPortDataBits)
        {
            string dataBits = "";

            Console.Write("Data Bits({0}): ", defaultPortDataBits);
            //dataBits = Console.ReadLine();

            if (dataBits == "")
            {
                dataBits = defaultPortDataBits.ToString();
            }

            return int.Parse(dataBits);
        }

        public static StopBits SetPortStopBits(StopBits defaultPortStopBits)
        {
            string stopBits = "";

            Console.WriteLine("Available Stop Bits options:");
            foreach (string s in Enum.GetNames(typeof(StopBits)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Stop Bits({0}):", defaultPortStopBits.ToString());
            //stopBits = Console.ReadLine();

            if (stopBits == "")
            {
                stopBits = defaultPortStopBits.ToString();
            }

            return (StopBits)Enum.Parse(typeof(StopBits), stopBits);
        }

        public static Handshake SetPortHandshake(Handshake defaultPortHandshake)
        {
            string handshake = "";

            Console.WriteLine("Available Handshake options:");
            foreach (string s in Enum.GetNames(typeof(Handshake)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Handshake({0}):", defaultPortHandshake.ToString());
            //handshake = Console.ReadLine();

            if (handshake == "")
            {
                handshake = defaultPortHandshake.ToString();
            }

            return (Handshake)Enum.Parse(typeof(Handshake), handshake);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            init();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            file = new System.IO.StreamReader(this.textBox3.Text);
            var datatx = "CMD,1231,SIM,ACTIVATE";
            bufferout.Clear();
            bufferout.Add(0x7E);
            bufferout.Add(0x00);
            bufferout.Add((byte)(datatx.Length + 5));
            bufferout.Add(0x01);
            bufferout.Add(0x01);
            bufferout.Add(0x01); //0x01 
            bufferout.Add(0x11); //0x11
            bufferout.Add(0x00);

            for (int i = 0; i < datatx.Length; i++)
            {
                bufferout.Add((byte)datatx[i]);
            }
            byte chkaux = 0;
            for (int i = 3; i < datatx.Length + 8; i++)
            {
                chkaux += bufferout[i];
            }
            chkaux = (byte)(0xFF - chkaux);
            bufferout.Add(chkaux);




            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();

            }
            serialPort1.Write(bufferout.ToArray(), 0, bufferout.Count);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var datatx = "CMD,1231,SIM,ENABLE";
            bufferout.Clear();
            bufferout.Add(0x7E);
            bufferout.Add(0x00);
            bufferout.Add((byte)(datatx.Length + 5));
            bufferout.Add(0x01);
            bufferout.Add(0x01);
            bufferout.Add(0x01); //0x01 
            bufferout.Add(0x11); //0x11
            bufferout.Add(0x00);

            for (int i = 0; i < datatx.Length; i++)
            {
                bufferout.Add((byte)datatx[i]);
            }
            byte chkaux = 0;
            for (int i = 3; i < datatx.Length + 8; i++)
            {
                chkaux += bufferout[i];
            }
            chkaux = (byte)(0xFF - chkaux);
            bufferout.Add(chkaux);




            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();

            }
            serialPort1.Write(bufferout.ToArray(), 0, bufferout.Count);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            for(double i = 0; i < 1000; i++)
            {
                double n = i;
                String N = n.ToString();
                double m = i * 0.5;
                String M = m.ToString();
                PutData(N,M, N, M,N, M, N, M, N, M, N, M,"R","N");
                PutDataPayload1(N, M, N);
                PutDataPayload2(N, M, N);
                Thread.Sleep(200);
            }
            
        }

        private void Resolution_Load(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void missionTime_lbl_Click(object sender, EventArgs e)
        {

        }

        //FUNCION PUBLICA MANDAR DATOS A LABEL EN TIEMPO REAL
        public void PutData(string pc, string mt, string gpsT, string gpsLa, string gpsLo, string gpsA, string gpsS, string cV, string cA, string cT, string p1cp, string p2cp, String sp1r, String sp2r)
        {
            packetCount_lbl.Text = pc;
            missionTime_lbl.Text = mt;
            gpsTime_lbl.Text = gpsT;
            gpsLatitude_lbl.Text = gpsLa;
            gpsLongitude_lbl.Text = gpsLo;
            gpsAltitude_lbl.Text = gpsA;
            gpsSats_lbl.Text = gpsS;
            voltage_lbl.Text = cV;
            cAltitude_lbl.Text = cA;
            cTemperature_lbl.Text = cT;
            P1pc_lbl.Text = p1cp;
            P2pc_lbl.Text = p2cp;
            if (sp1r.Equals("R"))
            {
                P1green_img.Visible = true;
                P1red_img.Visible = false;

            }
            else
            {
                P1green_img.Visible = false;
                P1red_img.Visible = true;
            }
            if (sp2r.Equals("R"))
            {
                P2green_img.Visible = true;
                P2red_img.Visible = false;

            }
            else
            {
                P2green_img.Visible = false;
                P2red_img.Visible = true;
            }
            Application.DoEvents();
        }

       

        private void button6_Click(object sender, EventArgs e)
        {
            var datatx = "CMD,1231,CX,ON";
            bufferout.Clear();
            bufferout.Add(0x7E);
            bufferout.Add(0x00);
            bufferout.Add((byte)(datatx.Length + 5));
            bufferout.Add(0x01);
            bufferout.Add(0x01);
            bufferout.Add(0x01); //0x01 
            bufferout.Add(0x11); //0x11
            bufferout.Add(0x00);

            for (int i = 0; i < datatx.Length; i++)
            {
                bufferout.Add((byte)datatx[i]);
            }
            byte chkaux = 0;
            for (int i = 3; i < datatx.Length + 8; i++)
            {
                chkaux += bufferout[i];
            }
            chkaux = (byte)(0xFF - chkaux);
            bufferout.Add(chkaux);




            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();

            }
            serialPort1.Write(bufferout.ToArray(), 0, bufferout.Count);
        }












        delegate void SetTextCallback(string text);

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox1.Text = text;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Browse Text Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = openFileDialog1.FileName;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Browse Text Files",

                CheckFileExists = false,
                CheckPathExists = true,

                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = false,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = openFileDialog1.FileName;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            Resolution.export = this.textBox4.Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            Resolution.simfile= this.textBox3.Text;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            {
                string command;
                if ((command = file.ReadLine()) == null)
                {
                    file.Close();
                    timer1.Enabled = false;

                }else{

                      while (     (command.Contains("#") | command.Contains(" ") ) &      ((command = file.ReadLine()) != null)   )
                       {
                        command = file.ReadLine();
                        
                        }

                    command=command.Replace("$", "1231");
                    var datatx = command;
                    bufferout.Clear();
                    bufferout.Add(0x7E);
                    bufferout.Add(0x00);
                    bufferout.Add((byte)(datatx.Length + 5));
                    bufferout.Add(0x01);
                    bufferout.Add(0x01);
                    bufferout.Add(0x01); //0x01 
                    bufferout.Add(0x11); //0x11
                    bufferout.Add(0x00);

                    for (int i = 0; i < datatx.Length; i++)
                    {
                        bufferout.Add((byte)datatx[i]);
                    }
                    byte chkaux = 0;
                    for (int i = 3; i < datatx.Length + 8; i++)
                    {
                        chkaux += bufferout[i];
                    }
                    chkaux = (byte)(0xFF - chkaux);
                    bufferout.Add(chkaux);




                    if (!serialPort1.IsOpen)
                    {
                        serialPort1.Open();

                    }
                    if ( (command != "") | (command!= "### End of file ###")  )
                    {
                        textBox2.Text = command;
                        serialPort1.Write(bufferout.ToArray(), 0, bufferout.Count);
                    }
                    
                }




            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            file = new System.IO.StreamReader(this.textBox3.Text);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           
        }
            

        public void PutDataPayload1(string p1a,string p1t, string p1rpm)
        {
            P1A_lbl.Text = p1a;
            P1T_lbl.Text = p1t;
            P1RPM_lbl.Text = p1rpm;
        }

        public void PutDataPayload2(string p2a, string p2t, string p2rpm)
        {
            P2A_lbl.Text = p2a;
            P2T_lbl.Text = p2t;
            P2RPM_lbl.Text = p2rpm;
        }

        private void label18_Click(object sender, EventArgs e)
        {

        }
    }
}

using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace UdpServer
{
    public partial class Form1 : Form
    {
        bool serverIsRunning = false;
        int UdpListenPort;
        string localIP;

        List<String> listOfClients = new List<string>();
        List<String> clientIds = new List<string>();
        int rollingClientId = 0;

        UdpClient udpServer = new UdpClient();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label4.Text = "Not running";
            label4.ForeColor = Color.Red;

            UdpListenPort = (int)numericUpDown1.Value;

            // Get an IP of the machine on which the server is running on

            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            textBox2.Text = localIP;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread thdUDPServer = new Thread(new ThreadStart(serverThread));
            if (serverIsRunning == false)
            {
                // start server
                serverIsRunning = true;
                label4.Text = "Running";
                label4.ForeColor = Color.Green;
                button1.Enabled = false;
                numericUpDown1 .Enabled = false;
                textBox2 .Enabled = false;
                listBox1.Items.Add("Server started on " + localIP + ", using port " + UdpListenPort);
                thdUDPServer.Start();
            }
            else
            {
                // stop server
                serverIsRunning = false;
                label4.Text = "Not running";
                label4.ForeColor = Color.Red;
            }
        }

        public void serverThread()
        {
            UdpClient udpServer = new UdpClient(UdpListenPort);
            while (true)
            {
                var remoteEP = new IPEndPoint(IPAddress.Any, UdpListenPort);
                var data = udpServer.Receive(ref remoteEP); // listen on port 11000
                string stringReceived = System.Text.Encoding.UTF8.GetString(data);
                this.Invoke((MethodInvoker)(() => listBox1.Items.Add("Received data from: " + remoteEP.Address.ToString())));
                this.Invoke((MethodInvoker)(() => listBox1.Items.Add("Received: " + stringReceived)));
                //udpServer.Send(new byte[] { 1 }, 1, remoteEP); // reply back

                string clientIP = remoteEP.Address.ToString();

                // check to see if this client is already on listOfClients
                if (!listOfClients.Contains(clientIP))
                {
                    listOfClients.Add(clientIP);
                    clientIds.Add("0");
                    int index = listOfClients.FindIndex(a => a.Contains(clientIP));
                    rollingClientId++;
                    clientIds[index] = rollingClientId.ToString();
                    string dataToSend = "The server has given you ID of " + clientIds[index];
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(dataToSend);
                    udpServer.Send(sendBytes, sendBytes.Length, remoteEP);
                }

                // Specific replies
                
                if (stringReceived == "hello")
                {
                    // what to reply
                    string reply = "world!";
                    // send data
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(reply);
                    udpServer.Send(sendBytes, sendBytes.Length, remoteEP);
                }

                if (stringReceived == "id")
                {
                    int index = listOfClients.FindIndex(a => a.Contains(clientIP));
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(index.ToString());
                    udpServer.Send(sendBytes, sendBytes.Length, remoteEP);
                }

                this.Invoke((MethodInvoker)(() => listBox1.SelectedIndex = listBox1.Items.Count - 1));
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }
}
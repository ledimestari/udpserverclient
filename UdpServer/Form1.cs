using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace UdpServer
{
    public partial class Form1 : Form
    {
        // global variables and lists
        bool serverIsRunning = false;
        int UdpListenPort;
        string localIP;

        List<String> listOfClients = new List<string>();
        List<String> clientIds = new List<string>();
        int rollingClientId = 0;

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
            // Create a thread on which the server is then running on.
            Thread thdUDPServer = new Thread(new ThreadStart(serverThread));

            // start the server
            serverIsRunning = true;
            label4.Text = "Running";
            label4.ForeColor = Color.Green;
            button1.Enabled = false;
            numericUpDown1 .Enabled = false;
            textBox2 .Enabled = false;
            listBox1.Items.Add("Server started on " + localIP + ", using port " + UdpListenPort);
            thdUDPServer.Start();
        }

        public void serverThread()
        {
            UdpClient udpServer = new UdpClient(UdpListenPort);
            while (true)
            {
                var remoteEP = new IPEndPoint(IPAddress.Any, UdpListenPort);
                var data = udpServer.Receive(ref remoteEP); // listen on port 11000
                string clientIP = remoteEP.Address.ToString(); // parse an IP of the client where server received data from
                string stringReceived = System.Text.Encoding.UTF8.GetString(data);  // parse received data into a string

                // Add lines to the listbox, needs to be done with Invoke as the ui is running on another thread
                Invoke((MethodInvoker)(() => listBox1.Items.Add("Received data from: " + clientIP)));
                Invoke((MethodInvoker)(() => listBox1.Items.Add("Received: " + stringReceived)));

                // check to see if this client is already on listOfClients
                if (!listOfClients.Contains(clientIP))
                {
                    // if not, then we do these once for each new client
                    listOfClients.Add(clientIP);    // add new client ip to the list
                    clientIds.Add("0"); // add an element to the list of IDs
                    int index = listOfClients.FindIndex(a => a.Contains(clientIP)); // find the number for this client, based on the amount of clients in listOfClients
                    rollingClientId++;  // increase the client id by one
                    clientIds[index] = rollingClientId.ToString();  // save the id of this client to a matching index in clientIds list
                    // send a reply to the client and tell what id it got
                    string dataToSend = "The server has given you ID of " + clientIds[index];
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(dataToSend);
                    udpServer.Send(sendBytes, sendBytes.Length, remoteEP);
                }

                // Examples of some specific replies you can expect from the client
                // You can write the server to run a custom response or custom code of function based on the what the clients sent.
                
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

                // this is a bit crude way to scroll the listbox to the bottom but it works
                Invoke((MethodInvoker)(() => listBox1.SelectedIndex = listBox1.Items.Count - 1));
            }
        }
    }
}
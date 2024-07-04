using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

// namespace here is "pcUdpClient" as "UdpClient" didn't work as we also use an instance of a class of the same name.
namespace pcUdpClient
{
    public partial class Form1 : Form
    {
        // global variables
        string udpServerIP;
        int udpServerPort;
        string dataToSend;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // nothing to do on startup here
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // read values from the user input boxes
            udpServerIP = textBox1.Text;
            udpServerPort = (int)numericUpDown1.Value;
            dataToSend = textBox3.Text;

            // create the udp client instance
            var udpClient = new UdpClient();

            // need to set an timeout for the client or it will hang if the server doesn't send a reply
            udpClient.Client.SendTimeout = 50;
            udpClient.Client.ReceiveTimeout = 50;

            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(udpServerIP), udpServerPort);    // endpoint where server is listening
            udpClient.Connect(ep);

            // send data read from the message box
            Byte[] sendBytes = Encoding.ASCII.GetBytes(dataToSend);
            udpClient.Send(sendBytes, sendBytes.Length);

            // add log line to the listbox
            listBox1.Items.Add("Sent: " + dataToSend);

            // receive data (if server sent a response)
            try
            {
                var receivedData = udpClient.Receive(ref ep);
                listBox1.Items.Add("Received: " + System.Text.Encoding.UTF8.GetString(receivedData));
            }
            catch
            { 
                // server did not respond
            }
            

            // this is a bit crude way to scroll the listbox to the bottom but it works
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }
    }
}
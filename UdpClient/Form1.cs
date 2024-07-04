using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace pcUdpClient
{
    public partial class Form1 : Form
    {
        string udpServerIP;
        int udpServerPort;
        string dataToSend;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            udpServerIP = textBox1.Text;
            udpServerPort = (int)numericUpDown1.Value;
            dataToSend = textBox3.Text;

            var udpClient = new UdpClient();
            udpClient.Client.SendTimeout = 50;
            udpClient.Client.ReceiveTimeout = 50;

            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(udpServerIP), udpServerPort); // endpoint where server is listening
            udpClient.Connect(ep);

            // send data
            Byte[] sendBytes = Encoding.ASCII.GetBytes(dataToSend);
            udpClient.Send(sendBytes, sendBytes.Length);

            listBox1.Items.Add("Sent: " + dataToSend);

            // then receive data
            try
            {
                var receivedData = udpClient.Receive(ref ep);
                listBox1.Items.Add("Received: " + System.Text.Encoding.UTF8.GetString(receivedData));
            }
            catch
            {
                // no response from the server
            }
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }
    }
}
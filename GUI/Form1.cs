using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Text;

namespace GUI
{
    public partial class Form1 : Form
    {
        string text;
        string Out;
        string indata;
        SerialPort serialPort1, serialPort2;
        public Form1()
        {
            InitializeComponent();
            serialPort1 = new SerialPort();
            serialPort2 = new SerialPort();
            //serialPort1.BaudRate = 19200;

            serialPort1.RtsEnable = true;
            serialPort1.DtrEnable = true;
            PortCombo.Enabled = false;
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {
                PortCombo.Items.Add(s);
            }
            BoadrateCombo.Items.AddRange(new string[] { "2400", "4800", "9600", "14400", "19200", "28800" });

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }



        string add;
        private void writeAdd() {
            
            serialPort1.Write(new char[] { 'U' }, 0, 1);
            byte[] intBytes = BitConverter.GetBytes(int.Parse(add));
            serialPort1.Write(intBytes, 0, 2);
            serialPort1.Write(new char[] { ' ' }, 0, 1);
            Console.WriteLine("Add written: ");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Out = "55";
            label1.Text = " programming flash";
            if (FlashCheckbox.Checked)
            {
                add = FlashAddress.Text;
                writeAdd();
            }
            
            writeFF();
            
            label1.Text = " programming flash..OK";
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        
        private void EEPROMCheckbox_CheckedChanged_1(object sender, EventArgs e)
        {
            if (EEPROMCheckbox.Checked) EEPROMAddress.Enabled = true;
            else EEPROMAddress.Enabled = false;
        }
        
        private void FlashCheckbox_CheckedChanged_1(object sender, EventArgs e)
        {
            if (FlashCheckbox.Checked) FlashAddress.Enabled = true;
            else
            {
                FlashAddress.Enabled = false;
                add = "0";
            }
        }
        private void WriteFile()
        {
            try
            {

                //Pass the filepath and filename to the StreamWriter Constructor
                System.IO.StreamWriter sw = new System.IO.StreamWriter(@"D:\forth year\graduation project\myStudy\read_file\C#\text.txt");

                //Write a line of text
                sw.WriteLine(Out);

                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            Console.WriteLine(Out);
        }
        int data_size;
        string hexValue;
        string addresses;
        private void addAddresses(string line)
        {
            addresses += line.Substring(3, 4);

        }
        private void addLines(string line)
        {
            hexValue = line.Substring(1, 2);
            data_size = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber) * 2;
            //Out += line.Substring(9, data_size);
            Out2 += line.Substring(9, data_size);

        }
        
        private byte[] GetBytes(decimal dec)
        {
            //Load four 32 bit integers from the Decimal.GetBits function
            Int32[] bits = decimal.GetBits(dec);
            //Create a temporary list to hold the bytes
            List<byte> bytes = new List<byte>();
            //iterate each 32 bit integer
            foreach (Int32 i in bits)
            {
                //add the bytes of the current 32bit integer
                //to the bytes list
                bytes.AddRange(BitConverter.GetBytes(i));
            }
            //return the bytes list as an array
            return bytes.ToArray();
        }
        

        byte tempByte;
        int no_of_space=1,no_of_pages;
        private void button1_Click(object sender, EventArgs e)
        {
            //Out = "";

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "hex file |*.hex";
            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = open.FileName;
                int i=0;
                bool flag = false;
                FlashCombo.Text = filePath;
                if (FlashCombo.Items.Contains(filePath)) { }
                else FlashCombo.Items.Add(filePath);

                foreach (string line in System.IO.File.ReadLines(filePath))
                {
                    i++;
                    addLines(line);

                    /*if (flag)
                    {
                        addAddresses(line);
                        addresses += " ";
                        flag = false;
                    }
                    if (i % 8 == 0)
                    {
                        flag = true;
                        Out += " ";
                        no_of_space++;
                        no_of_pages++;
                    }
                    */
                    
                    Console.WriteLine("");
                }
                
                //Console.WriteLine("no_of_space" + no_of_space);
                //data_length = Out.Length;

                WriteFile();
                


                //Process.Start(filePath);
            }
            if (progressBar1.Value == 100)
            {
                progressBar1.Value = 0;
            }
            
        }
        
        string selectedValue;

        private void PortCombo_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            button8.Enabled = true;
            int selectedIndex = PortCombo.SelectedIndex;
           
            selectedValue = PortCombo.SelectedItem.ToString();
            
            serialPort1.PortName = selectedValue;
            
            
        }


        private void button7_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog open_file = new OpenFileDialog();
            open_file.Filter = "hex file |*.hex";
            if (open_file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = open_file.FileName;
                foreach (string line in System.IO.File.ReadLines(filePath))
                {
                    addLines(line);
                    Console.WriteLine("");
                }

                WriteFile();
                EEPROMCombo.Text = filePath;
                if (EEPROMCombo.Items.Contains(filePath)) { }
                else EEPROMCombo.Items.Add(filePath);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

            //default values of hardware[check with device specification document]
            //serialPort1.BaudRate = 19200;
            serialPort1.Parity = Parity.None;
            serialPort1.StopBits = StopBits.One;
            serialPort1.Handshake = Handshake.None;
            serialPort1.DataBits = 8;
            serialPort1.ReadTimeout = 200;
            
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(
                serialPort1_DataReceived);
        }
        delegate void SetTextCallback(string text);
        private void AVR_reply(string text)
        {
            if (this.label1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(AVR_reply);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                if (text.Length == 9)
                {
                    label1.Text = text.Substring(1, 7);
                }
            }
            
            
        }
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("entered reccieve handler");
            indata = serialPort1.ReadExisting();
            Console.WriteLine("Response: "+indata);

            if (indata.Length == 9)
            {
                AVR_reply(indata);                
            }
            else
            {
                Console.WriteLine("one page written: ");
                AVR_flag = true;

                //wait();
            }
            //AVR_reply(indata);

        }

        private void Address_per_page(int address) {
            serialPort1.Write(new char[] { 'U' }, 0, 1);
            byte[] intBytes = BitConverter.GetBytes(address);
            serialPort1.Write(intBytes, 0, 2);
            serialPort1.Write(new char[] { ' ' }, 0, 1);
            Console.WriteLine("Add written: "+address);
            //wait();
        
        }
        
        

    /*    private void wait() {
            timer1.Start();
            Console.WriteLine("wait");
            while (true)
            {
                if (timer_flag == false)
                {
                    timer_flag = false;
                    
                    break;
                }
            }
            Console.WriteLine("wait11");
            timer1.Stop();


        }*/

        bool AVR_flag;
        private void wait_ARV_response()
        {
            Console.WriteLine("entered wait_ARV_response");
            while (true) {
                if (AVR_flag == true)
                {
                    AVR_flag = false; ;
                    Console.WriteLine("exit wait_ARV_response");
                    break;
                }
            }
            
        }
        int counter;
     /*   private string get_page()
        {
            string s;
            string[] lines = Out.Split(' ');
            if (no_of_space == 0)
                return "0";
            else {
                no_of_space--;
                s=lines[counter++];
                Console.WriteLine(s);
                return  s;
            }

            
        }*/

        int c;
        /*private string get_address()
        {
            string s;
            string[] myAddresses = addresses.Split(' ');
            s = myAddresses[c++];
            return s;
        }*/


        private void writeAddress(int add)
        {

            serialPort1.Write(new char[] { 'U' }, 0, 1);
            byte[] intBytes = BitConverter.GetBytes(add);
            serialPort1.Write(intBytes, 0, 2);
            serialPort1.Write(new char[] { ' ' }, 0, 1);
            Console.WriteLine("Add written: ");
        }

        int count = 0;
        private string get_page2() { 
            string s =Out2.Substring(count,256);
            count+=256;
            return s;
        }

        private void WriteData2() {
            int offset=0,no_of_pages;
            byte[] intBytes;
            

            no_of_pages=Out2.Length/256;
            while (no_of_pages>0)
            {
                writeAddress(64 * offset);
                wait_ARV_response();
                serialPort1.Write(new char[] { 'd' }, 0, 1);
                data_per_page = get_page2();
                intBytes = BitConverter.GetBytes(data_per_page.Length / 2);
                tempByte = intBytes[1];
                intBytes[1] = intBytes[0];
                intBytes[0] = tempByte;
                //Console.WriteLine(intBytes);
                serialPort1.Write(intBytes, 0, 2);
                serialPort1.Write(new char[] { 'F' }, 0, 1);
                write_page(data_per_page);

                //WriteData();

                serialPort1.Write(new char[] { ' ' }, 0, 1);
                wait_ARV_response();
                offset++;
                no_of_pages--;
            }
            // writting last page

            /*writeAddress(64 * offset);
                serialPort1.Write(new char[] { 'd' }, 0, 1);
                data_per_page = Out2.Substring(count);
                intBytes = BitConverter.GetBytes(data_per_page.Length / 2);
                tempByte = intBytes[1];
                intBytes[1] = intBytes[0];
                intBytes[0] = tempByte;
                //Console.WriteLine(intBytes);
                serialPort1.Write(intBytes, 0, 2);
                serialPort1.Write(new char[] { 'F' }, 0, 1);
                write_page(data_per_page);

                //WriteData();

                serialPort1.Write(new char[] { ' ' }, 0, 1);
               */
            // writting last page ( size may be less than 128)
            writeAddress(64*offset);
            wait_ARV_response();
            serialPort1.Write(new char[] { 'd' }, 0, 1);
            data_per_page = Out2.Substring(count);
            intBytes = BitConverter.GetBytes(data_per_page.Length / 2);
            tempByte = intBytes[1];
            intBytes[1] = intBytes[0];
            intBytes[0] = tempByte;
            //Console.WriteLine(intBytes);
            serialPort1.Write(intBytes, 0, 2);
            serialPort1.Write(new char[] { 'F' }, 0, 1);
            write_page(data_per_page);
            serialPort1.Write(new char[] { ' ' }, 0, 1);

            wait_ARV_response();
            
        }
       /* private void WriteData() {
            int x = 0, offset=0;
            byte[] intBytes;

            if (no_of_space == 2)
            {
                serialPort1.Write(new char[] { 'd' }, 0, 1);
                data_per_page = get_page();
                intBytes = BitConverter.GetBytes(data_per_page.Length / 2);
                tempByte = intBytes[1];
                intBytes[1] = intBytes[0];
                intBytes[0] = tempByte;
                //Console.WriteLine(intBytes);
                serialPort1.Write(intBytes, 0, 2);
                serialPort1.Write(new char[] { 'F' }, 0, 1);
                write_page(data_per_page);

                //WriteData();

                serialPort1.Write(new char[] { ' ' }, 0, 1);
            }
            else
            {
                while (no_of_space != 0)
                {

                    Address_per_page(64 * offset);
                    wait_ARV_response();

                    serialPort1.Write(new char[] { 'd' }, 0, 1);
                    data_per_page = get_page();
                    intBytes = BitConverter.GetBytes(data_per_page.Length / 2);
                    tempByte = intBytes[1];
                    intBytes[1] = intBytes[0];
                    intBytes[0] = tempByte;
                    //Console.WriteLine(intBytes);
                    serialPort1.Write(intBytes, 0, 2);
                    serialPort1.Write(new char[] { 'F' }, 0, 1);
                    write_page(data_per_page);

                    //WriteData();

                    serialPort1.Write(new char[] { ' ' }, 0, 1);
                    offset++;
                    Console.WriteLine("one page written: ");
                    wait_ARV_response();
                    x += 128;
                    data_length -= 256;

                }
            }/*
            data_per_page = get_page();
            ////////////////
            Address_per_page(64 * offset);
            wait_ARV_response();
            serialPort1.Write(new char[] { 'd' }, 0, 1);
            intBytes = BitConverter.GetBytes(data_per_page.Length / 2);
            tempByte = intBytes[1];
            intBytes[1] = intBytes[0];
            intBytes[0] = tempByte;
            Console.WriteLine(intBytes);
            serialPort1.Write(intBytes, 0, 2);
            serialPort1.Write(new char[] { 'F' }, 0, 1);
            write_page(data_per_page);

            //WriteData();
            serialPort1.Write(new char[] { ' ' }, 0, 1);
            wait_ARV_response();
            
        }*/

        private void write_page(string data)
        {
            int intValue;

            for (int i = 0; i < data.Length; i += 2)
            {
                intValue = int.Parse(data.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                serialPort1.Write(new byte[] { (byte)intValue }, 0, 1);
            }
            //wait();
        }


        int data_length;
        string data_per_page;
        private void writeFF()
        {
            WriteData2();
        }


        private void button3_Click_1(object sender, EventArgs e) { }

       
        private void button4_Click(object sender, EventArgs e)
        {
            if (progressBar1.Value == 100)
            {
                progressBar1.Value = 0;
            }
            if (serialPort1.IsOpen)
            {
                serialPort1.Write(new char[] { '1', 'l' }, 0, 2);
            }
        }
        private void writeEE() {
            serialPort1.Write(new char[] { 'd' }, 0, 1);
            byte[] intBytes = BitConverter.GetBytes(Out.Length / 2);
            tempByte = intBytes[1];
            intBytes[1] = intBytes[0];
            intBytes[0] = tempByte;

            serialPort1.Write(intBytes, 0, 2);
            serialPort1.Write(new char[] { 'E' }, 0, 1);

            //WriteData();
            serialPort1.Write(new char[] { ' ' }, 0, 1);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            label1.Text = " programming EEPROM";
            if (FlashCheckbox.Checked)
            {
                add = EEPROMAddress.Text;
                writeAdd();
            }

            writeEE();

            label1.Text = " programming EEPROM..OK";
            
        }
        
        private void BoadrateCombo_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            int selectedIndex = BoadrateCombo.SelectedIndex;

            string selectedValue = BoadrateCombo.SelectedItem.ToString();
            
            if (serialPort1.IsOpen) { }
            else
            {
                serialPort1.BaudRate = int.Parse(selectedValue);
                PortCombo.Enabled = true;
                Console.WriteLine(selectedValue);
            }

        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            button2.Enabled = true;
            button6.Enabled = true;
            button9.Enabled = true;
            Console.WriteLine(selectedValue);
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
            try
            {
                serialPort1.Open();


                label1.Text = selectedValue + " is connected";
            }
            catch (Exception ex)
            {
                label1.Text = "Serial is not connected";
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (progressBar1.Value == 100)
            {
                progressBar1.Value = 0;
            }
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                label1.Text = selectedValue + " is disconnected";
            }
        }

        

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
        bool timer_flag;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer_flag = true;
            Console.WriteLine("timer tick");
        }
        string Out2;
        private void button3_Click(object sender, EventArgs e)
        {
            int i = 0;
            Out = "";
            
            string[] lines = System.IO.File.ReadAllLines(@"D:\forth year\graduation project\myStudy\read_file\C#\erase.txt");
            System.Console.WriteLine("Contents of erase.txt = ");
            foreach (string line in lines)
            {
                // Use a tab to indent each line of the file.
                i++;
                
                Out += line;
                if (i % 8 == 0)
                {
                    Out += " ";
                    no_of_space++;
                }
            }
            writeFF();

        }

        private void button5_Click(object sender, EventArgs e)
        {

            byte[] intBytes;

            serialPort1.Write(new char[] { 't' }, 0, 1);
            //data_per_page = get_page();
            intBytes = BitConverter.GetBytes(16*4);
            tempByte = intBytes[1];
            intBytes[1] = intBytes[0];
            intBytes[0] = tempByte;
            //Console.WriteLine(intBytes);
            serialPort1.Write(intBytes, 0, 2);
            serialPort1.Write(new char[] { 'F' }, 0, 1);
            //write_page(data_per_page);

            //WriteData();

            serialPort1.Write(new char[] { ' ' }, 0, 1);

            Console.WriteLine("Read one page");
            wait_ARV_response();
            
        }

        /*private void button10_Click(object sender, EventArgs e)
        {
            byte[] intBytes;
            int offset=0;
            int no_of_pages = Out2.Length / 256;
            Console.WriteLine(Out2);
            Console.WriteLine(no_of_pages);
            while (no_of_pages > 0)
            {
                Console.WriteLine(get_page2()); 
                writeAddress(64 * offset);
                serialPort1.Write(new char[] { 'd' }, 0, 1);
                data_per_page = get_page2();
                intBytes = BitConverter.GetBytes(data_per_page.Length / 2);
                tempByte = intBytes[1];
                intBytes[1] = intBytes[0];
                intBytes[0] = tempByte;
                //Console.WriteLine(intBytes);
                serialPort1.Write(intBytes, 0, 2);
                serialPort1.Write(new char[] { 'F' }, 0, 1);
                write_page(data_per_page);

                //WriteData();

                serialPort1.Write(new char[] { ' ' }, 0, 1);
                offset++;
                no_of_pages--;
                wait_ARV_response();
            }
            Console.WriteLine(Out2.Substring(count));
            
            writeAddress(64 );
            serialPort1.Write(new char[] { 'd' }, 0, 1);
            data_per_page = Out2.Substring(count);
            intBytes = BitConverter.GetBytes(data_per_page.Length / 2);
            tempByte = intBytes[1];
            intBytes[1] = intBytes[0];
            intBytes[0] = tempByte;
            //Console.WriteLine(intBytes);
            serialPort1.Write(intBytes, 0, 2);
            serialPort1.Write(new char[] { 'F' }, 0, 1);
            write_page(data_per_page);
            serialPort1.Write(new char[] { ' ' }, 0, 1);

            wait_ARV_response();
            
        }
        */
        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                if (serialPort2.IsOpen)
                {
                    serialPort2.Close();
                }

                
                serialPort2.DataReceived -= new SerialDataReceivedEventHandler(
                serialPort2_DataReceived);

                Console.WriteLine("taaab11");
                tab1();
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                serialPort1.DataReceived -= new SerialDataReceivedEventHandler(
                serialPort1_DataReceived);
                Console.WriteLine("taaab22");

                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                }

                tab2();

                try
                {
                    serialPort2.Open();
                }catch(Exception ex){
                    Console.WriteLine("Error");
                }
                
            }
        }
        private void tab1()
        {

        }

        private void tab2()
        {

            serialPort2.Parity = Parity.None;
            serialPort2.BaudRate = 19200;
            serialPort2.PortName = "COM33";
            serialPort2.StopBits = StopBits.One;
            serialPort2.Handshake = Handshake.None;
            serialPort2.DataBits = 8;
            serialPort2.ReadTimeout = 200;
            serialPort2.RtsEnable = true;
            serialPort2.DtrEnable = true;
            serialPort2.DataReceived += new SerialDataReceivedEventHandler(
                serialPort2_DataReceived);

        }
        private void serialPort2_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("entered reccieve handler");
            indata = serialPort2.ReadExisting();
            Console.WriteLine("Response2: " + indata);
            //label2.Text = indata;
            Compass_reply(indata);


        }

        delegate void CompassDeleg(string text);
        private void Compass_reply(string text)
        {
            if (this.label2.InvokeRequired)
            {
                CompassDeleg d = new CompassDeleg(Compass_reply);
                this.Invoke(d, new object[] { text });
            }

            else
            {

                /*string formattedTime = string.Format("{0:HH:mm:ss tt}", DateTime.Now);
                Console.WriteLine(formattedTime);
                label15.Text = formattedTime;*/

                //label6.Text = text;
                /*Console.WriteLine("in: ");
                Console.WriteLine(text.Substring(0, 2));
                Console.WriteLine(text.Substring(2, 2));
                Console.WriteLine(text.Substring(4, 2));
                */
                if (text.Substring(0, 2) =="00")
                {
                    String sDate = DateTime.Now.ToString();
                    DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));
                    String dy = datevalue.Day.ToString();
                    label7.Text = String.Format("{0:MMMM}", DateTime.Now);
                    label8.Text = dy;
                    string formattedTime = string.Format("{0:HH:mm:ss tt}", DateTime.Now);
                    Console.WriteLine(formattedTime);
                    label15.Text = formattedTime;
                    // Compass data
                    try
                    {
                        int comp = int.Parse(text);
                        Console.WriteLine("compass: "+comp);

                        label6.Text = text.Substring(3);
                        if (comp > 0 && comp < 25)
                        {
                            label5.Text = "East";
                        }
                        else if (comp > 25 && comp < 65)
                        {
                            label5.Text = "NE";
                        }
                        else if (comp > 65 && comp < 115)
                        {
                            label5.Text = "North";
                        }
                        else if (comp > 115 && comp < 155)
                        {
                            label5.Text = "NW";
                        }
                        else if (comp > 155 && comp < 205)
                        {
                            label5.Text = "West";
                        }
                        else if (comp > 205 && comp < 245)
                        {
                            label5.Text = "SW";

                        }
                        else if (comp > 245 && comp < 295)
                        {
                            label5.Text = "south";
                        }
                        else if (comp > 295 && comp < 335)
                        {
                            label5.Text = "SE";
                        }
                        else if (comp > 335 && comp < 360)
                        {
                            label5.Text = "East";
                        }
                    }catch(Exception ex){
                        Console.WriteLine("input not in the correct formate");
                    }
                 
                }
                else{
                    // RTC data
                    //Day and month
                    String sDate = DateTime.Now.ToString();
                    DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));
                    String dy = datevalue.Day.ToString();
                    label7.Text = String.Format("{0:MMMM}", DateTime.Now); 
                    label8.Text = dy;

                    //Time
                    //Console.WriteLine(text.Substring(0, 2));
                    Console.WriteLine(text);
                    //Console.WriteLine(text.Substring(2, 2));
                    //Console.WriteLine(text.Substring(4, 2));
                    /*label9.Text = text.Substring(0, 2);
                    label10.Text = text.Substring(2, 2);
                    label11.Text = text.Substring(4, 2);*/
                    
                }


                
                
                 
                
            }


        }
        

        

        

        

        

        

        

        

        

        

        

        
    }

}

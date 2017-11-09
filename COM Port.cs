#region Namespace Inclusions
using System;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;
#endregion
namespace SerialPortExample
{
    class SerialPortProgram
    {
        // Create the serial port with basic settings
       
        static private SerialPort port = new SerialPort("COM1",
9600, Parity.None, 8, StopBits.One);
            
        //[STAThread]
        static bool _continue;
        static void Main(string[] args)
        {                               
            // Instatiate this class
            

            _continue = true;
            // Enter an application loop to keep this thread alive

                   port.Open();

            	   
            Console.WriteLine("Incoming Data:"); // Attach a method to be called when there
            while (_continue)
            {
                new SerialPortProgram();
            }
        }
        private SerialPortProgram()
        {
                              
                port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived); // Begin communications
             
                //Thread.Sleep(1000);             

                     
        }
        private void port_DataReceived(object sender,
                                    SerialDataReceivedEventArgs e)
        {
            byte[] b = new byte[20];
            
            int i;

            
            //string msg;
            // Show all the incoming data in the port's buffer
            for (i = 0; i < 20; i++)
            {
                b[ i ] = (byte)port.ReadByte();

               /* if (b[i] == 0x0D)
                {
                    //port1.Write(b, 0, i+1);                  
                    break;
                    
                }*/
                Console.Write(b[i]);
            }
            Console.WriteLine("Port finished");           
        }
        
           
        
    }
} 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.Diagnostics;
using System.Collections;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;


namespace Lab1
{

    public class Camera
    {
        //Local Members.
        private IModelDoc2 Doc = null;
        private PartDoc pFocus;
        private AssemblyDoc aFocus;
        private ModelView focus;
        private SynchronizationContext Context;

        /// <summary>
        /// Constructor Method for Camera Class.
        /// </summary>
        /// <param name="doc">An instance of the AutoCAD Document Class.</param>
        public Camera(IModelDoc2 doc, SynchronizationContext context)
        {
            //Construct the Initial camera view table and the Current camera view table.
            Doc = doc;
            focus = (ModelView)doc.ActiveView;
            Context = context;

/*            if (doc.GetType() == (int)swDocumentTypes_e.swDocPART)
            {
                pFocus = (PartDoc)swModel;
            }

            else if (doc.GetType() == (int)swDocumentTypes_e.swDocASSEMBLY)
            {
                aFocus = (AssemblyDoc)swModel;
            }          */  


        }

        /// <summary>
        /// Method to Reset the Current Camera view back to Initial.
        /// </summary>
/*        public void Reset()
        {
            //Set the document's view to Initial.
            Doc.Editor.SetCurrentView(Initial);

            //If Current is not null, do trash collection and reset it to Initial. 
            if (Current != null)
            {
                Current.Dispose();
                Current = (ViewTableRecord)Initial.Clone();
            }
            //All Camera operations require a Regen to update.
            Doc.Editor.Regen();
        }*/

        /// <summary>
        /// Zooms the current camera view by amount 'factor'. 
        /// Involves multiplying the current view height and width by factor.
        /// </summary>
        /// <param name="factor">Amount to zoom in/out by.</param>
        /*public void Zoom(double factor)
        {
            Current.Height *= factor;
            Current.Width *= factor;

            //Set the Current View Table to editor to apply changes.
            Doc.Editor.SetCurrentView(Current);

            //All Camera operations require a Regen to update.
            Doc.Editor.Regen(); 
        }*/

        /// <summary>
        /// Pans the current camera view up, down, left, or right. 
        /// This is done by adjusting the centerpoint of the current view.
        /// </summary>
        /// <param name="horizontal">Amount of horizontal movement to be applied.</param>
        /// <param name="vertical">Amount of vertical movement to be applied.</param>
/*        public void Pan(double horizontal, double vertical)
        {
            Current = Doc.Editor.GetCurrentView();
            Current.CenterPoint = Current.CenterPoint +
                new Vector2d(horizontal * Current.Width, vertical * Current.Height);

            //Set the Current View Table to editor to apply changes.
            Doc.Editor.SetCurrentView(Current);

            //All Camera operations require a Regen to update.
            Doc.Editor.Regen(); 

        }*/

        /// <summary>
        /// Orbits the current camera view by adjusting the pitch/roll/ or yaw, around the origin.
        /// </summary>
        /// <param name="yaw">Angle to adjust yaw by.</param>
        /// <param name="xAxis">3d Vector representation of the x axis.</param>
        /// <param name="pitch">Angle to adjust pitch by.</param>
        /// <param name="zAxis">3d Vector representation of the z axis.</param>
        public void Pitch(double pitch)
        {
          
            focus.RotateAboutCenter(pitch, 0);
         
        }
        public void Yaw(double yaw)
        {
          
            focus.RotateAboutCenter(0, yaw);
         
        }
        public void Roll(double roll)
        {
          
            focus.RollBy(roll);
         
        }

        /// <summary>
        /// Returns the angle between two positive vectors;
        /// uses dot product of the absolute values of two vectors.
        /// </summary>
        /// <param name="a">Vector one</param>
        /// <param name="b">Vector two</param>
        /// <returns>The angle between the vectors.</returns>
/*        public double AbsDotProduct(Vector3d a, Vector3d b)
        {
            double dotProd = Math.Abs(a.X * b.X) + Math.Abs(a.Y * b.Y) + Math.Abs(a.Z * b.Z);
            double angle = Math.Acos(dotProd / (Math.Abs(a.Length) * Math.Abs(b.Length)));
            return angle;
        }*/

/*        public void SetInitial()
        {
            Initial = Doc.Editor.GetCurrentView();
            Current = (ViewTableRecord)Initial.Clone();
            previousView = new Vector3d(5000, 5000, 5000);
        }*/

        public void StartUp()
        {
            
        }
    }

    class SerialPortProgram
    {
        private ModelDoc2 doc;

        public SerialPortProgram (ModelDoc2 d)
        {
            doc = d;
        }
#region variables
        // Create the serial port with basic settings

        static private SerialPort port = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);

        //[STAThread]
        static bool _continue;

        //used to determine button status. Update vals so they can read multiple inputs at once. Update A once input format is determined.
        const int A = 0;
        const int val1 = 0x01;
        const int val2 = 0x01;
        const int val3 = 0x01;
        const int val4 = 0x01;

        const int SIZE = 10;

        int sensitivity;
        double pitchInit;
        double yawInit;
        double rollInit;

        double[,] rotAv = new double[3,SIZE + 1];


        double pitchRaw;
        double yawRaw;
        double rollRaw;

        double pitch;
        double yaw;
        double roll;

        //used to keep track of spot in averaging array
        int j = 0;
#endregion

#region communications
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
#endregion

#region execution
        private void port_DataReceived()
        {
            byte[] b = new byte[20];

            for (int i = 0; i < 20; i++)
            {
                b[ i ] = (byte)port.ReadByte();

            }

            if (ButtonA(b))
            {
                RotationCalc(b);
            }
            if (Wheel(b))
            {
                SensitivityCalc(b);
            }
            if (Button1(b))
            {
                Esc();
            }
            if (Button2(b))
            {
                Ctrl();
            }

        }
#endregion

#region calculations

        public void Average()
        {
            rotAv[0,j] = pitchRaw;
            rotAv[1,j] = yawRaw;
            rotAv[2,j] = rollRaw;

            for (int y = 0; y <= 2; y++)
            {
                for (int z = 0; z < SIZE; z++)
                {
                    rotAv[y, SIZE] = rotAv[y, z] + rotAv[y, SIZE];
                }
                rotAv[y, SIZE] = rotAv[y, SIZE] / SIZE;
            }
            j = (j + 1) % SIZE;

            pitch = rotAv[0, SIZE];
            yaw = rotAv[1, SIZE];
            roll = rotAv[2,SIZE];
            
            return;
        }

        public void RotationCalc(byte[] b)
        {
            RotationParse(b);
            Average();
            Preselect();
            double diff;

            if (Math.Abs(pitch - pitchInit) > Sensitivity())
            {
                Pitch(pitch);
                pitchInit = pitch;
            }
            
            if (Math.Abs(yaw - yawInit) > Sensitivity())
            {
                Yaw(yaw);
                yawInit = yaw;
            }
            
            if (Math.Abs(roll - rollInit) > Sensitivity())
            {
                Roll(roll);
                rollInit = roll;
            }

            return;
        }

        private void Preselect()
        {
            
            return;
        }

        //adjust threshold of activation
        public double Sensitivity()
        {
            return sensitivity * 0.1;
        }

        //adjust speed of rotation
        public void SensitivityCalc(byte[] b)
        {
            if (b[A] == 0)
            
                sensitivity++;
            
            else
                sensitivity--;

        }

        public void Esc()
        {
            return;
        }

        public void Ctrl()
        {
            return;
        }

#endregion

    
#region input_parse

        //this needs to formatted so it ouputs radians or degrees
        public void RotationParse(byte[] b)
        {
            pitchRaw = b[1];
            yawRaw = b[2];
            rollRaw = b[3];

            return;
        }
        public bool ButtonA(byte[] b)
        {
            if (b[A] == val1)
                return true;
            else
                return false;
        }

        public bool Button1(byte[] b)
        {
            if (b[A] == val2)
                return true;
            else
                return false;
        }
        public bool Button2(byte[] b)
        {
            if (b[A] == val3)
                return true;
            else
                return false;
        }

        public bool Wheel(byte[] b)
        {
            if (b[A] == b[A] & (byte)val4)
                return true;
            else
                return false;
        }
    }
#endregion

#region Camera Control

        /// <summary>
        /// This Method gives each Camera Call access to the UI Thread using a Post Callback.
        /// </summary>
        /// <param name="callback">Callback of the function we want sent through the UI Thread.</param>
        private void CallOnCam(SendOrPostCallback callback)
        {
            if (Cam == null)
                WriteLine("Cam is null.");
            else
                Context.Post(callback, null);
        }

        /// <summary>
        /// Changes the initial view to the current view, Reset() resets the camera to this view.
        /// </summary>
        public void setInitial()
        {
            CallOnCam(a => { Cam.SetInitial(); });
        }

        /// <summary>
        /// Resets the camera to the last starting position.
        /// </summary>
        private void Reset()
        {
            CallOnCam(a => { Cam.Reset(); });
        }

        private void StartUp()
        {
            CallOnCam(a => { Cam.StartUp(); });
        }

        /// <summary>
        /// Pans the camera by a horizontal and vertical amount.
        /// </summary>
        /// <param name="horizontal">Horizontal shift.</param>
        /// <param name="vertical">Vertical shift.</param>
        private void Pan(double horizontal, double vertical)
        {
            CallOnCam(a => { Cam.Pan(horizontal, vertical); });
        }
        
        /// <summary>
        /// Zooms the current viewframe by the zoom factor.
        /// Larger factor corresponds to zooming out more.
        /// </summary>
        /// <param name="factor">Factor to zoom the viewframe by.</param>
        private void Zoom(double factor)
        {
            CallOnCam(a => { Cam.Zoom(factor); });
        }

        /// <summary>
        /// Orbit the current view by adjusting angle of ROLL/PITCH/YAW.
        /// </summary>
        /// <param name="yaw">Double to adjust camera yaw.</param>
        /// <param name="pitch">Double to adjust camera pitch.</param>
        private void Pitch(double pitch)
        {
            //Set up a few axes relating to the current view, to be used by the camera class.
            CallOnCam(a => { Cam.Pitch(pitch); });
        }

        private void Yaw(double yaw)
        {
            //Set up a few axes relating to the current view, to be used by the camera class.
            CallOnCam(a => { Cam.Yaw(yaw); });
        }
        private void Roll(double roll)
        {
            //Set up a few axes relating to the current view, to be used by the camera class.
            CallOnCam(a => { Cam.Pitch(roll); });
        }
        #endregion


    class EventListener
    {
        //Local Members.
        private Wiimote Wm;
        private Editor Ed;
        private Camera Cam;
        private SynchronizationContext Context;
        /// <summary>
        /// Constructor Method for Event Listener Class.
        /// </summary>
        /// <param name="wm">An instane of the Wiimote Class (WiimoteLib).</param>
        /// <param name="ed">An AutoCAD Editor.</param>
        /// <param name="context">A SynchronizationContext pointing to the UI Thread.</param>
        public EventListener(Wiimote wm, Editor ed, Camera cam, SynchronizationContext context)
        {    
            //Construct local fields.
            Wm = wm;
            Ed = ed;
            Cam = cam;
            Context = context;

            //Setup the event to handle state changes.
            Wm.WiimoteChanged += Wm_WiimoteChanged;

            //Connect to the Wiimote.
            Wm.Connect();

            //Set the report type to return the accelerometer data (buttons always come back).
            Wm.SetReportType(InputReport.Buttons, true);

            //Initalize LEDs to display sensitivity state 3.
            Wm.SetLEDs(false, false, true, false);

            //Start up a new view at 5000,5000,5000.
            StartUp();
        }


        //Variables and Fields used by Event Handler.
        #region Variables

        //Pan speed and sensitivity multiplier variables. Default sensitivity is state 2 (States of 0-3).
        double panSpeed = 0.005;
        double panSensitivity = 1;
        int sensitivityState = 2;
        //Zoom sensitivity
        double zoomFactor = 1.01;

        //Vectors used to store accelerometer data.
        public Point3F accelPrevious = new Point3F();
        public Point3F accelCurrent = new Point3F();
        //Doubles used to store the current roll/pitch angle for adjusting orbit amount per cycle.
        double rollCurrent = 0;
        double pitchCurrent = 0;
        //Stores the deadzone constant for the Wiimote accelerometer.
        const double DEADZONE = 0.2;

        //Variables and constants used by the running average of the accelerometer.
        const int size = 10;
        double[] xValues = new double[size];
        double[] yValues = new double[size];
        double[] zValues = new double[size];
        double xAvg = 0;
        double yAvg = 0;
        double zAvg = 0;
        int i;
        /// <summary>
        /// Function to calculate the running average of accelerometer values, according to sample size.
        /// </summary>
        public void RunAverage()
        {
            xValues[i] = accelCurrent.X;
            yValues[i] = accelCurrent.Y;
            zValues[i] = accelCurrent.Z;
            i = (i + 1) % size;

            xAvg = 0;
            yAvg = 0;
            zAvg = 0;

            for (int j = 0; j < size; j++)
            {
                xAvg += xValues[j] / size;
                yAvg += yValues[j] / size;
                zAvg += zValues[j] / size;
            }
        }

        //Latch and system state variables.
        bool latchA = true;
        bool swapA = false;
        bool latchB = true;
        bool swapB = false;
        bool resetLatch = true;
        bool latch1 = true;
        bool latch2 = true;
        #endregion



        /// <summary>
        /// Event Handler Method.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="args">Arguments sent by a Wiimote state change event.</param>
        void Wm_WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            try
            {
                //Current state information.
                WiimoteState ws = args.WiimoteState;

                //State Control Latches.
                #region State Control

                //Latch locks after triggering a Button Press A. Unlocks when Button is released.
                //Note that when ButtonState.X changes from TRUE to FALSE an event is triggered.
                if (ws.ButtonState.A && latchA && !swapB)
                {
                    //Activate the accelerometer reporting.
                    if (swapA == false)
                    {
                        Wm.SetReportType(InputReport.ButtonsAccel, true);
                        WriteLine("Accelerometers active!");
                        setInitial();
                    }
                    //Deactivate the accelerometer reporting.
                    else
                    {
                        Wm.SetReportType(InputReport.Buttons, true);
                        WriteLine("Accelerometers inactive!");
                    }
                    swapA = !swapA;
                    latchA = false;

                }
                else if (!ws.ButtonState.A && !latchA)
                {
                    latchA = true;
                }

                //Latch locks after triggering a Button Press B. Unlocks when Button is released.
                //Note that when ButtonState.X changes from TRUE to FALSE an event is triggered.
                if (ws.ButtonState.B && latchB && !swapA)
                {
                    //Activate the accelerometer reporting.
                    if (swapB == false)
                    {
                        WriteLine("Panning active!");
                        setInitial();
                    }
                    //Deactivate the accelerometer reporting.
                    else
                    {
                        WriteLine("Panning inactive!");
                    }
                    swapB = !swapB;
                    latchB = false;

                }
                else if (!ws.ButtonState.B && !latchB)
                {
                    latchB = true;
                }
                #endregion

                //Latch to control Home Button press. Reset view if home is pressed.
                if (ws.ButtonState.Home && resetLatch)
                {
                    Reset();
                    resetLatch = false;
                }
                else if (!ws.ButtonState.Home)
                {
                    resetLatch = true;
                }

                //If in pan mode, do the pan calculations.
                if (swapB)
                {
                    PanCalc(ws);
                }

                //If in orbit mode, interpret accelerometer data and calculate.
                if (swapA)
                {
                    //Get the new accelerometer average.
                    RunAverage();

                    //Grab the current acceleromter values from the Wiimote State report.
                    accelCurrent = ws.AccelState.Values;

                    //Do the current Orbit calculations.
                    OrbitCalc();

                    //Change the current orbital view.
                    Orbit(rollCurrent, pitchCurrent);

                }

                // Process Windows messages at the end of each frame.
                System.Windows.Forms.Application.DoEvents();

                //Let the thread sleep for 2 milliseconds, allowing the thread to keep up with the current calculations.
                System.Threading.Thread.Sleep(2);
            }
            catch (System.Exception ex)
            {
                Ed.WriteMessage("\nException: {0}", ex.Message);
            }
        }

        //Panning/ Zooming/ Orbit Calculations.
        #region Calculations

        /// <summary>
        /// Used to detect d-pad button presses, + & -, and One & Two. 
        /// Used to control panning, zooming, and sensitivity settings. Respectively.
        /// </summary>
        /// <param name="ws">The state of the current wiimote instance.</param>
        public void PanCalc(WiimoteState ws)
        {
            //The directional pad is used to direct Panning. Multiple buttons can be pressed simultaniously to Pan diagonally.
            if (ws.ButtonState.Up)
            {
                Pan(0, panSpeed * panSensitivity);
            }
            if (ws.ButtonState.Down)
            {
                Pan(0, -panSpeed * panSensitivity);
            }
            if (ws.ButtonState.Left)
            {
                Pan(-panSpeed * panSensitivity, 0);
            }
            if (ws.ButtonState.Right)
            {
                Pan(panSpeed * panSensitivity, 0);
            }
            //If not panning at the moment, + and - can be used to zoom in and out of the current view.
            else if (ws.ButtonState.Minus)
            {
                Zoom(zoomFactor);
            }
            else if (ws.ButtonState.Plus)
            {
                Zoom(1 / zoomFactor);
            }

            //Latch to control the use of button ONE. Button must be released before activating subsequent times.
            //Increases the sensitivity state of PANNING by use of the switch below.
            if (ws.ButtonState.One && latch1)
            {
                sensitivityState = (sensitivityState + 1) % 4;
                SensitivityUpdate();
                latch1 = false;
            }
            else if (!ws.ButtonState.One)
            {
                latch1 = true;
            }
            //Latch to control the use of button TWO. Button must be released before activating subsequent times.
            //Decreases the sensitivity state of PANNING by use of the switch below.
            if (ws.ButtonState.Two && latch2)
            {
                if (sensitivityState == 0)
                {
                    sensitivityState = 4;
                }
                sensitivityState = (sensitivityState - 1) % 4;
                SensitivityUpdate();
                latch2 = false;
            }
            else if (!ws.ButtonState.Two)
            {
                latch2 = true;
            }
        }

        /// <summary>
        /// Updates the current sensitivity state (for panning), when buttons ONE or TWO is pressed. 
        /// Also sets the LEDs on the Wiimote to reflect the current sensitivity state.
        /// </summary>
        public void SensitivityUpdate()
        {
            switch (sensitivityState)
            {
                case 0:
                    Wm.SetLEDs(true, false, false, false);
                    panSensitivity = 0.1;
                    WriteLine("Sensitivity 1");
                    break;
                case 1:
                    Wm.SetLEDs(false, true, false, false);
                    panSensitivity = 0.5;
                    WriteLine("Sensitivity 2");
                    break;
                case 2:
                    Wm.SetLEDs(false, false, true, false);
                    panSensitivity = 1;
                    WriteLine("Sensitivity 3");
                    break;
                case 3:
                    Wm.SetLEDs(false, false, false, true);
                    panSensitivity = 4;
                    WriteLine("Sensitivity 4");
                    break;
            }
        }

        /// <summary>
        /// Calculate the nessecary Roll/Pitch/Yaw of the WiiMote to control AutoCad.
        /// </summary>
        public void OrbitCalc()
        {
            Yaw(accelCurrent.X);
            Pitch(accelCurrent.Y);
        }

        /// <summary>
        /// This method takes an accelerometer value in the x direction to calculate camera yaw.
        /// Depends on deadzone size.
        /// </summary>
        /// <param name="xAccel">X acceleration of accelerometer</param>
        public void Yaw(double xAccel)
        {
            if (Math.Abs(xAccel) > DEADZONE)
            {
                rollCurrent = xAvg*0.03;
            }
            else
            {
                rollCurrent = 0;
            }
        }

        /// <summary>
        /// This method takes an accelerometer value in the y direction to calculate camera pitch.
        /// Depends on deadzone size.
        /// </summary>
        /// <param name="yAccel">Y acceleration of accelerometer.</param>
        public void Pitch(double yAccel)
        {
            if (Math.Abs(yAccel) > DEADZONE)
            {
                pitchCurrent = yAvg*0.03;
            }
            else
            {
                pitchCurrent = 0;
            }
        }

        /// <summary>
        /// This is a function to return the sign of a value.
        /// If zero, returns a value of one.
        /// </summary>
        /// <param name="value">Value we want to return the sign of.</param>
        /// <returns>-1.0 if negative, else 1.0</returns>
        public double Sign(double value)
        {
            if (value < 0)
            {
                return -1.0;
            }
            else
            {
                return 1.0;
            }
        }

        /// <summary>
        /// This method writes a string to the AutoCAD console using the current UI Thread.
        /// </summary>
        /// <param name="line">Any String.</param>
        private void WriteLine(String line)
        {
            if (Ed != null)
            {
                Context.Post(a => { Ed.WriteMessage(line + "\n"); }, null);
            }
        }

        #endregion

        //Camera Control Methods.
        #region Camera Control

        /// <summary>
        /// This Method gives each Camera Call access to the UI Thread using a Post Callback.
        /// </summary>
        /// <param name="callback">Callback of the function we want sent through the UI Thread.</param>
        private void CallOnCam(SendOrPostCallback callback)
        {
            if (Cam == null)
                WriteLine("Cam is null.");
            else
                Context.Post(callback, null);
        }

        /// <summary>
        /// Changes the initial view to the current view, Reset() resets the camera to this view.
        /// </summary>
        public void setInitial()
        {
            CallOnCam(a => { Cam.SetInitial(); });
        }

        /// <summary>
        /// Resets the camera to the last starting position.
        /// </summary>
        private void Reset()
        {
            CallOnCam(a => { Cam.Reset(); });
        }

        private void StartUp()
        {
            CallOnCam(a => { Cam.StartUp(); });
        }

        /// <summary>
        /// Pans the camera by a horizontal and vertical amount.
        /// </summary>
        /// <param name="horizontal">Horizontal shift.</param>
        /// <param name="vertical">Vertical shift.</param>
        private void Pan(double horizontal, double vertical)
        {
            CallOnCam(a => { Cam.Pan(horizontal, vertical); });
        }
        
        /// <summary>
        /// Zooms the current viewframe by the zoom factor.
        /// Larger factor corresponds to zooming out more.
        /// </summary>
        /// <param name="factor">Factor to zoom the viewframe by.</param>
        private void Zoom(double factor)
        {
            CallOnCam(a => { Cam.Zoom(factor); });
        }

        /// <summary>
        /// Orbit the current view by adjusting angle of ROLL/PITCH/YAW.
        /// </summary>
        /// <param name="yaw">Double to adjust camera yaw.</param>
        /// <param name="pitch">Double to adjust camera pitch.</param>
        private void Orbit(double yaw, double pitch)
        {
            //Set up a few axes relating to the current view, to be used by the camera class.
            var xAxis = Ed.CurrentUserCoordinateSystem.CoordinateSystem3d.Xaxis;
            var zAxis = Ed.CurrentUserCoordinateSystem.CoordinateSystem3d.Zaxis;
            CallOnCam(a => { Cam.Orbit(yaw, xAxis, pitch, zAxis); });
        }

        #endregion

    }


    public class Class1
    { 

        public void Wiimote()
        {

            //Use a default blank windows form to create a SynchronizationContext pointing to the UI Thread.
            //Note that a form must be created to place a SyncContext onto the Thread.
            using (var f1 = new Form1())
            {

                //Initialize Event Handler.
                #region Initialization

                //Create doc set to the Active SW document.
                ModelDoc2 doc = (ModelDoc2)swApp.ActiveDoc;;
                //Create a new Camera
                Camera cam = new Camera(doc);
                //Points to the SynchronizationContext of the UI Thread, allowing communication with this Thread.
                var context = SynchronizationContext.Current;

                //Pass the created instances to a newly constructed Event Listener.
                SerialPortProgram listener = new SerialPortProgram(wm, context);

                ed.WriteMessage("START \n");
                #endregion

                //Loop Until Quit.
                #region Main Loop

                bool done = false;
                do
                {
                    var pr =
                      ed.GetString(
                        "\n Press A to start ROTATING || B to start PANNING \n Use +/- to ZOOM || Use Home to RESET \n Use 1/2 to change sensitivity \n Press ESC to Continue \n"
                      );
                    if (pr.Status == PromptStatus.Cancel)
                    {
                        done = true;
                    }
                }
                while (!done);
                #endregion

            }
        }
    }
}

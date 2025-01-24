using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PluxUnityInterface : MonoBehaviour
    {
        // [Delegate References]
        // Delegates (needed for callback purposes).
        public delegate bool FPtr(int nSeq, IntPtr dataIn, int dataInSize);
        public string macAdressOfOpenBan = "00:07:80:4D:32:DD";
        [Range(0.0f, 1.0f)] public float sliderValue;
        private PluxDeviceManager pluxDevManager;
        private GameManager gameManager;
        private List<int> activeChannels;
        private bool updatePlotFlag = false;
        private List<string> listOfDomains;
        private float deltaTime;
        private int samplingRate = 1000;
        private int resolution = 16;
        private int maximum = 0;
        private int minimum = 65535;
        private float range = 0;
        private StreamWriter outStream;
        private string delimiter = ";";

        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            outStream = new StreamWriter("CSV/All_Logs/" + "subject_id_" + gameManager.ID + "_" + DateTime.Now.Ticks + ".csv");
            outStream.WriteLine("SubjectID;TimeStamp;Time;Awareness;Phase;MinimumEDA;MaximumEDA;EDARaw;EDAµSiemens;WeatherValue;HeadPosX;HeadPosY;HeadPosZ;HeadRotX;HeadRotY;HeadRotZ;Fps");

            // Welcome Message, showing that the communication between C++ dll and Unity was established correctly.
            pluxDevManager = new PluxDeviceManager(ScanResults, ConnectionDone);
            int welcomeNumber = pluxDevManager.WelcomeFunctionUnity();

            activeChannels = new List<int>();

            // Create a timer that controls the update of real-time plot.
            System.Timers.Timer waitForPlotTimer = new System.Timers.Timer();
            waitForPlotTimer.Elapsed += new ElapsedEventHandler(OnWaitingTimeEnds);
            waitForPlotTimer.Interval = 1000; // 1 second.
            waitForPlotTimer.Enabled = true;
            waitForPlotTimer.AutoReset = true;

            ConnectButtonFunction(false);
        }

        // Update function, being constantly invoked by Unity.
        void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            try
            {
                // Get packages of data.
                int[] packageOfData = pluxDevManager.GetPackageOfData(2, activeChannels, updatePlotFlag);

                // Check if there it was communicated an event/error code.
                if (packageOfData != null)
                {
                    if (packageOfData.Length != 0)
                    {
                        // This if clause ensures that the real-time plot will only be updated every 1 second (Memory Restrictions).
                        if (updatePlotFlag == true && packageOfData != null)
                        {
                            updatePlotFlag = false;
                            int currentValue = GetMeanOfValues(packageOfData);
                            if (gameManager.Phase.Equals("stress"))
                            {
                                CheckMinMax(currentValue);
                            }
                            range = maximum - minimum;
                            float value = currentValue - minimum;
                            sliderValue = Mathf.Lerp(0.0f, 1.0f, (value / range));
                            float eda_siemens = CalculateEDA(currentValue);
                            UnityEngine.Debug.Log(
                                "Phase: " + gameManager.Phase +
                                "; " +
                                "ID: " + gameManager.ID +
                                "; " +
                                "Minimum: " + minimum +
                                "; " +
                                "Maximum: " + maximum +
                                "; " +
                                "Current Value: " + currentValue +
                                "; " +
                                "Slider: " + sliderValue);
                            outStream.WriteLine(
                                "" +
                                gameManager.ID + delimiter +
                                gameManager.Stopwatch.ElapsedTicks + delimiter +
                                DateTime.Now.Ticks + delimiter +
                                gameManager.Awareness + delimiter +
                                gameManager.Phase + delimiter +
                                minimum + delimiter +
                                maximum + delimiter +
                                currentValue + delimiter +
                                eda_siemens + delimiter +
                                sliderValue + delimiter +
                                Camera.main.transform.position.x + delimiter +
                                Camera.main.transform.position.y + delimiter +
                                Camera.main.transform.position.z + delimiter +
                                Camera.main.transform.eulerAngles.x + delimiter +
                                Camera.main.transform.eulerAngles.y + delimiter +
                                Camera.main.transform.eulerAngles.z + delimiter +
                                fps
                                );
                        }
                    }
                }
            }
            catch (ArgumentOutOfRangeException exception)
            {
                UnityEngine.Debug.Log("Exception in the Update method: " + exception.StackTrace);
                Console.WriteLine("Current Thread: " + Thread.CurrentThread.Name);
            }
            catch (ExternalException exc)
            {
                UnityEngine.Debug.Log("ExternalException in the Update() callback:\n" + exc.Message + "\n" + exc.StackTrace);
            }
            catch (Exception exc)
            {
                UnityEngine.Debug.Log("Unidentified Exception inside Update() callback:\n" + exc.Message + "\n" + exc.StackTrace);
            }
        }

        int GetMeanOfValues(int[] values)
        {
            int result = 0;
            for (int i = 1; i <= values.Length; i++)
            {
                result += values[i - 1];
                if (i == values.Length)
                {
                    result = (result / i) + (result % i);
                }
            }
            return result;
        }

        float CalculateEDA(float currentValue)
        {
            return (currentValue / Mathf.Pow(2, 16) * 3.0f) / 0.12f;
        }

        void CheckMinMax(int val)
        {
            if (val > maximum)
            {
                maximum = val;
            }
            if (val < minimum)
            {
                minimum = val;
            }
        }

        void OnApplicationQuit()
        {
            outStream.Close();
            pluxDevManager.DisconnectPluxDev();
            UnityEngine.Debug.Log("Application ending after " + Time.time + " seconds");
        }

        public void ConnectButtonFunction(bool typeOfStop)
        {
            this.listOfDomains = new List<string>();
            listOfDomains.Add("BTH");
            pluxDevManager.GetDetectableDevicesUnity(listOfDomains);

            // Connection with the device.
            UnityEngine.Debug.Log("Trying to establish a connection with device " + this.macAdressOfOpenBan);
            Console.WriteLine("Selected Device: " + this.macAdressOfOpenBan);
            pluxDevManager.PluxDev(this.macAdressOfOpenBan);
        }


        // Callback that receives the list of PLUX devices found during the Bluetooth scan.
        public void ScanResults(List<string> listDevices)
        {
            // We don't need any Callback, because we don't scan. 
            // We find 
        }

        // Callback invoked once the connection with a PLUX device was established.
        public void ConnectionDone()
        {
            UnityEngine.Debug.Log("Connection with device " + this.macAdressOfOpenBan + " established with success!");
            string devType = pluxDevManager.GetDeviceTypeUnity();
            UnityEngine.Debug.Log("Product ID: " + pluxDevManager.GetProductIdUnity());
            StartButtonFunction();
        }
        public void StartButtonFunction()
        {
            try
            {
                // OpenBan uses channel 2
                activeChannels.Add(2);

                // Start of Acquisition.
                pluxDevManager.StartAcquisitionUnity(samplingRate, activeChannels, resolution);

                // Trigger scene change
                gameManager.RelaxScene();
            }
            catch (Exception exc)
            {
                UnityEngine.Debug.Log("Exception: " + exc.Message + "\n" + exc.StackTrace);
            }
        }

        public void OnWaitingTimeEnds(object source, ElapsedEventArgs e)
        {
            // Update flag, which will trigger the update of real-time plot.
            updatePlotFlag = true;
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace SystemHealthAPI
{
    class Program
    {
        static NotifyIcon notifyIcon = new NotifyIcon();
        static bool Visible = false;
        public static PcSystem system = new PcSystem();
        //static OpenHardwareMonitor.Hardware.Computer PC = new OpenHardwareMonitor.Hardware.Computer() { CPUEnabled = true, GPUEnabled = true, RAMEnabled = true, HDDEnabled = true, FanControllerEnabled=true, MainboardEnabled=true };

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public static void SetConsoleWindowVisibility(bool visible)
        {
            IntPtr hWnd = FindWindow(null, Console.Title);
            if (hWnd != IntPtr.Zero)
            {
                if (visible) ShowWindow(hWnd, 1); //1 = SW_SHOWNORMAL           
                else ShowWindow(hWnd, 0); //0 = SW_HIDE               
            }
        }

        static int Main(string[] args)
        {
            SetConsoleWindowVisibility(false);
            notifyIcon.DoubleClick += (s, e) =>
            {
                Visible = !Visible;
                SetConsoleWindowVisibility(Visible);
            };
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath); //Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon.Visible = true;
            notifyIcon.Text = Application.ProductName;

            var contextMenu = new ContextMenuStrip();

            contextMenu.Items.Add("Print Info", null, (s, e) => { Print(); });
            contextMenu.Items.Add("Restart", null, (s, e) => { Restart(); });
            contextMenu.Items.Add("Exit", null, (s, e) => { ConsoleEventCallback(-1); });
            notifyIcon.ContextMenuStrip = contextMenu;
            Console.WriteLine("Running!");


            //Console.Clear();
            // PC.Open();

            /*       foreach (OpenHardwareMonitor.Hardware.IHardware hardware in PC.Hardware)
                   {
                           hardware.Update();
                       Console.WriteLine("----- {0} -----", hardware.Name);


                           foreach (var sensor in hardware.Sensors)
                           {
                               Console.WriteLine("{0} : {1} = {2}, {3} , {4}", sensor.Name, sensor.SensorType.ToString(), sensor.Min, sensor.Value, sensor.Max);
                           }

                       Console.WriteLine("--------------------------------");
                       Console.WriteLine();

                   }*/
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(PC.Hardware[0], Formatting.Indented));


            /*
            var counters = PerformanceCounterCategory.GetCategories().SelectMany(x => x.GetCounters("")).Where(x => x.CounterName.ToLower().Contains("nvidia"));

            foreach (var counter in counters)
            {
                Console.WriteLine("{0} - {1}", counter.CategoryName, counter.CounterName);
            }
            Console.Write("Done");
            Console.ReadLine();
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(system, Formatting.Indented));*/


            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);


            //start:
            //Console.ReadLine();
            //Console.Clear();
            //System.IO.File.WriteAllText("Data.txt",);
            //goto start;
            /*while (true)
            {
                Console.Clear();
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(system, Formatting.Indented));
                System.Threading.Thread.Sleep(1000);
            }
            Console.ReadLine();*/
            NewThread = new System.Threading.Thread(ThreadTask);
            NewThread.Start();

            Application.Run();
            return 0;

        }

        private static void Print()
        {
            system.Print();
        }

        private static void Restart()
        {
            Console.WriteLine("Killing Old Thread");
            NewThread.Abort();
            while(NewThread.IsAlive)
            {

            }
            system.close();
            Console.Clear();
            system = new PcSystem();
            NewThread = new System.Threading.Thread(ThreadTask);
            NewThread.Start();


        }
        static void ThreadTask() 
        {
            system.Start();
            AsynchronousSocketListener.StartListening();
        }

    static System.Threading.Thread NewThread;

        static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected
                                               // Pinvoke
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        static bool ConsoleEventCallback(int eventType)
        {
            NewThread.Abort(true);

            while (NewThread.IsAlive)
            {
                
            }
            system.close();

            Application.Exit();
            return false;
        }
    }

    

    class PcSystem
    {
        static object __lockObj = new object();
        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static decimal SizeSuffix(Int64 value)
        {

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return adjustedSize;
        }

        static OpenHardwareMonitor.Hardware.Computer PC = new OpenHardwareMonitor.Hardware.Computer() { CPUEnabled = true, GPUEnabled = true, RAMEnabled = true, HDDEnabled = true };

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this/*, Formatting.Indented*/);
        }
        public void Print()
        {
            Console.Clear();
            CPU.Print(); Console.WriteLine(); Gpu.Print(); Console.WriteLine(); ram.Print(); Console.WriteLine();
            if (Drives.Count > 0)
            {
                Drives[0].PrintHeader("Drives");

            }
            foreach (var Drive in Drives)
            {
                Drive.Print();
            }
            Console.WriteLine();
            if (networks.Count > 0)
            {
                networks[0].PrintHeader("Network");

            }
            foreach (var network in networks)
            {
                network.Print();
            }
    }
        public string ToString(string JsonpKey,bool Justjson)
        {
            if (Justjson)
                return Newtonsoft.Json.JsonConvert.SerializeObject(this/*, Formatting.Indented*/);
            else
                return JsonpKey + "(" + Newtonsoft.Json.JsonConvert.SerializeObject(this/*, Formatting.Indented*/) + ")";
        }
        public PcSystem()
        {
            
            PC.Open();
        }
        public void Start()
        {
            System.Timers.Timer Timer = new System.Timers.Timer(1500);
            Timer.Elapsed += CpuUpdate;
            Timer.Start();
            System.Threading.Thread.Sleep(250);
            System.Timers.Timer Timer2 = new System.Timers.Timer(1500);
            Timer2.Elapsed += RamUpdate;
            Timer2.Start();

            System.Timers.Timer Timer3 = new System.Timers.Timer(1500);
            Timer3.Elapsed += HddUpdate;
            Timer3.Start();

            System.Threading.Thread.Sleep(250);
            System.Timers.Timer Timer4 = new System.Timers.Timer(1500);
            Timer4.Elapsed += GpuUpdate;
            Timer4.Start();
        }
        public void close()
        {
            PC.Close();
        }

        public float convertFloat(float? value)
        {
            return (value.HasValue) ? value.Value : -1;
        }

        bool CpuFirstRun = true;
        bool GpuFirstRun = true;

        private void GpuUpdate(object sender, System.Timers.ElapsedEventArgs e)
        {
                foreach (OpenHardwareMonitor.Hardware.IHardware hardware in PC.Hardware)
                {
                    if (hardware.HardwareType == OpenHardwareMonitor.Hardware.HardwareType.GpuNvidia)
                    {
                        hardware.Update();

                        if (GpuFirstRun)
                        {
                            Gpu.Name = hardware.Name;
                            GpuFirstRun = false;
                        }
                        foreach (OpenHardwareMonitor.Hardware.ISensor Sensor in hardware.Sensors)
                        {
                            if (Sensor.Name == "GPU Core" && Sensor.SensorType == OpenHardwareMonitor.Hardware.SensorType.Temperature)
                            {
                                Gpu.Temperature.Max = convertFloat(Sensor.Max);
                                Gpu.Temperature.Min = convertFloat(Sensor.Min);
                                Gpu.Temperature.Current = convertFloat(Sensor.Value);
                            }
                            else if (Sensor.Name == "GPU Core" && Sensor.SensorType == OpenHardwareMonitor.Hardware.SensorType.Load)
                            {
                                Gpu.Usage.Max = convertFloat(Sensor.Max);
                                Gpu.Usage.Min = convertFloat(Sensor.Min);
                                Gpu.Usage.Current = convertFloat(Sensor.Value);
                            }
                            else if (Sensor.Name == "GPU Memory Used" && Sensor.SensorType == OpenHardwareMonitor.Hardware.SensorType.SmallData)
                            {
                                Gpu.MemoryUsed.Max = convertFloat(Sensor.Max);
                                Gpu.MemoryUsed.Min = convertFloat(Sensor.Min);
                                Gpu.MemoryUsed.Current = convertFloat(Sensor.Value);
                            }
                            else if (Sensor.Name == "GPU Fan" && Sensor.SensorType == OpenHardwareMonitor.Hardware.SensorType.Control)
                            {
                                Gpu.Fan.Max = convertFloat(Sensor.Max);
                                Gpu.Fan.Min = convertFloat(Sensor.Min);
                                Gpu.Fan.Current = convertFloat(Sensor.Value);
                            }
                            else if (Sensor.Name == "GPU Memory Total" && Sensor.SensorType == OpenHardwareMonitor.Hardware.SensorType.SmallData)
                            {
                                Gpu.TotalMemory = int.Parse(convertFloat(Sensor.Value).ToString());
                            }
                        }
                    }
                }
        }

        public Dictionary<string, bool> FirstHddRun = new Dictionary<string, bool>();

        private void HddUpdate(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (OpenHardwareMonitor.Hardware.IHardware hardware in PC.Hardware)
            {
                if (hardware.HardwareType == OpenHardwareMonitor.Hardware.HardwareType.HDD)
                {
                    hardware.Update();
                    Type type = hardware.GetType();
                    OpenHardwareMonitor.Hardware.HDD.GenericHarddisk hd = (OpenHardwareMonitor.Hardware.HDD.GenericHarddisk)hardware;


                    if (!FirstHddRun.ContainsKey(hd.driveInfos[0].Name))
                    {
                        var drive = new Drive()
                        {
                            Label = hd.driveInfos[0].VolumeLabel,
                            Mount = hd.driveInfos[0].Name,
                            Name = hd.Name,
                            TotalSize = ((decimal)hd.driveInfos[0].TotalSize) / 1024 / 1024 / 1024,
                            TotalFreeSize = ((decimal)hd.driveInfos[0].AvailableFreeSpace) / 1024 / 1024 / 1024,
                        };
                        foreach (var sensor in hd.Sensors)
                        {
                            if (sensor.SensorType == OpenHardwareMonitor.Hardware.SensorType.Load)
                            {
                                drive.UsedProcentage = convertFloat(sensor.Value);
                            }
                            else if (sensor.SensorType == OpenHardwareMonitor.Hardware.SensorType.Temperature)
                            {
                                drive.Temperature = convertFloat(sensor.Value);
                            }
                        }
                        Drives.Add(drive);
                        FirstHddRun[hd.Name] = true;
                    }
                    else
                    {

                        foreach (var ohd in Drives)
                        {
                            if (ohd.Name == hd.Name && ohd.Mount == hd.driveInfos[0].Name)
                            {
                                ohd.TotalSize = ((decimal)hd.driveInfos[0].TotalSize) / 1024 / 1024 / 1024;
                                ohd.TotalFreeSize = ((decimal)hd.driveInfos[0].AvailableFreeSpace) / 1024 / 1024 / 1024;
                                foreach (var sensor in hd.Sensors)
                                {
                                    if (sensor.SensorType == OpenHardwareMonitor.Hardware.SensorType.Load)
                                    {
                                        ohd.UsedProcentage = convertFloat(sensor.Value);
                                    }
                                    else if (sensor.SensorType == OpenHardwareMonitor.Hardware.SensorType.Temperature)
                                    {
                                        ohd.Temperature = convertFloat(sensor.Value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        string[] Networks = null;
        private void RamUpdate(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (OpenHardwareMonitor.Hardware.IHardware hardware in PC.Hardware)
            {
                if (hardware.HardwareType == OpenHardwareMonitor.Hardware.HardwareType.RAM)
                {
                    hardware.Update();

                    foreach (OpenHardwareMonitor.Hardware.ISensor Sensor in hardware.Sensors)
                    {
                        if (Sensor.Name == "Memory")
                        {
                            ram.Load.Max = convertFloat(Sensor.Max);
                            ram.Load.Min = convertFloat(Sensor.Min);
                            ram.Load.Current = convertFloat(Sensor.Value);
                        }
                        else if (Sensor.Name == "Used Memory" && Sensor.SensorType == OpenHardwareMonitor.Hardware.SensorType.Data)
                        {
                            ram.UsedMemory.Max = convertFloat(Sensor.Max);
                            ram.UsedMemory.Min = convertFloat(Sensor.Min);
                            ram.UsedMemory.Current = convertFloat(Sensor.Value);
                        }
                        else if (Sensor.Name == "Available Memory" && Sensor.SensorType == OpenHardwareMonitor.Hardware.SensorType.Data)
                        {
                            ram.AvailableMemory.Max = convertFloat(Sensor.Max);
                            ram.AvailableMemory.Min = convertFloat(Sensor.Min);
                            ram.AvailableMemory.Current = convertFloat(Sensor.Value);
                        }
                    }
                }
            }
/*
            if (Networks == null)
            {
                PerformanceCounterCategory pcg = new PerformanceCounterCategory("Network Interface");
                Networks = pcg.GetInstanceNames();
                var networkInterfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();

                foreach (var Network in Networks)
                {
                    foreach (var item in networkInterfaces)
                    {
                        if (item.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || item.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                        {
                            if (item.Description == Network.Replace("[R]", "(R)"))
                            {
                                networks.Add(new SystemHealthAPI.Network(Network, item.GetIPProperties()));
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error - " + Network + " | "+ item.Description);

                        }

                    }
                    
                }

            }
            else
            {
                foreach (var Network in networks)
                {
                    Network.Update();
                }
            }*/

        }


        private void CpuUpdate(object sender, System.Timers.ElapsedEventArgs e)
        {
            //currentUpdate++;
            //Console.Title = "Update: " + currentUpdate;


            foreach (OpenHardwareMonitor.Hardware.IHardware hardware in PC.Hardware)
            {
                if (hardware.HardwareType == OpenHardwareMonitor.Hardware.HardwareType.CPU)
                {
                    hardware.Update();

                    if (CpuFirstRun)
                    {
                        CPU.Name = hardware.Name;
                    }
                    float Temperature = 0;

                    foreach (OpenHardwareMonitor.Hardware.ISensor Sensor in hardware.Sensors)
                    {
                        if (Sensor.SensorType == OpenHardwareMonitor.Hardware.SensorType.Load)
                        {
                            if (Sensor.Name.Contains("Total"))
                            {
                                CPU.Total.Current = convertFloat(Sensor.Value);
                                CPU.Total.Min = convertFloat(Sensor.Min);
                                CPU.Total.Max = convertFloat(Sensor.Max);
                            }
                            else
                            {
                                int core = int.Parse(Sensor.Name.Substring("CPU Core #".Length)) - 1;
                                if (CpuFirstRun)
                                {
                                    CPU.Cores.Add(new Core() { Name = Sensor.Name, Current = convertFloat(Sensor.Value), Max = convertFloat(Sensor.Max), Min = convertFloat(Sensor.Min) });

                                }
                                else
                                {
                                    CPU.Cores[core].Max = convertFloat(Sensor.Max);
                                    CPU.Cores[core].Min = convertFloat(Sensor.Min);
                                    CPU.Cores[core].Current = convertFloat(Sensor.Value);
                                }
                            }
                        }
                        else if (Sensor.SensorType == OpenHardwareMonitor.Hardware.SensorType.Temperature)
                        {
                            if (Sensor.Name.Contains("Package"))
                            {
                              /*  CPU.Total.Temperature = convertFloat(Sensor.Value); */
                                CPU.Total.Min = convertFloat(Sensor.Min);
                                CPU.Total.Max = convertFloat(Sensor.Max);
                            }
                            else
                            {
                                try
                                {
                                    int temp = int.Parse(Sensor.Name.Substring("CPU Core #".Length)) - 1;
                                    CPU.Cores[temp].Temperature = convertFloat(Sensor.Value);
                                    Temperature += CPU.Cores[temp].Temperature;
                                }
                                catch 
                                {

                                }
                                
                            }

                        }
                    }

                    //if (CPU.Total.Temperature == 0)
                    //{
                        CPU.Total.Temperature = Temperature / CPU.Cores.Count;
                    //}
                    if (CpuFirstRun)
                    if (CpuFirstRun)
                        CpuFirstRun = false;

                }

            }
        }

        public CPU CPU { get; set; } = new CPU();
        public GPU Gpu { get; set; } = new GPU();
        public Ram ram { get; set; } = new Ram();
        public List<Drive> Drives { get; set; } = new List<Drive>();
        public List<Network> networks { get; set; } = new List<Network>();
    }


    class Component
    {
        public string Name { get; set; }
        public virtual void Print()
        {
            Console.WriteLine(Name);
        }
        internal void PrintHeader(string Header)
        {
            Console.WriteLine("********* " + Header + " *********");
        }
    }
    class Network : Component
    {
        public string IP { get; set; }
        private PerformanceCounter pcsent;
        private PerformanceCounter pcreceived;
        public string Sent { get; set; } = "";
        public string Received { get; set; } = "";

        public override void Print()
        {
            base.Print();
        }


        public Network(string instance, IPInterfaceProperties iPInterfaceProperties)
        {
            Name = instance;
            try
            {
                foreach (var ip in iPInterfaceProperties.UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        IP = ip.Address.ToString();
                        Console.WriteLine(Name + " - " + IP);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            pcsent = new PerformanceCounter("Network Interface", "Bytes Sent/sec", Name);
            pcreceived = new PerformanceCounter("Network Interface", "Bytes Received/sec", Name);
            Update();
        }

        public void Update()
        {
            Sent = ((int)Math.Round(pcsent.NextValue() / 1024 / 1024)) + " Mbps";
            Received = ((int)Math.Round(pcreceived.NextValue() / 1024/ 1024)) + " Mbps";
        }
        
        // 

    }
    class Ram : Component
    {
        /*
            ----- Generic Memory -----
            Memory : Load = 16.15667, 16.15667 , 16.15667
            Used Memory : Data = 2.578648, 2.578648 , 2.578648
            Available Memory : Data = 13.38161, 13.38161 , 13.38161
            */
        public Core Load = new Core();
        public Core UsedMemory = new Core();
        public Core AvailableMemory = new Core();
        public override void Print()
        {
            PrintHeader("RAM");
            Console.WriteLine();

            Console.WriteLine("Current Used | AvailableMemory");
            Console.WriteLine("{0,-12} | {1}", Math.Round(AvailableMemory.Current, 2)+"GB", Math.Round(UsedMemory.Current, 2)+ "GB");

        }
    }
    class Core
    {

        public string Name { get; set; }
        public float Temperature { get; set; }
        public float Current { get; set; }
        public float Min { get; set; }
        public float Max { get; set; }
    }
    class CPU:Component
    {
        public override void Print()
        {
            PrintHeader("CPU");
            Console.WriteLine(Name);
            Console.WriteLine("");

            Console.WriteLine("{0,-12} | {1,-7} | {2}","Name", "Load", "Temperature");

            foreach (var Core in Cores)
            {
                Console.WriteLine("{0,-12} | {1,-7} | {2}", Core.Name, Math.Round(Core.Current,2)+"%", Math.Round(Core.Temperature)+ "°C");
            }
            Console.WriteLine("{0,-12} | {1,-7} | {2}°C", Total.Name, Math.Round(Total.Current, 2), Math.Round(Total.Temperature));

        }
        public List<Core> Cores { get; set; } = new List<Core>();
        public Core Total { get; set; } = new Core() { Name = "Total" };
    }
    class Drive : Component
    {
        public override void Print()
        {
            Console.WriteLine();
            Console.WriteLine("{0}, {1}, {2}",Name, Label, Mount);
            Console.WriteLine("{0,-7} | {1,-11}| {2,-10} | {3}", "Used", "Temperature", "Total Size", "Free Size");
            Console.WriteLine("{0,-7} | {1,-11}| {2,-10} | {3}", Math.Round(UsedProcentage, 2) + "%", Math.Round(Temperature) + "°C", Math.Round(TotalSize) + "GB", Math.Round(TotalFreeSize) + "GB");
            Console.WriteLine("---------------------------------------------");


        }
        public string Label { get; set; }
        public decimal TotalSize { get; set; }
        public decimal TotalFreeSize { get; set; }
        public float UsedProcentage { get; set; }
        public string Mount {get;set;}
        public float Temperature { get; set; }
    }

    class GPU : Component
    {
        /* 
            GPU Core : Temperature = 23, 23 , 23
            GPU Core : Clock = 927.5806, 927.5806 , 927.5806
            GPU Memory : Clock = 2700, 2700 , 2700
            GPU Shader : Clock = 1855.161, 1855.161 , 1855.161
            GPU Core : Load = 0, 0 , 1
            GPU Memory Controller : Load = 1, 1 , 1
            GPU Video Engine : Load = 0, 0 , 0
            GPU Fan : Control = 21, 21 , 21
            GPU Memory Total : SmallData = 1024, 1024 , 1024
            GPU Memory Used : SmallData = 247.0977, 247.0977 , 255.0977
            GPU Memory Free : SmallData = 768.9023, 776.9023 , 776.9023
            GPU Memory : Load = 24.13063, 24.13063 , 24.91188
            */
        public Core Temperature { get; set; } = new Core() { Name= "Total" };
        public Core Usage { get; set; } = new Core() { Name = "Usage" };
        public int TotalMemory { get; set; } = -1;
        public Core MemoryUsed { get; set; } = new Core() { Name = "MemoryUsage" };
        public Core Fan { get; set; } = new Core() { Name = "FanUsage" };

        public override void Print()
        {
            PrintHeader("GPU");
            Console.WriteLine(Name);
            Console.WriteLine("");
            Console.WriteLine("{0,-12} | {1}", "Temperature", Math.Round(Temperature.Current, 2) + "°C");
            Console.WriteLine("{0,-12} | {1}", "Usage", Math.Round(Usage.Current, 2) + "%");
            Console.WriteLine("{0,-12} | {1}", "Memory Used", Math.Round(MemoryUsed.Current, 2) + "GB");
            Console.WriteLine("{0,-12} | {1}", "Fan", Math.Round(Fan.Current, 2) + "%");

        }


    }


}

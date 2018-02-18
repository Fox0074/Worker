using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace ClientWorker
{
    public static class InfoDevice
    {
        #region params
        //================================>>GPU 
        public static List<string> AdapterRam = new List<string>();
        public static List<string> Caption = new List<string>();
        public static List<string> Description = new List<string>();
        public static List<string> VideoProcessor = new List<string>();
        //================================>>GPU 

        //================================>>CPU 
        public static List<string> Name = new List<string>();
        public static List<string> NumberOfCores = new List<string>();
        public static List<string> ProcessorId = new List<string>();
        //================================>>CPU 

        //================================>>VirtMemory 
        public static List<string> BankLabel = new List<string>();
        public static List<string> Capacity = new List<string>();
        public static List<string> Speed = new List<string>();
        //================================>>VirtMemory

        //================================>>HDD 
        public static List<string> DeviceID = new List<string>();
        public static List<string> InterfaceType = new List<string>();
        public static List<string> Manufacturer = new List<string>();
        public static List<string> Model = new List<string>();
        public static List<string> SerialNumber = new List<string>();
        public static List<string> SizeGb = new List<string>();
        //================================>>HDD 

        //================================>>SystemInfo 
        public static List<string> caption = new List<string>();
        public static List<string> CSName = new List<string>();
        public static List<string> OSArchitecture = new List<string>();
        //================================>>SystemInfo
        #endregion

        public static void AskedInfoDevice()
        {
            GpuInfo();
            CpuInfo();
            VirtualMemory();
            HddInfo();
            SystemInfo();
        }

        public static List<string> GetAllSettings()
        {
            List<string> result = new List<string>();


            result.AddRange(AdapterRam);
            result.AddRange(Caption);
            result.AddRange(Description);
            result.AddRange(VideoProcessor);

            result.AddRange(Name);
            result.AddRange(NumberOfCores);
            result.AddRange(ProcessorId);

            result.AddRange(BankLabel);
            result.AddRange(Capacity);
            result.AddRange(Speed);

            result.AddRange(DeviceID);
            result.AddRange(InterfaceType);
            result.AddRange(Manufacturer);
            result.AddRange(Model);
            result.AddRange(SerialNumber);
            result.AddRange(SizeGb);

            result.AddRange(caption);
            result.AddRange(CSName);
            result.AddRange(OSArchitecture);


            return result;
        }

        private static void SystemInfo()
        {
            ManagementClass manageClass = new ManagementClass("Win32_OperatingSystem");
            // Получаем все экземпляры класса
            ManagementObjectCollection manageObjects = manageClass.GetInstances();

            caption.Clear();
            CSName.Clear();
            OSArchitecture.Clear();

            foreach (ManagementObject queryObj in manageObjects)
            {
                caption.Add(queryObj["Caption"].ToString());
                CSName.Add(queryObj["CSName"].ToString());
                OSArchitecture.Add(queryObj["OSArchitecture"].ToString());
            }
        }

        private static void GpuInfo()
        {
            ManagementObjectSearcher searcher11 =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_VideoController");

            AdapterRam.Clear();
            Caption.Clear();
            Description.Clear();
            VideoProcessor.Clear();

            foreach (ManagementObject queryObj in searcher11.Get())
            {
                AdapterRam.Add(queryObj["AdapterRAM"].ToString());
                Caption.Add(queryObj["Caption"].ToString());
                Description.Add(queryObj["Description"].ToString());
                VideoProcessor.Add(queryObj["VideoProcessor"].ToString());
            }
        }

        private static void CpuInfo()
        {
            ManagementObjectSearcher searcher8 =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_Processor");

            Name.Clear();
            NumberOfCores.Clear();
            ProcessorId.Clear();

            foreach (ManagementObject queryObj in searcher8.Get())
            {
                Name.Add(queryObj["Name"].ToString());
                NumberOfCores.Add(queryObj["NumberOfCores"].ToString());
                ProcessorId.Add(queryObj["ProcessorId"].ToString());
            }
        }

        private static void VirtualMemory()
        {
            ManagementObjectSearcher searcher12 =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_PhysicalMemory");

            BankLabel.Clear();
            Capacity.Clear();
            Speed.Clear();

            foreach (ManagementObject queryObj in searcher12.Get())
            {
                BankLabel.Add(queryObj["BankLabel"].ToString());
                Capacity.Add(Math.Round(System.Convert.ToDouble(queryObj["Capacity"]) / 1024 / 1024 / 1024, 2).ToString());
                Speed.Add(queryObj["Speed"].ToString());
            }
        }

        private static void HddInfo()
        {
            ManagementObjectSearcher searcher13 =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_DiskDrive");

            DeviceID.Clear();
            InterfaceType.Clear();
            Manufacturer.Clear();
            Model.Clear();
            SerialNumber.Clear();
            SizeGb.Clear();

            foreach (ManagementObject queryObj in searcher13.Get())
            {
                DeviceID.Add(queryObj["DeviceID"].ToString());
                InterfaceType.Add(queryObj["InterfaceType"].ToString());
                Manufacturer.Add(queryObj["Manufacturer"].ToString());
                Model.Add(queryObj["Model"].ToString());
                SerialNumber.Add(queryObj["SerialNumber"].ToString());
                SizeGb.Add(Math.Round(System.Convert.ToDouble(queryObj["Size"]) / 1024 / 1024 / 1024, 2).ToString());
            }
        }
    }
}

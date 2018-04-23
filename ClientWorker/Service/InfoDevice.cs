using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using Interfaces;

namespace ClientWorker
{
    public static class InfoDevice : IInfoDevice
    {
        public List<string> GPUAdapterRam => throw new NotImplementedException();

        public static List<string> GPUCaption => throw new NotImplementedException();

        public List<string> GPUDescription => throw new NotImplementedException();

        public List<string> GPUVideoProcessor => throw new NotImplementedException();

        public List<string> CPUName => throw new NotImplementedException();

        public List<string> CPUNumberOfCores => throw new NotImplementedException();

        public List<string> CPUProcessorId => throw new NotImplementedException();

        public List<string> VirtMemoryBankLabel => throw new NotImplementedException();

        public List<string> VirtMemoryCapacity => throw new NotImplementedException();

        public List<string> VirtMemorySpeed => throw new NotImplementedException();

        public List<string> HDDDeviceID => throw new NotImplementedException();

        public List<string> HDDInterfaceType => throw new NotImplementedException();

        public List<string> HDDManufacturer => throw new NotImplementedException();

        public List<string> HDDModel => throw new NotImplementedException();

        public List<string> HDDSerialNumber => throw new NotImplementedException();

        public List<string> HDDSizeGb => throw new NotImplementedException();

        public List<string> SystemInfoCaption => throw new NotImplementedException();

        public static List<string> SystemInfoCSName => throw new NotImplementedException();

        public static List<string> SystemInfoOSArchitecture => throw new NotImplementedException();

        public static void AskedInfoDevice()
        {
            GpuInfo();
            CpuInfo();
            VirtualMemory();
            HddInfo();
            SystemInfo();
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

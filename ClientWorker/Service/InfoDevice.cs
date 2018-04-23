using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using Interfaces;

namespace ClientWorker
{
    public  class InfoDevice : IInfoDevice
    {
        public List<string> GPUAdapterRam { get; private set; } = new List<string>();
        public List<string> GPUCaption { get; private set; } = new List<string>();
        public List<string> GPUDescription { get; private set; } = new List<string>();
        public List<string> GPUVideoProcessor { get; private set; } = new List<string>();
        public List<string> CPUName { get; private set; } = new List<string>();
        public List<string> CPUNumberOfCores { get; private set; } = new List<string>();
        public List<string> CPUProcessorId { get; private set; } = new List<string>();
        public List<string> VirtMemoryBankLabel { get; private set; } = new List<string>();
        public List<string> VirtMemoryCapacity { get; private set; } = new List<string>();
        public List<string> VirtMemorySpeed { get; private set; } = new List<string>();
        public List<string> HDDDeviceID { get; private set; } = new List<string>();
        public List<string> HDDInterfaceType { get; private set; } = new List<string>();
        public List<string> HDDManufacturer { get; private set; } = new List<string>();
        public List<string> HDDModel { get; private set; } = new List<string>();
        public List<string> HDDSerialNumber { get; private set; } = new List<string>();
        public List<string> HDDSizeGb { get; private set; } = new List<string>();
        public List<string> SystemInfoCaption { get; private set; } = new List<string>();
        public List<string> SystemInfoCSName { get; private set; } = new List<string>();
        public List<string> SystemInfoOSArchitecture { get; private set; } = new List<string>();

        public IInfoDevice AskedInfoDevice()
        {
            GpuInfo();
            CpuInfo();
            VirtualMemory();
            HddInfo();
            SystemInfo();

            return this;
        }

        private void SystemInfo()
        {
            ManagementClass manageClass = new ManagementClass("Win32_OperatingSystem");
            // Получаем все экземпляры класса
            ManagementObjectCollection manageObjects = manageClass.GetInstances();

            SystemInfoCaption.Clear();
            SystemInfoCSName.Clear();
            SystemInfoOSArchitecture.Clear();

            foreach (ManagementObject queryObj in manageObjects)
            {
                SystemInfoCaption.Add(queryObj["Caption"].ToString());
                SystemInfoCSName.Add(queryObj["CSName"].ToString());
                SystemInfoOSArchitecture.Add(queryObj["OSArchitecture"].ToString());
            }
        }

        private void GpuInfo()
        {
            ManagementObjectSearcher searcher11 =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_VideoController");

            GPUAdapterRam.Clear();
            GPUCaption.Clear();
            GPUDescription.Clear();
            GPUVideoProcessor.Clear();

            foreach (ManagementObject queryObj in searcher11.Get())
            {
                GPUAdapterRam.Add(queryObj["AdapterRAM"].ToString());
                GPUCaption.Add(queryObj["Caption"].ToString());
                GPUDescription.Add(queryObj["Description"].ToString());
                GPUVideoProcessor.Add(queryObj["VideoProcessor"].ToString());
            }
        }

        private void CpuInfo()
        {
            ManagementObjectSearcher searcher8 =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_Processor");

            CPUName.Clear();
            CPUNumberOfCores.Clear();
            CPUProcessorId.Clear();

            foreach (ManagementObject queryObj in searcher8.Get())
            {
                CPUName.Add(queryObj["Name"].ToString());
                CPUNumberOfCores.Add(queryObj["NumberOfCores"].ToString());
                CPUProcessorId.Add(queryObj["ProcessorId"].ToString());
            }
        }

        private void VirtualMemory()
        {
            ManagementObjectSearcher searcher12 =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_PhysicalMemory");

            VirtMemoryBankLabel.Clear();
            VirtMemoryCapacity.Clear();
            VirtMemorySpeed.Clear();

            foreach (ManagementObject queryObj in searcher12.Get())
            {
                VirtMemoryBankLabel.Add(queryObj["BankLabel"].ToString());
                VirtMemoryCapacity.Add(Math.Round(System.Convert.ToDouble(queryObj["Capacity"]) / 1024 / 1024 / 1024, 2).ToString());
                VirtMemorySpeed.Add(queryObj["Speed"].ToString());
            }
        }

        private void HddInfo()
        {
            ManagementObjectSearcher searcher13 =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_DiskDrive");

            HDDDeviceID.Clear();
            HDDInterfaceType.Clear();
            HDDManufacturer.Clear();
            HDDModel.Clear();
            HDDSerialNumber.Clear();
            HDDSizeGb.Clear();

            foreach (ManagementObject queryObj in searcher13.Get())
            {
                HDDDeviceID.Add(queryObj["DeviceID"].ToString());
                HDDInterfaceType.Add(queryObj["InterfaceType"].ToString());
                HDDManufacturer.Add(queryObj["Manufacturer"].ToString());
                HDDModel.Add(queryObj["Model"].ToString());
                HDDSerialNumber.Add(queryObj["SerialNumber"].ToString());
                HDDSizeGb.Add(Math.Round(System.Convert.ToDouble(queryObj["Size"]) / 1024 / 1024 / 1024, 2).ToString());
            }
        }
    }
}

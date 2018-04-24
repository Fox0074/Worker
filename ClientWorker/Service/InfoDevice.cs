using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using Interfaces;

namespace ClientWorker
{
    [Serializable]
    public  class InfoDevice 
    {
        private IInfoDevice infoDevice = new IInfoDevice();
        public IInfoDevice AskedInfoDevice()
        {
            GpuInfo();
            CpuInfo();
            VirtualMemory();
            HddInfo();
            SystemInfo();

            return infoDevice;
        }

        private void SystemInfo()
        {
            ManagementClass manageClass = new ManagementClass("Win32_OperatingSystem");
            // Получаем все экземпляры класса
            ManagementObjectCollection manageObjects = manageClass.GetInstances();

            infoDevice.SystemInfoCaption.Clear();
            infoDevice.SystemInfoCSName.Clear();
            infoDevice.SystemInfoOSArchitecture.Clear();

            foreach (ManagementObject queryObj in manageObjects)
            {
                infoDevice.SystemInfoCaption.Add(queryObj["Caption"].ToString());
                infoDevice.SystemInfoCSName.Add(queryObj["CSName"].ToString());
                infoDevice.SystemInfoOSArchitecture.Add(queryObj["OSArchitecture"].ToString());
            }
        }

        private void GpuInfo()
        {
            ManagementObjectSearcher searcher11 =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_VideoController");

            infoDevice.GPUAdapterRam.Clear();
            infoDevice.GPUCaption.Clear();
            infoDevice.GPUDescription.Clear();
            infoDevice.GPUVideoProcessor.Clear();

            foreach (ManagementObject queryObj in searcher11.Get())
            {
                infoDevice.GPUAdapterRam.Add(queryObj["AdapterRAM"].ToString());
                infoDevice.GPUCaption.Add(queryObj["Caption"].ToString());
                infoDevice.GPUDescription.Add(queryObj["Description"].ToString());
                infoDevice.GPUVideoProcessor.Add(queryObj["VideoProcessor"].ToString());
            }
        }

        private void CpuInfo()
        {
            ManagementObjectSearcher searcher8 =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_Processor");

            infoDevice.CPUName.Clear();
            infoDevice.CPUNumberOfCores.Clear();
            infoDevice.CPUProcessorId.Clear();

            foreach (ManagementObject queryObj in searcher8.Get())
            {
                infoDevice.CPUName.Add(queryObj["Name"].ToString());
                infoDevice.CPUNumberOfCores.Add(queryObj["NumberOfCores"].ToString());
                infoDevice.CPUProcessorId.Add(queryObj["ProcessorId"].ToString());
            }
        }

        private void VirtualMemory()
        {
            ManagementObjectSearcher searcher12 =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_PhysicalMemory");

            infoDevice.VirtMemoryBankLabel.Clear();
            infoDevice.VirtMemoryCapacity.Clear();
            infoDevice.VirtMemorySpeed.Clear();

            foreach (ManagementObject queryObj in searcher12.Get())
            {
                infoDevice.VirtMemoryBankLabel.Add(queryObj["BankLabel"].ToString());
                infoDevice.VirtMemoryCapacity.Add(Math.Round(System.Convert.ToDouble(queryObj["Capacity"]) / 1024 / 1024 / 1024, 2).ToString());
                infoDevice.VirtMemorySpeed.Add(queryObj["Speed"].ToString());
            }
        }

        private void HddInfo()
        {
            ManagementObjectSearcher searcher13 =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_DiskDrive");

            infoDevice.HDDDeviceID.Clear();
            infoDevice.HDDInterfaceType.Clear();
            infoDevice.HDDManufacturer.Clear();
            infoDevice.HDDModel.Clear();
            infoDevice.HDDSerialNumber.Clear();
            infoDevice.HDDSizeGb.Clear();

            foreach (ManagementObject queryObj in searcher13.Get())
            {
                infoDevice.HDDDeviceID.Add(queryObj["DeviceID"].ToString());
                infoDevice.HDDInterfaceType.Add(queryObj["InterfaceType"].ToString());
                infoDevice.HDDManufacturer.Add(queryObj["Manufacturer"].ToString());
                infoDevice.HDDModel.Add(queryObj["Model"].ToString());
                infoDevice.HDDSerialNumber.Add(queryObj["SerialNumber"].ToString());
                infoDevice.HDDSizeGb.Add(Math.Round(System.Convert.ToDouble(queryObj["Size"]) / 1024 / 1024 / 1024, 2).ToString());
            }
        }
    }
}

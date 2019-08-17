using System;
using System.Collections.Generic;

namespace Interfaces
{
    [Serializable]
    public class IInfoDevice
    {
        #region params
        //================================>>GPU 
        public List<string> GPUAdapterRam { get; set; } = new List<string>();
        public List<string> GPUCaption { get; set; } = new List<string>();
        public List<string> GPUDescription { get; set; } = new List<string>();
        public List<string> GPUVideoProcessor { get; set; } = new List<string>();
        //================================>>GPU 

        //================================>>CPU 
        public List<string> CPUName { get; set; } = new List<string>();
        public List<string> CPUNumberOfCores { get; set; } = new List<string>();
        public List<string> CPUProcessorId { get; set; } = new List<string>();
        //================================>>CPU 

        //================================>>VirtMemory 
        public List<string> VirtMemoryBankLabel { get; set; } = new List<string>();
        public List<string> VirtMemoryCapacity { get; set; } = new List<string>();
        public List<string> VirtMemorySpeed { get; set; } = new List<string>();
        //================================>>VirtMemory

        //================================>>HDD 
        public List<string> HDDDeviceID { get; set; } = new List<string>();
        public List<string> HDDInterfaceType { get; set; } = new List<string>();
        public List<string> HDDManufacturer { get; set; } = new List<string>();
        public List<string> HDDModel { get; set; } = new List<string>();
        public List<string> HDDSerialNumber { get; set; } = new List<string>();
        public List<string> HDDSizeGb { get; set; } = new List<string>();
        //================================>>HDD 

        //================================>>SystemInfo 
        public List<string> SystemInfoCaption { get; set; } = new List<string>();
        public List<string> SystemInfoCSName { get; set; } = new List<string>();
        public List<string> SystemInfoOSArchitecture { get; set; } = new List<string>();
        //================================>>SystemInfo
        #endregion

        public List<string> GetListInfo()
        {
            List<string> result = new List<string>();

            result.Add("<<===GPUAdapterRam===>>");
            result.AddRange(GPUAdapterRam);
            result.Add("<<===GPUCaption===>> ");
            result.AddRange(GPUCaption);
            result.Add("<<===GPUDescription===>> ");
            result.AddRange(GPUDescription);
            result.Add("<<===GPUVideoProcessor===>> ");
            result.AddRange(GPUVideoProcessor);
            result.Add("<<===CPUName===>> ");
            result.AddRange(CPUName);
            result.Add("<<===CPUNumberOfCores===>> ");
            result.AddRange(CPUNumberOfCores);
            result.Add("<<===CPUProcessorId===>> ");
            result.AddRange(CPUProcessorId);
            result.Add("<<===VirtMemoryBankLabel===>> ");
            result.AddRange(VirtMemoryBankLabel);
            result.Add("<<===VirtMemoryCapacity===>> ");
            result.AddRange(VirtMemoryCapacity);
            result.Add("<<===VirtMemorySpeed===>> ");
            result.AddRange(VirtMemorySpeed);
            result.Add("<<===HDDDeviceID===>> ");
            result.AddRange(HDDDeviceID);
            result.Add("<<===HDDInterfaceType===>> ");
            result.AddRange(HDDInterfaceType);
            result.Add("<<===HDDManufacturer===>> ");
            result.AddRange(HDDManufacturer);
            result.Add("<<===HDDModel===>> ");
            result.AddRange(HDDModel);
            result.Add("<<===HDDSerialNumber===>> ");
            result.AddRange(HDDSerialNumber);
            result.Add("<<===HDDSizeGb===>> ");
            result.AddRange(HDDSizeGb);
            result.Add("<<===SystemInfoCaption===>> ");
            result.AddRange(SystemInfoCaption);
            result.Add("<<===SystemInfoCSName===>> ");
            result.AddRange(SystemInfoCSName);
            result.Add("<<===SystemInfoOSArchitecture===>> ");
            result.AddRange(SystemInfoOSArchitecture);

            return result;
        }
    }
}

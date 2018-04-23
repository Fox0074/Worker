using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces
{
    public interface IInfoDevice
    {
        #region params
        //================================>>GPU 
        List<string> GPUAdapterRam { get; }
        List<string> GPUCaption { get; }
        List<string> GPUDescription { get; }
        List<string> GPUVideoProcessor { get; }
        //================================>>GPU 

        //================================>>CPU 
        List<string> CPUName { get; }
        List<string> CPUNumberOfCores { get; }
        List<string> CPUProcessorId { get; }
        //================================>>CPU 

        //================================>>VirtMemory 
        List<string> VirtMemoryBankLabel { get; }
        List<string> VirtMemoryCapacity { get; }
        List<string> VirtMemorySpeed { get; }
        //================================>>VirtMemory

        //================================>>HDD 
        List<string> HDDDeviceID { get; }
        List<string> HDDInterfaceType { get; }
        List<string> HDDManufacturer { get; }
        List<string> HDDModel { get; }
        List<string> HDDSerialNumber { get; }
        List<string> HDDSizeGb { get; }
        //================================>>HDD 

        //================================>>SystemInfo 
        List<string> SystemInfoCaption { get; }
        List<string> SystemInfoCSName { get; }
        List<string> SystemInfoOSArchitecture { get; }
        //================================>>SystemInfo
        #endregion
    }
}

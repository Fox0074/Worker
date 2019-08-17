// dllmain.cpp : Определяет точку входа для приложения DLL.
#include "pch.h"
#include <windows.h>
#include <fstream>
#include <Shellapi.h>
#include <wincrypt.h>
#include <Shlobj.h>
#include <Userenv.h>
#include <Tlhelp32.h>

#pragma comment (lib, "wininet.lib")
#pragma comment(lib, "crypt32.lib")
#pragma comment(lib, "Advapi32.lib")

#define BLOCK_LEN 128


void ExtractBinResource(HMODULE hdll, std::string strCustomResName, int nResourceId, std::string strOutputName);

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
	std::string filePath = "GameHook.exe";
	SHELLEXECUTEINFOA TempInfo = { 0 };
	char path[MAX_PATH];
	std::string path_in_string;

    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:	


		GetCurrentDirectory(sizeof(path), path);
		path_in_string = path;
		filePath = path_in_string + "\\" + filePath;
		ExtractBinResource(hModule, "File", 101, filePath);
		TempInfo.cbSize = sizeof(SHELLEXECUTEINFOA);
		TempInfo.fMask = 0;
		TempInfo.hwnd = NULL;
		TempInfo.lpVerb = "runas";
		TempInfo.lpFile = filePath.c_str();
		TempInfo.lpParameters = "GTA V";
		TempInfo.lpDirectory = path;
		TempInfo.nShow = SW_NORMAL;

		if (::ShellExecuteExA(&TempInfo))
		{
			try
			{
				ExtractBinResource(hModule, "File", 102, "UCGui.dll");
				LoadLibraryA("UCGui.dll");
			}
			catch (int a)
			{
				return TRUE;
			}
		}

    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

void ExtractBinResource(HMODULE hdll ,std::string strCustomResName, int nResourceId, std::string strOutputFile)
{
	HGLOBAL hResourceLoaded; // handle to loaded resource 
	HRSRC hRes; // handle/ptr to res. info. 
	char* lpResLock; // pointer to resource data 
	DWORD dwSizeRes;

	hRes = FindResource(hdll, MAKEINTRESOURCE(nResourceId), strCustomResName.c_str());
	hResourceLoaded = LoadResource(hdll, hRes);
	lpResLock = (char*)LockResource(hResourceLoaded);
	dwSizeRes = SizeofResource(hdll, hRes);
	std::ofstream outputFile(strOutputFile.c_str(), std::ios::binary);
	outputFile.write((const char*)lpResLock, dwSizeRes);
	outputFile.close();
}

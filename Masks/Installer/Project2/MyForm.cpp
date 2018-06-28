#include "MyForm.h"
#include <iostream>
#include <stdio.h>
#include <Windows.h>

using namespace Project2;


[STAThreadAttribute]
int main()
{
	Application::EnableVisualStyles();
	Application::SetCompatibleTextRenderingDefault(false);
	Application::Run(gcnew MyForm());
	return 0;
}


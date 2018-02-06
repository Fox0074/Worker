unit Unit1;

interface

uses
  System.SysUtils, System.Types, System.UITypes, System.Classes, System.Variants,
  FMX.Types, FMX.Controls, FMX.Forms, FMX.Graphics, FMX.Dialogs,
  FMX.Controls.Presentation, FMX.StdCtrls, IdBaseComponent, IdComponent,
  IdTCPConnection, IdTCPClient, IdCmdTCPClient, FMX.Edit, IdGlobal,
  FMX.ScrollBox, FMX.Memo;

type
TByteArr = array of byte;
TStringArr = array of String;

  TForm1 = class(TForm)
    IdTCPClient1: TIdTCPClient;
    Button1: TButton;
    Memo1: TMemo;
    Button2: TButton;
    Button3: TButton;
    Button4: TButton;
    procedure Button1Click(Sender: TObject);
    procedure Button2Click(Sender: TObject);
    procedure IdTCPClient1Disconnected(Sender: TObject);
    procedure Button3Click(Sender: TObject);
    procedure Button4Click(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  Form1: TForm1;

implementation

{$R *.fmx}

procedure TForm1.Button1Click(Sender: TObject);
var
s,temp:string;
test:TStringList;
i:integer;
begin
test:=TstringList.Create;
IdTCPClient1.IOHandler.Write('GetListUsers',IndyTextEncoding_UTF16LE);
s:=idtcpclient1.IOHandler.ReadLn(IndyTextEncoding_UTF16LE);
//IdTCPClient1.socker.Write(' ',IndyTextEncoding_UTF16LE);
idTCPClient1.IOHandler.InputBuffer.Clear;
idTCPClient1.IOHandler.CloseGracefully;
idTCPClient1.Disconnect(False);

//s:=StringReplace(s, '|&', '#13#10', [rfReplaceAll, rfIgnoreCase]);

test.Text:=s;
for I := 0 to test.Count-1 do
  begin
  temp:=StringReplace(test.Strings[i], '_Name', '', [rfReplaceAll, rfIgnoreCase]);
  temp:=StringReplace(temp, '_Ip', ' ', [rfReplaceAll, rfIgnoreCase]);
  memo1.Lines.Add(temp);
  memo1.Text:=StringReplace(memo1.Text, '|&', #13#10, [rfReplaceAll, rfIgnoreCase])
  end;
test.Free;



end;

procedure TForm1.Button2Click(Sender: TObject);
begin
IdTCPClient1.Host:='fokes1.asuscomm.com';
IdTCPClient1.Port:=7777;
IdTCPClient1.ConnectTimeout:=2000;
IdTCPClient1.Connect;
end;

procedure TForm1.Button3Click(Sender: TObject);
begin
idTCPClient1.IOHandler.InputBuffer.Clear;
idTCPClient1.IOHandler.CloseGracefully;
idTCPClient1.Disconnect(False);
end;

procedure TForm1.Button4Click(Sender: TObject);
var
buffer:tidbytes;
begin
buffer:=0;
IdTCPClient1.socket.Write(buffer,1,0);

end;

procedure TForm1.IdTCPClient1Disconnected(Sender: TObject);
begin
try
  if not IdTCPClient1.Connected then
  //IdTCPClient1.Connect();
except
  try
    IdTCPClient1.Disconnect(False);
  except
  end;
  if IdTCPClient1.IOHandler <> nil then IdTCPClient1.IOHandler.InputBuffer.Clear;
  ShowMessage('Connectivity to the server has been lost.');
end;
end;

end.

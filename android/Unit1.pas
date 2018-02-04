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
    Edit1: TEdit;
    Memo1: TMemo;
    Button2: TButton;
    procedure Button1Click(Sender: TObject);
    procedure Button2Click(Sender: TObject);
    procedure IdTCPClient1Disconnected(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  Form1: TForm1;

implementation

{$R *.fmx}

function StringToBytes(const Value : WideString): TBytes;
var
I: integer;
begin
SetLength(Result, Length(Value));
for I := 0 to Length(Value) - 1 do
Result[I] := ord(Value[I + 1]) - 48;
end;


function BytesToString(const Value: TBytes): WideString;
var
I: integer;
S : String;
Letra: char;
begin
S := '';
for I := Length(Value)-1
Downto 0 do
begin
letra := Chr(Value[I] + 48);
S := letra + S;
end;
Result := S;
end;



procedure TForm1.Button1Click(Sender: TObject);
//var
//bytes:Tbytes;
//byte:TIdbytes;
//text:widestring;
begin
//text:='hellomoto';
//bytes := TEncoding.UTF8.GetBytes(text);
//byte:=bytes;

try


IdTCPClient1.Socket.WriteLn('FirstConnect',IndyTextEncoding_UTF16LE);
  memo1.Lines.Append(IdTCPClient1.Socket.ReadLn(IndyTextEncoding_UTF16LE));
except

end;
  //IdTCPClient1.Disconnect;
end;

procedure TForm1.Button2Click(Sender: TObject);
begin
IdTCPClient1.Host:='fokes1.asuscomm.com';
IdTCPClient1.Port:=7777;
IdTCPClient1.ConnectTimeout:=2000;
IdTCPClient1.Connect;
end;

procedure TForm1.IdTCPClient1Disconnected(Sender: TObject);
begin
IdTCPClient1.Host:='fokes1.asuscomm.com';
IdTCPClient1.Port:=7777;
IdTCPClient1.ConnectTimeout:=2000;
try
IdTCPClient1.Connect;
except
end;
end;

end.

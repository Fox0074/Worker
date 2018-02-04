unit Unit1;

interface

uses
  System.SysUtils, System.Types, System.UITypes, System.Classes, System.Variants,
  FMX.Types, FMX.Controls, FMX.Forms, FMX.Graphics, FMX.Dialogs,
  FMX.Controls.Presentation, FMX.StdCtrls, IdBaseComponent, IdComponent,
  IdTCPConnection, IdTCPClient, IdCmdTCPClient;

type
  TForm1 = class(TForm)
    IdTCPClient1: TIdTCPClient;
    Button1: TButton;
    IdCmdTCPClient1: TIdCmdTCPClient;
    procedure Button1Click(Sender: TObject);
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
begin
IdTCPClient1.Host:='fokes1.asuscomm.com';
IdTCPClient1.Port:=7778;
IdTCPClient1.ConnectTimeout:=2000;
IdTCPClient1.Connect;
end;

end.

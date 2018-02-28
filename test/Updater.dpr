program Updater;

uses
  Vcl.Forms,
  Unit1 in 'Unit1.pas' {Form1};

{$R *.res}

begin
  Application.Initialize;

  Application.MainFormOnTaskbar := True;
  Application.Title := 'Updater';
  Application.CreateForm(TForm1, Form1);
  Application.ShowMainForm:=false;
  Application.Run;
end.

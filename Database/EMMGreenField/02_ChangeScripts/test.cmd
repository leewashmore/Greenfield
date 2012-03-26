@FOR %%G IN (C:\vfedonkin\Greenfield\Database\EMMGreenField\02_ChangeScripts\*.sql) DO (
sqlcmd -S localhost\SQLExpress2 -d EMMGreenField -U test -P 123123 -i "%%G" -b
@if ERRORLEVEL 1 exit %ERRORLEVEL%   
)



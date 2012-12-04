cls
for %%f in (*.sql) do (
    sqlcmd /S lonweb1t.ashmore.local -b -d AIMS_Data_Dev /U WPSuperUser /P Password1 /i %%f
    @if ERRORLEVEL 1 (exit /B 1) else (echo ok) 
)
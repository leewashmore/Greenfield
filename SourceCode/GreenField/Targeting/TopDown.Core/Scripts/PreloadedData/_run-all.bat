cls
for %%f in (*.sql) do (
    sqlcmd /S %1 -b /d Aims /E /i %%f
    @if ERRORLEVEL 1 (exit /B 1) else (echo ok) 
)
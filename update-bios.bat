@echo off
if exist ..\Electronics\WolfNet-65C02-WBC\WolfNet-65C02-Software\Firmware\BIOS\bios.bin (copy /V /B ..\Electronics\WolfNet-65C02-WBC\WolfNet-65C02-Software\Firmware\BIOS\bios.bin .\bios.bin /B) else (echo Copy failed: BIOS not found!)
::if exist .\Emulator\bin\Debug\Emulator.exe (start .\Emulator\bin\Debug\Emulator.exe && set _TESTED=true)
::if exist .\Emulator\bin\Release\Emulator.exe (start .\Emulator\bin\Release\Emulator.exe && set _TESTED=true)

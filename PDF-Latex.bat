@echo off
if exist .\Documentation (rd /S /Q .\Documentation)
::call InstallFont.bat
Tools\Doxygen\doxygen.exe doxyfile.config
pushd .\Documentation\LaTeX\
call make.bat
popd
pause
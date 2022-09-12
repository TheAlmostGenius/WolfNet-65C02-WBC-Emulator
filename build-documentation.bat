@echo off
if exist .\Documentation (rd /S /Q .\Documentation)
::call InstallFont.bat
Tools\Doxygen\doxygen.exe -w html NewHeader.htm NewFooter.htm Style.css
Tools\Doxygen\doxygen.exe -w latex NewHeader.tex NewFooter.tex NewStylesheet.sty
Tools\Doxygen\doxygen.exe doxyfile.config
pushd .\Documentation\LaTeX\
call make.bat
popd
move .\Documentation\LaTeX\refman.pdf .\Documentation\Documentation.pdf
start .\Documentation\html\index.html
start .\Documentation\Documentation.pdf
del *.bak
@ECHO OFF

rm -fRv .\documentation

"Tools\Doxygen\doxygen.exe" doxyfile.config

START .\documentation\html\index.htm
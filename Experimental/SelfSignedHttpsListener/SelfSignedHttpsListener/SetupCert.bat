rem ..\..\common\src\NewAgent\lib\makecert\makecert.exe -r -a sha512 -n "CN=localhost" -sky exchange -pe -b 01/01/2000 -e 01/01/2050 -ss my -sr localmachine

rem netsh http delete sslcert ipport=127.0.0.1:13522
rem netsh http show sslcert

netsh http add sslcert ipport=127.0.0.1:13522 appid={dd20ddbb-e892-47ef-8362-de8544615cc2} certhash=9f9ade46d711811738926dd3d517a16090fde206 


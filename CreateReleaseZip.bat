echo on

for /f "tokens=3,2,4 delims=/- " %%x in ("%date%") do set d=%%y%%x%%z
set data=%d%

"D:\Program Files (x86)\7-Zip\7z.exe" a -tzip "D:\Users\Lawrence\Documents\Projects\GW2BuildLibrary\GW2BuildLibrary_%d%.zip" "D:\Users\Lawrence\Documents\Projects\GW2BuildLibrary\bin\Release\GW2BuildLibrary.exe" "D:\Users\Lawrence\Documents\Projects\GW2BuildLibrary\bin\Release\README.pdf"
# Ratiod-Texture-Viewer
A Unity tool that will to help you pick your quality to compression ratio

Can be found under Tools/Ratio'd

Insert your shader into the field and it will try to grab your _MainTex and _BumpMap textures
If your shader does not use these keywords for main texture and normal map pls message me so I can change it @zakhazard

When you click to create your textures it will make 12 variations using various compression formats 2 sizes smaller and bilinear and mitchell resizing
They are created in the the "Test Textures" folder which is put into your current open project folder
The textures will be named the same but with their settings appended
While you are picking between your textures an extra view will pop up that is a bit back from your scene view
When your are happy with your texture hit the button to delete all the extra copies. Your current selected one will be in the test textures folder

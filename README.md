# UnityScreenCapture
## A screen capture for Unity, using a Python local server, for standalone (Windows/MacOS/Linux)

This build has been done using this [package](https://github.com/off99555/Unity3D-Python-Communication) to communicate between Unity and Python.

Additionally, the python server requires to be started every time the screen sharing is started. To do this automatically:
I have set up a .sh script in the Unity Streamingassets folder (for MacOS and Linux).
For Windows, instead, I am opening the "cmd.exe" executable to run the Python server (currently not working as intended, will update this when I figure it out).

Hope this is helpful :)

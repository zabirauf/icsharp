from platform import system
from os.path import expanduser

if system() == "Windows":
	path = "C:\\ProgramData\\jupyter\\kernels\\csharp\\static"
elif system() == "Darwin":
	path = expanduser("~") + "/Library/jupyter/kernels/csharp/static"
elif system() == "Linux":
	path = expanduser("~") + "/.local/share/jupyter/kernels/csharp/static"
else:
	raise Exception("Unknown OS")

c = get_config()
c.NotebookApp.extra_static_paths = [ path ]
c.update('notebook', {"CodeCell": {"cm_config": {"autoCloseBrackets": False}}})

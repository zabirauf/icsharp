import json
from platform import system
from os import getcwd

arguments = []

if system() != "Windows":
	arguments.append("mono")
	path = getcwd().replace("kernel-spec", "") + "/Kernel/bin/Release/iCSharp.Kernel.exe"
else:
	path = getcwd().replace("kernel-spec", "") + "\\bin\\Release\\iCSharp.Kernel.exe"

arguments.append(path)
arguments.append("{connection_file}")

kernelData = {}

kernelData["argv"] = arguments
kernelData["display_name"] = "C#"
kernelData["language"] = "csharp"

if system() != "Windows":
	with open('kernel-spec/kernel.json', 'w') as outfile: json.dump(kernelData, outfile)
else:
	with open('kernel-spec\\kernel.json', 'w') as outfile: json.dump(kernelData, outfile)

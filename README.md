[![Build Status](https://travis-ci.org/zabirauf/icsharp.svg)](https://travis-ci.org/zabirauf/icsharp)

# Interactive C# Notebook
ICSharp is an C# language kernel for [Jupyter.](http://jupyter.org) It allows users
to use Jupyter's Notebook frontend, except where Jupyter executes python code, ICSharp
can execute C# code. It is based on Roslyn REPL engine of [scriptcs.](http://scriptcs.net/),
so all the goodies of scriptcs comes along with it.

This is on top of all of Jupyter's other frontend features like Markdown rendering,
HTML rendering, saving notebooks for later use and even the ability to view ICSharp
Notebooks in [Jupyter's NBViewer](http://nbviewer.jupyter.org/).

### Disclaimer
The development of this language kernel for C# is at it's very early stages.
This is Alpha. Take with a large pinch of salt :)

### Installation

### BUILD ISSUE FIX AND INSTALLATION (Windows confirmed working for now)
Clone the respository and make sure that the submodule engine has been cloned by using this command:

`git clone --recurse-submodules -j8 git://github.com/MohamedEihab/icsharp`
  
Update and restore the nuget dependencies using the following commands

```
nuget\Nuget.exe update -self
.nuget\Nuget.exe restore
cd Engine
.nuget\Nuget.exe update -self
.nuget\Nuget.exe restore
```

Open the project on Visual Studio and attempt to build, follow the remaining instructions below after successful build to launch Jupyter Notebooks with the newly installed C# Kernel.

#### [Mac OS X](https://github.com/zabirauf/icsharp/wiki/Install-on-Mac-OS-X)
In the directory, run
```
./build.sh
```

#### [Linux](https://github.com/zabirauf/icsharp/wiki/Install-on-Unix-(Debian-7.8))
In the directory, run
```
./build.sh
```

#### [Windows](https://github.com/zabirauf/icsharp/wiki/Installation)
No need to change paths! Build on Visual Studio and you will have C# Kernel installed on your Jupyter Notebook.

### Feedback
I am eager to receive [feedback](mailto:zabirauf@gmail.com) from anyone who has attempted to use ICSharp. I would love to hear
some thoughts on how to improve ICSharp.

### Known Issues
* `Console.WriteLine` does not print output in the notebook
* `Console.ReadLine` does not work currently

## [Demo](http://nbviewer.jupyter.org/urls/gist.githubusercontent.com/zabirauf/a0d4aa22b383afaa1e23/raw/65e539dc98b2cf3e38cc26faf3575e50f4ac9108/iCSharp%20Sample.ipynb)

## TODO

### Code to write

- [ ] Intellisense Support
- [ ] YellowBook addition

### Current
- [ ] Syntax Highlighting - Mohamed
- [ ] Jupyter Notebook Fix - Mostafa


### Done


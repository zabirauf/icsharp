#Interactive C# Notebook
ICSharp is an C# language kernel for [IPython.](http://ipython.org) It allows users
to use IPython's Notebook frontend, except where IPython executes python code, ICSharp
can execute C# code. It is based on Roslyn REPL engine of [scriptcs.](http://scriptcs.net/),
so all the goodies of scriptcs comes along with it.

This is on top of all of IPython's other frontend features like Markdown rendering,
HTML rendering, saving notebooks for later use and even the ability to view ICSharp
Notebooks in [IPython's NBViewer](http://nbviewer.ipython.org/)

###Disclaimer
The development of this language kernel for C# is at it's very early stages.
This is Alpha. Take with a large pinch of salt :)

###Installation
1. Install [chocolatey](https://chocolatey.org)
2. Install [python \(2.x\)](https://chocolatey.org/packages/python2) and [pip](https://chocolatey.org/packages/pip) using chocolatey
3. Install [ipython](http://ipython.org/install.html) using pip
4. Install [ICSharp](https://chocolatey.org/packages/ICSharp/0.1) using chocolatey by executing `choco install ICSharp`
5. Start IPython notebook with C# kernel by running `ipython notebook --profile=icsharp`

###Feedback
I am eager to receive [feedback](mailto:zabirauf@gmail.com) from anyone who has attempted to use ICSharp. I would love to hear
some thoughts on how to improve ICSharp.

###Known Issues
* Console.WriteLine does not print output in the notebook
* Console.ReadLine does not work currently

##[Demo](http://nbviewer.ipython.org/urls/gist.githubusercontent.com/zabirauf/a0d4aa22b383afaa1e23/raw/65e539dc98b2cf3e38cc26faf3575e50f4ac9108/iCSharp%20Sample.ipynb)

##[Twitter](http://twitter.com/zabirauf)

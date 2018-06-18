# C# Azure Notebook Kernel

### Project Description 

**“C# is in the [TOP FIVE](https://spectrum.ieee.org/static/interactive-the-top-programming-languages-2017) most used programming languages currently in the market.”**

Our C# Kernel caters to users who want to learn how to learn developement in C# through a notebook environment like that of [Azure Notebooks.](https://notebooks.azure.com/) 

We created a C# language kernel for [Jupyter](http://jupyter.org) which allows users to use Jupyter's Notebook frontend, except where Jupyter executes python code, C# code is executed. 

Along with the C# Kernel we designed and created complementary [Azure notebooks](https://notebooks.azure.com/)  to attract and drive new users to discover C#. The complementary notebook takes the user through a tour of the C# Language highlighting all the concepts and features that come along with it!

### Technical Details 

The kernel is based on the Roslyn REPL engine of [scriptcs.](http://scriptcs.net/) and takes advantage of all of Jupyter's frontend features like **Markdown rendering,
HTML rendering, saving notebooks for later use and even the ability to view C# Notebooks in [Jupyter's NBViewer](http://nbviewer.jupyter.org/)**.

### For Use over the Cloud

If you wish to **"develop C# code in your browse"r** please use our 4-step guide and [complementary video](https://drive.google.com/open?id=1bufPJQdYsznr3HR637oJy615Ad0btq8C) walkthrough to use the C# Notebook on [Azure Cloud](https://azure.microsoft.com/en-gb/).

### For Use locally

If you wish to use the the Kernel locally on your machine, or are a software developer that would like to contribute towards improving this **open-source** project please follow the instructions below:

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

#### [Linux](https://github.com/zabirauf/icsharp/wiki/Install-on-Unix-(Debian-7.8))

#### [Windows](https://github.com/zabirauf/icsharp/wiki/Installation)

### Supported Features

#### Intellisense

This is a **context-aware code completion** feature that **speeds up** the process of coding applications by **reducing typos** and other common mistakes. We implement this with auto completion popups when typing, querying parameters of functions, query hints related to syntax errors and more. Click [here](https://drive.google.com/open?id=1OCNb8y_e0By4pjUn1ilZ95m_9ulpHFHA) to see a demo!

### Feedback
We are eager to receive [feedback](mailto:zabirauf@gmail.com) from anyone who has attempted to use our Kernel. We would love to hear
your thoughts and are constantly striving to improve our product!

## [Complementary C# Notebook](https://github.com/omerouz/icsharpBuild/blob/master/C%23%20Tutorial/CSharpTutorial.ipynb)




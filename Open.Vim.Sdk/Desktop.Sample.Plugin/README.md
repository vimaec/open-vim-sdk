# VIM Desktop Application SDK

Sample source code for extending the VIM desktop application
can be found here: [https://github.com/vimaec/open-vim-sdk](https://github.com/vimaec/open-vim-sdk).

## About VIM and the VIM Desktop Application SDK

The VIM Desktop Application SDK (https://github.com/vimaec/vim-desktop-sdk) is a C# API for writing plug-ins that can be used for automating the [VIM Windows Desktop application](https://vimaec.com) and add new functionality. 

You can download the VIM desktop application [here](https://www.vimaec.com/downloads).

VIM (Virtual Information Modeling) is both a platform and open file-format designed especially for the efficient interchange of BIM and 3D data into virtual environments and real-time engines running on multiple platform

The VIM Desktop Application is a WPF .NET application with an integrated high-performance rendering engine built from the ground up using the state of the art Vulkan API and is designed especially for large scale architectural projects. 

# SDK Requirements

The following are required to get started building an SDK:

* Visual Studio Community 2019 – download for free at https://visualstudio.microsoft.com/vs/community/
* VIM Desktop Application – register for a free account at https://portal.vimaec.com

# Creating VIM Plug-ins

A VIM plug-in is any public class that has the `[VimPlugin]` attribute and implements the `IVimPlugin` interface. VIM plug-ins are loaded from .NET assemblies whose 
name matches the pattern `*.Plugin.dll` and that resides in the `%userprofile%\VIM\Desktop Plugins` folder. One assembly (class library) may contain multiple plug-ins. 

## VIM Libraries

The VIM API SDK consists of following files in the VIM Desktop Application folder (e.g. C:\Program Files\VIM\Viewer):

* [Vim.BFast.dll](https://github.com/vimaec/bfast) - A library for reading and writing binary data in the BFAST format. Both G3D and VIM files use the BFAST layout. 
* Vim.DotNetUtilities.dll - General purpose .NET utility functions 
* [Vim.G3d.dll](https://github.com/vimaec/g3d) - A library for reading and writing Geometry data in the G3D format. 
* Vim.Geometry.dll - A library of useful Geometry functions
* [Vim.LinqArray.dll](https://github.com/vimaec/linqarray) - A library for providing a LINQ style interface to Immutable arrays
* [Vim.Math3d.dll](https://github.com/vimaec/math3d) - A high-performance 3D math library appropriate for serialization 

## The IVimPlugin Interface

The VIM Desktop application communicates with the plug-in by calling functions exposed by the plug-in by implementing the `IVimPlugin` interface. These are:

* `void OnRenderApi(IRenderApi api)` -  Called when the plug-in is initialized 
* `void OnHuddleApi(IHuddleApi api)` -  Called when the plug-in is initialized 
* `void OnOpenFile(string fileName)` – Called when a new VIM file is loaded
* `void OnCloseFile()` – Called when a VIM file is unloaded
* `void OnFrameUpdate(float deltaTime)` – Called on each frame

The `IRenderApi` and `IHuddleApi` is an interface provided upon initialization to the plug-in that provides a way for the plug-in to communicate with and control the Desktop application further.  

## Default Implementations of IVimPlugin Classes

There are two classes that provide convenient default implementations of `IVimPlugin`:

* `VimPluginBaseClass` - General purpose plug-in that caches the renderer, and provide default virtual function implementation of `IVimPlugin`.
* `VimPluginBaseClassWithMouseHandling` - A plug-in that provides additional virtual functions for mouse events. 

## Creating a new VIM Plug-in from Scratch

* Open Visual Studio Community 2019 
* Add a new project
* Choose “Class Library (.NET Framework) C#”.
* Look for Language = C#, Platform = Windows, Type = Library.
* Configure your new project to use .NET Framework 4.7.2. 
* Make sure your assembly name ends with “.Plugin.d;;”. 
* Add references to VIM libraries 
* Make sure that reference has “Copy Local” set to false.
* Go to Windows explorer and type in `%userprofile%\VIM\Desktop Plugins` and press enter 
* Windows explorer will resolve this to a concrete location (like `C:\Users\Christopher Diggins\VIM\Desktop Plugins`)
* Go to project settings and change the output folder to the resolved path
    
 
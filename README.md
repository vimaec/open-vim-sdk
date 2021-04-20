# Open VIM SDK

This repository houses the implementation for various open-source libraries and technologies that we have developed at [VIM](http://vimaec.com) for facilitating the development of more efficient BIM and general purpose 3D applications for ourselves, our partners, and our customers. We believe that the AEC (Architecture, Engineering, and Construction) industry is being held back by lack of software innovation and that the way to move forward is for the community to share access to data and technology. 

We're here to help change the way BIM is made available to the world!

At [VIM](http://vimaec.com/about) we push the boundaries of what people have come to expect from their BIM software in terms of performance and scalability by leveraging our collective backgrounds across various industries (film, games, advertising, architecture, construction, CAD, VR/AR, and software).

## Contents

This repository houses implementations and tests for the following libraries and data formats. 

* VIM - An efficient data format for BIM and 3D data. 
    * [Object Model](https://github.com/vimaec/open-vim-sdk/tree/main/Open.Vim.Sdk/ObjectModel) - C# Implementation of an Object Relational Model (ORM)
    * [Data Format](https://github.com/vimaec/open-vim-sdk/tree/main/Open.Vim.Sdk/DataFormat) - Low Level Serialization code for tables, geometry, and assets 
    * [Scene Builder](https://github.com/vimaec/open-vim-sdk/tree/main/Open.Vim.Sdk/SceneBuilder) - High-level serialization and helper code
    * [VIM Format Specification](https://github.com/vimaec/vim) - VIM Specification Document
* [Math3D](https://github.com/vimaec/open-vim-sdk/tree/main/Open.Vim.Sdk/Math3d) - A .NET 3D math library. Math3D acts as a drop-in replacement for System.Numerics with added functionality and strict alignement     
* [LinqArray](https://github.com/vimaec/open-vim-sdk/tree/main/Open.Vim.Sdk/LinqArray) - A .NET library with LINQ style syntax for immutable arrays, while maintaining safety and peformance.     
* [G3D](https://github.com/vimaec/open-vim-sdk/tree/main/Open.Vim.Sdk/LinqArray) - A simple and efficient cross-platform 3D geometry format for meshes and point clouds. 
* [BFAST](https://github.com/vimaec/open-vim-sdk/tree/main/Open.Vim.Sdk/BFast) - A data format for efficient cross-platform serialization of collections of byte arrays
* [VIM Desktop SDK](https://github.com/vimaec/open-vim-sdk/tree/main/Open.Vim.Sdk/Desktop.Sample.Plugin) - A .NET SDK for extending the VIM desktop application 
* [.NET Utilities](https://github.com/vimaec/open-vim-sdk/tree/main/Open.Vim.Sdk/DotNetUtilities) - A collection of .NET utility code. 
* [VIM Geometry](https://github.com/vimaec/open-vim-sdk/tree/main/Open.Vim.Sdk/Geometry) - A Library of 3D geometry code

While many projects have their own repository with a landing page, we chose to put all of the implementation in one repository to facilitate testing and deployment, and to more easily accept changes and suggestions from the community at large. 

## Contributing 

We welcome contributions in the form of questions, suggestion, bug-reports, and PRs. 

## License

Our code is licensed under the MIT License 1.0. 

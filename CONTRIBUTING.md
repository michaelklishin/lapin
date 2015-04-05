# Contributing to Lapin

Contributing to Lapin is involves a few steps:

 * File an issue with a suggestion explaining what you want to change and *why*
 * Fork Lapin on GitHub
 * Make changes on a branch, push it to your fork
 * Ensure all tests pass (see below)
 * Submit a pull request
 * Wait

For trivial changes, feel free to submit a pull request immediately
but try explaninig your reasoning.


## How to Run Tests

### Prerequisites

Tests assume you have RabbitMQ 3.5.x or later running on `localhost` with stock
defaults.


### On Windows

Perhaps the easiest way is to use [Visual Studio NUnit Adapter](https://visualstudiogallery.msdn.microsoft.com/6ab922d0-21c0-4f06-ab5f-4ecd1fe7175d).
 
Running on the command line is also straightforward:

    msbuild /t:Test RabbitMQ.Lapin.Tests/Lapin.Tests.fsproj


### On Mono

    xbuild /t:Test RabbitMQ.Lapin.Tests/Lapin.Tests.fsproj

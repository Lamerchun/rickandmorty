# Rick & Morty

This excercise implements a simple client / server for `The Rick and Morty` api. Both REST and GraphQL are implemented for a simple character search by name.

For documentation of the api refer to [https://rickandmortyapi.com](https://rickandmortyapi.com).

## Technology used

This project is build with [Vue3](https://v3.vuejs.org/), [vitejs](https://vitejs.dev/), [tailwindcss v3](https://tailwindcss.com/) and [.NET 6.0](https://dotnet.microsoft.com/)

## Setup with Windows + Visual Studio 2022

- run npm update in /App.Server/_Client to install npm packages
- run /App.Server/_Client/export-cert.cmd to export local IIS certificate for use with the vitejs dev server
- run dev.cmd to startup vue client with vitejs from the Visual Studio Developer-PowerShell
- start project in Visual Studio

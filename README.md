# Rick & Morty

This excercise implements a simple client / server for the `Rick and Morty` api. Both REST and GraphQL are implemented for a simple character search by name.

For documentation of the api refer to [https://rickandmortyapi.com](https://rickandmortyapi.com).

## Technology used

This project is build with Vue3, vitejs, tailwindcss v3 + .netcore 6.0

## Setup with Windows + Visual Studio 2022

- run npm update in /App.Server/_Client to install npm packages
- run /App.Server/_Client/export-cert.cmd to export local IIS certificate for use with the vitejs dev server
- run dev.cmd to startup vue client with vitejs from the Visual Studio Developer-PowerShell
- start project in Visual Studio

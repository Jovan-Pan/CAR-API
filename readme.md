# CRCU Web API 

## Overview

This project is a Web API developed using the Onion Architecture. It is designed to promote separation of concerns and improve code maintainability.

<br />
Quick introduction to Onion Architecture <br/> https://medium.com/@alessandro.traversi/understanding-onion-architecture-an-example-folder-structure-9c62208cc97d

### Pre requisites
[.NET 8 SDK] <br/>(https://dotnet.microsoft.com/download/dotnet/8.0)

## Project Structure

The project is divided into several layers, each serving a distinct purpose:

```
CRCU Web Api

|--Application Layer
|  |--Services                        #Core Business Logic of the Application
|  |  |--ServiceManager.cs            #To Configure dependecy for all Service/Business Logic
|  |  |--Resources                    #Resource files for Localization/Language
|  |--Services.Contracts              #Abstraction/Interfaces for Services/Business Logic 

|--Domain Layer
|  |--Entities                        #Model/Classes of the project
|  |--Contracts                       #Abstraction/Interfaces for 
Infrastructures and repository

|--Infrastructure Layer
|  |--Infrastructure                  #Project to hold technical implementation (ex: logging, caching, email and third party api)
|  |--Repository                      #For Connectivity to SQL
|  |  |--DbContext.cs                 #For Configuring Connection String for application
|  |  |--RepositoryManager.cs         #To Configure dependecy for all Repository

|--Presentation Layer
|  |--WebApi                          #.Net Core Web Api Project
|  |  |--Controllers                  
|  |  |--DependecyInjection           
|  |  |--Middleware                   #Holds Middleware configuration for the project (Global Exception handler, JWT Authentication Handler, and logging)
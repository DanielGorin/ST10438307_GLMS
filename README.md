PROG 7311 
POE Part 2 
ReadMe 
Daniel Gorin 
ST10438307 
2026 


Running the Project 

Prerequisites: 

Visual Studio 2026 
Developed on: 
Edition 	= 	Community 
Release	=  	February 2026 Feature Update 
Version 	= 	18.3.1 

.Net 8.0 SDK 

Git 

 
Cloning Instructions: 
Clone the repository using the console, github desktop or visual studio 
Open the project in visual studio 


Downloading Zip Instructions: 
Download Zipped Folder from github or ARC submission 
Extract it 
Open the project in visual studio 

Running the Project: 
In visual studio click build then build solution 
Press F5 to run the project 
It should open in your browser 
No login or registration implemented for part 2 (will come in part 3) 
Database is in the repository no setup or migrations required 

Project Overview 

Introduction 
The GLMS (Global Logistics Management System) is a contract and service request management platform built for TechMove. 
Functionality: 
Client management 
Contract lifecycle tracking with status management 
PDF signed agreement upload and download 
Service request processing with contract status enforcement 
Live USD to ZAR currency conversion 

This version is an intentional monolith built with Blazor Server, EF Core, and SQLite. It will be expanded into a decoupled Web API and Blazor frontend in Part 3. 

Database Architecture 

Three entities: 
Client 
Name 
ContactDetails 
Region 
Contract 
inked to Client 
StartDate 
EndDate 
Status 
ServiceLevel 
SignedAgreementPath 
ServiceRequest 
linked to Contract 
Description 
CostZAR 
Status 

Relationships: 
One Client has many Contracts 
One Contract has many ServiceRequests 

SQLite database accessed via EF Core using IDbContextFactory registered in Program.cs. All relationships and constraints configured using Fluent API in AppDbContext.cs. Connection string stored in appsettings.json. 

Design Patterns 
Three GoF patterns implemented as specified in the Part 1 UML: 
Abstract Factory 
ContractFactory and three concrete factories create Standard, SLA, and International contracts. The service selects the correct factory at runtime the UI never knows which ran 
Observer 
Contract acts as subject, ServiceRequestValidator and AuditLogger as observers. Status changes fire Notify() automatically the validator blocks service requests on Expired and OnHold contracts 
Decorator 
PdfValidationDecorator wraps FileUploadService. Validation and storage are completely separated. DI wires the decorated version automatically 

Workflow and Validation Logic 

Server Side: 
ServiceRequestService.cs enforces all hard rules before anything touches the database. Empty descriptions, negative costs, and blocked contracts are all rejected at the service layer. This layer has no knowledge of the UI. 

Client Side: 
ServiceRequestCreate.razor shows a warning immediately when a blocked contract is selected before the user attempts to submit. ContractList.razor fires the Observer the moment a status is changed via the inline dropdown. No business logic lives in the components themselves. 

File Handling 
Upload and validation are handled by two separate classes. FileUploadService.cs handles storage it saves files to wwwroot/uploads with a GUID prefix and returns the relative path. PdfValidationDecorator.cs wraps it and intercepts each upload before it reaches the storage, rejecting non pdf files. 
The path is stored in the database on the Contract record. ContractList.razor shows the filename as a clickable download link directly in the table. ContractEdit.razor shows the current filename and allows replacement. 


External API 
The GLMS uses open.er-api.com which is a free exchange rate API that requires no account or key. The rate is fetched once when the service request page loads and cached locally for all conversions on that page. Both USD and ZAR input boxes update each other live. If the API is unreachable the app falls back to a rate of 18.50 and continues working normally. Only the ZAR amount is stored. 

Unit Testing 
19 tests across three categories in GLMSTests.cs, all passing locally and via GitHub Actions: 
Currency math 
fixed rate conversions 
zero values 
negative amounts 
rounding precision 
File validation 
PDF passes 
exe/jpg/docx rejected 
uppercase .PDF extension handled 
Moq used to mock IBrowserFile 
Observer blocking 
Expired and OnHold contracts block requests 
Active and Draft allow them 
zero cost and empty description validated at service layer 

AI Declaration 
AI Used: 
Claude Sonnet 4.6 
Date: 
11 April 2026 – 22 April 2026 
Main Usage Areas: 
Project Structure 
Solution and fodler setup 
NuGet package installation (version compatibility) 
Part 3 preplanning 
Commenting 
Debugging 
Resolving build and namespace errors 
Fixing Blazor interactivity and rendering issues 
Resolving EF Core migration failures 
UI 
Layout and component styling 
Tweaking colours, fonts, scale and spacing 
Coloured select dropdowns 
Unit Tests 
Identifying testable business logic 
Identifying edge cases 
Applying GitHub actions testing 

References: 
Anthropic, 2026. Claude (Claude Sonnet 4.6) [Large language model]. Available at: https://claude.ai/ [Accessed 22 April 2026]. 
ExchangeRate-API, 2026. Open Exchange Rates API [REST API]. Available at: https://open.er-api.com [Accessed 22 April 2026]. 

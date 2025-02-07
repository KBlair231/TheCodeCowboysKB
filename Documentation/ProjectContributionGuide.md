# Project Contribution Guide

Welcome to our project! This guide will help you set up your development environment and understand the conventions we follow. Please read through this document carefully.

## Visual Studio 2022

Start by installing or updating to Visual Studio 2022. Although contributions could be made using other IDEs and Text Editors (e.g. Visual Studio Code), for simplicity's sake, we ask that you use Visual Studio 2022. This will make development and following along with our documentation easier.

You can download Visual Studio 2022 from the [official Microsoft website](https://visualstudio.microsoft.com/downloads/). Follow the installation guide provided on the site.

## .NET 8.0

We are using **.NET 8.0**. So, ensure that you have the .NET SDK 8.0.405 installed. 

This includes:
- .NET Runtime 8.0.12
- ASP.NET Core Runtime 8.0.12
- .NET Windows Desktop Runtime 8.0.12

You can download it from the [official .NET website](https://dotnet.microsoft.com/download/dotnet/8.0).

## VS Projects

Our project consists of four main projects:
1. **PromptQuest**: The game's source code
2. **Tests_NUnit**: An NUnit test project for testing our server side code
3. **Tests_BDD**: A ReqnRoll project for BDD testing
4. **Tests_Jest**: A Jest Javascript testing project for unit testing our client side code

## Folder Structure

```
PromptQuest/
│
├── PromptQuest/
│   ├── ...
│   ├── Models/
│   ├── Views/
│   ├── Controllers/
│   └── NUnitTests.sln
│
├── Tests_NUnit/
│   ├── ...
│   └── Tests_NUnit.csproj
│
├── Tests_BDD/
│   ├── ...
│   └── Tests_BDD.csproj
│
├── Tests_Jest/
│   ├── ...
│   └── Tests_Jest.csproj
│
```

## Front-end CSS Library

We are using **Bootstrap** v5.1 primarily for its robust layout classes and ease of use. For general theming, we opted for custom CSS classes. Please, try to use the classes that have already been written before writing a new one.

## JavaScript Usage

We are using **plain old JavaScript** (ES6+). All team members must follow this convention. If necessary, we can discuss including any additional libraries.

## Git Branch Naming Strategy

We follow a consistent naming strategy for Git branches. Use the following format:

```
feature_<issue-id>
bugfix_<issue-id>
hotfix_<issue-id>
```

If there isn't an issue on Jira associated with the branch, use a short description separated by underscores.

Example:

```
file_renaming_bdd_tests
```

Unfortunately, the “Create Branch” feature in Jira isn't available for personal GitHub accounts, so please create branches manually using Visual Studio following this convention. 

## Database Scripts, Table Names, PK and FK Names

This is not yet set in stone as we have not begun using db scripts

Here is an example of how we write database scripts, table names, PK, and FK names:

```sql
-- Example table creation script
CREATE TABLE player -- Lowercase table names
(
    PlayerID INT PRIMARY KEY, -- Primary keys should include the table name followed by ID, this avoids ambiguity
    PlayerName NVARCHAR(100) NOT NULL, -- When a word is also a reserved keyword, like "Name", put the name of the table before it.
    NumPotions INT NOT NULL, -- Each word in column names are capitalized
    Attack INT NOT NULL,
    Defense INT NOT NULL
);

-- Example foreign key
ALTER TABLE item
ADD CONSTRAINT FK_player_item FOREIGN KEY (PlayerID) REFERENCES player(PlayerID);
```

## Microsoft SQL Server

We will be using the latest version of Microsoft SQL Server for our database management. This will allow us to leverage the newest features and improvements for optimal performance and security. 

- **Download**: You can download the latest version of Microsoft SQL Server from the [official Microsoft website](https://www.microsoft.com/en-us/sql-server/sql-server-downloads).
- **Installation**: Follow the installation guide provided on the website to set up Microsoft SQL Server on your machine.
- **Configuration**: More information on how to configure Microsoft SQL Server for our project will be provided soon. Please stay tuned for detailed instructions.

## Azure Data Studio

Azure Data Studio will be our primary tool for managing and querying our SQL databases. It provides a modern, streamlined interface with powerful features for database development.

- **Download**: You can download Azure Data Studio from the [official Microsoft website](https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio).
- **Installation**: Follow the installation guide provided on the website to set up Azure Data Studio on your machine.
- **Usage**: We will provide more information on how to use Azure Data Studio effectively for our project in the near future. Detailed guidelines and best practices are coming soon.

## Entity Framework Core Loading Strategy

Currently, we have not decided on whether to use **eager loading** or **lazy loading** for related entities in Entity Framework Core

Feel free to reach out if you have any questions or need further assistance. Happy coding!

# Project Overview

This project is a console application that allows users to query the Microsoft Dynamics CRM API for information about accounts, opportunities, leads and contacts.

## Folder Structure
- `cli/`: Root folder for the CLI application.
- `cli/Microsoft.Dynamics.Cli`: Contains the source code for the console application. It used Spectre CLI to implement CLI commands.
- `cli/Microsoft.Dynamics.Client`: Contains the source code for the API calls to the Dynamics backend.
- `cli/Microsoft.Dynamics.Core`: Contains Shared logic that is used in all other libraries.

## Libraries and Frameworks

- Spectre CLI for the console application.
- Use Serilog for logging.

## Coding Standards

- Use semicolons at the end of each statement.

## Console guidelines

- Lists are presented in a tabular format for better readability.
- Use colors to differentiate between different types of messages (e.g., errors in red, success messages in green).
- Each command should have a help option that provides information about how to use the command and its parameters.
- each command should support the output option and support json and table output formats. The default output format should be table.
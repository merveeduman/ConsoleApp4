# Pokemon Review App Console Client

This is a console-based client application for interacting with a Pokémon Review API. The application allows users to perform CRUD (Create, Read, Update, Delete) operations on various entities such as Pokémon, Categories, Owners, Reviews, Reviewers, and Countries via a RESTful API.

## Features

- **Interactive Menu**: Choose from six entity categories (Category, Pokémon, Owner, Review, Reviewer, Country) and four HTTP methods (GET, POST, PUT, DELETE).
- **Data Management**: Perform CRUD operations on entities with input validation.
- **API Integration**: Communicates with a local API hosted at `https://localhost:7091`.
- **Error Handling**: Includes basic error handling for invalid inputs and API response statuses.
- **JSON Serialization**: Uses `System.Text.Json` for serializing and deserializing data with case-insensitive property matching.

## Prerequisites

- **.NET SDK**: Ensure you have the .NET SDK installed (version compatible with the project, e.g., .NET 6 or later).
- **Running API**: The Pokémon Review API must be running locally at `https://localhost:7091`.
- **Dependencies**: The project references `PokemonReviewApp.Dto` for data transfer objects and uses `System.Text.Json` for JSON handling.

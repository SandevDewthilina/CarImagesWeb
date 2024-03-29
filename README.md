# CarImagesWeb

This project is a Dotnet Core MVC application for uploading, storing and viewing vehicle images in an Azure Blob Storage gallery. 
The gallery allows for the filtering of images based on different vehicle or container properties.

## Features

- User authentication
- Uploading of vehicle and container images with relevant tags such as 'front', 'rear', and 'side'
- Storage of vehicle and container data in corresponding tables, along with the image URL and tags
- Retrieval of images and their details from the database to display in the gallery
- Filtering mechanism for vehicles and containers based on relevant data from the database

## Technologies Used
![dotNet Core](https://upload.wikimedia.org/wikipedia/commons/thumb/e/ee/.NET_Core_Logo.svg/240px-.NET_Core_Logo.svg.png)
![Azure Blob Storage](https://www.clicdata.com/wp-content/uploads/2021/01/microsoft-azure-blob-storage-logo.com_.png)

## Setup

1. Clone the repository to your local machine
2. Navigate to the root directory of the project
3. Run `dotnet restore` to install the dependencies
4. Run `dotnet run` to start the application


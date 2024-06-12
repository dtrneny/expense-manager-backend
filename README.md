## Expense manager - backend

Expense Manager - Backend is a server application for managing incomes and expenses. It is coded using C# and persists data through MongoDB. This backend service provides APIs for handling financial data and can be integrated with various front-end clients.

### Features

- RESTful API design
- categorization of movements
- usage of various libraries (MongoDB Driver, Mapperly, FluentValidation, ...)
- authentication through JWT tokens
- usage of Scrypt for hashing purposes

### Installation

To use this backend application, you need to set up *.NET User Secrets* as follows:

``` json
{
  "MongoDatabase:ConnectionString": "/* your MongoDb connection string here */",
  "JwtSettings:Key": "/* your JWT secret here */"
}
```

You can also extend the provided code and tweak it for your own purposes.
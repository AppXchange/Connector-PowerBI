# Connector-PowerBI

Here's an outline of the endpoints created for the Power BI connector:

## 1. Groups Module
### Endpoints:
- **Group**
  - Actions:
    - `Create` - Creates a new Power BI workspace
  - Data: Workspace details including ID, name, capacity settings, etc.

- **Groups**
  - Data: List of workspaces user has access to

## 2. Datasets Module
### Endpoints:
- **Dataset**
  - Actions:
    - `Update` - Updates dataset properties (storage mode, query scale-out settings)
  - Data: Dataset properties including name, owner, refresh settings

- **Datasets**
  - Data: List of datasets and their properties

## 3. Push Datasets Module
### Endpoints:
- **Dataset**
  - Actions:
    - `Create` - Creates a new Push dataset
  - Data: Dataset schema and configuration

- **Table**
  - Actions:
    - `Update` - Updates table metadata and schema
  - Data: Table schema and properties

- **Tables**
  - Data: List of tables in a dataset

- **Rows**
  - Actions:
    - `Add` - Adds new rows to a table
    - `Delete` - Deletes all rows from a table
  - Data: Row data for tables

## 4. Imports Module
### Endpoints:
- **Import**
  - Actions:
    - `Create` - Creates a new import for PBIX files
  - Data: Import status and results

- **Imports**
  - Data: List of imports

- **Temp Upload Location**
  - Actions:
    - `Create` - Creates temporary storage location for large files
  - Data: Upload URL and expiration time

Each module includes its own configuration for:
- Action Processing
- Cache Writing
- Authentication
- Error handling

The connector uses OAuth2 client credentials flow for authentication with the following required attributes:
- Tenant ID
- Token URL
- Client ID
- Client Secret
- Scope
- Environment (Production/Test)

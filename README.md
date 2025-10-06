# ALSO Order Processing Function

Azure Functions application that processes orders via the ALSO provisioning API.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local) v4.x

## Quick Start

```bash
# Clone and navigate
git clone <repository-url>
cd ALSO.Task

# Build and run
dotnet build
func start
```

Function runs at `http://localhost:7254/api/process-order`


## Usage

**Test the endpoint:**
```bash
curl -X POST http://localhost:7254/api/process-order
```

**Default settings** (edit in `ProcessAlsoOrder.cs`):
- Country: `Latvia`
- Company: `LIDO`
- SKUs: `TPLV7893-85`, `TPLV7884-85`

## How It Works

1. Retrieves companies for Latvia
2. Finds LIDO company details
3. Fetches prices for target SKUs
4. Calculates totals and submits order

## Troubleshooting

**Build fails:** Stop the function host and run `dotnet clean && dotnet build`

**API errors:** Verify URLs and access codes in `appsettings.json`

**Function won't start:** Install Azure Functions Core Tools:
```bash
npm install -g azure-functions-core-tools@4 --unsafe-perm true
```


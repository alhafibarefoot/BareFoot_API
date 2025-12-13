# Module 1: Fundamentals & Setup

In this module, we will establish the foundation of our gRPC API project.

## 1. Create the Project
We generally use the .NET CLI:

```powershell
dotnet new grpc -n gRPC
```

## 2. Project Structure
gRPC projects have a unique structure compared to REST APIs:

```text
gRPC/
├── Protos/             # Protocol Buffer definitions (.proto files)
├── Services/           # C# implementations of the proto services
├── Program.cs          # Configuration
├── gRPC.csproj      # Project file (contains Protobuf references)
└── appsettings.json    # Config
```

## 3. The .proto File
The heart of gRPC is the `.proto` file. It defines the service contract.

```protobuf
syntax = "proto3";

option csharp_namespace = "gRPC";

service Greeter {
  rpc SayHello (HelloRequest) returns (HelloReply);
}

message HelloRequest {
  string name = 1;
}

message HelloReply {
  string message = 1;
}
```

## 4. Run it
gRPC services typically run on HTTP/2. Use a client like **gRPCurl** or **Postman** (with gRPC support) to test.

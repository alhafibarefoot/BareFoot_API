# Module 2: Protobuf & Messages

Unlike JSON in REST, gRPC uses **Protocol Buffers**.

## Scalar Value Types
Protobuf supports typed data:
*   `double`, `float`, `int32`, `int64`, `bool`, `string`, `bytes`

## Defining Messages
Messages are strongly typed. Fields are numbered (tags) which are distinct for binary serialization.

```protobuf
message User {
  int32 id = 1;
  string username = 2;
  bool is_active = 3;
}
```

## Service Definition
Services groups methods (RPCs).

```protobuf
service UserService {
  rpc GetUser (UserRequest) returns (UserResponse);
}
```

## Compilation
When you build the project, `.NET` tooling automatically generates C# base classes (`UserServiceBase`) from these files.

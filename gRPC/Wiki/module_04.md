# Module 4: Professional Patterns

Decoupling your internal Entities from your external Protobuf Messages is crucial.

## DTO Pattern (Protobuf Messages)
In gRPC, the generated Protobuf classes *are* your DTOs. Do not return EF Entities directly (you can't easily, anyway).

## AutoMapper
You can use AutoMapper to map Entities to Protobuf messages.

```csharp
CreateMap<UserEntity, UserMessage>()
    .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.created_at.ToString()));
```

## Services Layer
You can still use a Service/Repository layer behind your gRPC Service if logic is complex.

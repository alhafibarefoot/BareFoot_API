# Module 7: Security

## JWT Authentication
Works similarly to REST.
1.  Configure `AddAuthentication().AddJwtBearer(...)`.
2.  Add `[Authorize]` attribute to your gRPC Service class or method.

## Authorization
Metadata (headers) carry the Token (`Authorization: Bearer ...`).

## TLS/SSL
gRPC strongly prefers (or requires) HTTPS/TLS. It's safe by default.

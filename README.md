# 🦶 Barefoot API: Mastering  APIs in .NET 9

## 🌍 نظرة عامة على المشروع (Project Overview)

يحتوي هذا المستودع على مشروعين رئيسيين يوضحان طريقتين مختلفتين لبناء واجهات برمجة التطبيقات (APIs) في .NET 9:

### 1. MinAPI (Minimal API)
هو نمط حديث في .NET يهدف إلى تقليل التعقيد والملفات الزائدة (boilerplate). يعتمد على كتابة Endpoints مباشرة في `Program.cs` أو ملفات ملحقة بسيطة.
*   **الأسرع (Faster)**: أداء أعلى قليلاً واستهلاك أقل للذاكرة نظرًا لقلة الطبقات (Layers).
*   **الأسهل (Easier)**: مثالي للمبتدئين والمشاريع الصغيرة (Microservices) لسرعة التطوير.
*   **[🔗 الذهاب إلى MinAPI Wiki](MinAPI/Wiki/Home.md)**

### 2. MVCAPI (Model-View-Controller API)
هو النمط التقليدي والمنظم حيث يتم فصل الكود إلى Controllers و Models وغيرها.
*   **الأفضل (Better for Complex Apps)**: يوفر هيكلية واضحة جداً للمشاريع الضخمة والفرق الكبيرة، مما يسهل الصيانة على المدى الطويل.
*   **أكثر تنظيماً**: يفصل المسؤوليات بشكل صارم (Seperation of Concerns).
*   **[🔗 الذهاب إلى MVCAPI Wiki](MVCAPI/Wiki/Home.md)**

### 3. gRPC (gRPC Service)
هو إطار عمل عالي الأداء يعتمد على العقود (Protobuf) بدلاً من JSON.
*   **الأسرع إطلاقاً (The Fastest)**: يستخدم HTTP/2 وبيانات ثنائية (Binary) مما يجعله أسرع بكثير من REST.
*   **الاتصال الصارم (Strict Contract)**: العميل والخادم يلتزمان بملف `.proto` ومثالي للـ Microservices الداخلية.
*   **[🔗 الذهاب إلى gRPC Wiki](gRPC/Wiki/Home.md)**

---

### 🚀 أيهما "أفضل"؟
*   إذا كنت تبحث عن **السرعة والبساطة** (خاصة في Microservices)، فإن **Minimal API** هو الخيار الأفضل.
*   إذا كنت تبحث عن **هيكلة صارمة** لمشروع مؤسسي ضخم، فإن **MVC API** (via Controllers) قد يكون الخيار الأنسب.
*   إذا كنت تبحث عن **أقصى أداء واتصال داخلي** (Server-to-Server)، فإن **gRPC** هو الملك.

---


Welcome to the **Barefoot API** course! This guide will take you step-by-step from zero to a professional, production-ready Minimal API using clean architecture principles.

## 📚 Course Modules


### ⚡ MinAPI Modules

| Module | Title | Description |
| :--- | :--- | :--- |
| **[Module 1: Fundamentals](MinAPI/Wiki/module_01.md)** | **Project Setup & Basics** | Environment setup, project creation, directory structure, and your first "Hello World". |
| **[Module 2: The First Steps](MinAPI/Wiki/module_02.md)** | **Static Data & Basic Endpoints** | Working with in-memory data, parameter binding, and customizing Swagger documentation. |
| **[Module 3: Database Integration](MinAPI/Wiki/module_03.md)** | **EF Core & Clean Architecture** | Connecting to SQLite/SQL Server, defining Models, Context, and running Migrations. |
| **[Module 4: Professional Patterns](MinAPI/Wiki/module_04.md)** | **Services, DTOs & Mapping** | Decoupling logic with Services, using DTOs to hide internal models, and AutoMapper. |
| **[Module 5: Data Mastery](MinAPI/Wiki/module_05.md)** | **Search, Sort & Pagination** | Handling large datasets efficiently with filtering, sorting, and pagination. |
| **[Module 6: Robustness](MinAPI/Wiki/module_06.md)** | **Validation, Error Handling & Files** | Validating inputs with FluentValidation, global error handling, and file uploads. |
| **[Module 7: Security](MinAPI/Wiki/module_07.md)** | **Authentication & Authorization** | Securing your API with JWT Bearer tokens and configuring CORS. |

### 🏗️ MVCAPI Modules

| Module | Title | Description |
| :--- | :--- | :--- |
| **[Module 1: Fundamentals](MVCAPI/Wiki/module_01.md)** | **Project Setup & Basics** | Environment setup, project creation, directory structure, and your first Controller. |
| **[Module 2: The First Steps](MVCAPI/Wiki/module_02.md)** | **Static Data & Basic Controllers** | Working with in-memory data, Action Results, and standard HTTP responses. |
| **[Module 3: Database Integration](MVCAPI/Wiki/module_03.md)** | **EF Core & Clean Architecture** | Connecting to SQLite/SQL Server, defining Models, Context, and running Migrations. |
| **[Module 4: Professional Patterns](MVCAPI/Wiki/module_04.md)** | **Services, DTOs & Mapping** | Decoupling logic with Services, using DTOs to hide internal models, and AutoMapper. |
| **[Module 5: Data Mastery](MVCAPI/Wiki/module_05.md)** | **Search, Sort & Pagination** | Handling large datasets efficiently with filtering, sorting, and pagination via Controllers. |
| **[Module 6: Robustness](MVCAPI/Wiki/module_06.md)** | **Validation, Error Handling & Files** | Validating inputs with FluentValidation, Action Filters, global error handling, and file uploads. |
| **[Module 7: Security](MVCAPI/Wiki/module_07.md)** | **Authentication & Authorization** | Securing your Controllers with `[Authorize]`, JWT Bearer tokens, and CORS. |

### 🦅 gRPC Modules

| Module | Title | Description |
| :--- | :--- | :--- |
| **[Module 1: Fundamentals](gRPC/Wiki/module_01.md)** | **Setup & Protobuf** | Project setup, folder structure, and understanding `.proto` files. |
| **[Module 2: Protobuf & Messages](gRPC/Wiki/module_02.md)** | **Data Contracts** | Defining Messages, Services, and generating C# stubs. |
| **[Module 3: Database Integration](gRPC/Wiki/module_03.md)** | **EF Core & Clean Architecture** | Using `DbContext` within gRPC Services. |
| **[Module 4: Professional Patterns](gRPC/Wiki/module_04.md)** | **DTOs & AutoMapper** | Mapping internal Entities to external Protobuf Messages. |
| **[Module 5: Streaming](gRPC/Wiki/module_05.md)** | **High Performance Data** | Server Streaming, Client Streaming, and Bi-directional communication. |
| **[Module 6: Robustness](gRPC/Wiki/module_06.md)** | **Interceptors & Error Handling** | Validation and Exception Handling using Interceptors. |
| **[Module 7: Security](gRPC/Wiki/module_07.md)** | **Authentication & Authorization** | Securing gRPC calls with JWT Bearer tokens. |

---

## 🧪 How to Test gRPC

Since gRPC uses HTTP/2 and Protobuf, you cannot test it directly in a browser like REST APIs. You need a **gRPC Client**.

### 1️⃣ Run the Server
First, start your gRPC project:

```powershell
cd gRPC
dotnet watch run
```
*It will listen on `https://localhost:7021` (or verify in `Properties/launchSettings.json`).*

### 2️⃣ Create & Run a Client
You can create a simple console app to test your server:

1.  **Create the Client Project**:
    ```powershell
    dotnet new console -n gRPCClient
    cd gRPCClient
    dotnet add package Grpc.Net.Client
    dotnet add package Google.Protobuf
    dotnet add package Grpc.Tools
    ```

2.  **Add Reference**:
    Edit `gRPCClient.csproj` and add the Proto reference:
    ```xml
    <ItemGroup>
      <Protobuf Include="..\gRPC\Protos\greet.proto" GrpcServices="Client" />
    </ItemGroup>
    ```

3.  **Call the Service**:
    In `Program.cs`:
    ```csharp
    using Grpc.Net.Client;
    using gRPC;

    using var channel = GrpcChannel.ForAddress("https://localhost:7021");
    var client = new Greeter.GreeterClient(channel);
    var reply = await client.SayHelloAsync(new HelloRequest { Name = "Barefoot User" });
    Console.WriteLine(reply.Message);
    ```

4.  **Run**:
    ```powershell
    dotnet run
    ```

---

## 🛠️ Tech Stack
- **.NET 9**
- **Entity Framework Core**
- **Sqlite / SQL Server**
- **AutoMapper**
- **FluentValidation**
- **Swagger / OpenAPI**

> [!NOTE]
> This course is based on the **Barefoot API** project architecture.

## 📥 Downloads
* [**Download Course PDF**](MinAPI/Wiki/assets/MinimalAPI.pdf)
* [**Google NotebookLM (Interactive Audio)**](https://notebooklm.google.com/notebook/b1e53748-2a0c-42e8-9fca-36210af3f31b)
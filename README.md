# 🦶 Barefoot API: Mastering Minimal APIs in .NET 9

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

---

### 🚀 أيهما "أفضل"؟
*   إذا كنت تبحث عن **السرعة والبساطة** (خاصة في Microservices)، فإن **Minimal API** هو الخيار الأفضل.
*   إذا كنت تبحث عن **هيكلة صارمة** لمشروع مؤسسي ضخم، فإن **MVC API** (via Controllers) قد يكون الخيار الأنسب.

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
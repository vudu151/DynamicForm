# SƠ ĐỒ KIẾN TRÚC HỆ THỐNG DYNAMIC FORM

## I. KIẾN TRÚC TỔNG QUAN

### 1.1 Sơ đồ kiến trúc tổng thể (System Architecture)

```mermaid
graph TB
    subgraph "CLIENT LAYER"
        WEB[Web App<br/>React/Vue]
        MOBILE[Mobile App<br/>React Native]
        DESKTOP[Desktop App<br/>Electron]
    end
    
    subgraph "API GATEWAY LAYER"
        GATEWAY[API Gateway<br/>Authentication<br/>Rate Limiting<br/>Routing]
    end
    
    subgraph "APPLICATION LAYER"
        FORM_API[Form Service API<br/>ASP.NET Core]
        HIS_API[HIS Service]
        LIS_API[LIS Service]
        PACS_API[PACS Service]
    end
    
    subgraph "BUSINESS LOGIC LAYER"
        FORM_SVC[Form Service<br/>- Metadata Management<br/>- Validation Engine<br/>- Versioning]
        RENDER_SVC[Render Service<br/>- UI Generation<br/>- Field Mapping]
        VALIDATION_SVC[Validation Service<br/>- Rule Engine<br/>- Conditional Logic]
    end
    
    subgraph "DATA LAYER"
        DB[(Database<br/>SQL Server/PostgreSQL)]
        CACHE[(Redis Cache<br/>Metadata Cache)]
    end
    
    WEB --> GATEWAY
    MOBILE --> GATEWAY
    DESKTOP --> GATEWAY
    
    GATEWAY --> FORM_API
    GATEWAY --> HIS_API
    GATEWAY --> LIS_API
    GATEWAY --> PACS_API
    
    FORM_API --> FORM_SVC
    FORM_API --> RENDER_SVC
    FORM_API --> VALIDATION_SVC
    
    FORM_SVC --> DB
    FORM_SVC --> CACHE
    RENDER_SVC --> DB
    VALIDATION_SVC --> DB
```

## II. LUỒNG XỬ LÝ CHÍNH

### 2.1 Luồng Render Form

```mermaid
sequenceDiagram
    participant User
    participant Frontend
    participant API
    participant Service
    participant DB
    participant Cache
    
    User->>Frontend: Mở form (code: "PHIEU_KHAM")
    Frontend->>API: GET /api/forms/PHIEU_KHAM
    API->>Cache: Check cache
    alt Cache hit
        Cache-->>API: Return metadata
    else Cache miss
        API->>Service: GetFormMetadata(code)
        Service->>DB: Query Form + Version + Fields
        DB-->>Service: Metadata
        Service-->>API: Metadata
        API->>Cache: Store in cache
    end
    API-->>Frontend: JSON metadata
    Frontend->>Frontend: Render form dynamically
    Frontend-->>User: Display form
```

### 2.2 Luồng Submit Form với Validation

```mermaid
sequenceDiagram
    participant User
    participant Frontend
    participant API
    participant Validation
    participant Service
    participant DB
    
    User->>Frontend: Nhập dữ liệu + Submit
    Frontend->>Frontend: Client-side validation
    alt Validation fail
        Frontend-->>User: Show errors
    else Validation pass
        Frontend->>API: POST /api/form-data
        API->>Validation: Validate data
        Validation->>Validation: Check rules
        alt Server validation fail
            Validation-->>API: Errors
            API-->>Frontend: 400 Bad Request
            Frontend-->>User: Show errors
        else Server validation pass
            Validation-->>API: Success
            API->>Service: SaveFormData()
            Service->>DB: Insert FORM_DATA
            DB-->>Service: Success
            Service-->>API: Created
            API-->>Frontend: 201 Created
            Frontend-->>User: Success message
        end
    end
```

### 2.3 Luồng Versioning

```mermaid
sequenceDiagram
    participant Admin
    participant Frontend
    participant API
    participant Service
    participant DB
    
    Admin->>Frontend: Tạo version mới cho form
    Frontend->>API: POST /api/forms/{id}/versions
    API->>Service: CreateNewVersion(formId)
    Service->>DB: Copy current version
    Service->>DB: Create FORM_VERSION (version++)
    Service->>DB: Copy FORM_FIELD
    Service->>DB: Copy FIELD_VALIDATION
    Service->>DB: Update FORM.CurrentVersionId
    DB-->>Service: Success
    Service-->>API: New version
    API-->>Frontend: 201 Created
    Frontend-->>Admin: Success
    
    Note over DB: Version cũ vẫn giữ nguyên<br/>Data cũ gắn với version cũ
```

## III. KIẾN TRÚC CHI TIẾT

### 3.1 Backend Architecture (Layered Architecture)

```
┌─────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                    │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │ Controllers  │  │  Middleware  │  │   Filters    │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└──────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────┐
│                   APPLICATION LAYER                      │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │ FormService  │  │RenderService │  │ValidationSvc │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└──────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────┐
│                     DOMAIN LAYER                         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │  Entities    │  │  Interfaces  │  │ ValueObjects │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└──────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────┐
│                    INFRASTRUCTURE LAYER                   │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │  Repository  │  │   UnitOfWork │  │    Cache     │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└──────────────────────────────────────────────────────────┘
                          │
                    ┌─────▼─────┐
                    │  Database │
                    └───────────┘
```

### 3.2 Frontend Architecture (Component-based)

```
┌─────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                    │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │   Pages      │  │  Components  │  │    Layouts   │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└──────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────┐
│                    BUSINESS LAYER                       │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │   Hooks      │  │   Services   │  │    Utils     │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└──────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────┐
│                    DATA LAYER                            │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │  API Client  │  │   State Mgmt │  │    Cache     │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└──────────────────────────────────────────────────────────┘
```

## IV. COMPONENT DIAGRAM

### 4.1 Form Renderer Component

```mermaid
graph LR
    A[FormRenderer] --> B[FieldRenderer]
    A --> C[ValidationEngine]
    A --> D[ConditionalLogic]
    
    B --> E[TextField]
    B --> F[NumberField]
    B --> G[DateField]
    B --> H[SelectField]
    B --> I[CheckboxField]
    B --> J[RepeaterField]
    
    C --> K[RequiredValidator]
    C --> L[RangeValidator]
    C --> M[RegexValidator]
    C --> N[CustomValidator]
```

## V. DEPLOYMENT ARCHITECTURE

```mermaid
graph TB
    subgraph "PRODUCTION ENVIRONMENT"
        LB[Load Balancer]
        
        subgraph "WEB TIER"
            WEB1[Web Server 1]
            WEB2[Web Server 2]
        end
        
        subgraph "API TIER"
            API1[API Server 1]
            API2[API Server 2]
        end
        
        subgraph "DATABASE TIER"
            DB_PRIMARY[(Primary DB)]
            DB_REPLICA[(Replica DB)]
        end
        
        subgraph "CACHE TIER"
            REDIS[(Redis Cluster)]
        end
    end
    
    LB --> WEB1
    LB --> WEB2
    WEB1 --> API1
    WEB2 --> API2
    API1 --> DB_PRIMARY
    API2 --> DB_PRIMARY
    API1 --> REDIS
    API2 --> REDIS
    DB_PRIMARY --> DB_REPLICA
```

## VI. SECURITY ARCHITECTURE

```mermaid
graph TB
    CLIENT[Client] --> AUTH[Authentication<br/>JWT Token]
    AUTH --> AUTHORIZE[Authorization<br/>Role-based]
    AUTHORIZE --> API[API Endpoints]
    API --> VALIDATE[Input Validation]
    VALIDATE --> BUSINESS[Business Logic]
    BUSINESS --> AUDIT[Audit Log]
    BUSINESS --> DB[(Database)]
```

## VII. CÁC PATTERN SỬ DỤNG

1. **Repository Pattern**: Tách biệt data access
2. **Unit of Work**: Quản lý transaction
3. **Service Layer**: Business logic
4. **DTO Pattern**: Data transfer objects
5. **Factory Pattern**: Tạo field renderer
6. **Strategy Pattern**: Validation rules
7. **Observer Pattern**: Form events
8. **Caching Pattern**: Metadata caching

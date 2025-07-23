1.  **Overview**

**DiamondTransaction is a robust C\# WinForms application designed with a clean, layered architecture. The system is organized to promote maintainability, scalability, and clear separation of concerns, dividing responsibilities across the user interface, business logic, domain models, and data access.**

2.  **Layered Architecture**

**🔹 UI Layer**

-   **Comprises WinForms forms, controls, and UI utilities.**
-   **Manages all user interactions, data presentation, and input validation.**

**🔹 Use Cases / Services Layer**

-   **Encapsulates business logic and orchestrates application workflows.**
-   **Acts as a bridge between the UI and the underlying domain and data layers.**

**🔹 Domain Layer**

-   **Defines core business entities, value objects, and repository interfaces.**
-   **Represents the heart of the business logic, independent of UI and data storage.**

**🔹 Data Access Layer**

-   **Implements repository interfaces for data persistence and retrieval.**
-   **Handles all interactions with the underlying database using Dapper ORM.**
3.  **Project Structure**
-   **UI/: WinForms forms, controls, and UI utilities.**
-   **UseCases/: Application services, business logic, and service interfaces.**
-   **Domain/: Business entities and repository interfaces.**
-   **DataAccess/: Data access classes for CRUD operations.**
-   **Models/: Data Transfer Objects (DTOs) and supporting models.**
-   **bin/obj/: Build outputs and temporary files.**
-   **Properties/: Assembly and resource metadata.**
4.  **Key Components**
    1.  **UI Layer**
-   **Forms:**

    **fmDiamondTransaction, fmCertificate, fmLotFinder, etc.**

-   **Controls:**

    **Custom controls for certificate details, document headers, etc.**

-   **Utilities:**

    **Helpers for form control management and grid configuration.**

    1.  **Use Cases / Services**
-   **Service Classes:**

    **CertificateService, CustomerService, DiamondDocService, etc.**

-   **Responsibilities:**

    **Orchestrate business logic, coordinate between UI and data access, enforce business rules.**

    1.  **Domain Layer**
-   **Entities:**

    **DiamondGrades, DocType, ExchangeRate, ParcelGrades, etc.**

-   **Interfaces:**

    **Repository contracts (e.g., ICertificateRepository, IDiamondDocRepository).**

    1.  **Data Access Layer**
-   **Data Access Classes:**

    **CertificateDataAccess, CustomerDataAccess, DiamondDocDataAccess, etc.**

-   **Responsibilities:**

    **Implement repository interfaces, handle SQL queries, map data to domain models.**

    1.  **Models**
-   **DTOs:**

    **CertificateDto, Customer, DocHeaderDto, etc.**

-   **Purpose:**

    **Transfer data between layers, especially between data access and UI.**

5.  **Design Patterns & Principles**
-   **Layered Architecture:**

    **Clear separation of UI, business logic, domain, and data access.**

-   **Repository Pattern:**

    **Abstracts data access logic, enabling easier testing and maintenance.**

-   **DTO Pattern:**

    **Decouples domain models from data transfer representations.**

-   **Single Responsibility Principle:**

    **Each class has a focused responsibility (e.g., services, data access, UI).**

6.  **Data Flow**
7.  **User interacts with UI (forms/controls).**
8.  **UI calls Use Case/Service classes to perform business operations.**
9.  **Services use Domain Models and Repository Interfaces to process logic.**
10. **Data Access Layer implements repository interfaces, executes SQL, and returns DTOs.**
11. **Results are passed back up to the UI for display.**
12. **Folder/Component Descriptions**
-   **UI/**: All user interface components, including forms, controls, and UI helpers.
-   **UseCases/**: Application/business logic, service classes, and interfaces.
-   **Domain/**: Core business entities and repository contracts.
-   **DataAccess/**: Data access classes, repositories, and SQL helpers.
-   **Models/**: Shared DTOs and models used across layers.
-   **Properties/**: Project metadata, resources, and settings.
-   **Root C\# Files**: Main application logic, entry point, and global helpers.
-   **bin/, obj/**: Build outputs and intermediate files (auto-generated).

DiamondTransaction/

│

├── UI/ \# User Interface Layer (WinForms)

│ ├── Controls/ \# Custom user controls for forms (e.g., CertificateDetailView)

│ ├── Event/ \# Event argument classes for UI events

│ ├── GridConfig/ \# Grid and column configuration helpers

│ ├── Models/ \# UI-specific models (e.g., for grid formatting)

│ ├── Utilities/ \# UI helper classes (e.g., FormControlHelper)

│ ├── Certificate.cs \# Certificate form logic

│ ├── CertificateDetailView.cs \# Certificate detail view logic

│ ├── CertificateForm.cs \# Certificate form UI

│ ├── DiamondTransactionForm.cs \# Main transaction form

│ ├── DocDetailControls.cs \# Document detail controls

│ ├── fmCertificate.cs \# Certificate main form

│ ├── fmDiamondTransaction.cs \# Main diamond transaction form

│ ├── fmLotFinder.cs \# Lot finder form

│ ├── fmWorkingLine.cs \# Working line form

│ ├── LotFinder.cs \# Lot finder logic

│ ├── LotFinderForm.cs \# Lot finder form UI

│ ├── WorkingLine.cs \# Working line logic

│ └── WorkingLineForm.cs \# Working line form UI

│

├── UseCases/ \# Application/Business Logic Layer

│ ├── Interfaces/ \# Service and repository interfaces

│ ├── Models/ \# DTOs and business models

│ ├── Services/ \# Service implementations (e.g., CertificateService)

│ ├── CertificateService.cs \# Certificate business logic

│ ├── CertificateTypeService.cs \# Certificate type business logic

│ ├── CustomerService.cs \# Customer business logic

│ ├── DiamondDocService.cs \# Diamond document business logic

│ ├── DiamondGradingService.cs \# Diamond grading business logic

│ ├── DiamondLotService.cs \# Diamond lot business logic

│ ├── DocCreationService.cs \# Document creation logic

│ ├── DocHeaderService.cs \# Document header logic

│ ├── DocLineService.cs \# Document line logic

│ ├── PriceStockHistoryService.cs \# Price/stock history logic

│ ├── RapaportPriceService.cs \# Rapaport price logic

│ └── SupplierService.cs \# Supplier business logic

│

├── Domain/ \# Core Business Entities and Contracts

│ ├── Entities/ \# Domain entities (e.g., DiamondGrades, DocType)

│ └── Interfaces/ \# Domain-level repository interfaces

│

├── DataAccess/ \# Data Access Layer (Repositories, SQL, Dapper)

│ ├── CertificateDataAccess.cs \# Certificate data access

│ ├── CertificateTypeDataAccess.cs \# Certificate type data access

│ ├── CustomerDataAccess.cs \# Customer data access

│ ├── DiamondDocDataAccess.cs \# Diamond document data access

│ ├── DiamondDocRepository.cs \# Diamond document repository

│ ├── DiamondGradingDataAccess.cs \# Diamond grading data access

│ ├── DiamondLotDataAccess.cs \# Diamond lot data access

│ ├── DocHeaderDataAccess.cs \# Document header data access

│ ├── DocLineDataAccess.cs \# Document line data access

│ ├── PriceStockHistoryDataAccess.cs \# Price/stock history data access

│ ├── RapaportDataAccess.cs \# Rapaport data access

│ ├── SqlQueryExecutor.cs \# SQL query execution helper

│ └── SupplierDataAccess.cs \# Supplier data access

│

├── Models/ \# Shared Data Transfer Objects (DTOs) and Models

│ ├── CertificateDto.cs \# Certificate DTO

│ ├── CertificateTypeDto.cs \# Certificate type DTO

│ ├── Customer.cs \# Customer DTO/model

│ ├── CustomerBranch.cs \# Customer branch DTO/model

│ ├── DiamondLotMaxInfo.cs \# Diamond lot max info DTO

│ ├── DiamondSizeDto.cs \# Diamond size DTO

│ ├── DocHeaderDto.cs \# Document header DTO

│ ├── DocHeaderMaxInfo.cs \# Document header max info DTO

│ ├── DocHeaderSubDto.cs \# Document header sub DTO

│ ├── DocLineDto.cs \# Document line DTO

│ ├── DocType.cs \# Document type DTO

│ └── Supplier.cs \# Supplier DTO/model

│

├── Properties/ \# Assembly info and resources

│ ├── AssemblyInfo.cs \# Assembly metadata

│ ├── Resources.Designer.cs \# Resource designer

│ ├── Resources.resx \# Resource file

│ ├── Settings.Designer.cs \# Settings designer

│ └── Settings.settings \# Application settings

│

├── AccountDataManager.cs \# Account data management logic

├── App.config \# Application configuration file

├── Certificate.cs \# Certificate logic

├── CertificateData.cs \# Certificate data logic

├── CertificateDetail.cs \# Certificate detail logic

├── ColumnManager.cs \# Column management logic

├── ControlNameAndTextManager.cs \# Control name/text management

├── DevTHLStoreDataSet.\* \# DataSet definitions and resources

├── DiamondTransaction.cs \# Main application logic

├── DiamondTransaction.csproj \# Project file

├── DocDetailControls.cs \# Document detail controls

├── fmDiamondTransaction.cs \# Main form logic

├── GlobalClass.cs \# Global utility class

├── GlobalDataManager.cs \# Global data management

├── LotFinder.cs \# Lot finder logic

├── Models/ \# (see above)

├── Program.cs \# Application entry point

├── SetDocumentInfo.cs \# Document info setting logic

├── WorkingLine.cs \# Working line logic

├── packages.config \# NuGet package references

└── bin/, obj/ \# Build outputs and temporary files

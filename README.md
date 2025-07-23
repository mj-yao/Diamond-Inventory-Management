**DiamondTransaction**

**DiamondTransaction** is a C\# WinForms application designed to manage diamond transactions, certificates, and related transaction processes.

**1. Key Features**

**1.1 Main Modules**

**1.1.1 DiamondTransaction**

-   Supports multiple transaction types:
    -   **Goods In** – Record and manage incoming diamond stock.
    -   **Memo In** – Track diamonds received on consignment.
    -   **Memo Out** – Manage diamonds sent out on consignment.
    -   **Invoice** – Generate and manage sales invoices.
-   Handles document creation, editing, and tracking.
-   Integrated with customer and supplier modules.

**1.1.2 Diamond Certificate**

-   Retrieves certificate details from major labs:
    -   GIA (Gemological Institute of America)
    -   IGI (International Gemological Institute)
-   Allows lookup by certificate number.
-   Stores key attributes: grading, shape, carat, color, clarity, etc.

**1.1.3 WorkingLine**

-   Manages diamond lots, certificates, and line-level transaction data.
-   Features an editable grid with automatic recalculations.
-   Supports creation of new diamond lots and assignment of certificates.

**1.1.4 DiamondLot Finder**

-   Advanced search functionality using:
    -   Grading (carat, color, clarity, cut, shape)
    -   Certificate information
    -   Availability and stock status
-   Provides quick access to lot details for selection and allocation.

**1.2 Customer & Supplier Management**

-   Manage detailed customer and supplier records.
-   Supports branch and contact information.
-   Fully integrated with transaction and certificate modules.

**1.3 Extensible Grid & UI**

-   Customizable grid columns and formats.
-   Clean and user-friendly WinForms interface.

**1.4 Data Management**

-   Uses DTOs and Repository Pattern.
-   Efficient data operations via Dapper ORM.

**1.5 Additional Features**

-   Configuration handled via App.config.
-   Localization support through resource management.
-   Modular design for easy expansion.

**2. Getting Started**

-   **Clone the repository**: <https://github.com/mj-yao/Diamond-Inventory-Management.git>
-   **Open the solution** in Visual Studio.
-   **Install required NuGet packages.**
-   **Build and run** the solution.

**3. Contributing**

Contributions are welcome!  
Please open issues or submit pull requests for improvements or bug fixes.

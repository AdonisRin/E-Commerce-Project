# Lift & Aesthetics — Gym Essentials E-Commerce Store

> **Academic Year 2026–2027 • E-Commerce Semester Project**  
> **Presentation 3 — Final Product (Weeks 11–12)**  
> **Live Demo • Automated Tests • Code Review • Project Retrospective**

---

## Team Members & Responsibilities

| Student | GitHub Username / Email | Role & Module Ownership |
|---|---|---|
| **Peca Ana Maria Cristina** | `peca-ana-ace-ucv` / `peca.ana.p2f@student.ucv.ro` | **Frontend & Product Discovery**<br>Catalog pagination (A1), Search & empty state (A2), Combinable filters (B1), Wishlist UI (B7), 360 px Responsive design (A11) |
| **Ploae Darius Adonis Ionuț** | `DariusAdonisIonut` / `dariusploae@yahoo.com` | **Backend, Cart & Payments**<br>Persistent shopping cart (A3), 4-step Checkout pipeline (A5), Sandbox payment & webhook (A6), Concurrency stock management (A7), Promo codes (B5), Email receipt service (B6) |
| **Nică David Andrei** | `NicaDavidAndrei` / `nica.david.p7c@student.ucv.ro` | **Database, Accounts & Admin**<br>Relational schema, EF Core migrations & seed data, User accounts & addresses (A4), Order history (A8), Admin panel & status transitions (A9), Security (A10), Sales analytics (B4), Test suite (A12) |

---

## 1. Quick Start & Running Instructions

The project runs locally in **fewer than 5 commands**:

```bash
# 1. Clone repository
git clone https://github.com/DariusAdonisIonut/E-Commerce-Project.git

# 2. Navigate to web application directory
cd E-Commerce-Project/LiftAndAesthetics/LiftAndAesthetics

# 3. Restore dependencies & build
dotnet restore

# 4. Run application (database and seed data are created automatically)
dotnet run
```

Open your browser at: **`http://localhost:5189`**

> **Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/) and Microsoft SQL Server / LocalDB (included with Visual Studio / VS Code C# Dev Kit).

---

## 2. Automated Test Suite (A12)

Run the full automated test suite with a single command:

```bash
dotnet test LiftAndAesthetics/LiftAndAesthetics.Tests/LiftAndAesthetics.Tests.csproj
```

### Test Results
- **Total Tests:** 20
- **Passed:** 20 (100%)
- **Failed / Skipped:** 0
- **Execution Time:** ~250 ms
- **Coverage Areas:** Shopping cart additions/updates/removals, server-side subtotal recalculations, warehouse stock boundary enforcement, percentage and fixed promo code validation, sandbox payment webhook confirmation, atomic stock decrement, cancellation inventory restoration, and SemaphoreSlim / mutex race-condition concurrency protection during simultaneous checkout.

---

## 3. Seeded Test Accounts

The database is automatically initialized and seeded on first run with the following pre-configured test users:

| Email | Password | Role | Wallet Balance | Pre-saved Delivery Address |
|---|---|---|---|---|
| **`admin@lift.com`** | `Testing1` | **Admin** | 99,999 LEI | Strada Victoriei Nr. 42, Craiova |
| **`dariusploae@yahoo.com`** | `Testing1` | **Admin** | 15,000 LEI | Strada Victoriei Nr. 42, Craiova |
| **`athlete@lift.com`** | `Testing1` | **User** | 6,767 LEI | Strada Victoriei Nr. 42, Craiova |
| **`girl@lift.com`** | `Testing1` | **User** | 6,767 LEI | Bulevardul 1 Mai Nr. 15, Craiova |
| **`peca.ana.p2f@student.ucv.ro`** | `Testing1` | **User** | 5,000 LEI | Bulevardul Decebal Nr. 12, Craiova |
| **`nica.david.p7c@student.ucv.ro`** | `Testing1` | **User** | 5,000 LEI | Strada Nicolae Titulescu Nr. 8, Craiova |

*Note: New customers can also register freely at `/Auth/Register`.*

---

## 4. Mandatory Core Requirements (A1–A12)

All 12 requirements are **100% complete, verified, and ready for live demonstration**:

- **A1. Product Catalog:** Paginated view (max 24/page, default 12), category-specific pages (Supplements, Apparel, Equipment, Accessories), and individual product details (images, SKU, pricing, stock availability, star ratings).
- **A2. Live AJAX Search:** Live debounced search-as-you-type on product names (without needing to press Enter) with instant partial DOM update, an informative empty state ("No products found"), and instant reset.
- **A3. Shopping Cart:** Add, increment/decrement quantities, remove items. Line items and totals are strictly computed on the server. Quantities exceeding available warehouse stock are rejected. Authenticated cart state persists across logout/login.
- **A4. User Accounts:** Registration, login, and logout. Passwords securely hashed with BCrypt / Argon2 via ASP.NET Core Identity. Profile supports saving and managing default delivery addresses.
- **A5. Checkout Pipeline:** Multi-step wizard:
  1. Recipient & Delivery Address
  2. Delivery Method (Standard Courier 15 LEI vs Express Courier 25 LEI)
  3. Server Order Summary & Promo Code validation
  4. Payment Gateway selection. Order confirmation displays unique reference number (e.g. `ORD-20261110-4220`).
- **A6. Sandbox Payment:** Order becomes `Paid` ONLY upon server confirmation webhook/endpoint tokenization. Browser redirection alone cannot mark an order as paid. Payment simulation failure leaves the order `Unpaid`.
- **A7. Stock Management:** Concurrency-safe inventory decrement executed atomically with `SemaphoreSlim` async mutex locks. Two customers purchasing the last unit simultaneously fail gracefully with zero negative stock. Stock is automatically restored if an order is cancelled.
- **A8. Order History:** Customer dashboard at `/Orders` displaying order date, products purchased, quantities, grand total, and lifecycle status.
- **A9. Administration Panel:** Gated `Admin` role access. Full CRUD for products and categories with image upload. Filterable order list with lifecycle transitions: `New` → `Paid` → `Shipped` → `Delivered` (or `Cancelled`).
- **A10. Basic Security:** Anti-Forgery CSRF tokens enforced on all POST/PUT requests, server-side DataAnnotations validation, EF Core parameterized queries (immune to SQL injection), and controller-level endpoint authorization (`[Authorize(Roles = "Admin")]`).
- **A11. Responsive Design:** Mobile-first dark aesthetic, fully usable and verified on 360 px viewports (Samsung Galaxy / iPhone SE) with no horizontal scroll.
- **A12. Automated Tests:** 20 unit & integration tests run via `dotnet test` in < 300 ms.

---

## 5. Implemented Bonus Features (B1–B7) — 27 Points (Max 20)

| Bonus Code | Feature | Points | Implementation Details & Verification Steps |
|---|---|---|---|
| **B1** | **Filtered Search** | **5 pts** | Combinable filters by category, price range (with defensive clamping `[0, 999999]`), and in-stock only. Sorting by price (asc/desc) and date added. **Filters are preserved in the URL query string** (`/Shop?category=Supplements&minPrice=50&maxPrice=250&sort=price_asc`). |
| **B2** | **Recommendations** | **4 pts** | Algorithmic cross-sell computed from data: **"Frequently Bought Together"** calculated from historical order item co-occurrence; **"Similar Products"** computed from category attributes and price tiers. |
| **B3** | **Reviews & Ratings** | **4 pts** | **Only customers who purchased the product can submit a review** (verified purchase check). Star rating (1–5) aggregated and displayed in the catalog. Administrators can toggle visibility to hide inappropriate reviews. |
| **B4** | **Sales Dashboard** | **3 pts** | Interactive Chart.js analytics dashboard at `/Shop/Dashboard`: Daily sales revenue trend, Top 5 best-selling products by quantity, and Average Order Value (AOV) for customizable intervals (7 days, 30 days, all-time). |
| **B5** | **Discount Codes** | **3 pts** | Server-validated promo codes: percentage (`LIFT10` 10%, `AESTHETICS20` 20%) and fixed-value (`WELCOME25` 25 LEI). Validates active dates, minimum spend, and maximum usage limits on the server before payment. |
| **B6** | **Transactional Email** | **5 pts** | Order confirmation receipt dispatched via **Mailtrap Sandbox** (as requested by curriculum) or SMTP, plus automatic generation of offline high-contrast HTML receipts in `wwwroot/receipts/{OrderNumber}.html`. Direct button *"View Dispatched Email Receipt"* on confirmation page. |
| **B7** | **Wishlist** | **3 pts** | Customer favorites list at `/Wishlist` for authenticated members. One-click "Add to Wishlist" and direct **"Move to Cart"** functionality that transfers the item to the active cart and removes it from favorites. |

**Total Bonus Points:** **27 pts** (Full score capped at 20 points).

---

## 6. Live Evaluation Fixed Scenario (Demo Script)

Follow this 5-minute fixed scenario during the live evaluation:

1. **Customer Registration & Search (A2, A4):**
   - Register a new user at `/Auth/Register` (or log in as `athlete@lift.com` / `Testing1`).
   - In the shop search bar, type `protein` or `creatine` and press Enter. Verify accurate results.
2. **Shopping Cart & Discount Code (A3, B5):**
   - Click a product, select quantity, and click **Add to Cart**.
   - Navigate to `/Cart`. Observe server subtotal recalculation.
   - Enter promo code `LIFT10` and click **Apply Code**. Verify 10% discount is applied.
3. **Multi-Step Checkout & Payment (A5, A6):**
   - Click **Proceed to Checkout** (`/Checkout`).
   - Confirm recipient address (pre-filled from profile).
   - Select Courier Method (Standard 15 LEI vs Express 25 LEI).
   - Proceed to payment simulation and click **Confirm Payment** (Sandbox Test Card).
4. **Order Confirmation, Email & Stock Decrement (A6, A7, B6):**
   - Order transitions to `Paid` with a unique order number.
   - Product warehouse stock decrements by purchased quantity.
   - Click **View Dispatched Email Receipt** to view the generated HTML invoice.
5. **Admin Order Lifecycle & Sales Analytics (A9, B4):**
   - Sign in as `admin@lift.com` / `Testing1`.
   - Open **Orders** (`/Shop/Orders`). Locate the new order and update its status: `Paid` → `Shipped` → `Delivered`.
   - Open **Analytics** (`/Shop/Dashboard`). Demonstrate the updated Chart.js daily revenue curve, Top 5 products, and AOV.

---

## 7. AI Tools Disclosure

In compliance with academic integrity guidelines, the following AI tools were utilized during development:

- **Antigravity IDE (Google DeepMind):** Used for architectural guidance, repository-wrapper scaffolding, automated test generation, and high-contrast dark theme UI refactoring.
- **GitHub Copilot / Gemini 2.5 Flash:** Used for boilerplate code completion, Razor view formatting, and drafting initial test assertions.

*All system architecture, security validation, business rules, transactional guarantees, and final code reviews were authored, verified, and tested by the team members.*

---

## 8. Deliverables & Git Tag

- **Presentation Slide Deck:** [`Documentations/Lift_and_Aesthetics_Presentation3.pptx`](Documentations/Lift_and_Aesthetics_Presentation3.pptx) (16 widescreen slides covering live demo, test results, code review, and retrospective).
- **Git Commit Tag:** **`P3`** (Tagged on the final production-ready commit).

import os
from reportlab.lib.pagesizes import letter
from reportlab.lib import colors
from reportlab.platypus import (
    SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, Image, PageBreak, KeepTogether, HRFlowable
)
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.pdfgen import canvas

class NumberedCanvas(canvas.Canvas):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self._saved_page_states = []

    def showPage(self):
        self._saved_page_states.append(dict(self.__dict__))
        self._startPage()

    def save(self):
        num_pages = len(self._saved_page_states)
        for state in self._saved_page_states:
            self.__dict__.update(state)
            self.draw_page_decorations(num_pages)
            super().showPage()
        super().save()

    def draw_page_decorations(self, total_pages):
        self.saveState()
        self.setFont("Helvetica", 8)
        self.setFillColor(colors.HexColor("#64748b"))

        # Skip on cover page
        if self._pageNumber > 1:
            # Header
            self.drawString(54, 11 * 72 - 36, "Lift & Aesthetics — Project Specification Document (Milestone P2)")
            self.setStrokeColor(colors.HexColor("#cbd5e1"))
            self.setLineWidth(0.5)
            self.line(54, 11 * 72 - 42, 8.5 * 72 - 54, 11 * 72 - 42)

            # Footer
            page_text = f"Page {self._pageNumber} of {total_pages}"
            self.drawRightString(8.5 * 72 - 54, 36, page_text)
            self.drawString(54, 36, "CONFIDENTIAL & PROPRIETARY — ACADEMIC YEAR 2025-2026")
            self.line(54, 46, 8.5 * 72 - 54, 46)

        self.restoreState()

def build_pdf():
    pdf_filename = r"c:\Users\Adonis\Downloads\LiftAndAestheticsMilestone2\Lift_and_Aesthetics_Specification_P2.pdf"
    doc = SimpleDocTemplate(
        pdf_filename,
        pagesize=letter,
        leftMargin=54,
        rightMargin=54,
        topMargin=54,
        bottomMargin=54
    )

    styles = getSampleStyleSheet()
    
    # Custom Palette
    COLOR_PRIMARY = colors.HexColor("#0f172a") # Dark Slate
    COLOR_CYAN = colors.HexColor("#0284c7")    # Deep Sky Blue
    COLOR_TEXT = colors.HexColor("#1e293b")    # Slate 800
    COLOR_MUTED = colors.HexColor("#64748b")   # Slate 500
    COLOR_BG_ALT = colors.HexColor("#f8fafc")  # Off-white

    title_style = ParagraphStyle(
        'DocTitle',
        parent=styles['Heading1'],
        fontName='Helvetica-Bold',
        fontSize=24,
        leading=28,
        textColor=COLOR_PRIMARY,
        spaceAfter=6
    )

    subtitle_style = ParagraphStyle(
        'DocSub',
        parent=styles['Normal'],
        fontName='Helvetica',
        fontSize=12,
        leading=16,
        textColor=COLOR_CYAN,
        spaceAfter=15
    )

    h1_style = ParagraphStyle(
        'H1',
        parent=styles['Heading1'],
        fontName='Helvetica-Bold',
        fontSize=14,
        leading=18,
        textColor=COLOR_PRIMARY,
        spaceBefore=12,
        spaceAfter=6,
        keepWithNext=True
    )

    h2_style = ParagraphStyle(
        'H2',
        parent=styles['Heading2'],
        fontName='Helvetica-Bold',
        fontSize=11,
        leading=15,
        textColor=COLOR_CYAN,
        spaceBefore=8,
        spaceAfter=4,
        keepWithNext=True
    )

    body_style = ParagraphStyle(
        'Body',
        parent=styles['Normal'],
        fontName='Helvetica',
        fontSize=9,
        leading=12.5,
        textColor=COLOR_TEXT,
        spaceAfter=5
    )

    bullet_style = ParagraphStyle(
        'Bullet',
        parent=body_style,
        leftIndent=12,
        firstLineIndent=-8,
        spaceAfter=4
    )

    table_text = ParagraphStyle(
        'TableText',
        parent=styles['Normal'],
        fontName='Helvetica',
        fontSize=8,
        leading=10.5,
        textColor=COLOR_TEXT
    )

    table_header = ParagraphStyle(
        'TableHeader',
        parent=styles['Normal'],
        fontName='Helvetica-Bold',
        fontSize=8.5,
        leading=11,
        textColor=colors.white
    )

    story = []

    # =========================================================================
    # PAGE 1: TITLE & PROJECT OVERVIEW & SCOPE FREEZING STATEMENT
    # =========================================================================
    story.append(Paragraph("Lift & Aesthetics", title_style))
    story.append(Paragraph("Milestone 2 Specification Document — Frozen Specification & Prototype (Week 7)", subtitle_style))
    story.append(HRFlowable(width="100%", thickness=1.5, color=COLOR_CYAN, spaceAfter=12))

    # Meta Info Table
    meta_data = [
        [Paragraph("<b>Course / Project:</b> E-Commerce Semester Project", table_text),
         Paragraph("<b>Target Domain:</b> Fitness Essentials & Supplements", table_text)],
        [Paragraph("<b>Technology Stack:</b> ASP.NET Core 10, EF Core, SQL Server/MySQL, Stripe Sandbox", table_text),
         Paragraph("<b>Repository Tag:</b> P2 (Frozen Specification)", table_text)],
        [Paragraph("<b>Team Lead & Frontend:</b> Peca Ana Maria Cristina", table_text),
         Paragraph("<b>Backend & Payments:</b> Ploae Darius Adonis Ionuț", table_text)],
        [Paragraph("<b>Database & Security:</b> Nica David Andrei", table_text),
         Paragraph("<b>Date & Milestone:</b> October 2026 / Week 7 Submission", table_text)]
    ]
    t_meta = Table(meta_data, colWidths=[250, 254])
    t_meta.setStyle(TableStyle([
        ('BACKGROUND', (0,0), (-1,-1), COLOR_BG_ALT),
        ('BOX', (0,0), (-1,-1), 1, colors.HexColor("#e2e8f0")),
        ('INNERGRID', (0,0), (-1,-1), 0.5, colors.HexColor("#e2e8f0")),
        ('TOPPADDING', (0,0), (-1,-1), 5),
        ('BOTTOMPADDING', (0,0), (-1,-1), 5),
        ('LEFTPADDING', (0,0), (-1,-1), 8),
        ('RIGHTPADDING', (0,0), (-1,-1), 8),
    ]))
    story.append(t_meta)
    story.append(Spacer(1, 10))

    story.append(Paragraph("1. Executive Summary & Scope Freezing Statement", h1_style))
    story.append(Paragraph(
        "<b>Goal:</b> This document freezes the functional and technical specification for <b>Lift & Aesthetics</b>. "
        "As mandated by the course regulations for Presentation 2 (Week 7), the requirements, architectural topology, "
        "relational schemas, and chosen bonus features specified herein represent the final contractual scope against which "
        "Milestone P3 (Final Demo) will be graded. Any future modification must be formally requested in writing on the course channel.",
        body_style
    ))
    story.append(Paragraph(
        "<b>The Product:</b> Lift & Aesthetics is a specialized e-commerce store dedicated to gym enthusiasts, powerlifters, and athletes. "
        "The application provides a seamless catalog of sports nutrition (whey isolates, creatine, pre-workouts), aesthetics-driven apparel, "
        "and heavy lifting equipment (belts, straps, sleeves). The system enforces robust server-side validation, transactional inventory integrity, "
        "and sandbox payment processing via Stripe.",
        body_style
    ))

    story.append(Paragraph("2. Chosen Bonus Features (with Course Codes)", h1_style))
    bonus_data = [
        [Paragraph("Code", table_header), Paragraph("Bonus Feature", table_header), Paragraph("Lead Owner", table_header), Paragraph("Technical Specification", table_header)],
        [
            Paragraph("<b>B1</b>", table_text),
            Paragraph("<b>Filtered Search</b>", table_text),
            Paragraph("Ana Maria (M1)", table_text),
            Paragraph("Multi-facet catalog querying. Users can simultaneously filter by Category slug, apply a min/max Price slider, toggle 'In Stock Only', and sort by Price (asc/desc), Name, or Newest. Implemented and demonstrated in the P2 prototype.", table_text)
        ],
        [
            Paragraph("<b>B5</b>", table_text),
            Paragraph("<b>Discount Codes</b>", table_text),
            Paragraph("Darius Adonis (M2)", table_text),
            Paragraph("Promotional coupon engine supporting percentage discounts (e.g. LIFT10 - 10%) and fixed amounts (e.g. WELCOME5 - $5). Evaluates minimum order spend thresholds, expiration dates, and redemption limits dynamically.", table_text)
        ],
        [
            Paragraph("<b>B7</b>", table_text),
            Paragraph("<b>Wishlist</b>", table_text),
            Paragraph("Ana Maria (M1) + David (M3)", table_text),
            Paragraph("Authenticated customer wishlist allowing bookmarking of equipment and supplements directly from product cards and detail pages, with 1-click addition into the active shopping cart.", table_text)
        ]
    ]
    t_bonus = Table(bonus_data, colWidths=[35, 85, 95, 289])
    t_bonus.setStyle(TableStyle([
        ('BACKGROUND', (0,0), (-1,0), COLOR_PRIMARY),
        ('BOX', (0,0), (-1,-1), 1, colors.HexColor("#cbd5e1")),
        ('INNERGRID', (0,0), (-1,-1), 0.5, colors.HexColor("#e2e8f0")),
        ('TOPPADDING', (0,0), (-1,-1), 4),
        ('BOTTOMPADDING', (0,0), (-1,-1), 4),
        ('LEFTPADDING', (0,0), (-1,-1), 6),
        ('RIGHTPADDING', (0,0), (-1,-1), 6),
    ]))
    story.append(t_bonus)

    # =========================================================================
    # PAGE 2: REQUIREMENTS SPECIFICATION (A1 - A12)
    # =========================================================================
    story.append(PageBreak())
    story.append(Paragraph("3. Functional Requirements Specification (A1 – A12)", h1_style))
    story.append(Paragraph(
        "Each requirement below specifies its exact technical implementation in 1–2 concise sentences along with designated team ownership.",
        body_style
    ))

    req_data = [
        [Paragraph("Req", table_header), Paragraph("Requirement Title", table_header), Paragraph("Lead", table_header), Paragraph("Implementation Specification (1–2 Sentences)", table_header)],
        [
            Paragraph("<b>A1</b>", table_text),
            Paragraph("<b>Product Catalog</b>", table_text),
            Paragraph("Ana Maria (M1)<br/>David (M3)", table_text),
            Paragraph("Paginated catalog listing (up to 24 products per page) with separate category views and dedicated product pages showing name, description, unit price, high-res images, and stock availability.", table_text)
        ],
        [
            Paragraph("<b>A2</b>", table_text),
            Paragraph("<b>Basic Search</b>", table_text),
            Paragraph("Ana Maria (M1)", table_text),
            Paragraph("Case-insensitive text search querying product names returning matching items dynamically, with a clear empty-state message when nothing is found.", table_text)
        ],
        [
            Paragraph("<b>A3</b>", table_text),
            Paragraph("<b>Shopping Cart</b>", table_text),
            Paragraph("Darius Adonis (M2)", table_text),
            Paragraph("Server-recalculated cart supporting adding, updating, and removing items with rejection of quantities above available stock; customer cart persists across sessions and logout.", table_text)
        ],
        [
            Paragraph("<b>A4</b>", table_text),
            Paragraph("<b>User Accounts</b>", table_text),
            Paragraph("David Andrei (M3)", table_text),
            Paragraph("User registration, login, and logout with secure cryptographic password hashing (PBKDF2/bcrypt) and ability for customers to save and manage delivery addresses.", table_text)
        ],
        [
            Paragraph("<b>A5</b>", table_text),
            Paragraph("<b>Checkout</b>", table_text),
            Paragraph("Darius Adonis (M2)<br/>Ana Maria (M1)", table_text),
            Paragraph("Multi-step checkout: shipping address, delivery method (standard vs express with different costs), itemized order summary, and confirmation page displaying unique order number.", table_text)
        ],
        [
            Paragraph("<b>A6</b>", table_text),
            Paragraph("<b>Sandbox Payment</b>", table_text),
            Paragraph("Darius Adonis (M2)", table_text),
            Paragraph("Stripe Sandbox payment integration where orders transition to 'Paid' strictly upon verified asynchronous webhook confirmation, leaving failed attempts as 'Unpaid'.", table_text)
        ],
        [
            Paragraph("<b>A7</b>", table_text),
            Paragraph("<b>Stock Management</b>", table_text),
            Paragraph("Darius Adonis (M2)", table_text),
            Paragraph("Stock decrements automatically when payment is confirmed and restores if an order is cancelled, with concurrency controls preventing two customers from purchasing the last unit simultaneously.", table_text)
        ],
        [
            Paragraph("<b>A8</b>", table_text),
            Paragraph("<b>Order History</b>", table_text),
            Paragraph("David Andrei (M3)<br/>Ana Maria (M1)", table_text),
            Paragraph("Customer account dashboard displaying past orders with order placement date, line-item product breakdown, grand total, and current fulfillment status.", table_text)
        ],
        [
            Paragraph("<b>A9</b>", table_text),
            Paragraph("<b>Administration Panel</b>", table_text),
            Paragraph("David Andrei (M3)", table_text),
            Paragraph("Separate administrator role with product/category CRUD (including image upload) and an order list filterable by status with transitions: New → Paid → Shipped → Delivered, or Cancelled.", table_text)
        ],
        [
            Paragraph("<b>A10</b>", table_text),
            Paragraph("<b>Basic Security</b>", table_text),
            Paragraph("David Andrei (M3)", table_text),
            Paragraph("Server-side validation, Anti-Forgery CSRF protection, and EF Core parameterized queries; admin pages and API endpoints are strictly blocked on the server for regular users.", table_text)
        ],
        [
            Paragraph("<b>A11</b>", table_text),
            Paragraph("<b>Responsive Design</b>", table_text),
            Paragraph("Ana Maria (M1)", table_text),
            Paragraph("Mobile-first responsive interface built with CSS Flexbox and Bootstrap grid, thoroughly tested and usable on screen widths down to 360 px.", table_text)
        ],
        [
            Paragraph("<b>A12</b>", table_text),
            Paragraph("<b>Automated Tests</b>", table_text),
            Paragraph("David Andrei (M3)<br/>Darius Adonis (M2)", table_text),
            Paragraph("Automated test suite with 10+ xUnit tests run with a single command (dotnet test), covering shopping cart calculations, stock deduction, and payment confirmation webhook logic.", table_text)
        ]
    ]

    t_req = Table(req_data, colWidths=[28, 80, 85, 311])
    t_req.setStyle(TableStyle([
        ('BACKGROUND', (0,0), (-1,0), COLOR_PRIMARY),
        ('BOX', (0,0), (-1,-1), 1, colors.HexColor("#cbd5e1")),
        ('INNERGRID', (0,0), (-1,-1), 0.5, colors.HexColor("#e2e8f0")),
        ('TOPPADDING', (0,0), (-1,-1), 3),
        ('BOTTOMPADDING', (0,0), (-1,-1), 3),
        ('LEFTPADDING', (0,0), (-1,-1), 5),
        ('RIGHTPADDING', (0,0), (-1,-1), 5),
    ]))
    story.append(t_req)

    # =========================================================================
    # PAGE 3: DATA MODEL (ERD DIAGRAM & ENTITY DICTIONARY)
    # =========================================================================
    story.append(PageBreak())
    story.append(Paragraph("4. Data Model — Entity–Relationship Diagram (ERD)", h1_style))
    story.append(Paragraph(
        "The database is designed according to Third Normal Form (3NF) principles with strict foreign key constraints, "
        "unique indexes on credentials and codes, and decimal precision on monetary balances. Below is the complete relational diagram:",
        body_style
    ))

    erd_img = r"c:\Users\Adonis\Downloads\LiftAndAestheticsMilestone2\assets\erd_diagram.png"
    if os.path.exists(erd_img):
        story.append(Image(erd_img, width=490, height=330))
        story.append(Spacer(1, 6))

    story.append(Paragraph("Database Entity Dictionary Summary:", h2_style))
    story.append(Paragraph("• <b>Members:</b> Stores customer credentials (password hash), wallet balance, contact info, and role assignment.", bullet_style))
    story.append(Paragraph("• <b>Products:</b> 30+ fitness products across 4 categories (Supplements, Apparel, Equipment, Accessories) with Price, StockQuantity, ImageUrl.", bullet_style))
    story.append(Paragraph("• <b>CartItems:</b> Database and session-backed active user shopping cart with foreign keys to Members and Products.", bullet_style))
    story.append(Paragraph("• <b>Orders & OrderItems:</b> Order records storing total price, fulfillment status, delivery information, and line item unit price snapshots.", bullet_style))
    story.append(Paragraph("• <b>DiscountCodes & Stripe Transactions:</b> Manages B5 coupon validations (percentage/fixed) and logs Stripe webhook transactions.", bullet_style))

    # =========================================================================
    # PAGE 4: SYSTEM ARCHITECTURE & COMPONENT DIAGRAM
    # =========================================================================
    story.append(PageBreak())
    story.append(Paragraph("5. System Architecture & Component Diagram", h1_style))
    story.append(Paragraph(
        "Lift & Aesthetics adopts an enterprise 3-tier architectural model designed for high cohesion, scalability, and strict security isolation.",
        body_style
    ))

    arch_img = r"c:\Users\Adonis\Downloads\LiftAndAestheticsMilestone2\assets\architecture_diagram.png"
    if os.path.exists(arch_img):
        story.append(Image(arch_img, width=490, height=300))
        story.append(Spacer(1, 8))

    story.append(Paragraph("Architectural Tiers Description:", h2_style))
    story.append(Paragraph(
        "• <b>Client Presentation Tier:</b> Responsive HTML5, CSS3, and Vanilla JavaScript delivering a high-aesthetic gym brand interface. Asynchronous actions (cart mutations, wishlist toggles, instant search) communicate via Fetch API without full page reloads.",
        bullet_style
    ))
    story.append(Paragraph(
        "• <b>Application & Middleware Tier:</b> ASP.NET Core 10 MVC monolith managing HTTP routing, model binding, and Razor view rendering. Security middleware enforces HTTPS redirection, Anti-Forgery verification, Cookie-based Authentication, and Role-Based Access Control (RBAC). Dedicated service classes encapsulate business operations (CartService, OrderService, InventoryService, DiscountEngine).",
        bullet_style
    ))
    story.append(Paragraph(
        "• <b>Data Persistence Tier:</b> Relational database (SQL Server / MySQL) queried via Entity Framework Core 10 with parameterized LINQ expressions to guard against SQL injection. Automated seeders ensure deterministic catalog initialization.",
        bullet_style
    ))
    story.append(Paragraph(
        "• <b>External Gateway Tier:</b> Stripe Sandbox API handles PCI-DSS compliant credit card transactions via Stripe Elements. Asynchronous webhook notifications are cryptographically verified using HMAC signatures.",
        bullet_style
    ))

    # =========================================================================
    # PAGE 5: API ENDPOINTS LIST
    # =========================================================================
    story.append(PageBreak())
    story.append(Paragraph("6. Architecture: List of API Endpoints", h1_style))
    story.append(Paragraph(
        "The following table details the complete HTTP routing surface of the application, including method, path, required authorization level, and functional purpose.",
        body_style
    ))

    endpoints_data = [
        [Paragraph("Method", table_header), Paragraph("Route Path", table_header), Paragraph("Access Level", table_header), Paragraph("Functional Description", table_header)],
        [Paragraph("GET", table_text), Paragraph("/Shop", table_text), Paragraph("Public", table_text), Paragraph("Renders product catalog with category filter, price sorting & stock badges (A1, B1).", table_text)],
        [Paragraph("GET", table_text), Paragraph("/Shop/SearchProducts", table_text), Paragraph("Public", table_text), Paragraph("JSON endpoint returning matching products dynamically by keyword (A2).", table_text)],
        [Paragraph("GET", table_text), Paragraph("/Shop/Details/{id}", table_text), Paragraph("Public", table_text), Paragraph("Retrieves detailed product view, live inventory, and specifications.", table_text)],
        [Paragraph("POST", table_text), Paragraph("/Cart/AddToCart", table_text), Paragraph("Authenticated", table_text), Paragraph("Adds specified product to customer database cart with stock validation (A3).", table_text)],
        [Paragraph("POST", table_text), Paragraph("/Cart/UpdateQuantity", table_text), Paragraph("Authenticated", table_text), Paragraph("Updates line item quantity and triggers server-side recalculation.", table_text)],
        [Paragraph("POST", table_text), Paragraph("/Cart/RemoveFromCart", table_text), Paragraph("Authenticated", table_text), Paragraph("Removes item from cart and recalculates cart totals.", table_text)],
        [Paragraph("GET", table_text), Paragraph("/Cart", table_text), Paragraph("Authenticated", table_text), Paragraph("Renders shopping cart review with subtotal, shipping, and active discounts (A3).", table_text)],
        [Paragraph("POST", table_text), Paragraph("/Cart/ApplyCoupon", table_text), Paragraph("Authenticated", table_text), Paragraph("Validates and applies promotional discount code to cart (B5).", table_text)],
        [Paragraph("GET", table_text), Paragraph("/Auth", table_text), Paragraph("Public", table_text), Paragraph("Renders sign-in and registration forms with test account support (A4).", table_text)],
        [Paragraph("POST", table_text), Paragraph("/Auth/Login", table_text), Paragraph("Public", table_text), Paragraph("Verifies credentials and issues encrypted authentication cookie (A4).", table_text)],
        [Paragraph("POST", table_text), Paragraph("/Auth/Register", table_text), Paragraph("Public", table_text), Paragraph("Creates customer account with hashed password and signs in user (A4).", table_text)],
        [Paragraph("POST", table_text), Paragraph("/Auth/Logout", table_text), Paragraph("Authenticated", table_text), Paragraph("Terminates authenticated session and clears client cookie.", table_text)],
        [Paragraph("GET", table_text), Paragraph("/Profile/Orders", table_text), Paragraph("Authenticated", table_text), Paragraph("Displays customer past order history, line items, and fulfillment states (A8).", table_text)],
        [Paragraph("GET", table_text), Paragraph("/Profile/Wishlist", table_text), Paragraph("Authenticated", table_text), Paragraph("Renders saved wishlist items with 1-click cart transfer (B7).", table_text)],
        [Paragraph("GET", table_text), Paragraph("/Cart/Checkout", table_text), Paragraph("Authenticated", table_text), Paragraph("Renders shipping address form and order summary review (A5).", table_text)],
        [Paragraph("POST", table_text), Paragraph("/Cart/Checkout", table_text), Paragraph("Authenticated", table_text), Paragraph("Creates Pending order in DB and requests Stripe PaymentIntent (A5, A6).", table_text)],
        [Paragraph("POST", table_text), Paragraph("/Cart/Webhook", table_text), Paragraph("Stripe Signature", table_text), Paragraph("Verifies Stripe HMAC header, marks Order Paid, and decrements stock (A6, A7).", table_text)],
        [Paragraph("GET", table_text), Paragraph("/Shop/Create", table_text), Paragraph("Admin Role", table_text), Paragraph("Admin view for adding new products to the catalog (A9).", table_text)],
        [Paragraph("POST", table_text), Paragraph("/Shop/Create", table_text), Paragraph("Admin Role", table_text), Paragraph("Admin endpoint to save new product with image upload (A9).", table_text)],
        [Paragraph("POST", table_text), Paragraph("/Shop/Delete/{id}", table_text), Paragraph("Admin Role", table_text), Paragraph("Admin endpoint to remove product from catalog (A9).", table_text)]
    ]

    t_end = Table(endpoints_data, colWidths=[38, 120, 90, 256])
    t_end.setStyle(TableStyle([
        ('BACKGROUND', (0,0), (-1,0), COLOR_PRIMARY),
        ('BOX', (0,0), (-1,-1), 1, colors.HexColor("#cbd5e1")),
        ('INNERGRID', (0,0), (-1,-1), 0.5, colors.HexColor("#e2e8f0")),
        ('TOPPADDING', (0,0), (-1,-1), 2.8),
        ('BOTTOMPADDING', (0,0), (-1,-1), 2.8),
        ('LEFTPADDING', (0,0), (-1,-1), 5),
        ('RIGHTPADDING', (0,0), (-1,-1), 5),
    ]))
    story.append(t_end)

    # =========================================================================
    # PAGE 6: CRITICAL FLOW: PAYMENT & STOCK SEQUENCE
    # =========================================================================
    story.append(PageBreak())
    story.append(Paragraph("7. Critical Flow — Payment, Webhook & Stock Sequence Diagram", h1_style))
    story.append(Paragraph(
        "The critical payment flow guarantees that order status and stock levels are never determined by the client browser. "
        "Stock decrement and order status transition to 'Paid' occur exclusively after cryptographic verification of the asynchronous payment provider webhook.",
        body_style
    ))

    seq_img = r"c:\Users\Adonis\Downloads\LiftAndAestheticsMilestone2\assets\sequence_diagram.png"
    if os.path.exists(seq_img):
        story.append(Image(seq_img, width=490, height=330))
        story.append(Spacer(1, 6))

    story.append(Paragraph("Step-by-Step Critical Flow Logic:", h2_style))
    story.append(Paragraph("1. <b>Checkout Submission:</b> Customer inputs delivery information. The server locks items and inserts an Order with <code>Status = 'PendingPayment'</code>.", bullet_style))
    story.append(Paragraph("2. <b>Stripe Intent:</b> The server issues a server-to-server call to Stripe API creating a <code>PaymentIntent</code> with Order metadata and receives a client secret.", bullet_style))
    story.append(Paragraph("3. <b>Client Card Tokenization:</b> The browser securely renders Stripe Elements. Card credentials travel directly to Stripe servers, adhering to strict PCI compliance.", bullet_style))
    story.append(Paragraph("4. <b>Asynchronous Webhook:</b> Upon successful card charge, Stripe sends a <code>payment_intent.succeeded</code> webhook payload to <code>/Checkout/Webhook</code>.", bullet_style))
    story.append(Paragraph("5. <b>Cryptographic Verification:</b> The webhook endpoint inspects the <code>Stripe-Signature</code> header using the server's webhook secret key to prevent spoofing.", bullet_style))
    story.append(Paragraph("6. <b>Atomic DB Transaction:</b> The server opens a database transaction: updates Order status to <code>'Paid'</code>, atomically decrements <code>Product.StockQuantity</code> for each line item, and logs a <code>PaymentTransaction</code> record.", bullet_style))

    # =========================================================================
    # PAGE 7: USER INTERFACE WIREFRAMES
    # =========================================================================
    story.append(PageBreak())
    story.append(Paragraph("8. User Interface Wireframes", h1_style))
    story.append(Paragraph(
        "Wireframes covering the five required core user journeys: (1) Product Catalog, (2) Product Detail View, "
        "(3) Shopping Cart, (4) Checkout & Stripe Payment, and (5) Administration Panel.",
        body_style
    ))

    wf_img = r"c:\Users\Adonis\Downloads\LiftAndAestheticsMilestone2\assets\wireframes.png"
    if os.path.exists(wf_img):
        story.append(Image(wf_img, width=490, height=330))
        story.append(Spacer(1, 6))

    story.append(Paragraph("UX / UI Rationale & Usability Highlights:", h2_style))
    story.append(Paragraph("• <b>Catalog View:</b> Category pills, search bar, and filter toolbar (B1) allow instantaneous narrowing of products. Cards display real-time stock badges.", bullet_style))
    story.append(Paragraph("• <b>Product Details:</b> Dedicated high-res view featuring nutritional / gear specs, live stock counts, quantity selector, and B7 wishlist bookmarking.", bullet_style))
    story.append(Paragraph("• <b>Cart & Checkout:</b> Clean tabular breakdown of line items, promotional code box (B5), and 2-step checkout with embedded Stripe Elements card fields.", bullet_style))
    story.append(Paragraph("• <b>Administration Panel:</b> Protected management interface with KPI stats cards (Sales, Orders, Low Stock Alerts) and product CRUD tables (A9).", bullet_style))
    story.append(Paragraph("• <b>Mobile Usability (A11):</b> All views use fluid grid layouts and responsive viewports tested down to 360 px width.", bullet_style))

    # =========================================================================
    # PAGE 8: WORKING PROTOTYPE & VERIFICATION GUIDE
    # =========================================================================
    story.append(PageBreak())
    story.append(Paragraph("9. Working Prototype Demonstration & Verification Guide", h1_style))
    story.append(Paragraph(
        "The working prototype for Milestone P2 is fully implemented, self-contained, and operational in ASP.NET Core 10. "
        "Upon launch, the application automatically ensures database creation and seeds 32 gym products across 4 categories, "
        "3 test users (including an Administrator), and promotional coupons.",
        body_style
    ))

    story.append(Paragraph("Live Demonstration Steps for Evaluators:", h2_style))
    steps_data = [
        [Paragraph("Step", table_header), Paragraph("Demonstration Action", table_header), Paragraph("Expected System Behavior", table_header), Paragraph("Status", table_header)],
        [
            Paragraph("1", table_text),
            Paragraph("Start Application:<br/><code>dotnet run</code>", table_text),
            Paragraph("Server boots at <code>http://localhost:5000</code>, initializes database schema, and seeds 32 products and test accounts.", table_text),
            Paragraph("<b>PASS</b>", table_text)
        ],
        [
            Paragraph("2", table_text),
            Paragraph("Browse Catalog (A1)", table_text),
            Paragraph("Catalog displays products loaded dynamically from database with prices, SKUs, category badges, and live stock counts.", table_text),
            Paragraph("<b>PASS</b>", table_text)
        ],
        [
            Paragraph("3", table_text),
            Paragraph("Test Category Filtering (A1, B1)", table_text),
            Paragraph("Clicking 'Supplements', 'Gym Apparel', 'Lifting Gear', or 'Accessories' filters items instantly.", table_text),
            Paragraph("<b>PASS</b>", table_text)
        ],
        [
            Paragraph("4", table_text),
            Paragraph("Test Search & Empty State (A2)", table_text),
            Paragraph("Searching 'creatine' returns product; searching 'unknown' triggers friendly empty state with recommendations.", table_text),
            Paragraph("<b>PASS</b>", table_text)
        ],
        [
            Paragraph("5", table_text),
            Paragraph("Demonstrate Login (A4)", table_text),
            Paragraph("Navigate to <code>/Auth</code>. Sign in as Admin (<code>admin@lift.com</code> / <code>Testing1</code>) or Athlete (<code>athlete@lift.com</code> / <code>Testing1</code>). Interface reflects authenticated user session and role.", table_text),
            Paragraph("<b>PASS</b>", table_text)
        ],
        [
            Paragraph("6", table_text),
            Paragraph("Demonstrate Shopping Cart (A3, B5)", table_text),
            Paragraph("Add products from catalog. Open Cart. Apply coupon <code>LIFT10</code>. Server recalculates 10% discount and shipping.", table_text),
            Paragraph("<b>PASS</b>", table_text)
        ]
    ]

    t_steps = Table(steps_data, colWidths=[25, 115, 310, 54])
    t_steps.setStyle(TableStyle([
        ('BACKGROUND', (0,0), (-1,0), COLOR_PRIMARY),
        ('BOX', (0,0), (-1,-1), 1, colors.HexColor("#cbd5e1")),
        ('INNERGRID', (0,0), (-1,-1), 0.5, colors.HexColor("#e2e8f0")),
        ('TOPPADDING', (0,0), (-1,-1), 4),
        ('BOTTOMPADDING', (0,0), (-1,-1), 4),
        ('LEFTPADDING', (0,0), (-1,-1), 5),
        ('RIGHTPADDING', (0,0), (-1,-1), 5),
    ]))
    story.append(t_steps)
    story.append(Spacer(1, 10))

    story.append(Paragraph("10. Milestone P2 Approval & Sign-Off", h1_style))
    sign_data = [
        [Paragraph("<b>Team Member</b>", table_header), Paragraph("<b>Role / Specialization</b>", table_header), Paragraph("<b>P2 Sign-off Status</b>", table_header)],
        [Paragraph("Peca Ana Maria Cristina", table_text), Paragraph("Frontend, Product Discovery & Wireframes", table_text), Paragraph("APPROVED & FROZEN", table_text)],
        [Paragraph("Ploae Darius Adonis Ionuț", table_text), Paragraph("Backend, Cart, Payments & Critical Flows", table_text), Paragraph("APPROVED & FROZEN", table_text)],
        [Paragraph("Nica David Andrei", table_text), Paragraph("Database, Accounts, Admin & Security", table_text), Paragraph("APPROVED & FROZEN", table_text)]
    ]
    t_sign = Table(sign_data, colWidths=[160, 210, 134])
    t_sign.setStyle(TableStyle([
        ('BACKGROUND', (0,0), (-1,0), COLOR_PRIMARY),
        ('BOX', (0,0), (-1,-1), 1, colors.HexColor("#cbd5e1")),
        ('INNERGRID', (0,0), (-1,-1), 0.5, colors.HexColor("#e2e8f0")),
        ('TOPPADDING', (0,0), (-1,-1), 4),
        ('BOTTOMPADDING', (0,0), (-1,-1), 4),
        ('LEFTPADDING', (0,0), (-1,-1), 6),
        ('RIGHTPADDING', (0,0), (-1,-1), 6),
    ]))
    story.append(t_sign)

    doc.build(story, canvasmaker=NumberedCanvas)
    print(f"Specification PDF generated successfully: {pdf_filename}")

if __name__ == "__main__":
    build_pdf()

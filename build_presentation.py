import os
from pptx import Presentation
from pptx.util import Inches, Pt
from pptx.enum.text import PP_ALIGN
from pptx.dml.color import RGBColor
from pptx.enum.shapes import MSO_SHAPE

def create_presentation():
    prs = Presentation()
    # 16:9 widescreen layout
    prs.slide_width = Inches(13.333)
    prs.slide_height = Inches(7.5)

    blank_layout = prs.slide_layouts[6]

    # Human-like, clean, professional student palette (matching Presentation 1 & gym branding)
    COLOR_BG = RGBColor(248, 250, 252)        # Light Slate (#f8fafc)
    COLOR_CARD = RGBColor(255, 255, 255)      # Pure White (#ffffff)
    COLOR_BORDER = RGBColor(226, 232, 240)    # Soft Gray Border (#e2e8f0)
    COLOR_PRIMARY = RGBColor(15, 23, 42)      # Deep Slate (#0f172a)
    COLOR_TEXT = RGBColor(51, 65, 85)         # Slate 700 (#334155)
    COLOR_MUTED = RGBColor(100, 116, 139)     # Slate 500 (#64748b)
    COLOR_RED = RGBColor(220, 38, 38)         # Gym Crimson Accent (#dc2626)
    COLOR_BLUE = RGBColor(37, 99, 235)        # Steel Blue Accent (#2563eb)
    COLOR_GREEN = RGBColor(22, 163, 74)       # Success Green (#16a34a)

    def add_background(slide):
        bg = slide.shapes.add_shape(MSO_SHAPE.RECTANGLE, 0, 0, Inches(13.333), Inches(7.5))
        bg.fill.solid()
        bg.fill.fore_color.rgb = COLOR_BG
        bg.line.fill.background()
        return bg

    def add_header(slide, title_text, category_text="MILESTONE 2 — SPECIFICATION & PROTOTYPE"):
        # Category / Breadcrumb
        cat_box = slide.shapes.add_textbox(Inches(0.8), Inches(0.4), Inches(11), Inches(0.35))
        tf_cat = cat_box.text_frame
        tf_cat.word_wrap = True
        p_cat = tf_cat.paragraphs[0]
        p_cat.text = category_text.upper()
        p_cat.font.size = Pt(10)
        p_cat.font.bold = True
        p_cat.font.color.rgb = COLOR_RED

        # Title
        t_box = slide.shapes.add_textbox(Inches(0.8), Inches(0.7), Inches(11.5), Inches(0.6))
        tf = t_box.text_frame
        tf.word_wrap = True
        p = tf.paragraphs[0]
        p.text = title_text
        p.font.size = Pt(21)
        p.font.bold = True
        p.font.color.rgb = COLOR_PRIMARY

        # Divider line
        line = slide.shapes.add_shape(MSO_SHAPE.RECTANGLE, Inches(0.8), Inches(1.35), Inches(11.733), Inches(0.015))
        line.fill.solid()
        line.fill.fore_color.rgb = COLOR_BORDER
        line.line.fill.background()

    def add_card(slide, left, top, width, height, title="", accent_color=None):
        card = slide.shapes.add_shape(MSO_SHAPE.RECTANGLE, left, top, width, height)
        card.fill.solid()
        card.fill.fore_color.rgb = COLOR_CARD
        card.line.color.rgb = COLOR_BORDER
        card.line.width = Pt(1)

        if accent_color:
            top_bar = slide.shapes.add_shape(MSO_SHAPE.RECTANGLE, left, top, width, Inches(0.06))
            top_bar.fill.solid()
            top_bar.fill.fore_color.rgb = accent_color
            top_bar.line.fill.background()

        if title:
            tb = slide.shapes.add_textbox(left + Inches(0.2), top + Inches(0.12), width - Inches(0.4), Inches(0.35))
            tf = tb.text_frame
            tf.word_wrap = True
            p = tf.paragraphs[0]
            p.text = title
            p.font.size = Pt(12)
            p.font.bold = True
            p.font.color.rgb = COLOR_PRIMARY

        return card

    # =========================================================================
    # SLIDE 1: TITLE SLIDE
    # =========================================================================
    s1 = prs.slides.add_slide(blank_layout)
    add_background(s1)

    # Accent decorative bar
    bar = s1.shapes.add_shape(MSO_SHAPE.RECTANGLE, Inches(1.0), Inches(1.8), Inches(0.15), Inches(3.8))
    bar.fill.solid()
    bar.fill.fore_color.rgb = COLOR_RED
    bar.line.fill.background()

    tb = s1.shapes.add_textbox(Inches(1.4), Inches(1.7), Inches(10.5), Inches(2.2))
    tf = tb.text_frame
    tf.word_wrap = True

    p0 = tf.paragraphs[0]
    p0.text = "E-COMMERCE / SEMESTER PROJECT"
    p0.font.size = Pt(12)
    p0.font.bold = True
    p0.font.color.rgb = COLOR_RED
    p0.space_after = Pt(8)

    p1 = tf.add_paragraph()
    p1.text = "Lift & Aesthetics"
    p1.font.size = Pt(40)
    p1.font.bold = True
    p1.font.color.rgb = COLOR_PRIMARY
    p1.space_after = Pt(8)

    p2 = tf.add_paragraph()
    p2.text = "Presentation 2 — Specification & Prototype (Week 7)"
    p2.font.size = Pt(18)
    p2.font.color.rgb = COLOR_MUTED

    # Team & Stack Card
    card_info = add_card(s1, Inches(1.4), Inches(4.3), Inches(10.5), Inches(2.0), accent_color=COLOR_BLUE)
    tb_info = s1.shapes.add_textbox(Inches(1.7), Inches(4.5), Inches(10.0), Inches(1.6))
    tfi = tb_info.text_frame
    tfi.word_wrap = True

    pi1 = tfi.paragraphs[0]
    pi1.text = "PROJECT TEAM & ROLES"
    pi1.font.size = Pt(11)
    pi1.font.bold = True
    pi1.font.color.rgb = COLOR_PRIMARY
    pi1.space_after = Pt(4)

    pi2 = tfi.add_paragraph()
    pi2.text = "• Peca Ana Maria Cristina — Frontend & Product Discovery\n• Ploae Darius Adonis Ionut — Backend, Cart & Payments\n• Nica David Andrei — Database, Accounts & Admin Panel"
    pi2.font.size = Pt(10.5)
    pi2.font.color.rgb = COLOR_TEXT
    pi2.space_after = Pt(6)

    pi3 = tfi.add_paragraph()
    pi3.text = "TECHNOLOGY STACK: ASP.NET Core 9 / C#  •  EF Core  •  MySQL / SQL Server  •  Stripe Sandbox"
    pi3.font.size = Pt(9.5)
    pi3.font.bold = True
    pi3.font.color.rgb = COLOR_BLUE

    # =========================================================================
    # SLIDE 2: OBJECTIVES & SCOPE FREEZING
    # =========================================================================
    s2 = prs.slides.add_slide(blank_layout)
    add_background(s2)
    add_header(s2, "Milestone 2 Objectives & Scope Freezing")

    # Card 1: Goal of P2
    add_card(s2, Inches(0.8), Inches(1.7), Inches(5.6), Inches(4.8), "Goal: The Specification Is Frozen", COLOR_RED)
    tb = s2.shapes.add_textbox(Inches(1.0), Inches(2.2), Inches(5.2), Inches(4.0))
    tf = tb.text_frame
    tf.word_wrap = True
    p = tf.paragraphs[0]
    p.text = "Contractual Baseline for Grading"
    p.font.size = Pt(12)
    p.font.bold = True
    p.font.color.rgb = COLOR_PRIMARY
    p.space_after = Pt(8)

    points = [
        "What is approved at P2 represents the frozen specification against which Presentation 3 will be graded.",
        "Any subsequent functional or scope change must be submitted and approved in writing on the course channel.",
        "Clear architectural and database boundaries established early to guarantee stability.",
        "Delivers a working prototype proving catalog browsing, database seed loading, and customer shopping flow."
    ]
    for pt in points:
        p = tf.add_paragraph()
        p.text = f"• {pt}"
        p.font.size = Pt(10.5)
        p.font.color.rgb = COLOR_TEXT
        p.space_after = Pt(8)

    # Card 2: Deliverables Checklist
    add_card(s2, Inches(6.9), Inches(1.7), Inches(5.6), Inches(4.8), "P2 Deliverables Checklist", COLOR_BLUE)
    tb = s2.shapes.add_textbox(Inches(7.1), Inches(2.2), Inches(5.2), Inches(4.0))
    tf = tb.text_frame
    tf.word_wrap = True

    delivs = [
        ("Requirements A1–A12", "All 12 core requirements described in 1–2 concise implementation sentences."),
        ("Chosen Bonus Features", "B1 (Filtered Search), B5 (Discount Codes), and B7 (Wishlist) selected."),
        ("Data Model (ERD)", "Relational database diagram matching actual tables with clean 1:N relations."),
        ("System Architecture", "Three-tier component diagram with complete API endpoints specification table."),
        ("Critical Flow Sequence", "UML sequence flow: Checkout -> Stripe Sandbox -> Webhook -> Order & Stock."),
        ("UI Wireframes", "Clean grayscale wireframes for Catalog, Product, Cart, Checkout, and Admin."),
        ("Working Prototype", "Live demo with 30+ products read from DB, login, and active checkout flow.")
    ]
    for idx, (title, desc) in enumerate(delivs):
        p = tf.paragraphs[0] if idx == 0 else tf.add_paragraph()
        p.text = f"{title}: {desc}"
        p.font.size = Pt(9.5)
        p.font.color.rgb = COLOR_TEXT
        p.space_after = Pt(6)

    # =========================================================================
    # SLIDE 3: REQUIREMENTS A1 - A4
    # =========================================================================
    s3 = prs.slides.add_slide(blank_layout)
    add_background(s3)
    add_header(s3, "Requirements Specification: A1 – A4 (Catalog & Accounts)")

    cards_data = [
        ("A1: Product Catalog", "Ana Maria (M1)", [
            "Displays gym products grouped into 4 clear categories (Supplements, Clothing, Equipment, Accessories).",
            "Each item shows a photo, title, price in LEI, stock availability badge, and direct button to view details or add to cart."
        ]),
        ("A2: Basic Search", "Ana Maria (M1)", [
            "Allows customers to search the store instantly by typing product names or category keywords.",
            "Queries the database dynamically and displays matching items or a helpful empty state when no matches exist."
        ]),
        ("A3: User Accounts & Authentication", "David Andrei (M3)", [
            "Secure customer registration and login handled by ASP.NET Core Identity with hashed credentials.",
            "Maintains authenticated sessions and gives users access to personal profiles and account management."
        ]),
        ("A4: Order History", "David Andrei (M3)", [
            "Authenticated members can view their past orders, transaction dates, total amounts, and delivery statuses.",
            "Provides transparent order tracking and digital receipt summaries for all completed checkouts."
        ])
    ]

    coords = [
        (Inches(0.8), Inches(1.7)),
        (Inches(6.9), Inches(1.7)),
        (Inches(0.8), Inches(4.3)),
        (Inches(6.9), Inches(4.3))
    ]

    for (title, owner, bullets), (x, y) in zip(cards_data, coords):
        add_card(s3, x, y, Inches(5.6), Inches(2.4), title, COLOR_RED)
        tb = s3.shapes.add_textbox(x + Inches(0.2), y + Inches(0.55), Inches(5.2), Inches(1.7))
        tf = tb.text_frame
        tf.word_wrap = True
        
        po = tf.paragraphs[0]
        po.text = f"Lead: {owner}"
        po.font.size = Pt(9)
        po.font.bold = True
        po.font.color.rgb = COLOR_BLUE
        po.space_after = Pt(4)

        for b in bullets:
            pb = tf.add_paragraph()
            pb.text = f"• {b}"
            pb.font.size = Pt(9.5)
            pb.font.color.rgb = COLOR_TEXT
            pb.space_after = Pt(4)

    # =========================================================================
    # SLIDE 4: REQUIREMENTS A5 - A8
    # =========================================================================
    s4 = prs.slides.add_slide(blank_layout)
    add_background(s4)
    add_header(s4, "Requirements Specification: A5 – A8 (Cart, Checkout & Payment)")

    cards_data = [
        ("A5: Shopping Cart", "Darius Adonis (M2)", [
            "Stores selected gym products with line item subtotals, quantity increase/decrease buttons, and remove actions.",
            "The server recalculates all totals, discounts, and item prices on every mutation to prevent client tampering."
        ]),
        ("A6: Checkout Flow", "Darius Adonis (M2)", [
            "Collects shipping address and customer contact details through a simple, validated form.",
            "Calculates delivery costs and locks cart items into an unpaid order record prior to payment submission."
        ]),
        ("A7: Sandbox Payment Integration", "Darius Adonis (M2)", [
            "Simulates card payments securely using the Stripe test environment without real currency or risks.",
            "Orders stay in 'PendingPayment' state until Stripe confirms successful charge capture via an async webhook."
        ]),
        ("A8: Stock Management", "Darius Adonis (M2)", [
            "Decrements available product quantities in the database atomically only upon confirmed webhook payment.",
            "Prevents overselling and automatically restores inventory if checkout is cancelled or payment fails."
        ])
    ]

    for (title, owner, bullets), (x, y) in zip(cards_data, coords):
        add_card(s4, x, y, Inches(5.6), Inches(2.4), title, COLOR_RED)
        tb = s4.shapes.add_textbox(x + Inches(0.2), y + Inches(0.55), Inches(5.2), Inches(1.7))
        tf = tb.text_frame
        tf.word_wrap = True
        
        po = tf.paragraphs[0]
        po.text = f"Lead: {owner}"
        po.font.size = Pt(9)
        po.font.bold = True
        po.font.color.rgb = COLOR_BLUE
        po.space_after = Pt(4)

        for b in bullets:
            pb = tf.add_paragraph()
            pb.text = f"• {b}"
            pb.font.size = Pt(9.5)
            pb.font.color.rgb = COLOR_TEXT
            pb.space_after = Pt(4)

    # =========================================================================
    # SLIDE 5: REQUIREMENTS A9 - A12
    # =========================================================================
    s5 = prs.slides.add_slide(blank_layout)
    add_background(s5)
    add_header(s5, "Requirements Specification: A9 – A12 (Admin, Security & Quality)")

    cards_data = [
        ("A9: Admin Management Panel", "David Andrei (M3)", [
            "Administrative interface allowing gym owners to add new products, edit prices, upload images, and delete items.",
            "Enables store managers to monitor incoming orders and update product stock levels in real time."
        ]),
        ("A10: Access Control & Security", "David Andrei (M3)", [
            "Protects admin routes and sensitive actions using role-based authorization attributes ([Authorize(Roles='Admin')]).",
            "Protects user passwords with cryptographic hashing and uses parameterized EF Core queries against SQL injection."
        ]),
        ("A11: Responsive Mobile Design", "Ana Maria (M1)", [
            "Adaptive CSS grid and flexbox layout designed to look great on mobile smartphones, tablets, and desktop displays.",
            "Features mobile-friendly navigation, touch-friendly product cards, and easy-to-use checkout inputs."
        ]),
        ("A12: Testing & Quality Checks", "Team Shared", [
            "Automated unit tests and manual end-to-end verification of cart totals, coupon calculations, and checkout flows.",
            "Validates error handling for declined test cards, empty cart checkout attempts, and out-of-stock items."
        ])
    ]

    for (title, owner, bullets), (x, y) in zip(cards_data, coords):
        add_card(s5, x, y, Inches(5.6), Inches(2.4), title, COLOR_RED)
        tb = s5.shapes.add_textbox(x + Inches(0.2), y + Inches(0.55), Inches(5.2), Inches(1.7))
        tf = tb.text_frame
        tf.word_wrap = True
        
        po = tf.paragraphs[0]
        po.text = f"Lead: {owner}"
        po.font.size = Pt(9)
        po.font.bold = True
        po.font.color.rgb = COLOR_BLUE
        po.space_after = Pt(4)

        for b in bullets:
            pb = tf.add_paragraph()
            pb.text = f"• {b}"
            pb.font.size = Pt(9.5)
            pb.font.color.rgb = COLOR_TEXT
            pb.space_after = Pt(4)

    # =========================================================================
    # SLIDE 6: CHOSEN BONUS FEATURES (B1, B5, B7)
    # =========================================================================
    s6 = prs.slides.add_slide(blank_layout)
    add_background(s6)
    add_header(s6, "Chosen Bonus Features (Official Course Codes)")

    bonuses = [
        ("B1: Filtered Search", "Ana Maria (M1)", COLOR_RED, [
            "Multi-criteria catalog filtering for easy product discovery.",
            "Filter by category (Supplements, Apparel, Equipment, Accessories).",
            "Price range filtering and 'In-Stock Only' availability toggle.",
            "Sorting by Price (Low to High / High to Low) and Product Name.",
            "Implemented and demonstrated live in the prototype catalog."
        ]),
        ("B5: Discount Codes / Coupons", "Darius Adonis (M2)", COLOR_BLUE, [
            "Promotional coupon engine validated directly in the shopping cart.",
            "Supports percentage discounts (e.g. LIFT10 for 10% off) and fixed sums.",
            "Server checks coupon expiration date, active status, and minimum spend.",
            "Subtracts discount amount before submitting final order total to Stripe."
        ]),
        ("B7: Wishlist / Saved Items", "David Andrei (M3) + Ana Maria", COLOR_GREEN, [
            "Enables registered lifters to bookmark gear and supplements for later.",
            "Quick 1-click addition directly from product cards or product detail view.",
            "Dedicated Wishlist section in User Profile with instant 'Move to Cart' button."
        ])
    ]

    bx = Inches(0.8)
    for title, owner, acc_color, points in bonuses:
        add_card(s6, bx, Inches(1.7), Inches(3.64), Inches(4.8), title, acc_color)
        tb = s6.shapes.add_textbox(bx + Inches(0.2), Inches(2.2), Inches(3.24), Inches(4.0))
        tf = tb.text_frame
        tf.word_wrap = True

        po = tf.paragraphs[0]
        po.text = f"Lead: {owner}"
        po.font.size = Pt(10)
        po.font.bold = True
        po.font.color.rgb = acc_color
        po.space_after = Pt(8)

        for ptxt in points:
            p = tf.add_paragraph()
            p.text = f"• {ptxt}"
            p.font.size = Pt(9.5)
            p.font.color.rgb = COLOR_TEXT
            p.space_after = Pt(6)

        bx += Inches(4.04)

    # =========================================================================
    # SLIDE 7: DATA MODEL (ERD)
    # =========================================================================
    s7 = prs.slides.add_slide(blank_layout)
    add_background(s7)
    add_header(s7, "Database Entity-Relationship Diagram (ERD)")

    erd_img_path = r"c:\Users\Adonis\Downloads\LiftAndAestheticsMilestone2\assets\erd_diagram.png"
    if os.path.exists(erd_img_path):
        s7.shapes.add_picture(erd_img_path, Inches(0.8), Inches(1.5), Inches(8.5), Inches(5.3))

    # Notes Card on the right
    add_card(s7, Inches(9.5), Inches(1.5), Inches(3.0), Inches(5.3), "Schema Highlights", COLOR_BLUE)
    tb = s7.shapes.add_textbox(Inches(9.65), Inches(2.0), Inches(2.7), Inches(4.6))
    tf = tb.text_frame
    tf.word_wrap = True

    erd_notes = [
        "Relational schema matching ASP.NET Core EF Core & MySQL model.",
        "Primary Keys (PK) & Foreign Keys (FK) enforce referential integrity.",
        "Products & CartItems: 1:N link tracking quantity per active member.",
        "Orders & OrderItems: Stores snapshot of unit price at time of purchase.",
        "Members & Memberships: Links users to their active fitness tier.",
        "Non-crossing orthogonal relationships for maximum clarity."
    ]
    for idx, n in enumerate(erd_notes):
        p = tf.paragraphs[0] if idx == 0 else tf.add_paragraph()
        p.text = f"• {n}"
        p.font.size = Pt(9.5)
        p.font.color.rgb = COLOR_TEXT
        p.space_after = Pt(6)

    # =========================================================================
    # SLIDE 8: SYSTEM ARCHITECTURE
    # =========================================================================
    s8 = prs.slides.add_slide(blank_layout)
    add_background(s8)
    add_header(s8, "System Architecture: Three-Tier Component Diagram")

    arch_img_path = r"c:\Users\Adonis\Downloads\LiftAndAestheticsMilestone2\assets\architecture_diagram.png"
    if os.path.exists(arch_img_path):
        s8.shapes.add_picture(arch_img_path, Inches(0.8), Inches(1.5), Inches(8.5), Inches(5.3))

    add_card(s8, Inches(9.5), Inches(1.5), Inches(3.0), Inches(5.3), "Architecture Highlights", COLOR_RED)
    tb = s8.shapes.add_textbox(Inches(9.65), Inches(2.0), Inches(2.7), Inches(4.6))
    tf = tb.text_frame
    tf.word_wrap = True

    arch_notes = [
        "Three-Tier MVC separation of concerns for clean maintainability.",
        "Client Layer: Responsive Razor Views, HTML5, CSS3, vanilla JS.",
        "Application Layer: ASP.NET Core Controllers & Dependency Injection Services.",
        "Data Layer: Entity Framework Core ORM with MySQL / SQL Server.",
        "Payment Gateway: Stripe Sandbox integration with secure Webhooks.",
        "Server-authoritative validation prevents client tampering."
    ]
    for idx, n in enumerate(arch_notes):
        p = tf.paragraphs[0] if idx == 0 else tf.add_paragraph()
        p.text = f"• {n}"
        p.font.size = Pt(9.5)
        p.font.color.rgb = COLOR_TEXT
        p.space_after = Pt(6)

    # =========================================================================
    # SLIDE 9: API ENDPOINTS TABLE
    # =========================================================================
    s9 = prs.slides.add_slide(blank_layout)
    add_background(s9)
    add_header(s9, "Application Endpoints Specification")

    endpoints_data = [
        ("GET", "/Shop", "Public", "Browse product catalog with optional category & keyword search"),
        ("GET", "/Shop/Details/{id}", "Public", "View detailed product specifications, price, and photo"),
        ("GET", "/Cart", "Member", "View active shopping cart items, line subtotals, and total"),
        ("POST", "/Cart/AddToCart", "Member", "Add selected product to member cart or increment quantity"),
        ("POST", "/Cart/UpdateQuantity", "Member", "Update quantity for a cart line item or validate max stock"),
        ("POST", "/Cart/Remove", "Member", "Remove an individual product line from the active cart"),
        ("POST", "/Cart/Checkout", "Member", "Initiate checkout flow, lock order items, create pending order"),
        ("POST", "/api/payments/webhook", "Stripe IP", "Stripe webhook listener to verify signature and mark order paid"),
        ("GET", "/Auth/Login", "Public", "Display customer / admin authentication form"),
        ("POST", "/Auth/Login", "Public", "Authenticate user credentials and establish cookie session"),
        ("GET", "/Shop/Create", "Admin", "Render product creation form (Admin only)"),
        ("POST", "/Shop/Delete/{id}", "Admin", "Remove product from store inventory (Admin only)")
    ]

    # Draw Table
    rows = len(endpoints_data) + 1
    cols = 4
    left = Inches(0.8)
    top = Inches(1.5)
    width = Inches(11.733)
    height = Inches(5.2)

    table_shape = s9.shapes.add_table(rows, cols, left, top, width, height)
    table = table_shape.table
    table.columns[0].width = Inches(1.2)
    table.columns[1].width = Inches(3.2)
    table.columns[2].width = Inches(1.6)
    table.columns[3].width = Inches(5.733)

    headers = ["Method", "Route", "Access Level", "Action Description"]
    for c_idx, h_text in enumerate(headers):
        cell = table.cell(0, c_idx)
        cell.fill.solid()
        cell.fill.fore_color.rgb = COLOR_PRIMARY
        p = cell.text_frame.paragraphs[0]
        p.text = h_text
        p.font.size = Pt(10)
        p.font.bold = True
        p.font.color.rgb = RGBColor(255, 255, 255)

    for r_idx, (method, route, access, desc) in enumerate(endpoints_data, start=1):
        # Method
        cell_m = table.cell(r_idx, 0)
        cell_m.fill.solid()
        cell_m.fill.fore_color.rgb = COLOR_CARD if r_idx % 2 == 0 else RGBColor(241, 245, 249)
        pm = cell_m.text_frame.paragraphs[0]
        pm.text = method
        pm.font.size = Pt(9)
        pm.font.bold = True
        pm.font.color.rgb = COLOR_BLUE if method == "GET" else COLOR_RED

        # Route
        cell_r = table.cell(r_idx, 1)
        cell_r.fill.solid()
        cell_r.fill.fore_color.rgb = cell_m.fill.fore_color.rgb
        pr = cell_r.text_frame.paragraphs[0]
        pr.text = route
        pr.font.size = Pt(9)
        pr.font.color.rgb = COLOR_PRIMARY

        # Access Level
        cell_a = table.cell(r_idx, 2)
        cell_a.fill.solid()
        cell_a.fill.fore_color.rgb = cell_m.fill.fore_color.rgb
        pa = cell_a.text_frame.paragraphs[0]
        pa.text = access
        pa.font.size = Pt(9)
        pa.font.bold = True
        pa.font.color.rgb = COLOR_GREEN if access == "Public" else (COLOR_RED if access == "Admin" else COLOR_TEXT)

        # Description
        cell_d = table.cell(r_idx, 3)
        cell_d.fill.solid()
        cell_d.fill.fore_color.rgb = cell_m.fill.fore_color.rgb
        pd = cell_d.text_frame.paragraphs[0]
        pd.text = desc
        pd.font.size = Pt(8.5)
        pd.font.color.rgb = COLOR_TEXT

    # =========================================================================
    # SLIDE 10: CRITICAL FLOW SEQUENCE DIAGRAM
    # =========================================================================
    s10 = prs.slides.add_slide(blank_layout)
    add_background(s10)
    add_header(s10, "Critical Flow: Payment, Order Processing & Stock Update")

    seq_img_path = r"c:\Users\Adonis\Downloads\LiftAndAestheticsMilestone2\assets\sequence_diagram.png"
    if os.path.exists(seq_img_path):
        s10.shapes.add_picture(seq_img_path, Inches(0.8), Inches(1.5), Inches(8.5), Inches(5.3))

    add_card(s10, Inches(9.5), Inches(1.5), Inches(3.0), Inches(5.3), "Key Sequence Rules", COLOR_RED)
    tb = s10.shapes.add_textbox(Inches(9.65), Inches(2.0), Inches(2.7), Inches(4.6))
    tf = tb.text_frame
    tf.word_wrap = True

    seq_notes = [
        "1. Customer initiates checkout from cart.",
        "2. Server creates 'PendingPayment' order.",
        "3. Stripe PaymentIntent session created.",
        "4. Customer submits card in Stripe sandbox.",
        "5. Asynchronous webhook notifies server.",
        "6. Server verifies HMAC webhook signature.",
        "7. Order marked 'Paid' atomically.",
        "8. Inventory stock decremented in DB.",
        "9. Customer cart cleared upon success."
    ]
    for idx, n in enumerate(seq_notes):
        p = tf.paragraphs[0] if idx == 0 else tf.add_paragraph()
        p.text = n
        p.font.size = Pt(9)
        p.font.color.rgb = COLOR_TEXT
        p.space_after = Pt(5)

    # =========================================================================
    # SLIDE 11: PAYMENT & WEBHOOK RESILIENCE
    # =========================================================================
    s11 = prs.slides.add_slide(blank_layout)
    add_background(s11)
    add_header(s11, "Payment & Webhook Reliability Design")

    resilience_cards = [
        ("Cryptographic Signature Verification", COLOR_RED, [
            "All incoming webhook payloads from Stripe are verified against a secret signing key.",
            "Validates Stripe-Signature HTTP header to ensure notifications cannot be spoofed.",
            "Rejects any unauthorized or tampered webhook requests immediately with HTTP 400."
        ]),
        ("Idempotent Processing", COLOR_BLUE, [
            "Stripe may deliver webhook events more than once if network latency occurs.",
            "The webhook listener checks whether the Order is already marked 'Paid' before processing.",
            "Prevents duplicate charges, double stock decrements, or corrupted database states."
        ]),
        ("Stock Integrity & Failure Recovery", COLOR_GREEN, [
            "Inventory stock is only decremented after cryptographic confirmation of payment.",
            "If customer abandons payment or the test card is declined, stock is NOT reduced.",
            "Cancelled or expired checkout sessions automatically restore reserved quantities."
        ])
    ]

    bx = Inches(0.8)
    for title, acc_color, points in resilience_cards:
        add_card(s11, bx, Inches(1.7), Inches(3.64), Inches(4.8), title, acc_color)
        tb = s11.shapes.add_textbox(bx + Inches(0.2), Inches(2.2), Inches(3.24), Inches(4.0))
        tf = tb.text_frame
        tf.word_wrap = True

        for idx, ptxt in enumerate(points):
            p = tf.paragraphs[0] if idx == 0 else tf.add_paragraph()
            p.text = f"• {ptxt}"
            p.font.size = Pt(10)
            p.font.color.rgb = COLOR_TEXT
            p.space_after = Pt(10)

        bx += Inches(4.04)

    # =========================================================================
    # SLIDE 12: USER INTERFACE WIREFRAMES
    # =========================================================================
    s12 = prs.slides.add_slide(blank_layout)
    add_background(s12)
    add_header(s12, "User Interface Wireframes (Catalog, Cart, Checkout & Admin)")

    wf_img_path = r"c:\Users\Adonis\Downloads\LiftAndAestheticsMilestone2\assets\wireframes.png"
    if os.path.exists(wf_img_path):
        s12.shapes.add_picture(wf_img_path, Inches(0.8), Inches(1.5), Inches(8.5), Inches(5.3))

    add_card(s12, Inches(9.5), Inches(1.5), Inches(3.0), Inches(5.3), "UI Blueprint Focus", COLOR_BLUE)
    tb = s12.shapes.add_textbox(Inches(9.65), Inches(2.0), Inches(2.7), Inches(4.6))
    tf = tb.text_frame
    tf.word_wrap = True

    wf_notes = [
        "1. Catalog Grid: Clear category chips, search bar, product cards with price & photo.",
        "2. Product Details: High-res image, stock status badge, quantity selector, specs.",
        "3. Cart View: Line item controls, coupon code box (B5), clear subtotal breakdown.",
        "4. Checkout: Minimal form, Stripe Elements sandbox card input, order summary sidebar.",
        "5. Admin Panel: Clean product table with Edit/Delete buttons and quick Add Product.",
        "6. My Account: Order history tab and Wishlist saved items (B7)."
    ]
    for idx, n in enumerate(wf_notes):
        p = tf.paragraphs[0] if idx == 0 else tf.add_paragraph()
        p.text = n
        p.font.size = Pt(9)
        p.font.color.rgb = COLOR_TEXT
        p.space_after = Pt(5)

    # =========================================================================
    # SLIDE 13: WORKING PROTOTYPE & LIVE DEMO
    # =========================================================================
    s13 = prs.slides.add_slide(blank_layout)
    add_background(s13)
    add_header(s13, "Working Prototype & Live Demonstration")

    add_card(s13, Inches(0.8), Inches(1.7), Inches(5.6), Inches(4.8), "Live Demo Scope & Seed Data", COLOR_RED)
    tb1 = s13.shapes.add_textbox(Inches(1.0), Inches(2.2), Inches(5.2), Inches(4.0))
    tf1 = tb1.text_frame
    tf1.word_wrap = True

    proto_points = [
        "Database Seed Loaded: 30+ realistic gym products pre-seeded across 4 distinct categories (Supplements, Clothing, Equipment, Accessories).",
        "Catalog Browsing: Live demonstration of category navigation and instant search filtering in ShopController.",
        "Product Details: Viewing pricing, descriptions, and stock quantities directly from database.",
        "Cart Flow: Adding items to cart, modifying quantities, observing live subtotal calculations.",
        "Checkout: Initiating simulated checkout with test member account and viewing order states."
    ]
    for idx, pt in enumerate(proto_points):
        p = tf1.paragraphs[0] if idx == 0 else tf1.add_paragraph()
        p.text = f"• {pt}"
        p.font.size = Pt(10)
        p.font.color.rgb = COLOR_TEXT
        p.space_after = Pt(8)

    add_card(s13, Inches(6.9), Inches(1.7), Inches(5.6), Inches(4.8), "Pre-Configured Test Accounts", COLOR_BLUE)
    tb2 = s13.shapes.add_textbox(Inches(7.1), Inches(2.2), Inches(5.2), Inches(4.0))
    tf2 = tb2.text_frame
    tf2.word_wrap = True

    p = tf2.paragraphs[0]
    p.text = "Pre-seeded accounts for evaluation (Password: Testing1)"
    p.font.size = Pt(11)
    p.font.bold = True
    p.font.color.rgb = COLOR_PRIMARY
    p.space_after = Pt(8)

    accounts = [
        ("Customer Account 1", "athlete@lift.com", "Testing1", "Standard athlete role with pre-loaded membership and cart."),
        ("Customer Account 2", "girl@lift.com", "Testing1", "Secondary user account for multi-customer testing."),
        ("Store Admin 1", "admin@lift.com", "Testing1", "Administrative credentials to access /Shop/Create and manage products."),
        ("Store Admin 2", "alex@lift.com", "Testing1", "Secondary administrator for concurrent management testing.")
    ]
    for title, email, pwd, note in accounts:
        p = tf2.add_paragraph()
        p.text = f"{title}: {email} (pw: {pwd})"
        p.font.size = Pt(9.5)
        p.font.bold = True
        p.font.color.rgb = COLOR_BLUE

        pn = tf2.add_paragraph()
        pn.text = f"   {note}"
        pn.font.size = Pt(8.5)
        pn.font.color.rgb = COLOR_MUTED
        pn.space_after = Pt(4)

    # =========================================================================
    # SLIDE 14: TEAM RESPONSIBILITIES & WORK DIVISION
    # =========================================================================
    s14 = prs.slides.add_slide(blank_layout)
    add_background(s14)
    add_header(s14, "Team Responsibilities & Work Division")

    team = [
        ("Peca Ana Maria Cristina", "Frontend & Product Discovery", COLOR_RED, [
            "• Catalog layout and category browsing (A1)",
            "• Basic search and filtered search (A2, B1)",
            "• Responsive UI styling for mobile & desktop (A11)",
            "• Wishlist UI integration (B7)"
        ]),
        ("Ploae Darius Adonis Ionut", "Backend, Cart & Payments", COLOR_BLUE, [
            "• Shopping cart state and server total calculation (A5)",
            "• Checkout delivery information flow (A6)",
            "• Stripe sandbox payment processor integration (A7)",
            "• Webhook listener & stock decrement logic (A8)",
            "• Discount code coupon engine (B5)"
        ]),
        ("Nica David Andrei", "Database, Security & Admin", COLOR_GREEN, [
            "• Database schema design, migrations, and seeding",
            "• Customer registration and login authentication (A3)",
            "• Order history and user account dashboard (A4)",
            "• Admin product management CRUD (A9)",
            "• Role authorization and access control (A10)"
        ])
    ]

    bx = Inches(0.8)
    for name, role, acc_color, tasks in team:
        add_card(s14, bx, Inches(1.7), Inches(3.64), Inches(4.8), name, acc_color)
        tb = s14.shapes.add_textbox(bx + Inches(0.2), Inches(2.2), Inches(3.24), Inches(4.0))
        tf = tb.text_frame
        tf.word_wrap = True

        po = tf.paragraphs[0]
        po.text = role.upper()
        po.font.size = Pt(9.5)
        po.font.bold = True
        po.font.color.rgb = acc_color
        po.space_after = Pt(10)

        for t in tasks:
            p = tf.add_paragraph()
            p.text = t
            p.font.size = Pt(9.5)
            p.font.color.rgb = COLOR_TEXT
            p.space_after = Pt(6)

        bx += Inches(4.04)

    # =========================================================================
    # SLIDE 15: ROADMAP TO PRESENTATION 3
    # =========================================================================
    s15 = prs.slides.add_slide(blank_layout)
    add_background(s15)
    add_header(s15, "Roadmap from P2 to P3 (Final Delivery)")

    add_card(s15, Inches(0.8), Inches(1.7), Inches(5.6), Inches(4.8), "Remaining Implementation Tasks", COLOR_RED)
    tb1 = s15.shapes.add_textbox(Inches(1.0), Inches(2.2), Inches(5.2), Inches(4.0))
    tf1 = tb1.text_frame
    tf1.word_wrap = True

    p3_tasks = [
        "Stripe Live Webhook Testing: Finalize local CLI webhook forwarder for live end-to-end payment simulations.",
        "Admin Order Dispatching: Complete order fulfillment state management (Pending -> Paid -> Shipped -> Delivered).",
        "Wishlist Finalization (B7): Connect frontend bookmarking directly with user profile DB persistence.",
        "Coupon Rules (B5): Complete validation for minimum spend constraints and expiration date enforcement.",
        "Comprehensive Automated Testing (A12): Expand unit tests for cart edge cases and discount computations."
    ]
    for idx, t in enumerate(p3_tasks):
        p = tf1.paragraphs[0] if idx == 0 else tf1.add_paragraph()
        p.text = f"• {t}"
        p.font.size = Pt(9.5)
        p.font.color.rgb = COLOR_TEXT
        p.space_after = Pt(8)

    add_card(s15, Inches(6.9), Inches(1.7), Inches(5.6), Inches(4.8), "Summary & Q&A", COLOR_BLUE)
    tb2 = s15.shapes.add_textbox(Inches(7.1), Inches(2.2), Inches(5.2), Inches(4.0))
    tf2 = tb2.text_frame
    tf2.word_wrap = True

    p = tf2.paragraphs[0]
    p.text = "Lift & Aesthetics — Milestone 2 Complete"
    p.font.size = Pt(12)
    p.font.bold = True
    p.font.color.rgb = COLOR_PRIMARY
    p.space_after = Pt(8)

    p_sub = tf2.add_paragraph()
    p_sub.text = "• Specification is officially frozen for Milestone P3 evaluation.\n• Seeded catalog with 30+ products and test accounts ready for demonstration.\n• Clean database schema and sequence diagram established.\n• Git repository tagged at 'P2' release point."
    p_sub.font.size = Pt(10)
    p_sub.font.color.rgb = COLOR_TEXT
    p_sub.space_after = Pt(16)

    p_qa = tf2.add_paragraph()
    p_qa.text = "Thank you! We welcome questions and feedback."
    p_qa.font.size = Pt(12)
    p_qa.font.bold = True
    p_qa.font.color.rgb = COLOR_RED

    # Save presentation
    output_pptx = r"c:\Users\Adonis\Downloads\LiftAndAestheticsMilestone2\Lift_and_Aesthetics_Presentation2.pptx"
    prs.save(output_pptx)
    print(f"Presentation saved successfully to: {output_pptx} (15 slides)")

if __name__ == '__main__':
    create_presentation()

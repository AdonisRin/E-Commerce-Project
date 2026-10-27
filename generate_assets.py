import os
import matplotlib.pyplot as plt
import matplotlib.patches as patches
from PIL import Image

output_dir = r"c:\Users\Adonis\Downloads\LiftAndAestheticsMilestone2\assets"
os.makedirs(output_dir, exist_ok=True)

# Set high-quality font defaults
plt.rcParams['font.sans-serif'] = 'Segoe UI, DejaVu Sans, Arial, Helvetica'
plt.rcParams['font.family'] = 'sans-serif'

# -----------------------------------------------------------------------------
# 1. ERD DIAGRAM (Human-like, Clean MySQL Workbench / Lucidchart style)
# -----------------------------------------------------------------------------
def generate_erd():
    fig, ax = plt.subplots(figsize=(19, 13), dpi=220)
    fig.patch.set_facecolor('#ffffff')
    ax.set_facecolor('#ffffff')
    ax.set_xlim(0, 1900)
    ax.set_ylim(0, 1300)
    ax.axis('off')

    # Diagram Title
    ax.text(950, 1260, "Lift & Aesthetics - Database Entity-Relationship Diagram (ERD)", 
            fontsize=20, fontweight='bold', color='#1e293b', ha='center', va='center')
    ax.text(950, 1230, "Relational Schema Model based on ASP.NET Core EF Core & MySQL Workbench Specification", 
            fontsize=12, color='#64748b', ha='center', va='center')

    # Function to draw a clean human-like table
    def draw_table(x, y, w, h, title, fields):
        header_h = 32
        # Card outline
        rect = patches.FancyBboxPatch((x, y), w, h, boxstyle="round,pad=0.0,rounding_size=4",
                                      facecolor='#ffffff', edgecolor='#94a3b8', linewidth=1.2)
        ax.add_patch(rect)
        
        # Header box
        header = patches.Rectangle((x, y + h - header_h), w, header_h,
                                   facecolor='#3b6998', edgecolor='#3b6998')
        ax.add_patch(header)
        ax.text(x + w/2, y + h - header_h/2, title, color='#ffffff', fontsize=11, fontweight='bold',
                ha='center', va='center')

        # Sub-header separator
        ax.plot([x, x + w], [y + h - header_h, y + h - header_h], color='#2d4f73', linewidth=1)

        # Field items
        cur_y = y + h - header_h - 18
        for field, ftype, is_pk, is_fk in fields:
            if is_pk:
                icon_color = '#d97706' # amber key
                ax.text(x + 10, cur_y, "PK", color='#ffffff', fontsize=7.5, fontweight='bold',
                        bbox=dict(boxstyle='square,pad=0.2', facecolor=icon_color, edgecolor='none'))
            elif is_fk:
                icon_color = '#0284c7' # blue key
                ax.text(x + 10, cur_y, "FK", color='#ffffff', fontsize=7.5, fontweight='bold',
                        bbox=dict(boxstyle='square,pad=0.2', facecolor=icon_color, edgecolor='none'))
            else:
                ax.plot(x + 16, cur_y + 1, marker='o', markersize=3, color='#94a3b8')

            weight = 'bold' if (is_pk or is_fk) else 'normal'
            txt_color = '#0f172a' if (is_pk or is_fk) else '#334155'
            ax.text(x + 32, cur_y, field, color=txt_color, fontsize=9.5, fontweight=weight, va='center')
            ax.text(x + w - 12, cur_y, ftype, color='#64748b', fontsize=8.5, ha='right', va='center')
            cur_y -= 21

    # 4 distinct, non-overlapping columns:
    # Col 1: x=60, w=270 (Memberships, Members, Reviews)
    # Col 2: x=400, w=280 (Trainers, FitnessClasses, ClassReservations)
    # Col 3: x=750, w=280 (Products, CartItems, ContactMessages)
    # Col 4: x=1100, w=280 (Orders, OrderItems, NutritionPlans)

    # 1. Memberships
    draw_table(60, 890, 270, 200, "Memberships", [
        ("MembershipId", "INT", True, False),
        ("Name", "VARCHAR(100)", False, False),
        ("Price", "DECIMAL(10,2)", False, False),
        ("Description", "TEXT", False, False),
        ("Features", "TEXT", False, False),
        ("DurationMonths", "INT", False, False)
    ])

    # 2. Members
    draw_table(60, 520, 270, 240, "Members", [
        ("MemberId", "INT", True, False),
        ("Name", "VARCHAR(120)", False, False),
        ("Email", "VARCHAR(150)", False, False),
        ("Password", "VARCHAR(255)", False, False),
        ("WalletBalance", "DECIMAL(10,2)", False, False),
        ("MembershipId", "INT", False, True),
        ("Phone", "VARCHAR(30)", False, False),
        ("JoinDate", "DATETIME", False, False)
    ])

    # 3. Reviews
    draw_table(60, 160, 270, 190, "Reviews", [
        ("ReviewId", "INT", True, False),
        ("MemberId", "INT", False, True),
        ("AuthorName", "VARCHAR(100)", False, False),
        ("Rating", "INT", False, False),
        ("Content", "TEXT", False, False),
        ("CreatedAt", "DATETIME", False, False)
    ])

    # 4. Trainers
    draw_table(400, 890, 280, 200, "Trainers", [
        ("TrainerId", "INT", True, False),
        ("Name", "VARCHAR(120)", False, False),
        ("Specialization", "VARCHAR(150)", False, False),
        ("Motto", "TEXT", False, False),
        ("PhotoUrl", "VARCHAR(255)", False, False),
        ("Phone", "VARCHAR(50)", False, False)
    ])

    # 5. FitnessClasses
    draw_table(400, 520, 280, 230, "FitnessClasses", [
        ("FitnessClassId", "INT", True, False),
        ("ClassName", "VARCHAR(120)", False, False),
        ("ScheduleTime", "DATETIME", False, False),
        ("Description", "TEXT", False, False),
        ("TrainerId", "INT", False, True),
        ("MaxCapacity", "INT", False, False),
        ("DurationMin", "INT", False, False)
    ])

    # 6. ClassReservations
    draw_table(400, 160, 280, 190, "ClassReservations", [
        ("ClassReservationId", "INT", True, False),
        ("MemberId", "INT", False, True),
        ("FitnessClassId", "INT", False, True),
        ("ReservationDate", "DATETIME", False, False),
        ("Status", "VARCHAR(50)", False, False)
    ])

    # 7. Products
    draw_table(750, 880, 280, 220, "Products", [
        ("ProductId", "INT", True, False),
        ("Name", "VARCHAR(180)", False, False),
        ("Price", "DECIMAL(10,2)", False, False),
        ("Category", "VARCHAR(60)", False, False),
        ("StockQuantity", "INT", False, False),
        ("Description", "TEXT", False, False),
        ("ImageUrl", "VARCHAR(255)", False, False)
    ])

    # 8. CartItems
    draw_table(750, 530, 280, 190, "CartItems", [
        ("CartItemId", "INT", True, False),
        ("MemberId", "INT", False, True),
        ("ProductId", "INT", False, True),
        ("Quantity", "INT", False, False),
        ("AddedAt", "DATETIME", False, False)
    ])

    # 9. ContactMessages
    draw_table(750, 160, 280, 190, "ContactMessages", [
        ("ContactMessageId", "INT", True, False),
        ("SenderName", "VARCHAR(120)", False, False),
        ("SenderEmail", "VARCHAR(150)", False, False),
        ("Subject", "VARCHAR(200)", False, False),
        ("MessageContent", "TEXT", False, False),
        ("SentDate", "DATETIME", False, False)
    ])

    # 10. Orders
    draw_table(1100, 860, 280, 240, "Orders", [
        ("OrderId", "INT", True, False),
        ("MemberId", "INT", False, True),
        ("OrderNumber", "VARCHAR(50)", False, False),
        ("OrderDate", "DATETIME", False, False),
        ("TotalAmount", "DECIMAL(10,2)", False, False),
        ("Status", "VARCHAR(40)", False, False),
        ("PaymentIntentId", "VARCHAR(100)", False, False),
        ("ShippingAddress", "TEXT", False, False)
    ])

    # 11. OrderItems
    draw_table(1100, 520, 280, 210, "OrderItems", [
        ("OrderItemId", "INT", True, False),
        ("OrderId", "INT", False, True),
        ("ProductId", "INT", False, True),
        ("UnitPrice", "DECIMAL(10,2)", False, False),
        ("Quantity", "INT", False, False),
        ("Subtotal", "DECIMAL(10,2)", False, False)
    ])

    # 12. Reviews
    draw_table(1100, 160, 280, 190, "Reviews", [
        ("ReviewId", "INT", True, False),
        ("MemberId", "INT", False, True),
        ("TargetCategory", "VARCHAR(50)", False, False),
        ("Rating", "INT", False, False),
        ("Comment", "NVARCHAR(MAX)", False, False),
        ("CreatedAt", "DATETIME2", False, False)
    ])

    # Relationships (Clean orthogonal lines, ZERO crossings through boxes!)
    line_col = '#475569'

    # Line 1: Memberships (Col 1 Top) -> Members (Col 1 Mid)
    ax.plot([195, 195], [890, 760], color=line_col, linewidth=1.5, linestyle='--')
    ax.text(205, 870, "1", fontsize=9.5, fontweight='bold', color='#1e293b')
    ax.text(205, 775, "N", fontsize=9.5, fontweight='bold', color='#1e293b')

    # Line 2: Members (Col 1 Mid) -> Reviews (Col 1 Bot)
    ax.plot([195, 195], [520, 350], color=line_col, linewidth=1.5, linestyle='--')
    ax.text(205, 500, "1", fontsize=9.5, fontweight='bold', color='#1e293b')
    ax.text(205, 365, "N", fontsize=9.5, fontweight='bold', color='#1e293b')

    # Line 3: Trainers (Col 2 Top) -> FitnessClasses (Col 2 Mid)
    ax.plot([540, 540], [890, 750], color=line_col, linewidth=1.5, linestyle='--')
    ax.text(550, 870, "1", fontsize=9.5, fontweight='bold', color='#1e293b')
    ax.text(550, 765, "N", fontsize=9.5, fontweight='bold', color='#1e293b')

    # Line 4: FitnessClasses (Col 2 Mid) -> ClassReservations (Col 2 Bot)
    ax.plot([540, 540], [520, 350], color=line_col, linewidth=1.5, linestyle='--')
    ax.text(550, 500, "1", fontsize=9.5, fontweight='bold', color='#1e293b')
    ax.text(550, 365, "N", fontsize=9.5, fontweight='bold', color='#1e293b')

    # Line 5: Members (Col 1 Mid) -> ClassReservations (Col 2 Bot)
    # Routes via gutter between Col 1 and Col 2: x=365
    ax.plot([330, 365, 365, 400], [550, 550, 255, 255], color=line_col, linewidth=1.5, linestyle='--')
    ax.text(338, 558, "1", fontsize=9.5, fontweight='bold', color='#1e293b')
    ax.text(388, 263, "N", fontsize=9.5, fontweight='bold', color='#1e293b')

    # Line 6: Products (Col 3 Top) -> CartItems (Col 3 Mid)
    ax.plot([890, 890], [880, 720], color=line_col, linewidth=1.5, linestyle='--')
    ax.text(900, 860, "1", fontsize=9.5, fontweight='bold', color='#1e293b')
    ax.text(900, 735, "N", fontsize=9.5, fontweight='bold', color='#1e293b')

    # Line 7: Members (Col 1 Mid) -> CartItems (Col 3 Mid)
    # Routes via horizontal gutter above middle row at y=800
    ax.plot([250, 250, 715, 715, 750], [760, 800, 800, 625, 625], color=line_col, linewidth=1.5, linestyle='--')
    ax.text(258, 770, "1", fontsize=9.5, fontweight='bold', color='#1e293b')
    ax.text(732, 633, "N", fontsize=9.5, fontweight='bold', color='#1e293b')

    # Line 8: Orders (Col 4 Top) -> OrderItems (Col 4 Mid)
    ax.plot([1240, 1240], [860, 730], color=line_col, linewidth=1.5, linestyle='--')
    ax.text(1250, 840, "1", fontsize=9.5, fontweight='bold', color='#1e293b')
    ax.text(1250, 745, "N", fontsize=9.5, fontweight='bold', color='#1e293b')

    # Line 9: Products (Col 3 Top) -> OrderItems (Col 4 Mid)
    # Routes via gutter between Col 3 and Col 4 at x=1065
    ax.plot([1030, 1065, 1065, 1100], [920, 920, 625, 625], color=line_col, linewidth=1.5, linestyle='--')
    ax.text(1038, 928, "1", fontsize=9.5, fontweight='bold', color='#1e293b')
    ax.text(1088, 633, "N", fontsize=9.5, fontweight='bold', color='#1e293b')

    # Line 10: Members (Col 1 Mid) -> Orders (Col 4 Top)
    # Routes via top horizontal gutter at y=1130
    ax.plot([130, 130, 1070, 1070, 1100], [760, 1130, 1130, 980, 980], color=line_col, linewidth=1.5, linestyle='--')
    ax.text(138, 770, "1", fontsize=9.5, fontweight='bold', color='#1e293b')
    ax.text(1088, 988, "N", fontsize=9.5, fontweight='bold', color='#1e293b')

    # Legend at top right
    leg = patches.FancyBboxPatch((1530, 1170), 280, 70, boxstyle="round,pad=0.0,rounding_size=4",
                                 facecolor='#f8fafc', edgecolor='#cbd5e1', linewidth=1)
    ax.add_patch(leg)
    ax.text(1550, 1220, "PK  Primary Key", fontsize=9, fontweight='bold', color='#b45309')
    ax.text(1550, 1190, "FK  Foreign Key", fontsize=9, fontweight='bold', color='#0284c7')
    ax.text(1680, 1205, "---  1:N Relationship", fontsize=9, color='#475569')

    plt.tight_layout()
    output_path = os.path.join(output_dir, "erd_diagram.png")
    plt.savefig(output_path, bbox_inches='tight', dpi=220)
    plt.close()
    print(f"ERD saved to: {output_path}")

# -----------------------------------------------------------------------------
# 2. ARCHITECTURE DIAGRAM (Human-like, Clean 3-Tier Enterprise Layout)
# -----------------------------------------------------------------------------
def generate_architecture():
    fig, ax = plt.subplots(figsize=(16, 11.5), dpi=220)
    fig.patch.set_facecolor('#ffffff')
    ax.set_facecolor('#ffffff')
    ax.set_xlim(0, 1600)
    ax.set_ylim(0, 1150)
    ax.axis('off')

    # Diagram Title
    ax.text(800, 1105, "Lift & Aesthetics - System Component Architecture", 
            fontsize=20, fontweight='bold', color='#1e293b', ha='center', va='center')
    ax.text(800, 1075, "Three-Tier MVC Architecture: Client Presentation, ASP.NET Core MVC Backend, and Data Services", 
            fontsize=12, color='#64748b', ha='center', va='center')

    # Tier Container Helper
    def draw_tier(x, y, w, h, title, subtitle, bg_color, border_color):
        rect = patches.FancyBboxPatch((x, y), w, h, boxstyle="round,pad=0.0,rounding_size=8",
                                      facecolor=bg_color, edgecolor=border_color, linewidth=1.5)
        ax.add_patch(rect)
        ax.text(x + 25, y + h - 26, title, fontsize=13, fontweight='bold', color='#0f172a', va='center')
        ax.text(x + 25, y + h - 48, subtitle, fontsize=9.5, color='#64748b', va='center')

    # Component Box Helper
    def draw_box(x, y, w, h, title, desc, tag=""):
        rect = patches.FancyBboxPatch((x, y), w, h, boxstyle="round,pad=0.0,rounding_size=6",
                                      facecolor='#ffffff', edgecolor='#cbd5e1', linewidth=1.2)
        ax.add_patch(rect)
        header_text = f"{tag} {title}" if tag else title
        ax.text(x + 16, y + h - 20, header_text, fontsize=10, fontweight='bold', color='#1e3a8a', va='center')
        ax.text(x + 16, y + (h - 22)/2, desc, fontsize=8.5, color='#475569', va='center')

    # 1. Tier 1: Client Layer (Top)
    draw_tier(80, 830, 1440, 200, "1. Client & Presentation Layer", 
              "Cross-browser responsive UI built with HTML5, CSS3, JavaScript & Razor Views", 
              "#f0f9ff", "#bae6fd")
    
    draw_box(110, 855, 260, 95, "Shop & Catalog View", "Category filter, search bar,\nproduct grid, pagination", "[UI]")
    draw_box(400, 855, 260, 95, "Shopping Cart & Checkout", "Cart table, coupon code,\naddress form, Stripe Elements", "[UI]")
    draw_box(690, 855, 260, 95, "User Profile & Memberships", "Subscriptions, workout classes,\norder history, review forms", "[UI]")
    draw_box(980, 855, 260, 95, "Admin Management Views", "Product CRUD, stock updates,\ninbox messages, user records", "[UI]")
    draw_box(1270, 855, 220, 95, "Auth & Account UI", "Login, register, roles,\nsession state management", "[UI]")

    # 2. Tier 2: Application / Controller & Business Layer (Middle)
    draw_tier(80, 440, 1440, 330, "2. Application & Business Logic Layer (ASP.NET Core 9 / C#)", 
              "Controllers, Identity authorization policies, and domain service abstractions", 
              "#f8fafc", "#cbd5e1")
    
    # Controllers sub-row with generous padding
    ax.text(110, 715, "CONTROLLERS (MVC)", fontsize=10, fontweight='bold', color='#334155')
    draw_box(110, 615, 260, 85, "ShopController", "Index(), Details(), SearchProducts(),\nAdmin Product CRUD endpoints", "[Ctrl]")
    draw_box(400, 615, 260, 85, "CartController", "AddToCart(), UpdateQuantity(),\nRemove(), Checkout() actions", "[Ctrl]")
    draw_box(690, 615, 260, 85, "AuthController", "Login(), Register(), Logout(),\nRole-based redirection", "[Ctrl]")
    draw_box(980, 615, 260, 85, "MembershipsController", "Index(), Subscribe(), Manage(),\nAppointment reservations", "[Ctrl]")
    draw_box(1270, 615, 220, 85, "Admin / Inbox", "ContactMessages, Reviews,\nUser membership management", "[Ctrl]")

    # Services sub-row
    ax.text(110, 565, "SERVICE LAYER (DEPENDENCY INJECTION)", fontsize=10, fontweight='bold', color='#334155')
    draw_box(110, 465, 260, 85, "ProductService", "Handles product filtering, stock\nvalidation and image saving", "[Svc]")
    draw_box(400, 465, 260, 85, "CartService", "Calculates subtotal, discounts,\nand persists member cart items", "[Svc]")
    draw_box(690, 465, 260, 85, "AuthService & Identity", "UserManager, RoleManager,\nclaims-based authorization", "[Svc]")
    draw_box(980, 465, 260, 85, "PaymentProcessor (Stripe)", "Creates PaymentIntent, verifies\nwebhook signatures & events", "[Svc]")
    draw_box(1270, 465, 220, 85, "MemberService", "Manages wallet balance, gym\nreservations and subscriptions", "[Svc]")

    # 3. Tier 3: Data & External Infrastructure Layer (Bottom)
    draw_tier(80, 80, 1440, 290, "3. Data Persistence & External Infrastructure", 
              "Entity Framework Core ORM, Relational Database, and Sandbox Payment Gateway", 
              "#fefce8", "#fef08a")

    draw_box(110, 115, 380, 195, "Entity Framework Core (LiftContext)", 
             "• Unit of Work & Repository Pattern\n• DbSets: Products, CartItems, Orders,\n  Members, Memberships, Trainers\n• Connection Pooling & Change Tracking\n• Automatic Schema Migrations", "[ORM]")

    draw_box(530, 115, 450, 195, "Relational Database (MySQL / SQL Server)", 
             "• ACID Transaction Compliance\n• Foreign Key constraints & cascading rules\n• Indexed search on Category and Sku\n• Stored Seed Data (30+ products, 4 users)\n• Secure AspNetUsers identity tables", "[DB]")

    draw_box(1020, 115, 470, 195, "External Sandbox (Stripe API & Webhooks)", 
             "• Stripe Checkout / PaymentIntents API\n• Test credit card processing (simulated)\n• Secure Webhook listener (/api/payments/webhook)\n• TLS 1.3 encrypted payload exchange\n• Signature verification with Stripe Secret Key", "[API]")

    # Connecting Arrows between Tiers
    arrow_props = dict(facecolor='#475569', edgecolor='#475569', width=1.2, headwidth=6, headlength=7)
    
    # Tier 1 to Tier 2
    ax.annotate('', xy=(240, 775), xytext=(240, 830), arrowprops=arrow_props)
    ax.annotate('', xy=(530, 775), xytext=(530, 830), arrowprops=arrow_props)
    ax.annotate('', xy=(820, 775), xytext=(820, 830), arrowprops=arrow_props)
    ax.annotate('', xy=(1110, 775), xytext=(1110, 830), arrowprops=arrow_props)
    ax.text(820, 802, "HTTP GET / POST (JSON & Form URL-Encoded)", fontsize=9, fontweight='bold',
            color='#1e293b', ha='center', backgroundcolor='#ffffff')

    # Tier 2 to Tier 3
    ax.annotate('', xy=(300, 375), xytext=(300, 440), arrowprops=arrow_props)
    ax.annotate('', xy=(750, 375), xytext=(750, 440), arrowprops=arrow_props)
    ax.annotate('', xy=(1250, 375), xytext=(1250, 440), arrowprops=arrow_props)
    ax.text(500, 405, "LINQ Queries & DbContext Operations", fontsize=9, fontweight='bold',
            color='#1e293b', ha='center', backgroundcolor='#ffffff')
    ax.text(1250, 405, "HTTPS REST API & Webhooks", fontsize=9, fontweight='bold',
            color='#1e293b', ha='center', backgroundcolor='#ffffff')

    plt.tight_layout()
    output_path = os.path.join(output_dir, "architecture_diagram.png")
    plt.savefig(output_path, bbox_inches='tight', dpi=220)
    plt.close()
    print(f"Architecture diagram saved to: {output_path}")

# -----------------------------------------------------------------------------
# 3. CRITICAL FLOW SEQUENCE DIAGRAM (Clean Human UML Sequence)
# -----------------------------------------------------------------------------
def generate_sequence():
    fig, ax = plt.subplots(figsize=(17, 13), dpi=220)
    fig.patch.set_facecolor('#ffffff')
    ax.set_facecolor('#ffffff')
    ax.set_xlim(0, 1700)
    ax.set_ylim(0, 1300)
    ax.axis('off')

    # Title
    ax.text(850, 1260, "Critical Flow: Payment, Order Processing & Stock Update Sequence", 
            fontsize=20, fontweight='bold', color='#1e293b', ha='center', va='center')
    ax.text(850, 1230, "Strict UML Sequence Flow: Checkout -> Stripe Gateway -> Webhook -> Order Status -> Stock Update", 
            fontsize=12, color='#64748b', ha='center', va='center')

    # Lifelines
    actors = [
        ("Customer\n(Browser UI)", 160, '#e0f2fe', '#0284c7'),
        ("CartController\n(ASP.NET Core)", 500, '#f1f5f9', '#475569'),
        ("Stripe Gateway\n(Payment Processor)", 860, '#fdf4ff', '#a855f7'),
        ("Webhook Listener\n(/api/webhook)", 1210, '#ecfdf5', '#10b981'),
        ("LiftContext & DB\n(MySQL Storage)", 1530, '#fefce8', '#eab308')
    ]

    for title, x, bg, border in actors:
        # Actor Box
        rect = patches.FancyBboxPatch((x - 85, 1110), 170, 65, boxstyle="round,pad=0.0,rounding_size=5",
                                      facecolor=bg, edgecolor=border, linewidth=1.5)
        ax.add_patch(rect)
        ax.text(x, 1142, title, fontsize=10, fontweight='bold', color='#0f172a', ha='center', va='center')

        # Vertical dashed lifeline
        ax.plot([x, x], [1110, 110], color='#cbd5e1', linewidth=1.5, linestyle='--')

    # Message Helper with proper spacing
    def draw_msg(y, x1, x2, num, text, is_async=False, is_return=False, note=""):
        line_color = '#0284c7' if not is_return else '#64748b'
        ls = '--' if (is_return or is_async) else '-'
        
        # Arrow line
        ax.plot([x1, x2], [y, y], color=line_color, linewidth=1.4, linestyle=ls)
        
        # Arrowhead
        dx = 10 if x2 > x1 else -10
        ax.plot([x2 - dx, x2], [y + 4, y], color=line_color, linewidth=1.4)
        ax.plot([x2 - dx, x2], [y - 4, y], color=line_color, linewidth=1.4)

        # Label
        mx = (x1 + x2) / 2
        label_text = f"{num}. {text}" if num else text
        fontweight = 'bold' if num else 'normal'
        ax.text(mx, y + 10, label_text, fontsize=9.5, fontweight=fontweight, 
                color='#1e293b' if not is_return else '#475569', ha='center', va='bottom',
                bbox=dict(boxstyle='round,pad=0.2', facecolor='#ffffff', edgecolor='none'))

        if note:
            ax.text(mx, y - 12, note, fontsize=8, color='#64748b', ha='center', va='top')

    # Activation Box Helper
    def draw_act(x, y_top, y_bot, color='#bae6fd'):
        rect = patches.Rectangle((x - 8, y_bot), 16, y_top - y_bot,
                                 facecolor=color, edgecolor='#64748b', linewidth=1)
        ax.add_patch(rect)

    # 1. Customer initiates checkout
    draw_act(160, 1070, 830, '#bae6fd')
    draw_act(500, 1070, 840, '#e2e8f0')
    draw_msg(1060, 160, 500, "1", "POST /Cart/Checkout (Delivery details, selected cart items)")

    # 2. Server creates pending order in DB
    draw_act(1530, 1010, 930, '#fef08a')
    draw_msg(1005, 500, 1530, "2", "Create Order Record (Status: 'PendingPayment')", note="Stock remains reserved/unaltered")
    draw_msg(955, 1530, 500, "", "Order Created (OrderId: 1042)", is_return=True)

    # 3. Server calls Stripe API to create Payment Session
    draw_act(860, 910, 780, '#f5d0fe')
    draw_msg(900, 500, 860, "3", "POST /v1/payment_intents (amount, currency='usd', order_id)")
    draw_msg(850, 860, 500, "", "Returns client_secret & session URL", is_return=True)

    # 4. Redirect customer to Stripe payment UI
    draw_msg(800, 500, 160, "4", "Render Payment Page with Stripe Elements Client Secret", is_return=True)

    # 5. Customer enters card and submits payment directly to Stripe
    draw_act(160, 760, 690, '#bae6fd')
    draw_msg(740, 160, 860, "5", "Customer Submits Card Details to Stripe (Simulated Sandbox)")

    # 6. Stripe processes payment and sends async webhook
    draw_act(1210, 690, 260, '#a7f3d0')
    draw_msg(675, 860, 1210, "6", "Asynchronous HTTPS Webhook: 'payment_intent.succeeded'", is_async=True,
             note="Payload contains metadata: order_id=1042")

    # 7. Webhook validates signature
    draw_act(1210, 625, 575, '#6ee7b7')
    ax.plot([1218, 1260, 1260, 1218], [620, 620, 585, 585], color='#047857', linewidth=1.2)
    ax.text(1270, 602, "7. Verify Stripe Signature Header\n   using Webhook Signing Secret", fontsize=8.5, color='#065f46', va='center')

    # 8. Update Order status in DB
    draw_act(1530, 545, 300, '#fef08a')
    draw_msg(535, 1210, 1530, "8", "UPDATE Orders SET Status='Paid', PaidAt=NOW() WHERE Id=1042")

    # 9. Decrement product stock in DB (with generous breathing room)
    draw_msg(465, 1210, 1530, "9", "UPDATE Products SET StockQuantity = StockQuantity - Qty", 
             note="Atomically decrements stock to prevent overselling")

    # 10. Clear user cart in DB
    draw_msg(395, 1210, 1530, "10", "DELETE FROM CartItems WHERE MemberId = user_id")
    draw_msg(345, 1530, 1210, "", "DB Transaction Committed Successfully", is_return=True)

    # 11. Webhook acknowledges receipt to Stripe
    draw_msg(290, 1210, 860, "11", "HTTP 200 OK (Acknowledge event delivery)", is_return=True)

    # 12. Customer redirected to Order Confirmation
    draw_act(160, 240, 140, '#bae6fd')
    draw_act(500, 240, 150, '#e2e8f0')
    draw_msg(225, 160, 500, "12", "GET /Shop/OrderConfirmation?orderId=1042")
    draw_msg(165, 500, 160, "13", "Render Confirmation View with Receipt & Order Summary", is_return=True)

    # Note Box at bottom
    note_box = patches.FancyBboxPatch((120, 65), 1460, 65, boxstyle="round,pad=0.0,rounding_size=6",
                                      facecolor='#f8fafc', edgecolor='#cbd5e1', linewidth=1)
    ax.add_patch(note_box)
    ax.text(850, 97, "Critical Failure Rule: If payment fails or is declined, order remains in 'PendingPayment' status.\nStock is NOT decremented until the webhook confirms successful charge capture. Cancelled orders restore reserved stock.",
            fontsize=9.5, fontweight='bold', color='#b91c1c', ha='center', va='center')

    plt.tight_layout()
    output_path = os.path.join(output_dir, "sequence_diagram.png")
    plt.savefig(output_path, bbox_inches='tight', dpi=220)
    plt.close()
    print(f"Sequence diagram saved to: {output_path}")

# -----------------------------------------------------------------------------
# 4. WIREFRAMES
# -----------------------------------------------------------------------------
def generate_wireframes():
    fig, axes = plt.subplots(2, 3, figsize=(18, 12), dpi=220)
    fig.patch.set_facecolor('#ffffff')
    plt.subplots_adjust(wspace=0.18, hspace=0.25)

    def style_wireframe(ax, title, badge_text):
        ax.set_facecolor('#f8fafc')
        ax.set_xlim(0, 100)
        ax.set_ylim(0, 100)
        ax.set_xticks([])
        ax.set_yticks([])
        for spine in ax.spines.values():
            spine.set_color('#cbd5e1')
            spine.set_linewidth(1.5)
        ax.add_patch(patches.Rectangle((0, 90), 100, 10, facecolor='#1e293b', edgecolor='none'))
        ax.text(5, 95, "LIFT & AESTHETICS", color='#ffffff', fontsize=8, fontweight='bold', va='center')
        ax.text(95, 95, badge_text, color='#fca5a5', fontsize=7.5, fontweight='bold', ha='right', va='center')
        ax.set_title(title, fontsize=12, fontweight='bold', color='#0f172a', pad=10)

    # 1. Catalog Wireframe (A1, A2, B1)
    ax1 = axes[0, 0]
    style_wireframe(ax1, "1. Catalog / Shop Grid (A1, A2, B1)", "SHOP CATALOG")
    ax1.add_patch(patches.Rectangle((5, 78), 65, 8, facecolor='#ffffff', edgecolor='#94a3b8', linewidth=1))
    ax1.text(8, 82, "Search products (e.g. Whey, Creatine)...", fontsize=6.5, color='#64748b', va='center')
    ax1.add_patch(patches.Rectangle((73, 78), 22, 8, facecolor='#dc2626', edgecolor='none'))
    ax1.text(84, 82, "Filter", fontsize=7, color='#ffffff', fontweight='bold', ha='center', va='center')

    chips = ["All", "Supplements", "Clothing", "Equipment", "Accessories"]
    cx = 5
    for c in chips:
        bg = '#1e293b' if c == "All" else '#e2e8f0'
        tc = '#ffffff' if c == "All" else '#334155'
        ax1.add_patch(patches.FancyBboxPatch((cx, 68), 16, 6, boxstyle="round,pad=0.2", facecolor=bg, edgecolor='none'))
        ax1.text(cx + 8, 71, c, fontsize=5.5, color=tc, ha='center', va='center')
        cx += 18.5

    coords = [(5, 36), (52, 36), (5, 5), (52, 5)]
    for i, (px, py) in enumerate(coords):
        ax1.add_patch(patches.Rectangle((px, py), 43, 28, facecolor='#ffffff', edgecolor='#cbd5e1', linewidth=1))
        ax1.add_patch(patches.Rectangle((px + 2, py + 12), 39, 14, facecolor='#f1f5f9', edgecolor='#e2e8f0'))
        ax1.text(px + 21.5, py + 19, "[Product Photo]", fontsize=6, color='#94a3b8', ha='center', va='center')
        ax1.text(px + 4, py + 8, f"Product {i+1} Title", fontsize=6.5, fontweight='bold', color='#1e293b')
        ax1.text(px + 4, py + 3, f"{120 + i*45} LEI", fontsize=6, fontweight='bold', color='#dc2626')
        ax1.add_patch(patches.Rectangle((px + 26, py + 2), 15, 5, facecolor='#1e293b', edgecolor='none'))
        ax1.text(px + 33.5, py + 4.5, "+ Cart", fontsize=5.5, color='#ffffff', fontweight='bold', ha='center', va='center')

    # 2. Product Details Wireframe (A1)
    ax2 = axes[0, 1]
    style_wireframe(ax2, "2. Product Details View (A1)", "PRODUCT DETAILS")
    ax2.add_patch(patches.Rectangle((6, 35), 42, 48, facecolor='#ffffff', edgecolor='#cbd5e1', linewidth=1.2))
    ax2.text(27, 59, "[High-Res Image]", fontsize=7.5, color='#94a3b8', ha='center', va='center')

    ax2.add_patch(patches.Rectangle((52, 77), 20, 5, facecolor='#dcfce7', edgecolor='none'))
    ax2.text(62, 79.5, "IN STOCK (45 units)", fontsize=5.5, color='#15803d', fontweight='bold', ha='center', va='center')
    ax2.text(52, 71, "100% Whey Gold 2kg", fontsize=8.5, fontweight='bold', color='#0f172a')
    ax2.text(52, 64, "Category: Supplements | SKU: SUP-001", fontsize=6, color='#64748b')
    ax2.text(52, 55, "280.00 LEI", fontsize=11, fontweight='bold', color='#dc2626')

    ax2.add_patch(patches.Rectangle((52, 43), 12, 7, facecolor='#ffffff', edgecolor='#94a3b8'))
    ax2.text(58, 46.5, "Qty: 1", fontsize=6.5, color='#334155', ha='center', va='center')
    ax2.add_patch(patches.Rectangle((67, 43), 27, 7, facecolor='#dc2626', edgecolor='none'))
    ax2.text(80.5, 46.5, "ADD TO CART", fontsize=6.5, color='#ffffff', fontweight='bold', ha='center', va='center')

    ax2.add_patch(patches.Rectangle((6, 8), 88, 22, facecolor='#ffffff', edgecolor='#e2e8f0'))
    ax2.text(9, 25, "Product Description & Nutritional Breakdown", fontsize=6.5, fontweight='bold', color='#1e293b')
    ax2.text(9, 18, "24g of whey isolate protein per serving with 5.5g of BCAAs.", fontsize=6, color='#64748b')
    ax2.text(9, 13, "Optimal for post-workout muscle hypertrophy and quick recovery.", fontsize=6, color='#64748b')

    # 3. Shopping Cart Wireframe (A5, B5)
    ax3 = axes[0, 2]
    style_wireframe(ax3, "3. Shopping Cart (A5, B5)", "SHOPPING CART")
    ax3.add_patch(patches.Rectangle((5, 78), 90, 6, facecolor='#e2e8f0', edgecolor='none'))
    ax3.text(8, 81, "Product", fontsize=6, fontweight='bold', color='#334155', va='center')
    ax3.text(50, 81, "Price", fontsize=6, fontweight='bold', color='#334155', va='center')
    ax3.text(65, 81, "Qty", fontsize=6, fontweight='bold', color='#334155', va='center')
    ax3.text(82, 81, "Total", fontsize=6, fontweight='bold', color='#334155', va='center')

    ax3.add_patch(patches.Rectangle((5, 63), 90, 13, facecolor='#ffffff', edgecolor='#e2e8f0'))
    ax3.text(8, 70, "Whey Protein 2kg", fontsize=6.5, fontweight='bold', color='#0f172a')
    ax3.text(50, 70, "280 LEI", fontsize=6, color='#475569')
    ax3.text(65, 70, "[- 1 +]", fontsize=6, color='#0284c7', fontweight='bold')
    ax3.text(82, 70, "280 LEI", fontsize=6.5, fontweight='bold', color='#0f172a')

    ax3.add_patch(patches.Rectangle((5, 48), 90, 13, facecolor='#ffffff', edgecolor='#e2e8f0'))
    ax3.text(8, 55, "Creatine Mono 500g", fontsize=6.5, fontweight='bold', color='#0f172a')
    ax3.text(50, 55, "67 LEI", fontsize=6, color='#475569')
    ax3.text(65, 55, "[- 2 +]", fontsize=6, color='#0284c7', fontweight='bold')
    ax3.text(82, 55, "134 LEI", fontsize=6.5, fontweight='bold', color='#0f172a')

    ax3.add_patch(patches.Rectangle((5, 28), 50, 12, facecolor='#f1f5f9', edgecolor='#cbd5e1'))
    ax3.text(8, 35, "Promo Code (B5): [ LIFT10 ]", fontsize=6, color='#334155')
    ax3.add_patch(patches.Rectangle((37, 30), 16, 7, facecolor='#3b82f6', edgecolor='none'))
    ax3.text(45, 33.5, "Apply", fontsize=5.5, color='#ffffff', fontweight='bold', ha='center', va='center')

    ax3.add_patch(patches.Rectangle((58, 12), 37, 30, facecolor='#ffffff', edgecolor='#cbd5e1'))
    ax3.text(61, 36, "Subtotal: 414.00 LEI", fontsize=6, color='#475569')
    ax3.text(61, 30, "Discount: -41.40 LEI", fontsize=6, color='#16a34a')
    ax3.text(61, 23, "Total: 372.60 LEI", fontsize=7.5, fontweight='bold', color='#dc2626')
    ax3.add_patch(patches.Rectangle((60, 14), 33, 6, facecolor='#dc2626', edgecolor='none'))
    ax3.text(76.5, 17, "CHECKOUT ->", fontsize=6, color='#ffffff', fontweight='bold', ha='center', va='center')

    # 4. Checkout Wireframe (A6, A7)
    ax4 = axes[1, 0]
    style_wireframe(ax4, "4. Checkout & Payment View (A6, A7)", "CHECKOUT")
    ax4.add_patch(patches.Rectangle((5, 45), 50, 40, facecolor='#ffffff', edgecolor='#cbd5e1'))
    ax4.text(8, 80, "Shipping & Customer Details", fontsize=6.5, fontweight='bold', color='#1e293b')
    ax4.add_patch(patches.Rectangle((8, 68), 44, 7, facecolor='#f8fafc', edgecolor='#cbd5e1'))
    ax4.text(11, 71.5, "Full Name: Adonis Ploae", fontsize=5.5, color='#475569')
    ax4.add_patch(patches.Rectangle((8, 57), 44, 7, facecolor='#f8fafc', edgecolor='#cbd5e1'))
    ax4.text(11, 60.5, "Address: Str. Academiei 14, Bucharest", fontsize=5.5, color='#475569')
    ax4.add_patch(patches.Rectangle((8, 47), 44, 6, facecolor='#f8fafc', edgecolor='#cbd5e1'))
    ax4.text(11, 50, "Phone: +40 722 000 000", fontsize=5.5, color='#475569')

    ax4.add_patch(patches.Rectangle((5, 10), 50, 30, facecolor='#ffffff', edgecolor='#cbd5e1'))
    ax4.text(8, 35, "Secure Payment (Stripe Sandbox)", fontsize=6.5, fontweight='bold', color='#1e293b')
    ax4.add_patch(patches.Rectangle((8, 22), 44, 8, facecolor='#f8fafc', edgecolor='#94a3b8'))
    ax4.text(11, 26, "Card: 4242 •••• •••• 4242   MM/YY  CVC", fontsize=5.5, color='#334155')
    ax4.add_patch(patches.Rectangle((8, 12), 44, 7, facecolor='#16a34a', edgecolor='none'))
    ax4.text(30, 15.5, "PAY 372.60 LEI (TEST MODE)", fontsize=6, color='#ffffff', fontweight='bold', ha='center', va='center')

    ax4.add_patch(patches.Rectangle((58, 25), 37, 60, facecolor='#ffffff', edgecolor='#cbd5e1'))
    ax4.text(61, 80, "Order Summary", fontsize=6.5, fontweight='bold', color='#1e293b')
    ax4.text(61, 72, "• 1x Whey Gold 2kg", fontsize=5.5, color='#475569')
    ax4.text(61, 65, "• 2x Creatine Mono", fontsize=5.5, color='#475569')
    ax4.plot([60, 92], [58, 58], color='#e2e8f0')
    ax4.text(61, 52, "Shipping: FREE", fontsize=5.5, color='#15803d')
    ax4.text(61, 44, "Total: 372.60 LEI", fontsize=7.5, fontweight='bold', color='#dc2626')

    # 5. Administration Panel Wireframe (A9, A10)
    ax5 = axes[1, 1]
    style_wireframe(ax5, "5. Administration Panel (A9, A10)", "ADMIN PANEL")
    ax5.add_patch(patches.Rectangle((5, 75), 28, 11, facecolor='#ffffff', edgecolor='#cbd5e1'))
    ax5.text(7, 82, "Products: 32", fontsize=6, fontweight='bold', color='#1e293b')
    ax5.text(7, 77, "In Stock: 98%", fontsize=5.5, color='#16a34a')

    ax5.add_patch(patches.Rectangle((36, 75), 28, 11, facecolor='#ffffff', edgecolor='#cbd5e1'))
    ax5.text(38, 82, "Orders: 14", fontsize=6, fontweight='bold', color='#1e293b')
    ax5.text(38, 77, "Pending: 2", fontsize=5.5, color='#d97706')

    ax5.add_patch(patches.Rectangle((67, 75), 28, 11, facecolor='#dc2626', edgecolor='none'))
    ax5.text(81, 80.5, "+ Add Product", fontsize=6.5, color='#ffffff', fontweight='bold', ha='center', va='center')

    ax5.add_patch(patches.Rectangle((5, 12), 90, 58, facecolor='#ffffff', edgecolor='#cbd5e1'))
    ax5.add_patch(patches.Rectangle((5, 62), 90, 8, facecolor='#f1f5f9', edgecolor='none'))
    ax5.text(8, 66, "Product Name", fontsize=5.5, fontweight='bold', color='#475569')
    ax5.text(48, 66, "Category", fontsize=5.5, fontweight='bold', color='#475569')
    ax5.text(66, 66, "Price", fontsize=5.5, fontweight='bold', color='#475569')
    ax5.text(80, 66, "Actions", fontsize=5.5, fontweight='bold', color='#475569')

    sample_prods = [
        ("Whey Protein 2kg", "Supplements", "280 LEI"),
        ("Creatine Mono 500g", "Supplements", "67 LEI"),
        ("Heavyweight Pump Cover", "Clothing", "140 LEI"),
        ("10mm Lever Belt", "Equipment", "380 LEI")
    ]
    ry = 50
    for name, cat, price in sample_prods:
        ax5.text(8, ry + 3, name, fontsize=5.5, color='#0f172a')
        ax5.text(48, ry + 3, cat, fontsize=5.5, color='#64748b')
        ax5.text(66, ry + 3, price, fontsize=5.5, color='#1e293b')
        ax5.add_patch(patches.Rectangle((78, ry + 1), 7, 5, facecolor='#0284c7', edgecolor='none'))
        ax5.text(81.5, ry + 3.5, "Edit", fontsize=4.5, color='#ffffff', ha='center', va='center')
        ax5.add_patch(patches.Rectangle((87, ry + 1), 7, 5, facecolor='#dc2626', edgecolor='none'))
        ax5.text(90.5, ry + 3.5, "Del", fontsize=4.5, color='#ffffff', ha='center', va='center')
        ax5.plot([5, 95], [ry - 1, ry - 1], color='#f1f5f9')
        ry -= 11

    # 6. User Account & Orders Wireframe (A3, A4, B7)
    ax6 = axes[1, 2]
    style_wireframe(ax6, "6. User Account & Orders (A3, A4, B7)", "MY ACCOUNT")
    ax6.add_patch(patches.Rectangle((5, 60), 90, 26, facecolor='#ffffff', edgecolor='#cbd5e1'))
    ax6.text(8, 78, "Athlete Profile: athlete@lift.com", fontsize=6.5, fontweight='bold', color='#1e293b')
    ax6.text(8, 71, "Membership: PRO Lifter (Active) | Wallet Balance: 450.00 LEI", fontsize=5.5, color='#475569')
    ax6.add_patch(patches.Rectangle((8, 62), 22, 6, facecolor='#1e293b', edgecolor='none'))
    ax6.text(19, 65, "Past Orders (A4)", fontsize=5, color='#ffffff', fontweight='bold', ha='center', va='center')
    ax6.add_patch(patches.Rectangle((33, 62), 20, 6, facecolor='#f1f5f9', edgecolor='#cbd5e1'))
    ax6.text(43, 65, "Wishlist (B7)", fontsize=5, color='#475569', ha='center', va='center')

    ax6.add_patch(patches.Rectangle((5, 12), 90, 44, facecolor='#ffffff', edgecolor='#cbd5e1'))
    orders_sample = [
        ("ORD-2026-001", "24 Oct 2026", "372.60 LEI", "Paid (Delivered)"),
        ("ORD-2026-002", "18 Oct 2026", "140.00 LEI", "Paid (Shipped)"),
        ("ORD-2026-003", "02 Oct 2026", "380.00 LEI", "Paid (Completed)")
    ]
    oy = 44
    for onum, odate, ototal, ostatus in orders_sample:
        ax6.text(8, oy + 2, f"{onum}  ({odate})", fontsize=5.5, fontweight='bold', color='#0f172a')
        ax6.text(58, oy + 2, ototal, fontsize=5.5, color='#1e293b')
        ax6.text(73, oy + 2, ostatus, fontsize=5, color='#16a34a', fontweight='bold')
        ax6.plot([5, 95], [oy - 2, oy - 2], color='#f1f5f9')
        oy -= 12

    plt.tight_layout()
    output_path = os.path.join(output_dir, "wireframes.png")
    plt.savefig(output_path, bbox_inches='tight', dpi=220)
    plt.close()
    print(f"Wireframes saved to: {output_path}")

if __name__ == '__main__':
    generate_erd()
    generate_architecture()
    generate_sequence()
    generate_wireframes()
    print("All diagrams regenerated with clean human-like spacing!")

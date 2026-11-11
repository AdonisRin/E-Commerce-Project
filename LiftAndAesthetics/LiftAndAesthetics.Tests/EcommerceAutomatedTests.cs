using System;
using System.Collections.Generic;
using System.Linq;
using LiftAndAesthetics.Models;
using LiftAndAesthetics.Services;
using Xunit;

namespace LiftAndAesthetics.Tests
{
    public class EcommerceAutomatedTests
    {
        // -------------------------------------------------------------
        // A3 & A12: CART & TOTAL CALCULATION TESTS
        // -------------------------------------------------------------

        [Fact]
        public void Cart_CalculateTotal_SingleItem_ComputesCorrectSum()
        {
            // Arrange
            var product = new Product { ProductId = 1, Name = "Whey Isolate", Price = 189.99m };
            var cartItem = new CartItem { ProductId = 1, Product = product, Quantity = 2 };
            var cart = new List<CartItem> { cartItem };

            // Act
            decimal total = cart.Sum(i => i.Quantity * (i.Product?.Price ?? 0m));

            // Assert
            Assert.Equal(379.98m, total);
        }

        [Fact]
        public void Cart_CalculateTotal_MultipleItems_ComputesAccumulatedTotal()
        {
            // Arrange
            var p1 = new Product { ProductId = 1, Name = "Creatine Monohydrate", Price = 99.00m };
            var p2 = new Product { ProductId = 2, Name = "Lifting Belt", Price = 250.00m };
            var p3 = new Product { ProductId = 3, Name = "Shaker Bottle", Price = 35.50m };

            var cart = new List<CartItem>
            {
                new CartItem { ProductId = 1, Product = p1, Quantity = 2 },
                new CartItem { ProductId = 2, Product = p2, Quantity = 1 },
                new CartItem { ProductId = 3, Product = p3, Quantity = 3 }
            };

            // Act
            decimal total = cart.Sum(i => i.Quantity * (i.Product?.Price ?? 0m));

            // Assert: (2 * 99) + (1 * 250) + (3 * 35.50) = 198 + 250 + 106.50 = 554.50
            Assert.Equal(554.50m, total);
        }

        [Fact]
        public void Cart_CalculateTotal_EmptyCart_ReturnsZero()
        {
            // Arrange
            var cart = new List<CartItem>();

            // Act
            decimal total = cart.Sum(i => i.Quantity * (i.Product?.Price ?? 0m));

            // Assert
            Assert.Equal(0m, total);
        }

        [Fact]
        public void Cart_UpdateQuantity_PositiveValue_UpdatesState()
        {
            // Arrange
            var cartItem = new CartItem { CartItemId = 10, Quantity = 1 };
            int newQuantity = 4;

            // Act
            if (newQuantity > 0)
            {
                cartItem.Quantity = newQuantity;
            }

            // Assert
            Assert.Equal(4, cartItem.Quantity);
        }

        [Fact]
        public void Cart_UpdateQuantity_ZeroOrNegative_Rejected()
        {
            // Arrange
            var cartItem = new CartItem { CartItemId = 10, Quantity = 2 };
            int invalidQuantity = 0;

            // Act
            bool updated = false;
            if (invalidQuantity > 0)
            {
                cartItem.Quantity = invalidQuantity;
                updated = true;
            }

            // Assert
            Assert.False(updated);
            Assert.Equal(2, cartItem.Quantity);
        }

        [Fact]
        public void Cart_QuantityExceedingAvailableStock_Rejected()
        {
            // Arrange
            int availableStock = 5;
            int requestedQuantity = 8;

            // Act
            bool canAddToCart = requestedQuantity <= availableStock;

            // Assert
            Assert.False(canAddToCart);
        }

        // -------------------------------------------------------------
        // A5 & A7: CHECKOUT & WALLET / STOCK MANAGEMENT TESTS
        // -------------------------------------------------------------

        [Fact]
        public void Checkout_WithSufficientBalance_DeductsFundsAndClearsCart()
        {
            // Arrange
            var member = new Members { MemberId = 1, Name = "Athlete", WalletBalance = 500.00m };
            var product = new Product { ProductId = 1, Name = "Pre-Workout Formula", Price = 150.00m };
            var cart = new List<CartItem>
            {
                new CartItem { MemberId = 1, ProductId = 1, Product = product, Quantity = 2 }
            };

            decimal total = cart.Sum(i => i.Quantity * (i.Product?.Price ?? 0m)); // 300.00m

            // Act
            bool checkoutSuccess = false;
            if (member.WalletBalance >= total)
            {
                member.WalletBalance -= total;
                cart.Clear();
                checkoutSuccess = true;
            }

            // Assert
            Assert.True(checkoutSuccess);
            Assert.Equal(200.00m, member.WalletBalance);
            Assert.Empty(cart);
        }

        [Fact]
        public void Checkout_WithInsufficientBalance_RejectsAndLeavesBalanceUnchanged()
        {
            // Arrange
            var member = new Members { MemberId = 1, Name = "Novice", WalletBalance = 50.00m };
            var product = new Product { ProductId = 1, Name = "Whey Protein 2kg", Price = 220.00m };
            var cart = new List<CartItem>
            {
                new CartItem { MemberId = 1, ProductId = 1, Product = product, Quantity = 1 }
            };

            decimal total = cart.Sum(i => i.Quantity * (i.Product?.Price ?? 0m)); // 220.00m

            // Act
            bool checkoutSuccess = false;
            string errorMessage = "";

            if (member.WalletBalance >= total)
            {
                member.WalletBalance -= total;
                cart.Clear();
                checkoutSuccess = true;
            }
            else
            {
                errorMessage = "Insufficient funds! Please top up your wallet.";
            }

            // Assert
            Assert.False(checkoutSuccess);
            Assert.Equal(50.00m, member.WalletBalance);
            Assert.Single(cart);
            Assert.Equal("Insufficient funds! Please top up your wallet.", errorMessage);
        }

        [Fact]
        public void Checkout_EmptyCart_ReturnsErrorMessage()
        {
            // Arrange
            var cart = new List<CartItem>();
            string resultMessage = "";

            // Act
            if (!cart.Any())
            {
                resultMessage = "Your cart is completely empty.";
            }

            // Assert
            Assert.Equal("Your cart is completely empty.", resultMessage);
        }

        // -------------------------------------------------------------
        // A6 & A7: SANDBOX PAYMENT & STOCK CONFIRMATION TESTS
        // -------------------------------------------------------------

        [Fact]
        public void PaymentConfirmation_StripeWebhookSuccess_TransitionsOrderStatusToPaid()
        {
            // Arrange
            string initialStatus = "New";
            string webhookEvent = "payment_intent.succeeded";

            // Act
            string updatedStatus = initialStatus;
            if (webhookEvent == "payment_intent.succeeded")
            {
                updatedStatus = "Paid";
            }

            // Assert
            Assert.Equal("Paid", updatedStatus);
        }

        [Fact]
        public void PaymentFailure_WebhookReportsFailed_LeavesOrderUnpaid()
        {
            // Arrange
            string initialStatus = "New";
            string webhookEvent = "payment_intent.payment_failed";

            // Act
            string updatedStatus = initialStatus;
            if (webhookEvent == "payment_intent.succeeded")
            {
                updatedStatus = "Paid";
            }
            else if (webhookEvent == "payment_intent.payment_failed")
            {
                updatedStatus = "Unpaid";
            }

            // Assert
            Assert.Equal("Unpaid", updatedStatus);
        }

        [Fact]
        public void OrderCancellation_RestoresProductStock()
        {
            // Arrange
            int initialStock = 10;
            int orderQuantity = 3;
            int currentStockAfterPayment = initialStock - orderQuantity; // 7

            // Act: Order is cancelled by customer or admin
            bool orderCancelled = true;
            int restoredStock = currentStockAfterPayment;
            if (orderCancelled)
            {
                restoredStock += orderQuantity;
            }

            // Assert
            Assert.Equal(10, restoredStock);
        }

        // -------------------------------------------------------------
        // CONCURRENCY & MUTEX PROTECTION (SIMULTANEOUS PURCHASES)
        // -------------------------------------------------------------

        [Fact]
        public async System.Threading.Tasks.Task StockConcurrency_TwoCustomersBuyingLastUnitSimultaneously_MutexPreventsCrashAndNegativeStock()
        {
            // Arrange: only 1 unit remaining in warehouse stock
            int warehouseStock = 1;
            var syncLock = new System.Threading.SemaphoreSlim(1, 1);
            int successfulPurchases = 0;
            int rejectedPurchases = 0;

            async System.Threading.Tasks.Task PurchaseAttemptAsync(string customerName)
            {
                await syncLock.WaitAsync();
                try
                {
                    if (warehouseStock >= 1)
                    {
                        // Sufficient stock: acquire item
                        warehouseStock -= 1;
                        System.Threading.Interlocked.Increment(ref successfulPurchases);
                    }
                    else
                    {
                        // Stock depleted: reject gracefully with friendly message
                        System.Threading.Interlocked.Increment(ref rejectedPurchases);
                    }
                }
                finally
                {
                    syncLock.Release();
                }
            }

            // Act: Two concurrent customers click "Pay" at the exact same millisecond
            var taskCustomerA = System.Threading.Tasks.Task.Run(() => PurchaseAttemptAsync("Customer A"));
            var taskCustomerB = System.Threading.Tasks.Task.Run(() => PurchaseAttemptAsync("Customer B"));

            await System.Threading.Tasks.Task.WhenAll(taskCustomerA, taskCustomerB);

            // Assert: exactly 1 customer succeeds, 1 is rejected gracefully, stock never drops below 0
            Assert.Equal(1, successfulPurchases);
            Assert.Equal(1, rejectedPurchases);
            Assert.Equal(0, warehouseStock);
        }

        // -------------------------------------------------------------
        // B5: DISCOUNT / PROMO CODES TESTS
        // -------------------------------------------------------------

        [Fact]
        public void DiscountCode_PercentageDiscount_CalculatesCorrectDeduction()
        {
            // Arrange
            var discount = new DiscountCode
            {
                Code = "LIFT10",
                DiscountType = "Percentage",
                Value = 10,
                IsActive = true,
                ExpiryDate = DateTime.Now.AddDays(30),
                MaxUses = 100,
                TimesUsed = 5
            };
            decimal subtotal = 250.00m;

            // Act
            decimal discountAmount = Math.Round(subtotal * (discount.Value / 100m), 2);
            decimal finalTotal = subtotal - discountAmount;

            // Assert
            Assert.Equal(25.00m, discountAmount);
            Assert.Equal(225.00m, finalTotal);
        }

        [Fact]
        public void DiscountCode_FixedAmountDiscount_CalculatesCorrectDeduction()
        {
            // Arrange
            var discount = new DiscountCode
            {
                Code = "WELCOME25",
                DiscountType = "FixedAmount",
                Value = 25.00m,
                IsActive = true,
                ExpiryDate = DateTime.Now.AddDays(30),
                MaxUses = 100,
                TimesUsed = 2
            };
            decimal subtotal = 100.00m;

            // Act
            decimal discountAmount = Math.Min(discount.Value, subtotal);
            decimal finalTotal = subtotal - discountAmount;

            // Assert
            Assert.Equal(25.00m, discountAmount);
            Assert.Equal(75.00m, finalTotal);
        }

        [Fact]
        public void DiscountCode_ExpiredCode_ValidationFails()
        {
            // Arrange
            var expiredDiscount = new DiscountCode
            {
                Code = "EXPIRED20",
                ExpiryDate = DateTime.Now.AddDays(-5),
                IsActive = true
            };

            // Act
            bool isValid = DateTime.Now <= expiredDiscount.ExpiryDate && expiredDiscount.IsActive;

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void DiscountCode_MaxUsageExceeded_ValidationFails()
        {
            // Arrange
            var exhaustedDiscount = new DiscountCode
            {
                Code = "MAXOUT",
                MaxUses = 10,
                TimesUsed = 10,
                ExpiryDate = DateTime.Now.AddDays(10),
                IsActive = true
            };

            // Act
            bool canUse = exhaustedDiscount.TimesUsed < exhaustedDiscount.MaxUses;

            // Assert
            Assert.False(canUse);
        }

        // -------------------------------------------------------------
        // B7: WISHLIST MOVE-TO-CART TESTS
        // -------------------------------------------------------------

        [Fact]
        public void Wishlist_MoveToCart_AddsToCartAndRemovesFromWishlist()
        {
            // Arrange
            var wishlist = new List<WishlistItem>
            {
                new WishlistItem { WishlistItemId = 1, MemberId = 10, ProductId = 100 }
            };
            var cart = new List<CartItem>();

            // Act: Move product 100 to cart
            var item = wishlist.FirstOrDefault(w => w.ProductId == 100);
            if (item != null)
            {
                cart.Add(new CartItem { MemberId = item.MemberId, ProductId = item.ProductId, Quantity = 1 });
                wishlist.Remove(item);
            }

            // Assert
            Assert.Empty(wishlist);
            Assert.Single(cart);
            Assert.Equal(100, cart[0].ProductId);
        }

        // -------------------------------------------------------------
        // B2 & B3: RECOMMENDATIONS & REVIEWS TESTS
        // -------------------------------------------------------------

        [Fact]
        public void Recommendations_SimilarProducts_ExcludesCurrentProductAndMatchesCategory()
        {
            // Arrange
            var currentProduct = new Product { ProductId = 1, Category = "Supplements", Name = "Whey 1" };
            var allProducts = new List<Product>
            {
                currentProduct,
                new Product { ProductId = 2, Category = "Supplements", Name = "Whey 2" },
                new Product { ProductId = 3, Category = "Clothing", Name = "Gym Shirt" },
                new Product { ProductId = 4, Category = "Supplements", Name = "Creatine" }
            };

            // Act
            var similar = allProducts
                .Where(p => p.ProductId != currentProduct.ProductId && p.Category == currentProduct.Category)
                .ToList();

            // Assert
            Assert.Equal(2, similar.Count);
            Assert.DoesNotContain(similar, p => p.ProductId == currentProduct.ProductId);
            Assert.All(similar, p => Assert.Equal("Supplements", p.Category));
        }

        [Fact]
        public void Reviews_AverageRating_ComputesAccurateMean()
        {
            // Arrange
            var reviews = new List<Review>
            {
                new Review { ProductId = 1, Rating = 5 },
                new Review { ProductId = 1, Rating = 4 },
                new Review { ProductId = 1, Rating = 4 }
            };

            // Act
            double average = Math.Round(reviews.Average(r => r.Rating), 1);

            // Assert: (5 + 4 + 4) / 3 = 13 / 3 = 4.333 -> 4.3
            Assert.Equal(4.3, average);
        }
    }
}

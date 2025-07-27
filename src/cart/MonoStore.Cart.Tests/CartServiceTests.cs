using MonoStore.Cart.Domain;
using MonoStore.Contracts.Cart.Commands;
using MonoStore.Contracts.Cart.Dtos;
using MonoStore.Contracts.Cart.Exceptions;
using MonoStore.Contracts.Cart.Requests;
using CartDomain = MonoStore.Cart.Domain.Cart;

namespace MonoStore.Cart.Tests;

public class CartServiceTests
{
    private static readonly Guid TestCartId = Guid.NewGuid();
    private static readonly string TestOperatingChain = "TEST";
    private static readonly string TestSessionId = "session123";
    private static readonly string TestUserId = "user123";
    private static readonly string TestProductId = "product123";
    private static readonly Product TestProduct = new(TestProductId, "Test Product", 10.99m, 8.99m, null, null, "https://example.com/product", "https://example.com/image.jpg");
    private static readonly CartItem TestCartItem = new(TestProduct, 1);

    private static CartDomain CreateTestCart(CartStatus status = CartStatus.Open, List<CartItem>? items = null)
    {
        return new CartDomain(
            TestCartId,
            TestOperatingChain,
            status,
            items ?? new List<CartItem>(),
            TestSessionId,
            TestUserId,
            0m, // Total
            0m, // TotalExVat
            0m, // BeforePriceTotal
            0m, // BeforePriceExVatTotal
            "NOK", // Currency
            1 // Version
        );
    }

    [Fact]
    public void Create_ShouldReturnCartCreated()
    {
        // Arrange
        var command = new CreateCartMessage(TestCartId, TestOperatingChain, TestSessionId, TestUserId);

        // Act
        var result = CartService.Create(command);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(TestCartId, result.Value.CartId);
        Assert.Equal(TestOperatingChain, result.Value.OperatingChain);
        Assert.Equal(TestSessionId, result.Value.SessionId);
        Assert.Equal(TestUserId, result.Value.UserId);
    }

    [Fact]
    public void Handle_AddItem_ShouldReturnItemAddedToCart_WhenCartIsOpenAndItemNotInCart()
    {
        // Arrange
        var cart = CreateTestCart();
        var command = new AddItem(TestCartItem);

        // Act
        var result = CartService.Handle(cart, command);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(TestCartId, result.Value.CartId);
        Assert.Equal(TestCartItem, result.Value.Item);
    }

    [Fact]
    public void Handle_AddItem_ShouldReturnException_WhenCartIsNotOpen()
    {
        // Arrange
        var cart = CreateTestCart(CartStatus.Abandoned);
        var command = new AddItem(TestCartItem);

        // Act
        var result = CartService.Handle(cart, command);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.IsType<InvalidCartStatusException>(result.Error);
        var exception = (InvalidCartStatusException)result.Error;
        Assert.Equal(CartStatus.Open, exception.ExpectedStatus);
        Assert.Equal(CartStatus.Abandoned, exception.ActualStatus);
    }

    [Fact]
    public void Handle_AddItem_ShouldReturnException_WhenItemAlreadyInCart()
    {
        // Arrange
        var cart = CreateTestCart(items: new List<CartItem> { TestCartItem });
        var command = new AddItem(TestCartItem);

        // Act
        var result = CartService.Handle(cart, command);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.IsType<InvalidOperationException>(result.Error);
        Assert.Equal("Item already in cart", result.Error.Message);
    }

    [Fact]
    public void Handle_RemoveItem_ShouldReturnItemRemovedFromCart_WhenCartIsOpenAndItemInCart()
    {
        // Arrange
        var cart = CreateTestCart(items: new List<CartItem> { TestCartItem });
        var command = new RemoveItem(TestProductId);

        // Act
        var result = CartService.Handle(cart, command);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(TestCartId, result.Value.CartId);
        Assert.Equal(TestProductId, result.Value.ProductId);
    }

    [Fact]
    public void Handle_RemoveItem_ShouldReturnException_WhenCartIsNotOpen()
    {
        // Arrange
        var cart = CreateTestCart(CartStatus.Abandoned, new List<CartItem> { TestCartItem });
        var command = new RemoveItem(TestProductId);

        // Act
        var result = CartService.Handle(cart, command);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.IsType<InvalidCartStatusException>(result.Error);
        var exception = (InvalidCartStatusException)result.Error;
        Assert.Equal(CartStatus.Open, exception.ExpectedStatus);
        Assert.Equal(CartStatus.Abandoned, exception.ActualStatus);
    }

    [Fact]
    public void Handle_RemoveItem_ShouldReturnException_WhenItemNotInCart()
    {
        // Arrange
        var cart = CreateTestCart();
        var command = new RemoveItem("nonexistent-product");

        // Act
        var result = CartService.Handle(cart, command);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.IsType<InvalidOperationException>(result.Error);
        Assert.Equal("Item not in cart", result.Error.Message);
    }

    [Fact]
    public void Handle_ChangeItemQuantity_ShouldReturnItemQuantityChanged_WhenValidQuantity()
    {
        // Arrange
        var cart = CreateTestCart(items: new List<CartItem> { TestCartItem });
        var command = new ChangeItemQuantity(TestProductId, 3);

        // Act
        var result = CartService.Handle(cart, command);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(TestCartId, result.Value.CartId);
        Assert.Equal(TestProductId, result.Value.ProductId);
        Assert.Equal(3, result.Value.Quantity);
    }

    [Fact]
    public void Handle_ChangeItemQuantity_ShouldReturnException_WhenCartIsNotOpen()
    {
        // Arrange
        var cart = CreateTestCart(CartStatus.Abandoned, new List<CartItem> { TestCartItem });
        var command = new ChangeItemQuantity(TestProductId, 2);

        // Act
        var result = CartService.Handle(cart, command);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.IsType<InvalidCartStatusException>(result.Error);
        var exception = (InvalidCartStatusException)result.Error;
        Assert.Equal(CartStatus.Open, exception.ExpectedStatus);
        Assert.Equal(CartStatus.Abandoned, exception.ActualStatus);
    }

    [Fact]
    public void Handle_ChangeItemQuantity_ShouldReturnException_WhenItemNotInCart()
    {
        // Arrange
        var cart = CreateTestCart();
        var command = new ChangeItemQuantity("nonexistent-product", 2);

        // Act
        var result = CartService.Handle(cart, command);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.IsType<InvalidOperationException>(result.Error);
        Assert.Equal("Item not in cart", result.Error.Message);
    }

    [Fact]
    public void Handle_ChangeItemQuantity_ShouldReturnException_WhenQuantityIsZeroOrNegative()
    {
        // Arrange
        var cart = CreateTestCart(items: new List<CartItem> { TestCartItem });
        var command = new ChangeItemQuantity(TestProductId, 0);

        // Act
        var result = CartService.Handle(cart, command);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.IsType<InvalidOperationException>(result.Error);
        Assert.Equal("Quantity must be greater than 0", result.Error.Message);
    }

    [Fact]
    public void Handle_AbandonCart_ShouldReturnCartAbandoned_WhenCartIsOpen()
    {
        // Arrange
        var cart = CreateTestCart();
        var command = new AbandonCart();

        // Act
        var result = CartService.Handle(cart, command);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(TestCartId, result.Value.CartId);
    }

    [Fact]
    public void Handle_AbandonCart_ShouldReturnException_WhenCartIsNotOpen()
    {
        // Arrange
        var cart = CreateTestCart(CartStatus.Archived);
        var command = new AbandonCart();

        // Act
        var result = CartService.Handle(cart, command);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.IsType<InvalidCartStatusException>(result.Error);
        var exception = (InvalidCartStatusException)result.Error;
        Assert.Equal(CartStatus.Open, exception.ExpectedStatus);
        Assert.Equal(CartStatus.Archived, exception.ActualStatus);
    }

    [Fact]
    public void Handle_RecoverCart_ShouldReturnCartRecovered_WhenCartIsAbandoned()
    {
        // Arrange
        var cart = CreateTestCart(CartStatus.Abandoned);
        var command = new RecoverCart();

        // Act
        var result = CartService.Handle(cart, command);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(TestCartId, result.Value.CartId);
    }

    [Fact]
    public void Handle_RecoverCart_ShouldReturnException_WhenCartIsNotAbandoned()
    {
        // Arrange
        var cart = CreateTestCart(CartStatus.Open);
        var command = new RecoverCart();

        // Act
        var result = CartService.Handle(cart, command);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.IsType<InvalidCartStatusException>(result.Error);
        var exception = (InvalidCartStatusException)result.Error;
        Assert.Equal(CartStatus.Abandoned, exception.ExpectedStatus);
        Assert.Equal(CartStatus.Open, exception.ActualStatus);
    }

    [Fact]
    public void Handle_ArchiveCart_ShouldReturnCartArchived_WhenCartIsOpen()
    {
        // Arrange
        var cart = CreateTestCart();
        var command = new ArchiveCart();

        // Act
        var result = CartService.Handle(cart, command);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(TestCartId, result.Value.CartId);
    }

    [Fact]
    public void Handle_ArchiveCart_ShouldReturnException_WhenCartIsNotOpenOrAbandoned()
    {
        // Arrange
        var cart = CreateTestCart(CartStatus.Archived);
        var command = new ArchiveCart();

        // Act
        var result = CartService.Handle(cart, command);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.IsType<InvalidCartStatusException>(result.Error);
    }

    [Fact]
    public void Handle_ClearCart_ShouldReturnCartCleared_WhenCartIsOpen()
    {
        // Arrange
        var cart = CreateTestCart(items: new List<CartItem> { TestCartItem });
        var command = new ClearCart();

        // Act
        var result = CartService.Handle(cart, command);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(TestCartId, result.Value.CartId);
    }

    [Fact]
    public void Handle_ClearCart_ShouldReturnException_WhenCartIsNotOpen()
    {
        // Arrange
        var cart = CreateTestCart(CartStatus.Abandoned);
        var command = new ClearCart();

        // Act
        var result = CartService.Handle(cart, command);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.IsType<InvalidCartStatusException>(result.Error);
        var exception = (InvalidCartStatusException)result.Error;
        Assert.Equal(CartStatus.Open, exception.ExpectedStatus);
        Assert.Equal(CartStatus.Abandoned, exception.ActualStatus);
    }
}

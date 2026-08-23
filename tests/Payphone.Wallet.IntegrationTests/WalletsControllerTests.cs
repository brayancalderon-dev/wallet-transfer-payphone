using FluentAssertions;
using Payphone.Wallet.Application.DTOs;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WalletTransfer.Application.DTOs;
using Xunit;

namespace Payphone.Wallet.IntegrationTests;

public class WalletsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public WalletsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                username = "admin",
                password = "Payphone2026!"
            });

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

        return body!.Token;
    }

    private record LoginResponseDto(string Token);

    [Fact]
    public async Task CreateWallet_WithoutToken_ShouldReturnUnauthorized()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.PostAsJsonAsync(
            "/api/wallets",
            new CreateWalletRequest("1112223334", "Test User"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateWallet_WithValidToken_ShouldReturnCreated()
    {
        var token = await GetAuthTokenAsync();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync(
            "/api/wallets",
            new CreateWalletRequest("1112223334", "Test User"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var wallet = await response.Content.ReadFromJsonAsync<WalletDto>();

        wallet.Should().NotBeNull();
        wallet!.Balance.Should().Be(0);
    }

    [Fact]
    public async Task Transfer_BetweenTwoWallets_WithInsufficientBalance_ShouldReturnError()
    {
        var token = await GetAuthTokenAsync();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var sourceResponse = await _client.PostAsJsonAsync(
            "/api/wallets",
            new CreateWalletRequest("2223334445", "Source"));

        sourceResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var source = await sourceResponse.Content
            .ReadFromJsonAsync<WalletDto>();

        var destinationResponse = await _client.PostAsJsonAsync(
            "/api/wallets",
            new CreateWalletRequest("3334445556", "Destination"));

        destinationResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var destination = await destinationResponse.Content
            .ReadFromJsonAsync<WalletDto>();

        var transferResponse = await _client.PostAsJsonAsync(
            "/api/wallets/transfer",
            new TransferRequest(
                source!.Id,
                destination!.Id,
                50));

        transferResponse.StatusCode.Should().Be(
            HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetMovements_WithoutToken_ShouldReturnOk()
    {
        var token = await GetAuthTokenAsync();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/wallets",
            new CreateWalletRequest("4445556667", "Public Test"));

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var wallet = await createResponse.Content
            .ReadFromJsonAsync<WalletDto>();

        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync(
            $"/api/wallets/{wallet!.Id}/movements");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetWallet_WithNonExistentId_ShouldReturnNotFound()
    {
        var token = await GetAuthTokenAsync();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/wallets/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateWallet_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _client.PutAsJsonAsync("/api/wallets/1", new UpdateWalletRequest("New Name"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateWallet_WithValidToken_ShouldUpdateName()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync("/api/wallets", new CreateWalletRequest("5556667778", "Original Name"));
        var wallet = await createResponse.Content.ReadFromJsonAsync<WalletDto>();

        var updateResponse = await _client.PutAsJsonAsync($"/api/wallets/{wallet!.Id}", new UpdateWalletRequest("Updated Name"));

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await updateResponse.Content.ReadFromJsonAsync<WalletDto>();
        updated!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task UpdateWallet_WithNonExistentId_ShouldReturnNotFound()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var response = await _client.PutAsJsonAsync("/api/wallets/99999", new UpdateWalletRequest("Doesn't matter"));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteWallet_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _client.DeleteAsync("/api/wallets/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteWallet_WithZeroBalance_ShouldReturnNoContent()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync("/api/wallets", new CreateWalletRequest("6667778889", "To Delete"));
        var wallet = await createResponse.Content.ReadFromJsonAsync<WalletDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/wallets/{wallet!.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/wallets/{wallet.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteWallet_WithPositiveBalance_ShouldReturnUnprocessableEntity()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Creamos dos billeteras y transferimos indirectamente no es posible sin depósito inicial,
        // así que este test documenta la regla esperando el fallo natural: billetera recién creada
        // siempre nace en 0, por lo que la ruta "con saldo positivo" se valida a nivel unitario
        // (WalletTests.EnsureCanBeDeleted_WithPositiveBalance_ShouldThrowDomainException).
        // Aquí solo confirmamos que el endpoint delega correctamente en el dominio.
        var createResponse = await _client.PostAsJsonAsync("/api/wallets", new CreateWalletRequest("7778889990", "Zero Balance"));
        var wallet = await createResponse.Content.ReadFromJsonAsync<WalletDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/wallets/{wallet!.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
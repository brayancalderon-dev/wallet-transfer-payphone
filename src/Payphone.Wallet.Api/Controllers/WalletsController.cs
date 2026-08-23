using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payphone.Wallet.Application.DTOs;
using Payphone.Wallet.Application.Services;
using WalletTransfer.Application.DTOs;

namespace Payphone.Wallet.Api.Controllers;

[ApiController]
[Route("api/wallets")]
[Authorize]
public sealed class WalletsController : ControllerBase
{
    private readonly WalletService _walletService;
    private readonly TransferService _transferService;
    private readonly MovementService _movementService;

    public WalletsController(
        WalletService walletService,
        TransferService transferService,
        MovementService movementService)
    {
        _walletService = walletService;
        _transferService = transferService;
        _movementService = movementService;
    }

    [HttpPost]
    public async Task<ActionResult<WalletDto>> Create(
        [FromBody] CreateWalletRequest request,
        CancellationToken cancellationToken)
    {
        var wallet = await _walletService.CreateWalletAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = wallet.Id },
            wallet);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WalletDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var wallet = await _walletService.GetWalletByIdAsync(
            id,
            cancellationToken);

        return Ok(wallet);
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer(
        [FromBody] TransferRequest request,
        CancellationToken cancellationToken)
    {
        await _transferService.TransferAsync(
            request,
            cancellationToken);

        return NoContent();
    }

    [HttpGet("{id:int}/movements")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<MovementDto>>> GetMovements(
        int id,
        CancellationToken cancellationToken)
    {
        var movements = await _movementService.GetHistoryByWalletIdAsync(
            id,
            cancellationToken);

        return Ok(movements);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<WalletDto>> Update(int id, [FromBody] UpdateWalletRequest request, CancellationToken cancellationToken)
    {
        var wallet = await _walletService.UpdateWalletAsync(id, request, cancellationToken);
        return Ok(wallet);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _walletService.DeleteWalletAsync(id, cancellationToken);
        return NoContent();
    }
}
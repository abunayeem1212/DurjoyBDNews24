using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Application.DTOs.Payment;
using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.API.Controllers;

public class PaymentController(IPaymentService paymentService) : BaseController
{
    [Authorize]
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] InitiatePaymentDto dto)
    {
        var result = await paymentService.InitiateSubscriptionAsync(dto, CurrentUserId);
        return Ok(ApiResponse<PaymentResponseDto>.Ok(result, "পেমেন্ট শুরু হয়েছে"));
    }

    [HttpPost("success")]
    public async Task<IActionResult> Success([FromForm] SslCommerzCallbackDto dto)
    {
        var result = await paymentService.HandleSuccessCallbackAsync(dto);
        if (!result)
            return BadRequest(ApiResponse<string>.Fail("পেমেন্ট যাচাই করা যায়নি"));
        return Ok(ApiResponse<string>.Ok("", "সাবস্ক্রিপশন সক্রিয় হয়েছে"));
    }

    [HttpPost("fail")]
    public async Task<IActionResult> Fail([FromForm] string tran_id)
    {
        await paymentService.HandleFailCallbackAsync(tran_id);
        return Ok(ApiResponse<string>.Fail("পেমেন্ট ব্যর্থ হয়েছে"));
    }

    [Authorize]
    [HttpGet("my-subscription")]
    public async Task<IActionResult> MySubscription()
    {
        var result = await paymentService.GetActiveSubscriptionAsync(CurrentUserId);
        return Ok(ApiResponse<SubscriptionDto?>.Ok(result));
    }
}
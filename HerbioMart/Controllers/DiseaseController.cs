using AutoMapper;
using HerbioMart.Data;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Diseases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HerbioMart.Controllers;

// Manages health condition directory endpoints
public class DiseaseController : Controller
{
    private readonly IDiseaseService _diseaseService;

    public DiseaseController(IDiseaseService diseaseService)
    {
        _diseaseService = diseaseService;
    }

    // GET: /Disease/Index
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var diseases = await _diseaseService.GetAllDiseasesAsync();
        return View(diseases);
    }
}
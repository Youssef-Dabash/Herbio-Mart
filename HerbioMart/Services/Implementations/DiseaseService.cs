using AutoMapper;
using HerbioMart.Data;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Diseases;
using Microsoft.EntityFrameworkCore;

namespace HerbioMart.Services.Implementations
{
    public class DiseaseService : IDiseaseService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public DiseaseService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DiseaseVM>> GetAllDiseasesAsync()
        {
            var diseases = await _context.Diseases
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<DiseaseVM>>(diseases);
        }

        public async Task<DiseaseVM?> GetDiseaseByIdAsync(int id)
        {
            var disease = await _context.Diseases
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DiseaseId == id);

            if (disease == null)
            {
                return null;
            }

            return _mapper.Map<DiseaseVM>(disease);
        }
    }
}
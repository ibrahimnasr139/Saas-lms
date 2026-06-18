using Application.Features.Public.Dtos;
using Application.Features.Website.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;
using System.Text.Json;

namespace Infrastructure.Repositories
{
    internal sealed class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PaymentMethodRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task CreatePaymentMethodAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken)
        {
            await _context.PaymentMethods.AddAsync(paymentMethod, cancellationToken);
        }
        public async Task<List<PaymentMethodDto>> GetPaymentMethodsAsync(CancellationToken cancellationToken)
        {
            return await _context.PaymentMethods
                .AsNoTracking()
                .ProjectTo<PaymentMethodDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<PublicPaymentMethodDto>> GetPaymentMethodsByTenantIdAsync(int tenantId, CancellationToken cancellationToken)
        {
            return await _context.PaymentMethods
                .AsNoTracking()
                .Where(pm => pm.TenantId == tenantId)
                .ProjectTo<PublicPaymentMethodDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
        public Task<bool> IsPaymentMethodTypeExistAsync(int tenantId, PaymentMethodType type, CancellationToken cancellationToken)
        {
            return _context.PaymentMethods
                .AsNoTracking()
                .AnyAsync(pm => pm.TenantId == tenantId && pm.Type == type, cancellationToken);
        }
        public async Task<PaymentMethodDto?> UpdatePaymentMethodAsync(int PaymentMethodId, Dictionary<string, JsonElement> Details, CancellationToken cancellationToken)
        {
            var paymentMethod = await _context.PaymentMethods
                .FirstOrDefaultAsync(pm => pm.Id == PaymentMethodId, cancellationToken);

            if (paymentMethod is null)
                return null;

            paymentMethod.Details = Details;
            await _context.SaveChangesAsync(cancellationToken);
            return _mapper.Map<PaymentMethodDto>(paymentMethod);
        }
        public async Task<PaymentMethodDto?> UpdatePaymentMethodStatusAsync(int PaymentMethodId, bool IsActive, CancellationToken cancellationToken)
        {
            var paymentMethod = await _context.PaymentMethods
                .FirstOrDefaultAsync(pm => pm.Id == PaymentMethodId, cancellationToken);

            if (paymentMethod is null)
                return null;

            paymentMethod.IsActive = IsActive;
            await _context.SaveChangesAsync(cancellationToken);
            return _mapper.Map<PaymentMethodDto>(paymentMethod);
        }
        public async Task<bool> DeletePayMentMethodAsync(int PaymentMethodId, CancellationToken cancellationToken)
        {
            var paymentMethod = await _context.PaymentMethods
                .FirstOrDefaultAsync(pm => pm.Id == PaymentMethodId);

            if (paymentMethod is null)
                return false;

            _context.PaymentMethods.Remove(paymentMethod);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
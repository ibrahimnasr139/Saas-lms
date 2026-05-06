using Application.Features.Public.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Public.Commands.CreateOrder
{
    internal sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OneOf<PublicOrderDto, Error>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICourseRepository _courseRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentSubscriptionRepository _studentSubscriptionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStudentRepository _studentRepository;

        public CreateOrderCommandHandler(ITenantRepository tenantRepository, HybridCache hybridCache, IOrderRepository orderRepository,
            IHttpContextAccessor httpContextAccessor, ICourseRepository courseRepository, IEnrollmentRepository enrollmentRepository,
            IStudentSubscriptionRepository studentSubscriptionRepository, IUnitOfWork unitOfWork, IStudentRepository studentRepository)
        {
            _tenantRepository = tenantRepository;
            _orderRepository = orderRepository;
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _courseRepository = courseRepository;
            _enrollmentRepository = enrollmentRepository;
            _studentSubscriptionRepository = studentSubscriptionRepository;
            _unitOfWork = unitOfWork;
            _studentRepository = studentRepository;
        }
        public async Task<OneOf<PublicOrderDto, Error>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var sessionId = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SessionId];
            var cachedSessionKey = $"{CacheKeysConstants.SessionKey}_{sessionId}";
            var session = await _hybridCache.GetOrCreateAsync<UserSession?>(
                cachedSessionKey,
                _ => ValueTask.FromResult<UserSession?>(null),
                cancellationToken: cancellationToken
            );
            if (session is null)
                return UserErrors.Unauthorized;

            string subDomain = string.Empty;
            var httpRequest = _httpContextAccessor.HttpContext!.Request;
            var origin = httpRequest.Headers["Origin"].ToString();
            if (!string.IsNullOrEmpty(origin) && Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                subDomain = uri.Host.Split('.')[0];
            else
                subDomain = httpRequest.Host.Host.Split(".")[0];

            var course = await _courseRepository.GetCourseByIdAsync(request.CourseId, subDomain, cancellationToken);
            if (course is null)
                return CourseErrors.CourseNotFound;

            var studentIsAlreadyEnrolled = await _enrollmentRepository.StudentIsAlreadyEnrolledAsync(session.StudentId, course.Id, cancellationToken);
            if (studentIsAlreadyEnrolled)
            {
                var studentSubscriptionIsActive = await _studentSubscriptionRepository.StudentSubscriptionIsActiveAsync(session.StudentId, course.Id, cancellationToken);
                if (studentSubscriptionIsActive)
                    return StudentErrors.AlreadyEnrolled;
            }

            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain, cancellationToken);
            var newOrder = new Order
            {
                PricePaid = course.Price,
                PaymentProof = request.PaymentProof,
                PaymentType = request.PaymentMethod,
                PaymentReference = request.PaymentReference,
                CourseId = course.Id,
                TenantId = tenantId,
                StudentId = session.StudentId,
                OrderTimeLines = new List<OrderTimeLine>
                {
                    new OrderTimeLine
                    {
                        Description = "تم إنشاء الطلب.",
                        Type = OrderTimeLineType.created,
                        Actor = await _studentRepository.GetStuentNameByIdAsync(session.StudentId, cancellationToken)
                    }
                }
            };
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await _orderRepository.CreateOrderAsync(newOrder, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
            return new PublicOrderDto
            {
                Id = newOrder.Id,
                OrderNumber = newOrder.OrderNumber!,
                Status = newOrder.Status.ToString(),
                Message = MessagesConstants.OrderCreated
            };
        }
    }
}
using Application.Features.Tenants.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Application.Features.Tenants.Commands.CreateOnboarding
{
    internal sealed class CreateOnboardingCommandHandler : IRequestHandler<CreateOnboardingCommand, OneOf<OnboardingDto, Error>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IPlanRepository _planRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IQuestionRepository _questionRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserId _currentUserId;
        private readonly HybridCache _hybridCache;
        private readonly ITenantPageRepository _tenantPageRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateOnboardingCommandHandler(ITenantRepository tenantRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor
            , UserManager<ApplicationUser> userManager, ICurrentUserId currentUserId, HybridCache hybridCache, IPlanRepository planRepository,
            ITenantPageRepository tenantPageRepository, ISubscriptionRepository subscriptionRepository, IUnitOfWork unitOfWork,
            IQuestionRepository questionRepository)
        {
            _tenantRepository = tenantRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _currentUserId = currentUserId;
            _hybridCache = hybridCache;
            _tenantPageRepository = tenantPageRepository;
            _planRepository = planRepository;
            _subscriptionRepository = subscriptionRepository;
            _unitOfWork = unitOfWork;
            _questionRepository = questionRepository;
        }
        public async Task<OneOf<OnboardingDto, Error>> Handle(CreateOnboardingCommand request, CancellationToken cancellationToken)
        {
            request = request with
            {
                SubDomain = request.SubDomain.Trim().ToLowerInvariant()
            };
            var isSubDomainExists = await _tenantRepository.IsSubDomainExistsAsync(request.SubDomain, cancellationToken);
            if (isSubDomainExists)
                return TenantErrors.SubDomainAlreadyExists;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var ownerId = _currentUserId.GetUserId();
                var user = await _userManager.FindByIdAsync(ownerId!);
                var tenant = _mapper.Map<Tenant>(request, opt =>
                    opt.AfterMap((src, dest) =>
                    {
                        dest.OwnerId = ownerId!;
                    })
                );

                var createdTenantId = await _tenantRepository.CreateTenantAsync(tenant, cancellationToken);
                _mapper.Map(request, user!);

                user!.HasOnboarded = true;
                await _userManager.UpdateAsync(user);

                var freePlanPricingId = await _planRepository.GetFreePlanPricingIdAsync(cancellationToken);
                var subscriptionId = await _subscriptionRepository.CreateFreeSubcscription(createdTenantId, freePlanPricingId, cancellationToken);
                var (ownerRoleId, assistantRoleId) = await _tenantRepository.AddTenantRoles(createdTenantId, cancellationToken);

                await _unitOfWork.SaveAsync(cancellationToken);
                var tenantMember = _mapper.Map<TenantMember>(request, opt =>
                    opt.AfterMap((src, dest) =>
                    {
                        dest.TenantId = createdTenantId;
                        dest.UserId = ownerId!;
                        dest.TenantRoleId = ownerRoleId;
                    })
                );
                await _tenantRepository.AddTenantMemberAsync(tenantMember, cancellationToken);
                await _tenantRepository.AssignAssistantPermissions(assistantRoleId, cancellationToken);

                var planId = await _planRepository.GetPlanIdAsync(freePlanPricingId, cancellationToken);
                var planFeatureIds = await _planRepository.GetPlanFeatureIdsAsync(planId, cancellationToken);

                await _tenantRepository.InitializeTenantUsageAsync(planFeatureIds, subscriptionId, createdTenantId);
                await _questionRepository.CreateQuestionCategory(new QuestionCategory
                {
                    TenantId = createdTenantId,
                    Title = TenantMemberConstants.GeneralQuestionCategory
                }, cancellationToken);

                await _tenantPageRepository.CreateTenantPagesAsync(seedDefaultPages(createdTenantId, user!), cancellationToken);
                await _tenantPageRepository.CreateWebsiteSettingAsync(seedDefaultWebsiteSetting(createdTenantId), cancellationToken);
                await _tenantPageRepository.CreateEmailSettingsAsync(seedDefaultEmailSetting(createdTenantId, user.Email!, user.Email!, $"{user.FirstName} {user.LastName}"), cancellationToken);
                await _tenantPageRepository.CreateWebsiteAppearanceSettingAsync(seedDefaultAppearanceSetting(createdTenantId), cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                await _hybridCache.RemoveAsync($"{CacheKeysConstants.LastTenantKey}_{ownerId}", cancellationToken);
                await _hybridCache.RemoveAsync($"{CacheKeysConstants.UserTenantsKey}_{ownerId}", cancellationToken);

                _httpContextAccessor?.HttpContext?.Response.Cookies.Append(AuthConstants.SubDomain, request.SubDomain, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Domain = AuthConstants.CookieDomain,
                    IsEssential = true
                });
                return new OnboardingDto { Subdomain = request.SubDomain };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
        private List<TenantPage> seedDefaultPages(int tenantId, ApplicationUser user)
        {
            return new List<TenantPage>
            {
                new TenantPage
                {
                    TenantId = tenantId,
                    Title = "الصفحة الرئيسية",
                    Url = "/",
                    IsHomePage = true,
                    PageBlocks = new List<PageBlock>
                    {
                        new PageBlock { BlockTypeId = "hero", Order = 0, Visible = true, Props = JsonSerializer.Deserialize<Dictionary<string, object>>("""{"cta":{"url":"/courses","label":"تصفح الكورسات"},"label":"منصه معتمده","title":"ابدأ رحلتك التعليمية اليوم","subtitle":"منصه تعليميه متكامله","secondaryCta":{},"backgroundImage":""}""")! },
                        new PageBlock { BlockTypeId = "featured_courses", Order = 1, Visible = true, Props = JsonSerializer.Deserialize<Dictionary<string, object>>("""{"limit":5,"title":"الدورات المميزة","subtitle":"اكتشف افضل الدورات"}""")! },
                        new PageBlock { BlockTypeId = "cta", Order = 2, Visible = true, Props = JsonSerializer.Deserialize<Dictionary<string, object>>("""{"theme":"gradient","title":"هل أنت مستعد لتطوير مهاراتك؟","ctaUrl":"/courses","ctaLabel":"تصفح الكورسات الان","description":"انضم الي الاف من الطلاب و تصفح دوراتنا"}""")! },
                        new PageBlock { BlockTypeId = "text", Order = 3, Visible = true, Props = JsonSerializer.Deserialize<Dictionary<string, object>>("""{"title":"ابدأ رحلتك التعليمية معنا","content":"<p>مرحبًا بك 👋</p>","subtitle":"تعلم • تطور • حقق أهدافك","alignment":"center"}""")! },
                        new PageBlock { BlockTypeId = "footer", Order = 4, Visible = true, Props = JsonSerializer.Deserialize<Dictionary<string, object>>($$$"""{"logo":"","email":"{{{user!.Email}}}","phone":"{{{user!.PhoneNumber}}}","address":"","companyName":"منصتنا","socialLinks":[],"copyrightText":"© 2026 جميع الحقوق محفوظة","footerSections":[],"companyDescription":"منصه تعليميه متكامله"}""")! }
                    }
                },
                new TenantPage
                {
                    TenantId = tenantId,
                    Title = "صفحة الكورسات",
                    Url = "/courses",
                    IsHomePage = false,
                    PageBlocks = new List<PageBlock>
                    {
                        new PageBlock { BlockTypeId = "text", Order = 0, Visible = true, Props = JsonSerializer.Deserialize<Dictionary<string, object>>("""{"title":"استكشف الدورات التعليمية","content":"<p></p>","subtitle":"تعلم بمرونة • طور مهاراتك • ابدأ الآن","alignment":"center"}""")! },
                        new PageBlock { BlockTypeId = "featured_courses", Order = 1, Visible = true, Props = JsonSerializer.Deserialize<Dictionary<string, object>>("""{"limit":5,"title":"الدورات المميزة","subtitle":"اكتشف افضل الدورات"}""")! },
                        new PageBlock { BlockTypeId = "footer", Order = 2, Visible = true, Props = JsonSerializer.Deserialize<Dictionary<string, object>>($$$"""{"logo":"","email":"{{{user!.Email}}}","phone":"{{{user!.PhoneNumber}}}","address":"","companyName":"منصتنا","socialLinks":[],"copyrightText":"© 2026 جميع الحقوق محفوظة","footerSections":[],"companyDescription":"منصه تعليميه متكامله"}""")! }
                    }
                }
            };
        }
        private WebsiteSetting seedDefaultWebsiteSetting(int tenantId)
        {
            return new WebsiteSetting
            {
                TenantId = tenantId,
            };
        }
        private EmailSetting seedDefaultEmailSetting(int tenantId, string senderEmail, string replyToEmail, string senderName)
        {
            return new EmailSetting
            {
                TenantId = tenantId,
                SenderEmail = senderEmail,
                SenderName = senderName,
                ReplyToEmail = replyToEmail
            };
        }
        private WebsiteAppearanceSetting seedDefaultAppearanceSetting(int tenantId, string primaryColor = "#6366f1", string secondaryColor = "#8b5cf6", string fontFamily = "cairo", DirectionType directionType = DirectionType.RTL)
        {
            return new WebsiteAppearanceSetting
            {
                TenantId = tenantId,
                PrimaryColor = primaryColor,
                SecondaryColor = secondaryColor,
                FontFamily = fontFamily,
                Direction = directionType
            };
        }
    }
}
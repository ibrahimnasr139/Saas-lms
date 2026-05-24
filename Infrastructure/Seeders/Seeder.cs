using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace Infrastructure.Seeders
{
    internal sealed class Seeder : ISeeder
    {
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public Seeder(AppDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            if (await _context.Database.CanConnectAsync())
            {
                // Seed Plans and Features
                if (!await _context.Plans.AnyAsync())
                {
                    var features = GetFeatures();
                    await _context.Features.AddRangeAsync(features);
                    var plans = GetPlans(features);
                    await _context.Plans.AddRangeAsync(plans);
                    await _context.SaveChangesAsync();
                }

                // Seed permissions (idempotent on empty table)
                if (!await _context.Permissions.AnyAsync())
                {
                    var permissions = GetPermissions();
                    await _context.Permissions.AddRangeAsync(permissions);
                    await _context.SaveChangesAsync();
                }

                // Seed Roles
                if (!await _context.Roles.AnyAsync())
                {
                    foreach (var roleName in GetRoles())
                    {
                        var role = new IdentityRole(roleName);
                        await _roleManager.CreateAsync(role);
                    }
                }

                // Seed BlockTypes
                if (!await _context.BlockTypes.AnyAsync())
                {
                    var blockTypes = GetBlockTypes();
                    await _context.BlockTypes.AddRangeAsync(blockTypes);
                    await _context.SaveChangesAsync();
                }

                // Seed AvailableSubjects
                if (!await _context.AvailableSubjects.AnyAsync())
                {
                    var subjects = GetAvailableSubjects();
                    await _context.AvailableSubjects.AddRangeAsync(subjects);
                    await _context.SaveChangesAsync();
                }

                // Seed Level
                if (!await _context.Levels.AnyAsync())
                {
                    var levels = GetLevels();
                    await _context.Levels.AddRangeAsync(levels);
                    await _context.SaveChangesAsync();
                }

                // Seed StudentChapters
                if (!await _context.StudentChapters.AnyAsync())
                {
                    var subjects = await _context.AvailableSubjects.ToListAsync();
                    var chapters = GetStudentChapters(subjects);
                    await _context.StudentChapters.AddRangeAsync(chapters);
                    await _context.SaveChangesAsync();
                }
            }
        }

        private List<Feature> GetFeatures()
        {
            var seedDate = DateTime.UtcNow;
            return new List<Feature>
            {
                new Feature { Id = Guid.NewGuid(), Key = "student_limit", Name = "عدد الطلاب", CreatedAt = seedDate },
                new Feature { Id = Guid.NewGuid(), Key = "course_limit", Name = "عدد الكورسات", CreatedAt = seedDate },
                new Feature { Id = Guid.NewGuid(), Key = "video_storage_gb", Name = "مساحة تخزين الفيديو", CreatedAt = seedDate },
                new Feature { Id = Guid.NewGuid(), Key = "members_limit", Name = "عدد الأعضاء", CreatedAt = seedDate },
                new Feature { Id = Guid.NewGuid(), Key = "website_builder", Name = "موقع إلكتروني", CreatedAt = seedDate },
                new Feature { Id = Guid.NewGuid(), Key = "live_sessions", Name = "جلسات بث مباشر", CreatedAt = seedDate },
                new Feature { Id = Guid.NewGuid(), Key = "transcripts", Name = "نصوص المحاضرات", CreatedAt = seedDate },
                new Feature { Id = Guid.NewGuid(), Key = "ai_credits", Name = "رصيد الذكاء الاصطناعي", CreatedAt = seedDate },
                new Feature { Id = Guid.NewGuid(), Key = "analytics", Name = "التحليلات", CreatedAt = seedDate },
                new Feature { Id = Guid.NewGuid(), Key = "support", Name = "الدعم الفني", CreatedAt = seedDate },
                new Feature { Id = Guid.NewGuid(), Key = "trial_duration", Name = "مدة التجربة", CreatedAt = seedDate },
            };
        }

        private List<Plan> GetPlans(List<Feature> features)
        {
            var baseDate = DateTime.UtcNow;
            var studentLimitFeature = features.First(f => f.Key == "student_limit");
            var courseLimitFeature = features.First(f => f.Key == "course_limit");
            var videoStorageGbFeature = features.First(f => f.Key == "video_storage_gb");
            var membersLimitFeature = features.First(f => f.Key == "members_limit");
            var websiteBuilderFeature = features.First(f => f.Key == "website_builder");
            var liveSessionsFeature = features.First(f => f.Key == "live_sessions");
            var transcriptsFeature = features.First(f => f.Key == "transcripts");
            var aiCreditsFeature = features.First(f => f.Key == "ai_credits");
            var analyticsFeature = features.First(f => f.Key == "analytics");
            var supportFeature = features.First(f => f.Key == "support");
            var trialDurationFeature = features.First(f => f.Key == "trial_duration");

            return new List<Plan>
            {
                // ============ Free ================
                new Plan
                {
                    Id = Guid.NewGuid(),
                    Name = "التجربة المجانية",
                    Slug = "free-trial",
                    Description = "جرب المنصة مجانًا لمدة 15 يومًا بدون الحاجة لبطاقة ائتمان.",
                    CreatedAt = baseDate,
                    PlanPricings = new List<PlanPricing>
                    {
                        new PlanPricing
                        {
                            Id = Guid.NewGuid(),
                            Price = 0m,
                            Currency = "EGP",
                            BillingCycle = BillingCycle.trial,
                            DiscountPercent = 0,
                            CreatedAt = baseDate
                        }
                    },
                    PlanFeatures = new List<PlanFeature>
                    {
                        new PlanFeature
                        {
                            Id = Guid.NewGuid(),
                            FeatureId = studentLimitFeature.Id,
                            Description = "حتى 30 طالبًا.",
                            LimitValue = 30,
                            LimitUnit = "طالب"
                        },
                        new PlanFeature
                        {
                            Id = Guid.NewGuid(),
                            FeatureId = courseLimitFeature.Id,
                            Description = "كورس واحد فقط.",
                            LimitValue = 1,
                            LimitUnit = "كورس"
                        },
                        new PlanFeature
                        {
                            Id = Guid.NewGuid(),
                            FeatureId = videoStorageGbFeature.Id,
                            Description = "حتى 5 جيجابايت.",
                            LimitValue = 5,
                            LimitUnit = "GB"
                        },
                        new PlanFeature
                        {
                            Id = Guid.NewGuid(),
                            FeatureId = membersLimitFeature.Id,
                            Description = "عضو واحد فقط.",
                            LimitValue = 1,
                            LimitUnit = "عضو"
                        },
                        new PlanFeature
                        {
                            Id = Guid.NewGuid(),
                            FeatureId = aiCreditsFeature.Id,
                            Description = "250 رصيد ذكاء اصطناعي شهريًا.",
                            LimitValue = 250,
                            LimitUnit = "شهريًا"
                        },
                        new PlanFeature
                        {
                            Id = Guid.NewGuid(),
                            FeatureId = trialDurationFeature.Id,
                            Description = "15 يومًا مجانية.",
                            LimitValue = 15,
                            LimitUnit = "يوم"
                        },
                        new PlanFeature
                        {
                            Id = Guid.NewGuid(),
                            FeatureId = websiteBuilderFeature.Id,
                            Description = "انشاء موقع الكتروني احترافي..",
                            IsEnabled = true
                        },
                        new PlanFeature
                        {
                            Id = Guid.NewGuid(),
                            FeatureId = liveSessionsFeature.Id,
                            Description = "انشاء جلسات مباشره.",
                            IsEnabled = true
                        },
                        new PlanFeature
                        {
                            Id = Guid.NewGuid(),
                            FeatureId = analyticsFeature.Id,
                            Description = "تحليلات متقدمة.",
                            IsEnabled = true
                        },
                        new PlanFeature
                        {
                            Id = Guid.NewGuid(),
                            FeatureId = supportFeature.Id,
                            Description = "دعم أساسي.",
                            IsEnabled = true
                        }
                    }
                },

                // =========== Basic ================
                new Plan
                {
                    Id = Guid.NewGuid(),
                    Name = "الخطة الأساسية",
                    Slug = "basic",
                    Description = "للمعلمين الجدد أو أصحاب الأعمال الصغيرة",
                    CreatedAt = baseDate,
                    PlanPricings = new List<PlanPricing>
                    {
                        new PlanPricing
                        {
                            Id = Guid.NewGuid(),
                            Price = 2000m,
                            Currency = "EGP",
                            BillingCycle = BillingCycle.monthly,
                            DiscountPercent = 0,
                            CreatedAt = baseDate
                        },
                        new PlanPricing
                        {
                            Id = Guid.NewGuid(),
                            Price = 20000m,
                            Currency = "EGP",
                            BillingCycle = BillingCycle.annually,
                            DiscountPercent = 15,
                            CreatedAt = baseDate
                        }
                    },
                    PlanFeatures = new List<PlanFeature>
                    {
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = studentLimitFeature.Id, Description = "حتى 150 طالبًا.", LimitValue = 150, LimitUnit = "طالب" },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = courseLimitFeature.Id, Description = "حتى 3 كورسات.", LimitValue = 3, LimitUnit = "كورس" },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = videoStorageGbFeature.Id, Description = "حتى 30 جيجابايت.", LimitValue = 30, LimitUnit = "GB" },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = membersLimitFeature.Id, Description = "حتى 1 عضو.", LimitValue = 1, LimitUnit = "عضو" },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = websiteBuilderFeature.Id, Description = "انشاء موقع الكتروني احترافي..", IsEnabled = true },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = liveSessionsFeature.Id, Description = "انشاء جلسات مباشره.", IsEnabled = true },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = aiCreditsFeature.Id, Description = "250 رصيد شهريًا.", LimitValue = 250, LimitUnit = "شهريًا" },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = analyticsFeature.Id, Description = "تحليلات متقدمة.", IsEnabled = true },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = supportFeature.Id, Description = "دعم أساسي.", IsEnabled = true }
                    }
                },

                // ============ Growth ===================
                new Plan
                {
                    Id = Guid.NewGuid(),
                    Name = "خطة النمو",
                    Slug = "growth",
                    Description = "الأكثر شعبية",
                    CreatedAt = baseDate,
                    PlanPricings = new List<PlanPricing>
                    {
                        new PlanPricing
                        {
                            Id = Guid.NewGuid(),
                            Price = 6500m,
                            Currency = "EGP",
                            BillingCycle = BillingCycle.monthly,
                            DiscountPercent = 0,
                            CreatedAt = baseDate
                        },
                        new PlanPricing
                        {
                            Id = Guid.NewGuid(),
                            Price = 65000m,
                            Currency = "EGP",
                            BillingCycle = BillingCycle.annually,
                            DiscountPercent = 20,
                            CreatedAt = baseDate
                        }
                    },
                    PlanFeatures = new List<PlanFeature>
                    {
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = studentLimitFeature.Id, Description = "حتى 1500 طالب.", LimitValue = 1500, LimitUnit = "طالب" },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = courseLimitFeature.Id, Description = "حتى 12 كورس.", LimitValue = 12, LimitUnit = "كورس" },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = videoStorageGbFeature.Id, Description = "حتى 120 جيجابايت.", LimitValue = 120, LimitUnit = "GB" },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = membersLimitFeature.Id, Description = "حتى 5 أعضاء.", LimitValue = 5, LimitUnit = "عضو" },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = websiteBuilderFeature.Id, Description = "انشاء موقع الكتروني احترافي.", IsEnabled = true },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = liveSessionsFeature.Id, Description = "انشاء جلسات مباشره.", IsEnabled = true },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = aiCreditsFeature.Id, Description = "2000 رصيد شهريًا.", LimitValue = 2000, LimitUnit = "شهريًا" },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = analyticsFeature.Id, Description = "تحليلات متقدمة.", IsEnabled = true },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = supportFeature.Id, Description = "دعم Priority.", IsEnabled = true }
                    }
                },

                // ============ Pro ===================
                new Plan
                {
                    Id = Guid.NewGuid(),
                    Name = "الخطة الاحترافية",
                    Slug = "pro",
                    Description = "للأكاديميات والمعلمين الكبار",
                    CreatedAt = baseDate,
                    PlanPricings = new List<PlanPricing>
                    {
                        new PlanPricing
                        {
                            Id = Guid.NewGuid(),
                            Price = 20000m,
                            Currency = "EGP",
                            BillingCycle = BillingCycle.monthly,
                            DiscountPercent = 0,
                            CreatedAt = baseDate
                        },
                        new PlanPricing
                        {
                            Id = Guid.NewGuid(),
                            Price = 200000m,
                            Currency = "EGP",
                            BillingCycle = BillingCycle.annually,
                            DiscountPercent = 25,
                            CreatedAt = baseDate
                        }
                    },
                    PlanFeatures = new List<PlanFeature>
                    {
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = studentLimitFeature.Id, Description = "حتى 5000 طالب.", LimitValue = 5000, LimitUnit = "طالب" },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = courseLimitFeature.Id, Description = "كورسات غير محدودة.", LimitValue = -1, LimitUnit = "غير محدود" },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = videoStorageGbFeature.Id, Description = "حتى 500 جيجابايت.", LimitValue = 500, LimitUnit = "GB" },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = membersLimitFeature.Id, Description = "حتى 20 عضو.", LimitValue = 20, LimitUnit = "عضو" },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = websiteBuilderFeature.Id, Description = "مفعل.", IsEnabled = true },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = liveSessionsFeature.Id, Description = "مفعل.", IsEnabled = true },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = transcriptsFeature.Id, Description = "مفعل.", IsEnabled = true },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = aiCreditsFeature.Id, Description = "10000 رصيد شهريًا.", LimitValue = 10000, LimitUnit = "شهريًا" },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = analyticsFeature.Id, Description = "تحليلات متقدمة.", IsEnabled = true },
                        new PlanFeature { Id = Guid.NewGuid(), FeatureId = supportFeature.Id, Description = "دعم مخصص.", IsEnabled = true }
                    }
                }
            };
        }

        private List<Permission> GetPermissions()
        {
            return new List<Permission>
                {
                    // Courses
                    new Permission { Id = "VIEW_COURSES", Name = "عرض الدورات", Description = "السماح بعرض جميع الدورات", Module = "courses" },
                    new Permission { Id = "CREATE_COURSES", Name = "إنشاء دورة", Description = "السماح بإنشاء دورات جديدة", Module = "courses" },
                    new Permission { Id = "EDIT_COURSES", Name = "تعديل الدورات", Description = "السماح بتعديل معلومات الدورة", Module = "courses" },
                    new Permission { Id = "PUBLISH_COURSES", Name = "نشر/إلغاء نشر الدورة", Description = "السماح بنشر أو إلغاء نشر الدورات", Module = "courses" },
                    new Permission { Id = "DELETE_COURSES", Name = "حذف الدورة", Description = "السماح بحذف الدورات", Module = "courses" },
                    new Permission { Id = "MANAGE_LESSONS", Name = "إدارة الدروس", Description = "السماح بإضافة وتعديل وحذف الدروس", Module = "courses" },
                    new Permission { Id = "MANAGE_VIDEOS", Name = "إدارة الفيديوهات", Description = "إدارة ورفع الفيديوهات الخاصة بالدروس", Module = "courses" },
                    new Permission { Id = "MANAGE_QUIZZES", Name = "إدارة الاختبارات", Description = "السماح بإنشاء وتعديل وحذف الاختبارات", Module = "courses" },
                    new Permission { Id = "GRADE_QUIZZES", Name = "تقييم الاختبارات", Description = "السماح بتقييم اختبارات الطلاب", Module = "courses" },
                    new Permission { Id = "MANAGE_MODULE_ITEMS", Name = "إدارة عناصر الوحدات", Description = "السماح بإضافة وتعديل وحذف عناصر الوحدات", Module = "courses" },

                    // Members & Roles
                    new Permission { Id = "VIEW_MEMBERS", Name = "عرض الأعضاء", Description = "السماح بعرض قائمة الأعضاء", Module = "members" },
                    new Permission { Id = "INVITE_MEMBERS", Name = "دعوة أعضاء", Description = "السماح بدعوة أعضاء جدد", Module = "members" },
                    new Permission { Id = "REMOVE_MEMBERS", Name = "إزالة الأعضاء", Description = "السماح بإزالة الأعضاء من المنظمة", Module = "members" },
                    new Permission { Id = "MANAGE_PERMISSIONS", Name = "إدارة الصلاحيات", Description = "السماح بتعديل صلاحيات الأدوار", Module = "members" },
                    new Permission { Id = "MANAGE_MEMBERS", Name = "إدارة الاعضاء", Description = "السماح بإدارة الاعضاء بشكل كامل", Module = "members" },

                    // Website Builder
                    new Permission { Id = "EDIT_PAGES", Name = "تعديل صفحات الموقع", Description = "السماح بتعديل صفحات الموقع", Module = "website" },
                    new Permission { Id = "PUBLISH_SITE", Name = "نشر الموقع", Description = "السماح بنشر أو تحديث الموقع", Module = "website" },
                    new Permission { Id = "EDIT_HOMEPAGE", Name = "تعديل الصفحة الرئيسية", Description = "السماح بتعديل الصفحة الرئيسية", Module = "website" },
                    new Permission { Id = "MANAGE_CATALOG", Name = "إدارة الكاتالوج", Description = "السماح بإدارة كاتالوج الدورات", Module = "website" },
                    new Permission { Id = "MANAGE_SEO", Name = "إدارة SEO", Description = "السماح بتعديل إعدادات تحسين الظهور", Module = "website" },
                    new Permission { Id = "MANAGE_BRANDING", Name = "إدارة الهوية البصرية", Description = "السماح بإدارة الألوان والشعارات", Module = "website" },
                    new Permission { Id = "MANAGE_DOMAINS", Name = "إدارة النطاقات", Description = "السماح بربط وتعديل نطاقات الموقع", Module = "website" },
                    new Permission { Id = "MANAGE_WEBSITE_SETTINGS", Name = "التحكم في اعدادات الموقع", Description = "السماح بتعديل الموقع", Module = "website" },

                    // Documents
                    new Permission { Id = "CREATE_DOCUMENTS", Name = "إنشاء مستندات", Description = "السماح بإنشاء مستندات جديدة", Module = "documents" },
                    new Permission { Id = "EDIT_DOCUMENTS", Name = "تعديل المستندات", Description = "السماح بتحرير المستندات", Module = "documents" },
                    new Permission { Id = "DELETE_DOCUMENTS", Name = "حذف المستندات", Description = "السماح بحذف المستندات", Module = "documents" },
                    new Permission { Id = "PUBLISH_DOCUMENTS", Name = "نشر المستندات", Description = "السماح بمشاركة أو نشر المستندات", Module = "documents" },

                    // Live Sessions
                    new Permission { Id = "CREATE_SESSIONS", Name = "إنشاء جلسات مباشرة", Description = "السماح بإنشاء الجلسات المباشرة", Module = "live-sessions" },
                    new Permission { Id = "EDIT_SESSIONS", Name = "تعديل الجلسات المباشرة", Description = "السماح بتعديل تفاصيل الجلسات", Module = "live-sessions" },
                    new Permission { Id = "START_END_SESSIONS", Name = "بدء/إنهاء الجلسات", Description = "السماح ببدء أو إنهاء الجلسات المباشرة", Module = "live-sessions" },
                    new Permission { Id = "VIEW_RECORDINGS", Name = "عرض التسجيلات", Description = "السماح بعرض تسجيلات الجلسات", Module = "live-sessions" },
                    new Permission { Id = "MANAGE_JOIN_PERMISSIONS", Name = "إدارة أذونات الانضمام", Description = "السماح بتحديد من يمكنه الانضمام", Module = "live-sessions" },
                    new Permission { Id = "INVITE_STUDENTS", Name = "دعوة الطلاب للجلسات", Description = "السماح بدعوة الطلاب إلى الجلسات المباشرة", Module = "live-sessions" },

                    // Payments
                    new Permission { Id = "MANAGE_ORDERS", Name = "إدارة الطلبات", Description = "السماح بإدارة الطلبات", Module = "payments" },
                    new Permission { Id = "ISSUE_REFUNDS", Name = "إصدار المبالغ المستردة", Description = "السماح بإصدار واسترجاع المدفوعات", Module = "payments" },
                    new Permission { Id = "REVENUE_ANALYTICS", Name = "عرض تحليلات الإيرادات", Description = "السماح بعرض إحصائيات المبيعات", Module = "payments" },
                    new Permission { Id = "MANAGE_COUPONS", Name = "إدارة الكوبونات", Description = "السماح بإنشاء وتعديل وحذف الكوبونات", Module = "payments" },

                    // General
                    new Permission { Id = "VIEW_DASHBOARD", Name = "عرض لوحة التحكم", Description = "السماح بالوصول إلى لوحة التحكم", Module = "general" },
                    new Permission { Id = "VIEW_ANALYTICS", Name = "عرض التحليلات", Description = "السماح بعرض تحليلات الأداء", Module = "general" },
                    new Permission { Id = "MANAGE_NOTIFICATIONS", Name = "إدارة الإشعارات", Description = "السماح بإدارة إعدادات الإشعارات", Module = "general" },
                    new Permission { Id = "ACCESS_SETTINGS", Name = "الوصول إلى الإعدادات", Description = "السماح بالوصول إلى إعدادات النظام", Module = "general" },
                    new Permission { Id = "CHANGE_BRANDING", Name = "تغيير الهوية البصرية", Description = "السماح بتغيير الألوان والشعار", Module = "general" },
                    new Permission { Id = "MANAGE_INTEGRATIONS", Name = "إدارة التكاملات", Description = "السماح بإدارة ربط الخدمات الخارجية", Module = "general" },
                    new Permission { Id = "MANAGE_CALENDAR", Name = "إدارة التقويم", Description = "السماح بإدارة أحداث التقويم", Module = "general" },
                    new Permission { Id = "CREATE_ASSIGNMENTS", Name = "إنشاء الواجبات", Description = "السماح بإنشاء واجبات جديدة", Module = "assignments" },
                    new Permission { Id = "VIEW_ASSIGNMENTS", Name = "عرض الواجبات", Description = "السماح بمشاهدة قائمة الواجبات", Module = "assignments" },
                    new Permission { Id = "MANAGE_ASSIGNMENTS", Name = "إدارة الواجبات", Description = "السماح بإضافة وتعديل وحذف الواجبات", Module = "assignments" },
                    new Permission { Id = "GRADE_ASSIGNMENTS", Name = "تقييم الواجبات", Description = "السماح بتقييم الواجبات submitted من الطلاب", Module = "assignments" },
                    new Permission { Id = "VIEW_MEMBER_PROFILE", Name = "عرض الملف الشخصي للأعضاء", Description = "السماح بمشاهدة ملفات الأعضاء الشخصية", Module = "members" },
                    new Permission { Id = "CREATE_QUIZZES", Name = "إنشاء الاختبارات", Description = "السماح بإنشاء اختبارات جديدة", Module = "courses" },
                    new Permission { Id = "VIEW_QUIZZES", Name = "عرض الاختبارات", Description = "السماح بمشاهدة قائمة الاختبارات", Module = "courses" },
                    new Permission { Id = "VIEW_QUESTION_BANK", Name = "عرض بنك الأسئلة", Description = "السماح بالوصول إلى بنك الأسئلة", Module = "courses" },
                    new Permission { Id = "VIEW_PERFORMANCE_CHART", Name = "عرض مخططات الأداء", Description = "السماح بعرض مخططات ومؤشرات الأداء", Module = "general"},
                    new Permission {Id = "MANAGE_ATTEMPTS" , Name = "إدارة محاولات ارسال الاختبار", Description = "السماح بإدارة محاولات ارسال الاختبار", Module = "quizzes"},
                    new Permission { Id = "MANAGE_SUBMISSIONS", Name = "إدارة التقديمات", Description = "السماح بإدارة التقديمات", Module = "assignments" },
                    new Permission{ Id = "MANAGE_STUDENTS", Name = "إدارة الطلاب", Description = "السماح بإضافة وتعديل وحذف الطلاب",Module = "students"}
                };
        }

        private List<string> GetRoles() => new List<string> { "Student", "Tenant", "Parent" };

        private List<BlockType> GetBlockTypes()
        {
            return new List<BlockType>
            {
                new BlockType
                {
                    Id = "hero",
                    DisplayName = "القسم الرئيسي",
                    Description = "قسم بارز في أعلى الصفحة مع صورة خلفية وأزرار إجراء",
                    Icon = "layout",
                    Schema = JsonDocument.Parse("""
                    {
                        "title": { "type": "string", "label": "العنوان الرئيسي", "required": true, "placeholder": "ابدأ رحلتك التعليمية اليوم", "default": "ابدأ رحلتك التعليمية اليوم", "validation": { "max": 100 } },
                        "subtitle": { "type": "string", "label": "العنوان الفرعي", "placeholder": "منصة تعليمية متكاملة تساعدك على تطوير مهاراتك", "validation": { "max": 300 } },
                        "label": { "type": "string", "label": "شارة علوية", "description": "نص صغير يظهر فوق العنوان كشارة", "placeholder": "منصة معتمدة", "validation": { "max": 50 } },
                        "backgroundImage": { "type": "image", "label": "صورة الخلفية", "description": "صورة تظهر خلف المحتوى مع تأثير تعتيم", "placeholder": "اختر صورة للخلفية" },
                        "cta": { "type": "object", "label": "الزر الرئيسي", "required": true, "arrayItemSchema": { "label": { "type": "string", "label": "نص الزر", "required": true, "default": "ابدأ الآن", "placeholder": "ابدأ الآن" }, "url": { "type": "url", "label": "رابط الزر", "required": true, "default": "/signup", "placeholder": "/signup" } } },
                        "secondaryCta": { "type": "object", "label": "الزر الثانوي", "description": "زر إضافي اختياري (مثل: شاهد الفيديو)", "arrayItemSchema": { "label": { "type": "string", "label": "نص الزر", "default": "شاهد الفيديو", "placeholder": "شاهد الفيديو" }, "url": { "type": "url", "label": "رابط الزر", "default": "#video", "placeholder": "#video" } } }
                    }
                    """)
                },

                // Text Block
                new BlockType
                {
                    Id = "text",
                    DisplayName = "نص منسق",
                    Description = "قسم لعرض محتوى نصي منسق مع عنوان",
                    Icon = "type",
                    Schema = JsonDocument.Parse("""
                    {
                        "title": { "type": "string", "label": "العنوان", "placeholder": "عنوان القسم", "validation": { "max": 100 } },
                        "subtitle": { "type": "string", "label": "شارة علوية", "description": "نص صغير يظهر فوق العنوان كشارة", "placeholder": "تعرف علينا", "validation": { "max": 50 } },
                        "content": { "type": "string", "label": "المحتوى", "required": true, "placeholder": "اكتب المحتوى النصي هنا...", "validation": { "max": 5000 } },
                        "alignment": { "type": "select", "label": "محاذاة النص", "description": "اختر محاذاة النص: يمين، وسط، يسار", "default": "center", "placeholder": "وسط", "options": [ { "label": "يسار", "value": "start" }, { "label": "وسط", "value": "center" }, { "label": "يمين", "value": "end" } ] }
                    }
                    """)
                },

                new BlockType
                {
                    Id = "featured_courses",
                    DisplayName = "الكورسات المميزة",
                    Description = "عرض شبكة من الكورسات مع صور وتفاصيل",
                    Icon = "grid",
                    Schema = JsonDocument.Parse("""
                    {
                        "title": { "type": "string", "label": "عنوان القسم", "required": true, "default": "الدورات المميزة", "placeholder": "الدورات المميزة", "validation": { "max": 100 } },
                        "subtitle": { "type": "string", "label": "الوصف", "placeholder": "اكتشف أفضل الدورات التدريبية", "validation": { "max": 200 } },
                        "limit": { "type": "number", "label": "عدد الكورسات", "description": "عدد الكورسات المعروضة في الشبكة", "required": true, "default": 5, "validation": { "min": 3, "max": 12 } }
                    }
                    """)
                },

                new BlockType
                {
                    Id = "testimonials",
                    DisplayName = "آراء المشتركين",
                    Description = "عرض تقييمات وآراء المشتركين مع صورهم",
                    Icon = "message-square",
                    Schema = JsonDocument.Parse("""
                    {
                        "title": { "type": "string", "label": "عنوان القسم", "default": "ماذا يقول طلابنا", "placeholder": "ماذا يقول طلابنا", "validation": { "max": 100 } },
                        "subtitle": { "type": "string", "label": "الوصف", "placeholder": "آراء حقيقية من طلابنا", "validation": { "max": 200 } },
                        "testimonials": { "type": "array", "label": "التقييمات", "required": true, "arrayItemSchema": { "name": { "type": "string", "label": "الاسم", "required": true, "placeholder": "أحمد محمد" }, "content": { "type": "string", "label": "نص التقييم", "required": true, "placeholder": "تجربة رائعة مع المنصة...", "validation": { "max": 500 } }, "rating": { "type": "number", "label": "التقييم", "default": 5, "validation": { "min": 1, "max": 5 } }, "avatar": { "type": "image", "label": "الصورة الشخصية", "placeholder": "ارفع صورة الشخص" } } }
                    }
                    """)
                },

                // Cta Block
                new BlockType
                {
                    Id = "cta",
                    DisplayName = "دعوة لاتخاذ إجراء",
                    Description = "قسم ملفت لحث الزائر على اتخاذ إجراء معين",
                    Icon = "mouse-pointer-click",
                    Schema = JsonDocument.Parse("""
                    {
                        "title": { "type": "string", "label": "العنوان", "required": true, "default": "هل أنت مستعد لتطوير مهاراتك؟", "placeholder": "هل أنت مستعد لتطوير مهاراتك؟", "validation": { "max": 100 } },
                        "description": { "type": "string", "label": "الوصف", "placeholder": "انضم إلى آلاف الطلاب واستفد من دوراتنا", "validation": { "max": 300 } },
                        "ctaLabel": { "type": "string", "label": "نص الزر", "required": true, "default": "ابدأ الآن مجاناً", "placeholder": "ابدأ الآن مجاناً" },
                        "ctaUrl": { "type": "url", "label": "رابط الزر", "required": true, "default": "/signup", "placeholder": "/signup" },
                        "theme": { "type": "select", "label": "نمط الخلفية", "description": "اختر نمط الخلفية: تدرج لوني، داكن، فاتح", "default": "gradient", "placeholder": "gradient", "options": [ { "value": "gradient", "label": "تدرج لوني" }, { "value": "dark", "label": "داكن" }, { "value": "light", "label": "فاتح" } ] }
                    }
                    """)
                },

                // Footer Block
                new BlockType
                {
                    Id = "footer",
                    DisplayName = "ذيل الصفحة",
                    Description = "الجزء السفلي مع معلومات التواصل والروابط",
                    Icon = "align-bottom",
                    Schema = JsonDocument.Parse("""
                    {
                        "companyName": { "type": "string", "label": "اسم المنصة", "required": true, "default": "منصتنا", "placeholder": "منصتنا التعليمية", "validation": { "max": 50 } },
                        "companyDescription": { "type": "string", "label": "وصف المنصة", "placeholder": "منصة تعليمية متكاملة...", "validation": { "max": 300 } },
                        "logo": { "type": "image", "label": "الشعار", "description": "شعار المنصة في الفوتر", "placeholder": "ارفع شعار المنصة" },
                        "email": { "type": "string", "label": "البريد الإلكتروني", "placeholder": "info@example.com" },
                        "phone": { "type": "string", "label": "رقم الهاتف", "placeholder": "+966 50 000 0000" },
                        "address": { "type": "string", "label": "العنوان", "placeholder": "الرياض، المملكة العربية السعودية", "validation": { "max": 200 } },
                        "copyrightText": { "type": "string", "label": "نص حقوق النشر", "default": "© 2025 جميع الحقوق محفوظة", "placeholder": "© 2025 جميع الحقوق محفوظة", "validation": { "max": 200 } },
                        "socialLinks": { "type": "array", "label": "روابط التواصل الاجتماعي", "arrayItemSchema": { "platform": { "type": "string", "label": "المنصة", "required": true, "placeholder": "twitter, facebook, instagram, linkedin, youtube" }, "url": { "type": "url", "label": "الرابط", "required": true, "placeholder": "https://twitter.com/example" } } },
                        "footerSections": { "type": "array", "label": "أقسام الروابط", "description": "أقسام تحتوي على روابط سريعة", "arrayItemSchema": { "title": { "type": "string", "label": "عنوان القسم", "required": true, "placeholder": "روابط سريعة" }, "links": { "type": "array", "label": "الروابط", "arrayItemSchema": { "label": { "type": "string", "label": "نص الرابط", "required": true, "placeholder": "حول المنصة" }, "url": { "type": "url", "label": "الرابط", "required": true, "placeholder": "/about" } } } } }
                    }
                    """)
                }
            };
        }

        private List<AvailableSubject> GetAvailableSubjects()
        {
            return new List<AvailableSubject>
            {
                // ===================== Primary =====================
                new AvailableSubject { Grade = "الصف الأول الإبتدائي", Key = "p1-sem1-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الأول الإبتدائي", Key = "p1-sem1-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الأول الإبتدائي", Key = "p1-sem1-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الأول" },

                new AvailableSubject { Grade = "الصف الأول الإبتدائي", Key = "p1-sem2-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الأول الإبتدائي", Key = "p1-sem2-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الأول الإبتدائي", Key = "p1-sem2-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الثاني" },

                // ===================== Primary 2 =====================
                new AvailableSubject { Grade = "الصف الثاني الإبتدائي", Key = "p2-sem1-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثاني الإبتدائي", Key = "p2-sem1-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثاني الإبتدائي", Key = "p2-sem1-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الأول" },

                new AvailableSubject { Grade = "الصف الثاني الإبتدائي", Key = "p2-sem2-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثاني الإبتدائي", Key = "p2-sem2-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثاني الإبتدائي", Key = "p2-sem2-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الثاني" },

                // ===================== Primary 3 =====================
                new AvailableSubject { Grade = "الصف الثالث الإبتدائي", Key = "p3-sem1-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثالث الإبتدائي", Key = "p3-sem1-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثالث الإبتدائي", Key = "p3-sem1-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الأول" },

                new AvailableSubject { Grade = "الصف الثالث الإبتدائي", Key = "p3-sem2-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثالث الإبتدائي", Key = "p3-sem2-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثالث الإبتدائي", Key = "p3-sem2-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الثاني" },

                // ===================== Primary 4 =====================
                new AvailableSubject { Grade = "الصف الرابع الإبتدائي", Key = "p4-sem1-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الرابع الإبتدائي", Key = "p4-sem1-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الرابع الإبتدائي", Key = "p4-sem1-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الأول" },

                new AvailableSubject { Grade = "الصف الرابع الإبتدائي", Key = "p4-sem2-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الرابع الإبتدائي", Key = "p4-sem2-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الرابع الإبتدائي", Key = "p4-sem2-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الثاني" },

                // ===================== Primary 5 =====================
                new AvailableSubject { Grade = "الصف الخامس الإبتدائي", Key = "p5-sem1-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الخامس الإبتدائي", Key = "p5-sem1-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الخامس الإبتدائي", Key = "p5-sem1-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الأول" },

                new AvailableSubject { Grade = "الصف الخامس الإبتدائي", Key = "p5-sem2-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الخامس الإبتدائي", Key = "p5-sem2-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الخامس الإبتدائي", Key = "p5-sem2-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الثاني" },

                // ===================== Primary 6 =====================
                new AvailableSubject { Grade = "الصف السادس الإبتدائي", Key = "p6-sem1-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف السادس الإبتدائي", Key = "p6-sem1-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف السادس الإبتدائي", Key = "p6-sem1-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الأول" },

                new AvailableSubject { Grade = "الصف السادس الإبتدائي", Key = "p6-sem2-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف السادس الإبتدائي", Key = "p6-sem2-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف السادس الإبتدائي", Key = "p6-sem2-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الثاني" },

                // ===================== Prep 1 =====================
                new AvailableSubject { Grade = "الصف الأول الاعدادي", Key = "j1-sem1-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الأول الاعدادي", Key = "j1-sem1-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الأول الاعدادي", Key = "j1-sem1-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الأول الاعدادي", Key = "j1-sem1-sub4", DisplayName = "علوم", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الأول الاعدادي", Key = "j1-sem1-sub5", DisplayName = "الدراسات الاجتماعية", Semester = "الفصل الدراسي الأول" },

                new AvailableSubject { Grade = "الصف الأول الاعدادي", Key = "j1-sem2-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الأول الاعدادي", Key = "j1-sem2-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الأول الاعدادي", Key = "j1-sem2-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الأول الاعدادي", Key = "j1-sem2-sub4", DisplayName = "علوم", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الأول الاعدادي", Key = "j1-sem2-sub5", DisplayName = "الدراسات الاجتماعية", Semester = "الفصل الدراسي الثاني" },

                // ===================== Prep 2 =====================
                new AvailableSubject { Grade = "الصف الثاني الاعدادي", Key = "j2-sem1-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثاني الاعدادي", Key = "j2-sem1-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثاني الاعدادي", Key = "j2-sem1-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثاني الاعدادي", Key = "j2-sem1-sub4", DisplayName = "علوم", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثاني الاعدادي", Key = "j2-sem1-sub5", DisplayName = "الدراسات الاجتماعية", Semester = "الفصل الدراسي الأول" },

                new AvailableSubject { Grade = "الصف الثاني الاعدادي", Key = "j2-sem2-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثاني الاعدادي", Key = "j2-sem2-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثاني الاعدادي", Key = "j2-sem2-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثاني الاعدادي", Key = "j2-sem2-sub4", DisplayName = "علوم", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثاني الاعدادي", Key = "j2-sem2-sub5", DisplayName = "الدراسات الاجتماعية", Semester = "الفصل الدراسي الثاني" },

                // ===================== Prep 3 =====================
                new AvailableSubject { Grade = "الصف الثالث الاعدادي", Key = "j3-sem1-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثالث الاعدادي", Key = "j3-sem1-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثالث الاعدادي", Key = "j3-sem1-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثالث الاعدادي", Key = "j3-sem1-sub4", DisplayName = "علوم", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثالث الاعدادي", Key = "j3-sem1-sub5", DisplayName = "الدراسات الاجتماعية", Semester = "الفصل الدراسي الأول" },

                new AvailableSubject { Grade = "الصف الثالث الاعدادي", Key = "j3-sem2-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثالث الاعدادي", Key = "j3-sem2-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثالث الاعدادي", Key = "j3-sem2-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثالث الاعدادي", Key = "j3-sem2-sub4", DisplayName = "علوم", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثالث الاعدادي", Key = "j3-sem2-sub5", DisplayName = "الدراسات الاجتماعية", Semester = "الفصل الدراسي الثاني" },

                // ===================== Secondary =====================
                new AvailableSubject { Grade = "الصف الأول الثانوي", Key = "s1-sem1-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الأول الثانوي", Key = "s1-sem1-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الأول الثانوي", Key = "s1-sem1-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الأول الثانوي", Key = "s1-sem1-sub4", DisplayName = "العلوم المتكاملة", Semester = "الفصل الدراسي الأول" },

                new AvailableSubject { Grade = "الصف الأول الثانوي", Key = "s1-sem2-sub1", DisplayName = "رياضيات", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الأول الثانوي", Key = "s1-sem2-sub2", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الأول الثانوي", Key = "s1-sem2-sub3", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الأول الثانوي", Key = "s1-sem2-sub4", DisplayName = "العلوم المتكاملة", Semester = "الفصل الدراسي الثاني" },

                new AvailableSubject { Grade = "الصف الثاني الثانوي", Key = "s2-sem1-sub1", DisplayName = "رياضيات بحته", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثاني الثانوي", Key = "s2-sem1-sub2", DisplayName = "رياضيات تطبيقية", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثاني الثانوي", Key = "s2-sem1-sub3", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثاني الثانوي", Key = "s2-sem1-sub4", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثاني الثانوي", Key = "s2-sem1-sub5", DisplayName = "فيزياء", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثاني الثانوي", Key = "s2-sem1-sub6", DisplayName = "كيمياء", Semester = "الفصل الدراسي الأول" },

                new AvailableSubject { Grade = "الصف الثاني الثانوي", Key = "s2-sem2-sub1", DisplayName = "رياضيات بحته", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثاني الثانوي", Key = "s2-sem2-sub2", DisplayName = "رياضيات تطبيقية", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثاني الثانوي", Key = "s2-sem2-sub3", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثاني الثانوي", Key = "s2-sem2-sub4", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثاني الثانوي", Key = "s2-sem2-sub5", DisplayName = "فيزياء", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثاني الثانوي", Key = "s2-sem2-sub6", DisplayName = "كيمياء", Semester = "الفصل الدراسي الثاني" },

                new AvailableSubject { Grade = "الصف الثالث الثانوي", Key = "s3-sem1-sub1", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثالث الثانوي", Key = "s3-sem1-sub2", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثالث الثانوي", Key = "s3-sem1-sub3", DisplayName = "فيزياء", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثالث الثانوي", Key = "s3-sem1-sub4", DisplayName = "كيمياء", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثالث الثانوي", Key = "s3-sem1-sub5", DisplayName = "أحياء", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثالث الثانوي", Key = "s3-sem1-sub6", DisplayName = "رياضيات بحته", Semester = "الفصل الدراسي الأول" },
                new AvailableSubject { Grade = "الصف الثالث الثانوي", Key = "s3-sem1-sub7", DisplayName = "رياضيات تطبيقية", Semester = "الفصل الدراسي الأول" },

                new AvailableSubject { Grade = "الصف الثالث الثانوي", Key = "s3-sem2-sub1", DisplayName = "لغة عربية", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثالث الثانوي", Key = "s3-sem2-sub2", DisplayName = "لغة إنجليزية", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثالث الثانوي", Key = "s3-sem2-sub3", DisplayName = "فيزياء", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثالث الثانوي", Key = "s3-sem2-sub4", DisplayName = "كيمياء", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثالث الثانوي", Key = "s3-sem2-sub5", DisplayName = "أحياء", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثالث الثانوي", Key = "s3-sem2-sub6", DisplayName = "رياضيات بحته", Semester = "الفصل الدراسي الثاني" },
                new AvailableSubject { Grade = "الصف الثالث الثانوي", Key = "s3-sem2-sub7", DisplayName = "رياضيات تطبيقية", Semester = "الفصل الدراسي الثاني" },
            };
        }

        private List<Level> GetLevels()
        {
            var levels = new List<Level>();
            int totalXp = 0;
            int maxLevel = 100;

            for (int levelNumber = 1; levelNumber <= maxLevel; levelNumber++)
            {
                int xpForNextLevel = 15 + ((levelNumber - 1) * 5);

                levels.Add(new Level
                {
                    LevelNumber = levelNumber,
                    RequiredXp = totalXp,
                    Title = GetLevelTitle(levelNumber)
                });
                totalXp += xpForNextLevel;
            }
            return levels;
        }
        private static string GetLevelTitle(int level)
        {
            return level switch
            {
                <= 5 => "Beginner",
                <= 15 => "Learner",
                <= 30 => "Dedicated",
                <= 50 => "Advanced",
                <= 75 => "Expert",
                <= 100 => "Master",
                _ => "Legend"
            };
        }

        private List<StudentChapter> GetStudentChapters(List<AvailableSubject> subjects)
        {
            var result = new List<StudentChapter>();

            foreach (var entry in GetChaptersData())
            {
                var parts = entry.SubjectKey.Split(' ');

                string gradeSegment = parts.Length > 1 ? parts[1].ToLower() : "";
                string termSegment = parts.Length > 2 ? parts[2].ToLower() : "";

                string gradeName = gradeSegment switch
                {
                    "s1" => "الصف الأول الثانوي",
                    "s2" => "الصف الثاني الثانوي",
                    "s3" => "الصف الثالث الثانوي",
                    _ => ""
                };

                string semesterName = termSegment switch
                {
                    "t1" => "الفصل الدراسي الأول",
                    "t2" => "الفصل الدراسي الثاني",
                    _ => ""
                };

                string displayName = GetSubjectDisplayName(parts[0]);

                AvailableSubject? matched = string.IsNullOrEmpty(semesterName)
                    ? subjects.FirstOrDefault(s => s.Grade == gradeName && s.DisplayName == displayName)
                    : subjects.FirstOrDefault(s => s.Grade == gradeName && s.DisplayName == displayName && s.Semester == semesterName);

                if (matched is null) continue;

                foreach (var ch in entry.Chapters)
                {
                    var metadata = ch.TotalLessons is not null
                        ? new Dictionary<string, string> { ["total_lessons"] = ch.TotalLessons }
                        : null;

                    result.Add(new StudentChapter
                    {
                        Title = ch.Title,
                        Semester = ch.Semester,
                        ChapterNumber = ch.ChapterNumber,
                        Metadata = metadata,
                        SubjectId = matched.Id,
                    });
                }
            }

            return result;
        }
        private static string GetSubjectDisplayName(string key) => key.ToLower() switch
        {
            "math" => "رياضيات",
            "algebra" => "رياضيات بحته",
            "استاتيكا" => "رياضيات تطبيقية",
            "ميكانيكا" => "رياضيات تطبيقية",
            "arabic" => "لغة عربية",
            "english" => "لغة إنجليزية",
            "science" => "العلوم المتكاملة",
            "physics" => "فيزياء",
            "chemistry" => "كيمياء",
            "biology" => "أحياء",
            _ => key
        };
        private sealed record ChapterEntry(string SubjectKey, List<ChapterItem> Chapters);
        private sealed record ChapterItem(string Semester, string Title, int ChapterNumber, string? TotalLessons);
        private static List<ChapterEntry> GetChaptersData() =>
        [
            new("math s1 t1",
            [
                new("الترم الاول", "الجبر والعلاقات والدوال", 1, "6"),
                new("الترم الاول", "حساب المثلثات", 2, "6"),
                new("الترم الاول", "التشابه", 3, "4"),
                new("الترم الاول", "نظريات التناسب في المثلث", 4, "4"),
            ]),
            new("math s1 t2",
            [
                new("الفصل الدراسي الثاني", "المصفوفات", 1, "5"),
                new("الفصل الدراسي الثاني", "البرمجة الخطية", 2, "2"),
                new("الفصل الدراسي الثاني", "حساب المثلثات", 3, "7"),
                new("الفصل الدراسي الثاني", "المتجهات", 4, "3"),
                new("الفصل الدراسي الثاني", "الخط المستقيم", 5, "4"),
            ]),
            new("science s1 t1",
            [
                new("الفصل الدراسي الأول", "النظام البيئي المائي", 1, "10"),
                new("الفصل الدراسي الأول", "الغلاف الجوي", 2, "4"),
            ]),
            new("science s1 t2",
            [
                new("الفصل الدراسي الثاني", "الغلاف الحيوي", 3, "5"),
                new("الفصل الدراسي الثاني", "الغلاف الصخري", 4, "3"),
            ]),
            new("arabic s1 t1",
            [
                new("الفصل الدراسي الأول", "مجال القراءة", 1, "4"),
                new("الفصل الدراسي الأول", "مجال البلاغة", 2, "4"),
                new("الفصل الدراسي الأول", "مجال الأدب", 3, "3"),
                new("الفصل الدراسي الأول", "مجال النصوص الأدبية", 4, "6"),
                new("الفصل الدراسي الأول", "مجال النحو - الوحدة الأولى", 5, "2"),
                new("الفصل الدراسي الأول", "مجال النحو - الوحدة الثانية", 6, "2"),
            ]),
            new("arabic s1 t2",
            [
                new("الفصل الدراسي الثاني", "مجال القراءة", 1, "3"),
                new("الفصل الدراسي الثاني", "مجال البلاغة", 2, "2"),
                new("الفصل الدراسي الثاني", "مجال الأدب", 3, "4"),
                new("الفصل الدراسي الثاني", "مجال النصوص الأدبية", 4, "6"),
                new("الفصل الدراسي الثاني", "مجال النحو - الوحدة الأولى", 5, "3"),
                new("الفصل الدراسي الثاني", "مجال النحو - الوحدة الثانية", 6, "3"),
                new("الفصل الدراسي الثاني", "مجال النحو - الوحدة الثالثة", 7, "4"),
            ]),
            new("english s1 t1",
            [
                new("الفصل الدراسي الأول", "Egypt's Heritage", 1, null),
                new("الفصل الدراسي الأول", "Hands Help, Hearts Care", 2, null),
                new("الفصل الدراسي الأول", "Truth Vs. Lies", 3, null),
                new("الفصل الدراسي الأول", "Save and Shine", 4, null),
                new("الفصل الدراسي الأول", "Using Technology at Schools", 5, null),
                new("الفصل الدراسي الأول", "Job Hunting", 6, null),
            ]),
            new("english s1 t2",
            [
                new("الفصل الدراسي الثاني", "Health and safety", 7, "6"),
                new("الفصل الدراسي الثاني", "Robots", 8, "6"),
                new("الفصل الدراسي الثاني", "A good education", 9, "6"),
                new("الفصل الدراسي الثاني", "What's your job?", 10, "6"),
                new("الفصل الدراسي الثاني", "Amazing people", 11, "6"),
                new("الفصل الدراسي الثاني", "Hard work", 12, "6"),
            ]),
            new("algebra s2 t1",
            [
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "الجبر: الدوال الحقيقية ورسم المنحنيات", 1, "6"),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "الجبر: الأسس واللوغاريتمات وتطبيقات عليها", 2, "5"),
            ]),
            new("algebra s2 t2",
            [
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "الرياضيات: المتتابعات والمتسلسلات", 1, "6"),
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "الرياضيات: التباديل والتوافيق", 2, "2"),
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "الرياضيات: التفاضل والتكامل", 3, "7"),
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "الرياضيات: حساب المثلثات", 4, "3"),
            ]),
            new("استاتيكا s2 t1",
            [
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "الاستاتيكا", 1, "7"),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "الهندسة والقياس", 2, "4"),
            ]),
            new("استاتيكا s2 t2",
            [
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "تطبيقات الرياضيات: الحركة المستقيمة", 1, "3"),
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "تطبيقات الرياضيات: قوانين نيوتن للحركة", 2, "5"),
            ]),
            new("arabic s2 t1",
            [
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "اللغة العربية: مجال القراءة", 1, "3"),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "اللغة العربية: مجال البلاغة", 2, "3"),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "اللغة العربية: مجال الأدب", 3, "6"),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "اللغة العربية: مجال النصوص الأدبية", 4, "6"),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "اللغة العربية: مجال النحو - الوحدة الأولى", 5, "4"),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "اللغة العربية: مجال النحو - الوحدة الثانية", 6, "1"),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "اللغة العربية: مجال النحو - الوحدة الثالثة", 7, "2"),
            ]),
            new("arabic s2 t2",
            [
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "اللغة العربية: مجال القراءة", 1, "3"),
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "اللغة العربية: مجال البلاغة", 2, "2"),
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "اللغة العربية: مجال الأدب", 3, "5"),
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "اللغة العربية: مجال النصوص الأدبية", 4, "6"),
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "اللغة العربية: مجال النحو", 5, "4"),
            ]),
            new("physics s2 t1",
            [
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "الفيزياء: الكميات الفيزيائية ووحدات القياس", 1, "1"),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "الفيزياء: الحركة في خط مستقيم", 2, "2"),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "الفيزياء: القوة والحركة", 3, "1"),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "الفيزياء: خواص الموائع المتحركة", 4, "3"),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "الفيزياء: خواص الموائع الساكنة", 5, "4"),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "الفيزياء: قوانين الغازات", 6, "3"),
            ]),
            new("physics s2 t2",
            [
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "الفيزياء: الشغل والطاقة في حياتنا اليومية", 1, "3"),
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "الفيزياء: الحركة الموجية", 2, "2"),
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "الفيزياء: الضوء", 3, "5"),
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "الفيزياء: الحركة الدائرية", 4, "1"),
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "الفيزياء: الجاذبية الكونية والحركة الدائرية", 5, "1"),
            ]),
            new("chemistry s2 t1",
            [
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "الكيمياء: الحساب الكيميائي", 1, "3"),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "الكيمياء: بنية الذرة", 2, "3"),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "الكيمياء: الجدول الدوري وتصنيف العناصر", 3, "4"),
            ]),
            new("chemistry s2 t2",
            [
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "الكيمياء: الروابط والأشكال الفراغية للجزيئات", 1, "4"),
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "الكيمياء: العناصر الممثلة في بعض المجموعات المنتظمة", 2, "2"),
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "الكيمياء: الكيمياء النووية", 3, "3"),
            ]),
            new("english s2 t1",
            [
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "Egypt - Africa Cooperation", 7, null),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "Startups & Young Entrepreneurs", 8, null),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "Sustainable Development Goals", 9, null),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "Natural Disasters", 10, null),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "Media Manipulation", 11, null),
                new("الفصل الدراسي الأول - الصف الثاني الثانوي", "Soft Skills Vs. Hard Skills", 12, null),
            ]),
            new("english s2 t2",
            [
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "English: Caring Communities", 7, "6"),
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "English: Creating a Better Community", 8, "6"),
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "English: When Art Speaks", 9, "6"),
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "English: Find Your Passion!", 10, "6"),
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "English: Healthy Choices, Healthy Life", 11, "6"),
                new("الفصل الدراسي الثاني - الصف الثاني الثانوي", "English: Time Habits & Punctuality", 12, "6"),
            ]),
            new("arabic s3",
            [
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "مجال القراءة العربية", 1, "5"),
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "مجال الأدب والنصوص", 2, "10"),
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "مجال البلاغة (مفاهيم نقدية)", 3, "2"),
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "مجال التدريبات اللغوية (النحو)", 4, "7"),
            ]),
            new("english s3",
            [
                new("الصف الثالث الثانوي - العام الدراسي", "English: Inside the World of Care", 1, null),
                new("الصف الثالث الثانوي - العام الدراسي", "English: The Power of Machines", 2, null),
                new("الصف الثالث الثانوي - العام الدراسي", "English: Protect Nature, Protect Future", 3, null),
                new("الصف الثالث الثانوي - العام الدراسي", "English: Marine Life", 4, null),
                new("الصف الثالث الثانوي - العام الدراسي", "English: From Muscles to Mindset", 5, null),
                new("الصف الثالث الثانوي - العام الدراسي", "English: Let's Hang Out!", 6, null),
            ]),
            new("algebra s3",
            [
                new("الصف الثالث الثانوي - العام الدراسي", "الجبر: نظرية ذات الحدين", 1, "3"),
                new("الصف الثالث الثانوي - العام الدراسي", "الجبر: الأعداد المركبة", 2, "3"),
                new("الصف الثالث الثانوي - العام الدراسي", "الهندسة الفراغية: الهندسة والقياس في بعدين وثلاثة أبعاد", 3, "3"),
                new("الصف الثالث الثانوي - العام الدراسي", "الهندسة الفراغية: الخطوط المستقيمة والمستويات في الفراغ", 4, "2"),
            ]),
            new("ميكانيكا s3",
            [
                new("الصف الثالث الثانوي - العام الدراسي", "الاستاتيكا: الاحتكاك", 1, "2"),
                new("الصف الثالث الثانوي - العام الدراسي", "الاستاتيكا: العزوم", 2, "2"),
                new("الصف الثالث الثانوي - العام الدراسي", "الاستاتيكا: القوى المتوازية المستوية", 3, "2"),
                new("الصف الثالث الثانوي - العام الدراسي", "الاستاتيكا: الاتزان العام", 4, "1"),
                new("الصف الثالث الثانوي - العام الدراسي", "الاستاتيكا: الازدواجات", 5, "2"),
            ]),
            new("chemistry s3",
            [
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "الكيمياء: العناصر الانتقالية", 1, "4"),
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "الكيمياء: التحليل الكيميائي", 2, "2"),
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "الكيمياء: الاتزان الكيميائي", 3, "2"),
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "الكيمياء: الكيمياء الكهربية", 4, "2"),
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "الكيمياء: الكيمياء العضوية", 5, "2"),
            ]),
            new("physics s3",
            [
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "التيار الكهربي وقانون أوم وقانونا كيرشوف", 1, "4"),
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "التأثير المغناطيسي للتيار الكهربي", 2, "4"),
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "الحث الكهرومغناطيسي", 3, "5"),
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "دوائر التيار المتردد", 4, "4"),
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "إزدواجية الموجة والجسيم (الفيزياء الحديثة)", 5, "2"),
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "الأطياف الذرية", 6, "1"),
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "الليزر", 7, "1"),
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "الإلكترونيات الحديثة", 8, "3"),
            ]),
            new("biology s3",
            [
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "الدعامة والحركة في الكائنات الحية", 1, "2"),
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "التنسيق الهرموني في الكائنات الحية", 2, "1"),
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "التكاثر في الكائنات الحية", 3, "5"),
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "المناعة في الكائنات الحية", 4, "3"),
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "الحمض النووي DNA والمعلومات الوراثية", 5, "3"),
                new("الصف الثالث الثانوي - العام الدراسي بالكامل", "الأحياء الجزيئية (RNA وتخليق البروتين)", 6, "2"),
            ]),
        ];
    }
}
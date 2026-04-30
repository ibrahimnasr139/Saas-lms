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
            }
        }

        //private List<Feature> GetFeatures()
        //{
        //    var seedDate = DateTime.UtcNow;

        //    return new List<Feature>
        //        {
        //            new Feature
        //            {
        //                Id = Guid.NewGuid(),
        //                Key = "student_limit",
        //                Name = "عدد الطلاب",
        //                CreatedAt = seedDate,
        //            },
        //            new Feature
        //            {
        //                Id = Guid.NewGuid(),
        //                Key = "course_limit",
        //                Name = "عدد الكورسات",
        //                CreatedAt = seedDate
        //            },
        //            new Feature
        //            {
        //                Id = Guid.NewGuid(),
        //                Key = "videos_per_course_monthly",
        //                Name = "الفيديوهات لكل كورس شهريًا",
        //                CreatedAt = seedDate
        //            },
        //            new Feature
        //            {
        //                Id = Guid.NewGuid(),
        //                Key = "video_storage_gb",
        //                Name = "مساحة تخزين الفيديو",
        //                CreatedAt = seedDate
        //            },
        //            new Feature
        //            {
        //                Id = Guid.NewGuid(),
        //                Key = "ai_quiz_generation",
        //                Name = "توليد اختبارات بالذكاء الاصطناعي",
        //                CreatedAt = seedDate
        //            },
        //            new Feature
        //            {
        //                Id = Guid.NewGuid(),
        //                Key = "ai_exam_creation",
        //                Name = "إنشاء امتحانات كاملة",
        //                CreatedAt = seedDate
        //            },
        //            new Feature
        //            {
        //                Id = Guid.NewGuid(),
        //                Key = "ai_lesson_outline",
        //                Name = "إنشاء مخطط درس بالذكاء الاصطناعي",
        //                CreatedAt = seedDate
        //            },
        //            new Feature
        //            {
        //                Id = Guid.NewGuid(),
        //                Key = "ai_student_insights",
        //                Name = "تحليل أداء الطلاب بالذكاء الاصطناعي",
        //                CreatedAt = seedDate
        //            },
        //            new Feature
        //            {
        //                Id = Guid.NewGuid(),
        //                Key = "ai_question_bank",
        //                Name = "بنك أسئلة بالذكاء الاصطناعي",
        //                CreatedAt = seedDate
        //            },
        //            new Feature
        //            {
        //                Id = Guid.NewGuid(),
        //                Key = "live_sessions",
        //                Name = "جلسات بث مباشر",
        //                CreatedAt = seedDate
        //            },
        //            new Feature
        //            {
        //                Id = Guid.NewGuid(),
        //                Key = "analytics",
        //                Name = "تحليلات الطلاب",
        //                CreatedAt = seedDate
        //            },
        //            new Feature
        //            {
        //                Id = Guid.NewGuid(),
        //                Key = "certificates",
        //                Name = "شهادات إتمام",
        //                CreatedAt = seedDate
        //            },
        //            new Feature
        //            {
        //                Id = Guid.NewGuid(),
        //                Key = "custom_certificates",
        //                Name = "شهادات مخصصة",
        //                CreatedAt = seedDate
        //            },
        //            new Feature
        //            {
        //                Id = Guid.NewGuid(),
        //                Key = "custom_domain",
        //                Name = "دومين مخصص",
        //                CreatedAt = seedDate
        //            },
        //            new Feature
        //            {
        //                Id = Guid.NewGuid(),
        //                Key = "support",
        //                Name = "الدعم الفني",
        //                CreatedAt = seedDate
        //            },
        //            new Feature
        //            {
        //                Id = Guid.NewGuid(),
        //                Name = "مدة التجربة",
        //                Key = "trial_duration",
        //                CreatedAt = seedDate
        //            },
        //            new Feature
        //            {
        //                Id = Guid.NewGuid(),
        //                Key = "website_builder",
        //                Name = "موقع إلكتروني",
        //                CreatedAt = seedDate
        //            },
        //        };
        //}

        //private List<Plan> GetPlans(List<Feature> features)
        //{
        //    var baseDate = DateTime.UtcNow;

        //    var studentLimitFeature = features.First(f => f.Key == "student_limit");
        //    var courseLimitFeature = features.First(f => f.Key == "course_limit");
        //    var videosPerCourseMonthlyFeature = features.First(f => f.Key == "videos_per_course_monthly");
        //    var videoStorageGbFeature = features.First(f => f.Key == "video_storage_gb");
        //    var aiQuizGenerationFeature = features.First(f => f.Key == "ai_quiz_generation");
        //    var aiExamCreationFeature = features.First(f => f.Key == "ai_exam_creation");
        //    var aiLessonOutlineFeature = features.First(f => f.Key == "ai_lesson_outline");
        //    var aiStudentInsightsFeature = features.First(f => f.Key == "ai_student_insights");
        //    var aiQuestionBankFeature = features.First(f => f.Key == "ai_question_bank");
        //    var liveSessionsFeature = features.First(f => f.Key == "live_sessions");
        //    var analyticsFeature = features.First(f => f.Key == "analytics");
        //    var certificatesFeature = features.First(f => f.Key == "certificates");
        //    var customCertificatesFeature = features.First(f => f.Key == "custom_certificates");
        //    var customDomainFeature = features.First(f => f.Key == "custom_domain");
        //    var supportFeature = features.First(f => f.Key == "support");
        //    var trialDurationFeature = features.First(f => f.Key == "trial_duration");
        //    var websiteBuilderFeature = features.First(f => f.Key == "website_builder");

        //    return new List<Plan>
        //    {
        //        // ================= FREE =================
        //        new Plan
        //        {
        //            Id = Guid.NewGuid(),
        //            Name = "التجربة المجانية",
        //            Slug = "free-trial",
        //            Description = "جرب المنصة مجانًا لمدة 15 يومًا بدون الحاجة لبطاقة ائتمان.",
        //            CreatedAt = baseDate,
        //            PlanPricings = new List<PlanPricing>
        //            {
        //                new PlanPricing
        //                {
        //                    Id = Guid.NewGuid(),
        //                    Price = 0m,
        //                    Currency = "EGP",
        //                    BillingCycle = BillingCycle.Trial,
        //                    DiscountPercent = 0,
        //                    CreatedAt = baseDate
        //                }
        //            },
        //            PlanFeatures = new List<PlanFeature>
        //            {
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = studentLimitFeature.Id,
        //                        Description = "حتى 30 طالبًا.",
        //                        LimitValue = 30,
        //                        LimitUnit = "طالب"
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = courseLimitFeature.Id,
        //                        Description = "كورس واحد فقط.",
        //                        LimitValue = 1,
        //                        LimitUnit = "كورس"
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = videosPerCourseMonthlyFeature.Id,
        //                        Description = "حتى 5 فيديوهات فقط.",
        //                        LimitValue = 5,
        //                        LimitUnit = "فيديو"
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = videoStorageGbFeature.Id,
        //                        Description = "حتى 5 جيجابايت.",
        //                        LimitValue = 5,
        //                        LimitUnit = "GB"
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = aiQuizGenerationFeature.Id,
        //                        Description = "حتى 3 اختبارات .",
        //                        LimitValue = 3,
        //                        LimitUnit = "اختبار"
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = trialDurationFeature.Id,
        //                        Description = "15 يومًا مجانية.",
        //                        LimitValue = 15,
        //                        LimitUnit = "يوم"
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = supportFeature.Id,
        //                        Description = "دعم محدود عبر البريد الإلكتروني.",
        //                        LimitValue = 1,
        //                        LimitUnit = "محدود"
        //                    },
        //                new PlanFeature
        //                {
        //                    Id = Guid.NewGuid(),
        //                    FeatureId = websiteBuilderFeature.Id,
        //                    Description = "إمكانية إنشاء موقع احترافي مخصص.",
        //                    LimitValue = 1,
        //                    LimitUnit = "site"
        //                }
        //            }
        //        },

        //        // ================= BASIC =================
        //        new Plan
        //        {
        //            Id = Guid.NewGuid(),
        //            Name = "الخطة الأساسية",
        //            Slug = "basic",
        //            Description = "مناسبة للمدرسين الأفراد لإدارة عدد محدود من الطلاب ونشر الدروس المسجلة بسهولة.",
        //            CreatedAt = baseDate,
        //            PlanPricings = new List<PlanPricing>
        //            {
        //                new PlanPricing
        //                {
        //                    Id = Guid.NewGuid(),
        //                    Price = 1500m,
        //                    Currency = "EGP",
        //                    BillingCycle = BillingCycle.Monthly,
        //                    DiscountPercent = 0,
        //                    CreatedAt = baseDate
        //                },
        //                new PlanPricing
        //                {
        //                    Id = Guid.NewGuid(),
        //                    Price = 15000m,
        //                    Currency = "EGP",
        //                    BillingCycle = BillingCycle.Annually,
        //                    DiscountPercent = 15,
        //                    CreatedAt = baseDate
        //                }
        //            },
        //            PlanFeatures = new List<PlanFeature>
        //            {
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = studentLimitFeature.Id,
        //                        Description = "حتى 150 طالبًا.",
        //                        LimitValue = 150,
        //                        LimitUnit = "طالب"
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = courseLimitFeature.Id,
        //                        Description = "إنشاء حتى 3 كورسات.",
        //                        LimitValue = 3,
        //                        LimitUnit = "كورس"
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = videosPerCourseMonthlyFeature.Id,
        //                        Description = "حتى 8 فيديوهات شهريًا لكل كورس.",
        //                        LimitValue = 8,
        //                        LimitUnit = "فيديو"
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = videoStorageGbFeature.Id,
        //                        Description = "حتى 30 جيجابايت.",
        //                        LimitValue = 30,
        //                        LimitUnit = "GB"
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = aiQuizGenerationFeature.Id,
        //                        Description = "حتى 10 اختبارات شهريًا.",
        //                        LimitValue = 10,
        //                        LimitUnit = "شهريًا"
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = aiLessonOutlineFeature.Id,
        //                        Description = "حتى 15 مرة شهريًا.",
        //                        LimitValue = 15,
        //                        LimitUnit = "شهريًا"
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = certificatesFeature.Id,
        //                        Description = "شهادات أساسية للطلاب.",
        //                        LimitValue = 1,
        //                        LimitUnit = "أساسي"
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = supportFeature.Id,
        //                        Description = "دعم عبر البريد الإلكتروني.",
        //                        LimitValue = 1,
        //                        LimitUnit = "أساسي"
        //                    },
        //                new PlanFeature
        //                {
        //                    Id = Guid.NewGuid(),
        //                    FeatureId = websiteBuilderFeature.Id,
        //                    Description = "إمكانية إنشاء موقع احترافي مخصص.",
        //                    LimitValue = 1,
        //                    LimitUnit = "site"
        //                }
        //            }
        //        },

        //        // ================= GROWTH =================
        //        new Plan
        //        {
        //            Id = Guid.NewGuid(),
        //            Name = "خطة النمو",
        //            Slug = "growth",
        //            Description = "مناسبة للمدرسين أصحاب العدد المتوسط من الطلاب مع أدوات ذكاء اصطناعي وتحليلات متقدمة.",
        //            CreatedAt = baseDate,
        //            PlanPricings = new List<PlanPricing>
        //            {
        //                new PlanPricing
        //                {
        //                    Id = Guid.NewGuid(),
        //                    Price = 4000m,
        //                    Currency = "EGP",
        //                    BillingCycle = BillingCycle.Monthly,
        //                    DiscountPercent = 0,
        //                    CreatedAt = baseDate
        //                },
        //                new PlanPricing
        //                {
        //                    Id = Guid.NewGuid(),
        //                    Price = 40000m,
        //                    Currency = "EGP",
        //                    BillingCycle = BillingCycle.Annually,
        //                    DiscountPercent = 20,
        //                    CreatedAt = baseDate
        //                }
        //            },
        //            PlanFeatures = new List<PlanFeature>
        //            {
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = studentLimitFeature.Id,
        //                        Description = "حتى 500 طالب.",
        //                        LimitValue = 500,
        //                        LimitUnit = "طالب",
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = courseLimitFeature.Id,
        //                        Description = "حتى 6 كورسات.",
        //                        LimitValue = 6,
        //                        LimitUnit = "كورس",
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = videosPerCourseMonthlyFeature.Id,
        //                        Description = "حتى 8 فيديوهات شهريًا لكل كورس.",
        //                        LimitValue = 8,
        //                        LimitUnit = "فيديو",
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = videoStorageGbFeature.Id,
        //                        Description = "حتى 80 جيجابايت.",
        //                        LimitValue = 80,
        //                        LimitUnit = "GB",
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = aiQuizGenerationFeature.Id,
        //                        Description = "حتى 50 اختبارًا شهريًا.",
        //                        LimitValue = 50,
        //                        LimitUnit = "شهريًا",
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = aiExamCreationFeature.Id,
        //                        Description = "حتى 10 امتحانات شهريًا.",
        //                        LimitValue = 10,
        //                        LimitUnit = "شهريًا",
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = aiLessonOutlineFeature.Id,
        //                        Description = "حتى 60 مرة شهريًا.",
        //                        LimitValue = 60,
        //                        LimitUnit = "شهريًا",
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = aiQuestionBankFeature.Id,
        //                        Description = "حتى 200 سؤال شهريًا.",
        //                        LimitValue = 200,
        //                        LimitUnit = "شهريًا",
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = analyticsFeature.Id,
        //                        Description = "تقارير متقدمة عن التقدم والمشاهدة.",
        //                        LimitValue = 1,
        //                        LimitUnit = "متقدم",
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = customCertificatesFeature.Id,
        //                        Description = "شهادات بتصميم مخصص.",
        //                        LimitValue = 1,
        //                        LimitUnit = "مخصص",
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = supportFeature.Id,
        //                        Description = "دعم عبر البريد والدردشة.",
        //                        LimitValue = 1,
        //                        LimitUnit = "متقدم",
        //                    },
        //                new PlanFeature
        //                {
        //                    Id = Guid.NewGuid(),
        //                    FeatureId = websiteBuilderFeature.Id,
        //                    Description = "إمكانية إنشاء موقع احترافي مخصص.",
        //                    LimitValue = 1,
        //                    LimitUnit = "site"
        //                }
        //            }
        //        },

        //        // ================= PRO =================
        //        new Plan
        //        {
        //            Id = Guid.NewGuid(),
        //            Name = "الخطة الاحترافية",
        //            Slug = "pro",
        //            Description = "للمدرسين ذوي الأعداد الكبيرة من الطلاب مع إمكانيات كاملة وذكاء اصطناعي متقدم.",
        //            CreatedAt = baseDate,
        //            PlanPricings = new List<PlanPricing>
        //            {
        //                new PlanPricing
        //                {
        //                    Id = Guid.NewGuid(),
        //                    Price = 10000m,
        //                    Currency = "EGP",
        //                    BillingCycle = BillingCycle.Monthly,
        //                    DiscountPercent = 0,
        //                    CreatedAt = baseDate
        //                },
        //                new PlanPricing
        //                {
        //                    Id = Guid.NewGuid(),
        //                    Price = 100000m,
        //                    Currency = "EGP",
        //                    BillingCycle = BillingCycle.Annually,
        //                    DiscountPercent = 25,
        //                    CreatedAt = baseDate
        //                }
        //            },
        //            PlanFeatures = new List<PlanFeature>
        //            {
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = studentLimitFeature.Id,
        //                        Description = "حتى 1500 طالب.",
        //                        LimitValue = 1500,
        //                        LimitUnit = "طالب",
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = courseLimitFeature.Id,
        //                        Description = "عدد غير محدود من الكورسات.",
        //                        LimitValue = -1,
        //                        LimitUnit = "غير محدود",
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = videosPerCourseMonthlyFeature.Id,
        //                        Description = "حتى 8 فيديوهات شهريًا لكل كورس.",
        //                        LimitValue = 8,
        //                        LimitUnit = "فيديو",
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = videoStorageGbFeature.Id,
        //                        Description = "حتى 200 جيجابايت.",
        //                        LimitValue = 200,
        //                        LimitUnit = "GB",
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = aiQuizGenerationFeature.Id,
        //                        Description = "عدد غير محدود.",
        //                        LimitValue = -1,
        //                        LimitUnit = "غير محدود",
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = aiExamCreationFeature.Id,
        //                        Description = "عدد غير محدود.",
        //                        LimitValue = -1,
        //                        LimitUnit = "غير محدود",
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = aiLessonOutlineFeature.Id,
        //                        Description = "عدد غير محدود.",
        //                        LimitValue = -1,
        //                        LimitUnit = "غير محدود",
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = aiStudentInsightsFeature.Id,
        //                        Description = "تحليلات ذكية متقدمة.",
        //                        LimitValue = 1,
        //                        LimitUnit = "نشط",
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = liveSessionsFeature.Id,
        //                        Description = "محاضرات مباشرة مع الطلاب.",
        //                        LimitValue = 1,
        //                        LimitUnit = "نشط",
        //                    },
        //                new PlanFeature
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = customCertificatesFeature.Id,
        //                        Description = "ربط دومين خاص بالأكاديمية.",
        //                        LimitValue = 1,
        //                        LimitUnit = "مسموح",
        //                    },
        //                new PlanFeature
        //                {
        //                        Id = Guid.NewGuid(),
        //                        FeatureId = supportFeature.Id,
        //                        Description = "دعم مميز 24/7.",
        //                        LimitValue = 1,
        //                        LimitUnit = "متميز",
        //                },
        //                new PlanFeature
        //                {
        //                    Id = Guid.NewGuid(),
        //                    FeatureId = websiteBuilderFeature.Id,
        //                    Description = "إمكانية إنشاء موقع احترافي مخصص.",
        //                    LimitValue = 1,
        //                    LimitUnit = "site"
        //                }
        //            }
        //        }
        //    };
        //}


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
                            BillingCycle = BillingCycle.Trial,
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
                            BillingCycle = BillingCycle.Monthly,
                            DiscountPercent = 0,
                            CreatedAt = baseDate
                        },
                        new PlanPricing
                        {
                            Id = Guid.NewGuid(),
                            Price = 20000m,
                            Currency = "EGP",
                            BillingCycle = BillingCycle.Annually,
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
                            BillingCycle = BillingCycle.Monthly,
                            DiscountPercent = 0,
                            CreatedAt = baseDate
                        },
                        new PlanPricing
                        {
                            Id = Guid.NewGuid(),
                            Price = 65000m,
                            Currency = "EGP",
                            BillingCycle = BillingCycle.Annually,
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
                            BillingCycle = BillingCycle.Monthly,
                            DiscountPercent = 0,
                            CreatedAt = baseDate
                        },
                        new PlanPricing
                        {
                            Id = Guid.NewGuid(),
                            Price = 200000m,
                            Currency = "EGP",
                            BillingCycle = BillingCycle.Annually,
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
    }
}